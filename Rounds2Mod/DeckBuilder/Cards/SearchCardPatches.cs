using System;
using System.Reflection;
using DeckBuilder.CardDelete;
using DeckBuilder.Data;
using DeckBuilder.GameIntegration;
using DeckBuilder.Networking;
using HarmonyLib;
using UnboundLib;
using UnityEngine;

namespace DeckBuilder.Cards;

/// <summary>
/// Harmony patches for Rare Search and Legendary Search deck-search flows.
/// </summary>
[HarmonyPatch]
public static class SearchCardPatches
{
    public static bool IsSearchFlowActive;

    private static bool _capAtRare;
    private static int _pickerID = -1;
    private static CardInfo _searchCard;
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
        if (card == null)
            return true;

        int pickerID = CardChoice.instance.pickrID;
        if (!IsLocalPicker(pickerID))
            return true;

        if (card.cardName == RareSearchCard.CardDisplayName)
        {
            SearchCardLog.Section("ApplyStats intercepted — Rare Search");
            StartFlow(pickerID, card, capAtRare: true);
            return false;
        }

        if (card.cardName == LegendarySearchCard.CardDisplayName)
        {
            SearchCardLog.Section("ApplyStats intercepted — Legendary Search");
            StartFlow(pickerID, card, capAtRare: false);
            return false;
        }

        return true;
    }

    [HarmonyPatch(typeof(CardChoice), "RPCA_DoEndPick")]
    [HarmonyPrefix]
    static bool RPCA_DoEndPick_Prefix()
    {
        if (!IsSearchFlowActive)
            return true;

        SearchCardLog.Line("RPCA_DoEndPick suppressed (search modal open).");
        return false;
    }

    [HarmonyPatch(typeof(CardBarHandler), "AddCard")]
    [HarmonyPrefix]
    static bool CardBarHandler_AddCard_Prefix(CardInfo card)
    {
        if (!IsSearchFlowActive || card == null)
            return true;

        if (card.cardName == RareSearchCard.CardDisplayName
            || card.cardName == LegendarySearchCard.CardDisplayName)
        {
            SearchCardLog.Line($"Blocked card-bar add for '{card.cardName}'.");
            return false;
        }

        return true;
    }

    private static void StartFlow(int pickerID, CardInfo searchCard, bool capAtRare)
    {
        _capAtRare = capAtRare;
        _pickerID = pickerID;
        _searchCard = searchCard;
        _pickerPlayer = PlayerManager.instance?.players?.Find(p => p.playerID == pickerID);

        var pool = CardSearchPool.BuildSearchableCards(pickerID, capAtRare, searchCard?.cardName);
        string title = capAtRare ? RareSearchCard.CardDisplayName : LegendarySearchCard.CardDisplayName;
        string message = capAtRare
            ? "Select a Rare or lower card from your deck."
            : "Select any card from your deck.";

        IsSearchFlowActive = true;
        SearchCardLog.Line($"Opening search modal with {pool.Count} card(s).");

        CardSearchModalUI.instance?.Show(pickerID, title, message, pool,
            onConfirm: OnConfirm,
            onCancel: OnCancel);
    }

    private static void OnConfirm(CardInfo selectedCard)
    {
        SearchCardLog.Section("Confirm");
        SearchCardLog.Line($"selectedCard='{selectedCard?.cardName ?? "null"}'");
        IsSearchFlowActive = false;

        if (_pickerPlayer != null && selectedCard != null)
        {
            try
            {
                SpecialCardPatches.SuppressApplyStatsPostfix = true;
                ModdingUtils.Utils.Cards.instance.AddCardToPlayer(_pickerPlayer, selectedCard,
                    reassign: true, twoLetterCode: "", forceDisplay: 0f, forceDisplayDelay: 0f, addToCardBar: true);
                SpecialCardPatches.SuppressApplyStatsPostfix = false;
                SearchCardLog.Line($"Added '{selectedCard.cardName}' to player {_pickerID}.");
            }
            catch (Exception ex)
            {
                SpecialCardPatches.SuppressApplyStatsPostfix = false;
                SearchCardLog.Error($"AddCardToPlayer failed: {ex}");
            }

            ConsumeSelectedCardFromDeck(selectedCard);
        }

        ConsumeSearchCard();
        SearchCardLog.Line("Ending pick phase.");
        CardDeleteManager.EndPickPhase();
        BroadcastRemainingCount();
    }

    private static void OnCancel()
    {
        SearchCardLog.Section("Cancel");
        IsSearchFlowActive = false;

        CardChoice cc = CardChoice.instance;
        if (cc == null)
        {
            SearchCardLog.Warn("CardChoice.instance is null — cannot restore state.");
            return;
        }

        cc.StopAllCoroutines();
        cc.pickrID = _pickerID;

        if (s_isPlayingField != null)
            s_isPlayingField.SetValue(cc, false);

        cc.IsPicking = true;

        if (_searchCard != null)
        {
            var applyStats = _searchCard.GetComponentInChildren<ApplyCardStats>();
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

        SearchCardLog.Line("Search modal closed — resuming current draft hand.");
    }

    private static void ConsumeSearchCard()
    {
        if (_searchCard == null)
            return;

        int before = DeckManager.GetRemainingCount(_pickerID);
        DeckManager.ConsumeCard(_pickerID, _searchCard);
        int after = DeckManager.GetRemainingCount(_pickerID);
        SearchCardLog.Line($"Consumed search card '{_searchCard.cardName}'. Deck: {before} -> {after}");
    }

    private static void ConsumeSelectedCardFromDeck(CardInfo selectedCard)
    {
        if (selectedCard == null)
            return;

        if (!DeckManager.RuntimeDecks.TryGetValue(_pickerID, out var deck) || deck == null)
            return;

        for (int i = 0; i < deck.Count; i++)
        {
            var card = deck[i];
            if (card == null)
                continue;

            if (!string.Equals(card.cardName, selectedCard.cardName, StringComparison.OrdinalIgnoreCase))
                continue;

            deck.RemoveAt(i);
            SearchCardLog.Line($"Removed '{selectedCard.cardName}' from runtime deck.");
            return;
        }

        SearchCardLog.Warn($"Selected card '{selectedCard.cardName}' not found in runtime deck.");
    }

    private static void BroadcastRemainingCount()
    {
        int remaining = DeckManager.GetRemainingCount(_pickerID);
        if (remaining >= 0)
            DeckNetworkSync.BroadcastRemainingCount(_pickerID, remaining);
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
