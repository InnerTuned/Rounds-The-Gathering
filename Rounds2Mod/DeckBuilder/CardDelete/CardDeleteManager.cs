using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using ModdingUtils.Utils;
using UnboundLib;
using UnboundLib.GameModes;
using UnboundLib.Networking;
using UnityEngine;

namespace DeckBuilder.CardDelete;

/// <summary>
/// During pick phases, lets the active player delete a card from their hand instead of drafting.
/// </summary>
public class CardDeleteManager : MonoBehaviour
{
    public static CardDeleteManager instance;
    public static bool isPickPhase;
    public static int currentPickerID = -1;

    private static readonly MethodInfo s_rpca_assignCard = AccessTools.Method(
        typeof(Cards),
        "RPCA_AssignCard",
        new Type[] { typeof(string), typeof(int), typeof(bool), typeof(string), typeof(float), typeof(float), typeof(bool) });

    private static readonly MethodInfo s_rpca_donePicking = AccessTools.Method(typeof(CardChoice), "RPCA_DonePicking");
    private static readonly FieldInfo s_spawnedCardsField = AccessTools.Field(typeof(CardChoice), "spawnedCards");
    private static readonly FieldInfo s_isPlayingField = AccessTools.Field(typeof(CardChoice), "isPlaying");
    private static readonly FieldInfo s_picksField = AccessTools.Field(typeof(CardChoice), "picks");

    private static MethodInfo s_photonDestroy;
    private static PropertyInfo s_photonOfflineMode;

    private CardDeleteConfirmModal _confirmModal;

    private void Start()
    {
        instance = this;
        _confirmModal = gameObject.AddComponent<CardDeleteConfirmModal>();
        GameModeManager.AddHook(GameModeHooks.HookPlayerPickStart, OnPlayerPickStart);
        GameModeManager.AddHook(GameModeHooks.HookPlayerPickEnd, OnPlayerPickEnd);
        CardDeleteLog.Section("CardDeleteManager initialized");
        CardDeleteLog.Line("Registered HookPlayerPickStart / HookPlayerPickEnd.");
    }

    private IEnumerator OnPlayerPickStart(IGameModeHandler gm)
    {
        CardDeleteLog.Section("HookPlayerPickStart (info only)");
        CardDeleteLog.Line($"gameMode={gm?.Name ?? "unknown"}; waiting for Harmony Show/StartPick patch to fire.");
        yield break;
    }

    public void BeginPickPhase(int pickerID, string source)
    {
        if (pickerID < 0)
        {
            CardDeleteLog.Warn($"BeginPickPhase ignored (pickerID={pickerID}, source={source}).");
            return;
        }

        if (isPickPhase && currentPickerID == pickerID)
        {
            CardDeleteLog.Line($"BeginPickPhase skipped — already active for player {pickerID} (source={source}).");
            return;
        }

        currentPickerID = pickerID;
        isPickPhase = true;

        CardDeleteLog.Section("Pick phase active");
        CardDeleteLog.Line($"pickerID={pickerID}, source={source}");
        CardDeleteLog.Line($"IsPicking={CardChoice.instance?.IsPicking}, pickrID={CardChoice.instance?.pickrID}, localTurn={IsLocalPlayerTurn()}");

        EnableDeleteMode(pickerID);
    }

    private IEnumerator OnPlayerPickEnd(IGameModeHandler gm)
    {
        CardDeleteLog.Section("Pick phase ended (HookPlayerPickEnd)");
        CardDeleteLog.Line($"Was picker playerID={currentPickerID}");

        isPickPhase = false;
        currentPickerID = -1;
        _confirmModal?.Hide();
        DisableDeleteMode();
        yield break;
    }

    private static readonly FieldInfo s_cardBarButtonCard =
        AccessTools.Field(typeof(CardBarButton), "card");

    private void EnableDeleteMode(int pickerID)
    {
        if (CardBarHandler.instance == null)
        {
            CardDeleteLog.Warn("EnableDeleteMode: CardBarHandler.instance is null.");
            return;
        }

        CardBar[] bars = Traverse.Create(CardBarHandler.instance)
            .Field("cardBars")
            .GetValue<CardBar[]>();

        if (bars == null || pickerID < 0 || pickerID >= bars.Length)
        {
            CardDeleteLog.Warn($"EnableDeleteMode: invalid pickerID={pickerID}, bars={(bars?.Length ?? 0)}");
            return;
        }

        int attached = 0;
        int skippedInactive = 0;
        int skippedNullCard = 0;
        CardBar bar = bars[pickerID];

        foreach (Transform child in bar.transform)
        {
            if (!child.gameObject.activeSelf)
            {
                skippedInactive++;
                continue;
            }

            CardBarButton btn = child.GetComponent<CardBarButton>();
            if (btn == null) continue;

            CardInfo card = s_cardBarButtonCard?.GetValue(btn) as CardInfo;
            if (card == null)
            {
                skippedNullCard++;
                continue;
            }

            if (child.GetComponent<CardBarDeleteButton>() == null)
            {
                child.gameObject.AddComponent<CardBarDeleteButton>();
                attached++;
            }
        }

        CardDeleteLog.Line(
            $"EnableDeleteMode: attached={attached}, skipped(inactive)={skippedInactive}, skipped(noCard)={skippedNullCard} for player {pickerID}.");

        if (attached == 0)
            CardDeleteLog.Line("Player has no deletable cards this turn (deck is empty).");
    }

    private void DisableDeleteMode()
    {
        int count = FindObjectsOfType<CardBarDeleteButton>().Length;
        foreach (var btn in FindObjectsOfType<CardBarDeleteButton>())
            Destroy(btn);

        CardDeleteLog.Line($"DisableDeleteMode: removed {count} CardBarDeleteButton component(s).");
    }

    public void ShowDeleteConfirm(CardInfo card)
    {
        if (card == null || !isPickPhase || !IsLocalPlayerTurn())
            return;

        Player player = PlayerManager.instance?.players.Find(p => p.playerID == currentPickerID);
        if (player == null)
        {
            CardDeleteLog.Warn("ShowDeleteConfirm: picker player not found.");
            return;
        }

        _confirmModal?.Show(player, card, () => RequestDelete(card));
    }

    public void RequestDelete(CardInfo card)
    {
        CardDeleteLog.Section("Delete requested (local click)");
        CardDeleteLog.Line($"Card='{card?.cardName}' objectName='{card?.name}', pickerID={currentPickerID}");
        CardDeleteLog.Line("Broadcasting URPC_SyncDelete to all clients.");

        NetworkingManager.RPC(typeof(CardDeleteManager), nameof(URPC_SyncDelete),
            currentPickerID, card.name);
    }

    [UnboundRPC]
    public static void URPC_SyncDelete(int playerID, string cardObjectName)
    {
        CardDeleteLog.Section("URPC_SyncDelete (all clients)");
        CardDeleteLog.Line($"playerID={playerID}, deleteCardObjectName='{cardObjectName}'");

        Player player = PlayerManager.instance.players.Find(p => p.playerID == playerID);
        if (player == null)
        {
            CardDeleteLog.Error($"URPC_SyncDelete: player {playerID} not found.");
            return;
        }

        int beforeCount = player.data.currentCards?.Count ?? 0;
        List<string> survivors = player.data.currentCards
            .Where(c => c != null && c.name != cardObjectName)
            .Select(c => c.name)
            .ToList();

        CardDeleteLog.Section("Deletion logic — snapshot");
        CardDeleteLog.Line($"currentCards before reset: {beforeCount}");
        CardDeleteLog.Line($"Removing: '{cardObjectName}'");
        CardDeleteLog.Line($"Survivors to reapply ({survivors.Count}): [{string.Join(", ", survivors)}]");

        CardDeleteLog.Section("Deletion logic — stat reset");
        try
        {
            Cards.RPCA_FullReset(playerID);
            CardDeleteLog.Line("RPCA_FullReset completed.");
        }
        catch (Exception ex)
        {
            CardDeleteLog.Error($"RPCA_FullReset failed: {ex}");
        }

        try
        {
            Cards.RPCA_ClearCardBar(playerID);
            CardDeleteLog.Line("RPCA_ClearCardBar completed.");
        }
        catch (Exception ex)
        {
            CardDeleteLog.Error($"RPCA_ClearCardBar failed: {ex}");
        }

        CardDeleteLog.Section("Deletion logic — reapply survivors");
        if (s_rpca_assignCard == null)
        {
            CardDeleteLog.Error("RPCA_AssignCard not found via reflection.");
        }
        else
        {
            foreach (string name in survivors)
            {
                try
                {
                    s_rpca_assignCard.Invoke(null, new object[] { name, playerID, true, "", 0f, 0f, true });
                    CardDeleteLog.Line($"Reapplied '{name}' (reassign=true).");
                }
                catch (Exception ex)
                {
                    CardDeleteLog.Error($"Failed to reapply '{name}': {ex}");
                }
            }
        }

        CardDeleteLog.Line($"currentCards after reapply: {player.data.currentCards?.Count ?? 0}");

        EndPickPhase();
    }

    private static void EndPickPhase()
    {
        CardDeleteLog.Section("EndPickPhase");

        CardChoice cc = CardChoice.instance;
        if (cc == null)
        {
            CardDeleteLog.Error("CardChoice.instance is null.");
            return;
        }

        CardDeleteLog.Line($"Before: IsPicking={cc.IsPicking}, pickrID={cc.pickrID}");

        cc.StopAllCoroutines();
        CardDeleteLog.Line("Stopped all CardChoice coroutines.");

        if (s_isPlayingField != null)
            s_isPlayingField.SetValue(cc, false);
        if (s_picksField != null)
            s_picksField.SetValue(cc, 0);

        CleanupSpawnedDraftCards(cc);

        if (s_rpca_donePicking == null)
        {
            CardDeleteLog.Warn("RPCA_DonePicking not found; falling back to IsPicking=false.");
            cc.IsPicking = false;
        }
        else
        {
            try
            {
                s_rpca_donePicking.Invoke(cc, null);
                CardDeleteLog.Line("Invoked CardChoice.RPCA_DonePicking().");
            }
            catch (Exception ex)
            {
                CardDeleteLog.Error($"RPCA_DonePicking failed: {ex}; setting IsPicking=false directly.");
                cc.IsPicking = false;
            }
        }

        CardDeleteLog.Line($"After: IsPicking={cc.IsPicking}");
    }

    private static void CleanupSpawnedDraftCards(CardChoice cc)
    {
        CardDeleteLog.Section("EndPickPhase — cleanup draft cards");

        if (s_spawnedCardsField == null)
        {
            CardDeleteLog.Warn("spawnedCards field not found.");
            return;
        }

        var spawned = s_spawnedCardsField.GetValue(cc) as List<GameObject>;
        if (spawned == null || spawned.Count == 0)
        {
            CardDeleteLog.Line("No spawned draft cards to clean up.");
            return;
        }

        CardDeleteLog.Line($"Cleaning {spawned.Count} spawned draft card(s).");

        foreach (GameObject go in spawned.ToList())
        {
            if (go == null) continue;

            try
            {
                CardVisuals visuals = go.GetComponentInChildren<CardVisuals>();
                if (visuals != null)
                    visuals.Leave();
            }
            catch (Exception ex)
            {
                CardDeleteLog.Warn($"CardVisuals.Leave on '{go.name}': {ex.Message}");
            }

            DestroyDraftCardObject(go);
        }

        spawned.Clear();
        CardDeleteLog.Line("spawnedCards cleared.");
    }

    private static void DestroyDraftCardObject(GameObject go)
    {
        if (go == null) return;

        try
        {
            if (TryPhotonDestroy(go))
            {
                CardDeleteLog.Line($"Photon-destroyed draft card '{go.name}'.");
                return;
            }
        }
        catch (Exception ex)
        {
            CardDeleteLog.Warn($"Photon destroy failed for '{go.name}': {ex.Message}");
        }

        UnityEngine.Object.Destroy(go);
        CardDeleteLog.Line($"Destroyed draft card '{go.name}' locally.");
    }

    private static bool TryPhotonDestroy(GameObject go)
    {
        if (s_photonDestroy == null)
        {
            Type photonType = AccessTools.TypeByName("Photon.Pun.PhotonNetwork");
            if (photonType == null) return false;

            s_photonOfflineMode = photonType.GetProperty("OfflineMode", BindingFlags.Public | BindingFlags.Static);
            s_photonDestroy = photonType.GetMethod("Destroy", BindingFlags.Public | BindingFlags.Static, null,
                new Type[] { typeof(GameObject) }, null);
        }

        if (s_photonDestroy == null) return false;

        if (s_photonOfflineMode != null && (bool)s_photonOfflineMode.GetValue(null, null))
            return false;

        Type photonViewType = AccessTools.TypeByName("Photon.Pun.PhotonView");
        if (photonViewType == null) return false;

        if (go.GetComponent(photonViewType) == null) return false;

        s_photonDestroy.Invoke(null, new object[] { go });
        return true;
    }

    public static bool IsLocalPlayerTurn()
    {
        if (currentPickerID < 0) return false;

        Player picker = PlayerManager.instance.players.Find(p => p.playerID == currentPickerID);
        if (picker == null) return false;

        object view = AccessTools.Field(typeof(CharacterData), "view").GetValue(picker.data);
        if (view == null) return true;

        return (bool)AccessTools.Property(view.GetType(), "IsMine").GetValue(view, null);
    }
}
