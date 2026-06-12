using System;
using DeckBuilder.Data;
using DeckBuilder.GameIntegration;
using UnboundLib;

namespace DeckBuilder.Cards;

/// <summary>
/// Handler for the Rare Search and Legendary Search two-step flows. Registers itself
/// with <see cref="TwoStepCardFlow"/>; all Harmony plumbing lives in TwoStepCardPatches.
/// </summary>
public static class SearchCardPatches
{
    private static bool _capAtRare;
    private static int _pickerID = -1;
    private static CardInfo _searchCard;
    private static CardInfo _pendingKeyboundCard;
    private static Player _pickerPlayer;

    /// <summary>Registers the Rare/Legendary search cards as two-step cards. Call once at startup.</summary>
    public static void RegisterFlows()
    {
        TwoStepCardFlow.Register(RareSearchCard.CardDisplayName,
            (pickerID, card) => StartFlow(pickerID, card, capAtRare: true));
        TwoStepCardFlow.Register(LegendarySearchCard.CardDisplayName,
            (pickerID, card) => StartFlow(pickerID, card, capAtRare: false));
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

        SearchCardLog.Line($"Opening search modal with {pool.Count} card(s).");

        CardSearchModalUI.instance?.Show(pickerID, title, message, pool,
            onConfirm: OnConfirm,
            onCancel: OnCancel);
    }

    private static void OnConfirm(CardInfo selectedCard)
    {
        SearchCardLog.Section("Confirm");
        SearchCardLog.Line($"selectedCard='{selectedCard?.cardName ?? "null"}'");

        bool selectedIsKeybound = selectedCard != null && KeyboundCardBridge.IsKeybound(selectedCard);

        if (_pickerPlayer != null && selectedCard != null)
        {
            if (selectedIsKeybound)
            {
                // Nested two-step: defer consumption until keybind is confirmed.
                // Cancelling keybind restores the draft hand (Rare Search still in hand).
                SearchCardLog.Line($"Selected card '{selectedCard.cardName}' is keybound — chaining keybind step (consumption deferred).");
                _pendingKeyboundCard = selectedCard;

                if (!KeyboundCardBridge.TryStartKeybindFromSearch(
                        _pickerID, selectedCard,
                        onBound: _ => FinalizeKeyboundPickFromSearch(),
                        onCancel: OnKeybindCancelledFromSearch))
                {
                    SearchCardLog.Warn("Keybound unavailable — restoring draft hand.");
                    _pendingKeyboundCard = null;
                    TwoStepCardFlow.Complete(_pickerID, _searchCard, TwoStepOutcome.Cancelled);
                }
                return;
            }

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
        TwoStepCardFlow.Complete(_pickerID, _searchCard, TwoStepOutcome.ActionAccepted);
    }

    private static void OnCancel()
    {
        SearchCardLog.Section("Cancel");
        _pendingKeyboundCard = null;
        SearchCardLog.Line($"Reporting Cancelled to TwoStepCardFlow (picker={_pickerID}, searchCard='{_searchCard?.cardName ?? "null"}').");
        TwoStepCardFlow.Complete(_pickerID, _searchCard, TwoStepOutcome.Cancelled);
    }

    private static void FinalizeKeyboundPickFromSearch()
    {
        SearchCardLog.Section("Keybind confirmed from search — finalizing pick");
        if (_pendingKeyboundCard != null)
            ConsumeSelectedCardFromDeck(_pendingKeyboundCard);
        _pendingKeyboundCard = null;
        ConsumeSearchCard();
        TwoStepCardFlow.Complete(_pickerID, _searchCard, TwoStepOutcome.ActionAccepted);
    }

    private static void OnKeybindCancelledFromSearch()
    {
        SearchCardLog.Section("Keybind cancelled from search — restoring draft");
        SearchCardLog.Line("No cards consumed — returning to draft hand.");
        _pendingKeyboundCard = null;
        TwoStepCardFlow.Complete(_pickerID, _searchCard, TwoStepOutcome.Cancelled);
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
}
