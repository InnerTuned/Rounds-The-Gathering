using System.Reflection;
using HarmonyLib;
using ShieldsMod.Cards;
using ShieldsMod.Integration;
using ShieldsMod.Resistance;
using ShieldsMod.UI;
using UnityEngine;

namespace ShieldsMod.Patches;

[HarmonyPatch]
internal static class ResistanceCardPatches
{
    internal static bool IsResistanceFlowActive;

    private static int _pickerID = -1;
    private static CardInfo _upgradeCard;
    private static Player _pickerPlayer;

    private static readonly FieldInfo s_isPlayingField =
        AccessTools.Field(typeof(CardChoice), "isPlaying");
    private static readonly FieldInfo s_spawnedField =
        AccessTools.Field(typeof(CardChoice), "spawned");

    [HarmonyPatch(typeof(ApplyCardStats), "ApplyStats")]
    [HarmonyPrefix]
    static bool ApplyStats_Prefix(ApplyCardStats __instance)
    {
        if (CardChoice.instance == null || !CardChoice.instance.IsPicking)
            return true;

        CardInfo card = __instance.GetComponentInParent<CardInfo>();
        if (card == null || card.cardName != UpgradeResistanceCard.CardDisplayName)
            return true;

        int pickerID = CardChoice.instance.pickrID;
        if (!IsLocalPicker(pickerID))
            return true;

        SLog.Section("ResistanceCardPatches — ApplyStats intercepted");
        DeckBuilderPickBridge.SetSuppressDeckPickPostfix(true);
        StartFlow(pickerID, card);
        return false;
    }

    [HarmonyPatch(typeof(CardChoice), "RPCA_DoEndPick")]
    [HarmonyPrefix]
    static bool RPCA_DoEndPick_Prefix()
    {
        if (!IsResistanceFlowActive)
            return true;

        SLog.Line("RPCA_DoEndPick suppressed (resistance modal open).");
        return false;
    }

    [HarmonyPatch(typeof(CardBarHandler), "AddCard")]
    [HarmonyPrefix]
    static bool CardBarHandler_AddCard_Prefix(CardInfo card)
    {
        if (!IsResistanceFlowActive || card == null)
            return true;

        if (card.cardName == UpgradeResistanceCard.CardDisplayName)
        {
            SLog.Line($"Blocked card-bar add for '{card.cardName}'.");
            return false;
        }

        return true;
    }

    private static void StartFlow(int pickerID, CardInfo upgradeCard)
    {
        _pickerID = pickerID;
        _upgradeCard = upgradeCard;
        _pickerPlayer = PlayerManager.instance?.players?.Find(p => p.playerID == pickerID);
        IsResistanceFlowActive = true;

        ResistanceSelectModalUI.instance?.Show(pickerID,
            onConfirm: OnConfirm,
            onCancel: OnCancel);
    }

    private static void OnConfirm(ResistanceType type)
    {
        SLog.Section("Resistance modal — Confirm");
        SLog.Line($"type={type}");
        IsResistanceFlowActive = false;

        CardInfo marker = ResistanceCardCatalog.GetMarkerCard(type);
        if (_pickerPlayer != null && marker != null)
        {
            try
            {
                using (DeckBuilderPickBridge.SuppressDeckPickPostfix())
                {
                    ModdingUtils.Utils.Cards.instance.AddCardToPlayer(
                        _pickerPlayer,
                        marker,
                        reassign: true,
                        twoLetterCode: "",
                        forceDisplay: 0f,
                        forceDisplayDelay: 0f,
                        addToCardBar: true);
                }

                SLog.Line($"Added marker '{marker.cardName}' to player {_pickerID}.");
            }
            catch (System.Exception ex)
            {
                SLog.Error($"AddCardToPlayer failed: {ex}");
            }
        }
        else
        {
            SLog.Warn("Confirm failed — missing player or marker card.");
        }

        SLog.Line("Upgrade Resistance stays in runtime deck (recyclable pick).");
        DeckBuilderPickBridge.EndPickPhase();
        DeckBuilderPickBridge.SetSuppressDeckPickPostfix(false);
    }

    private static void OnCancel()
    {
        SLog.Section("Resistance modal — Cancel");
        IsResistanceFlowActive = false;

        CardChoice cc = CardChoice.instance;
        if (cc == null)
        {
            SLog.Warn("CardChoice.instance is null — cannot restore state.");
            return;
        }

        cc.StopAllCoroutines();
        cc.pickrID = _pickerID;

        if (s_isPlayingField != null)
            s_isPlayingField.SetValue(cc, false);

        cc.IsPicking = true;

        if (_upgradeCard != null)
        {
            var applyStats = _upgradeCard.GetComponentInChildren<ApplyCardStats>();
            if (applyStats != null)
            {
                var doneField = AccessTools.Field(typeof(ApplyCardStats), "done");
                doneField?.SetValue(applyStats, false);
            }
        }

        var spawned = s_spawnedField?.GetValue(cc) as System.Collections.IList;
        if (spawned != null)
        {
            foreach (var obj in spawned)
            {
                var go = obj as GameObject;
                if (go == null)
                    continue;

                var stats = go.GetComponentInChildren<ApplyCardStats>();
                if (stats == null)
                    continue;

                var doneField = AccessTools.Field(typeof(ApplyCardStats), "done");
                doneField?.SetValue(stats, false);
            }
        }

        DeckBuilderPickBridge.SetSuppressDeckPickPostfix(false);
        SLog.Line("Resistance modal closed — resuming current draft hand.");
    }

    private static bool IsLocalPicker(int pickerID)
    {
        Player picker = PlayerManager.instance?.players?.Find(p => p.playerID == pickerID);
        if (picker == null)
            return false;

        var viewField = AccessTools.Field(typeof(CharacterData), "view");
        if (viewField == null)
            return true;

        object view = viewField.GetValue(picker.data);
        if (view == null)
            return true;

        var isMine = AccessTools.Property(view.GetType(), "IsMine");
        if (isMine == null)
            return true;

        return (bool)isMine.GetValue(view, null);
    }
}
