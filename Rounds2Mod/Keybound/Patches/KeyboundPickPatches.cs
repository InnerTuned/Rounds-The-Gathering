using System;
using Keybound.Compat;
using Keybound.Core;
using Keybound.UI;

namespace Keybound.Patches;

/// <summary>
/// Handler for the keybound-card two-step flow. Registers a predicate-based handler with
/// DeckBuilder's TwoStepCardFlow at startup; the shared ApplyStats/RPCA_DoEndPick/AddCard
/// patches (owned by DeckBuilder) drive interception. Completion is reported back through
/// <see cref="DeckBuilderBridge"/>.
/// </summary>
internal static class KeyboundPickPatches
{
    // Outcome codes mirror DeckBuilder.GameIntegration.TwoStepOutcome.
    private const int OutcomeCancelled = 0;
    private const int OutcomeAccepted = 1;

    private static int _pickerID = -1;
    private static CardInfo _pendingCard;

    /// <summary>True when launched as a nested step from the search modal (callbacks owned by SearchCardPatches).</summary>
    private static bool _fromSearch;
    private static Action<int> _searchOnBound;
    private static Action _searchOnCancel;

    /// <summary>Registers keybound cards as two-step cards with DeckBuilder. Call once at startup.</summary>
    internal static void RegisterFlow()
    {
        Func<CardInfo, bool> predicate = c => c != null && KeyboundCardRegistry.IsKeybound(c.cardName);
        Action<int, CardInfo> starter = (pickerID, card) => StartBindFlow(pickerID, card, fromSearch: false);

        if (DeckBuilderBridge.RegisterTwoStep(predicate, starter))
            KLog.Line("Keybound two-step flow registered.");
    }

    /// <summary>
    /// Nested keybind step after the search modal picks a keybound card. The search flow
    /// defers consumption until onBound fires; onCancel restores the draft hand.
    /// </summary>
    public static void StartKeybindFromSearch(int pickerID, CardInfo card, Action<int> onBound, Action onCancel)
    {
        KLog.Section($"Keybind search sub-flow — '{card?.cardName}'");
        _searchOnBound = onBound;
        _searchOnCancel = onCancel;
        StartBindFlow(pickerID, card, fromSearch: true);
    }

    private static void StartBindFlow(int pickerID, CardInfo card, bool fromSearch)
    {
        _pickerID = pickerID;
        _pendingCard = card;
        _fromSearch = fromSearch;

        KLog.Line($"StartBindFlow: picker={pickerID}, card='{card?.cardName}', fromSearch={fromSearch}");
        KeybindModalUI.instance?.Show(pickerID, card,
            onConfirm: OnConfirm,
            onCancel: OnCancel);
    }

    private static void OnConfirm(int key)
    {
        KLog.Section($"Keybind confirmed — {_pendingCard?.cardName} -> {key}");

        if (_pendingCard != null)
            EffectStackManager.instance?.AddBinding(_pickerID, _pendingCard, key);

        if (_fromSearch)
        {
            KLog.Line("Search sub-flow confirm — delegating finalize to SearchCardPatches.");
            var cb = _searchOnBound;
            ClearSearchCallbacks();
            _pendingCard = null;
            cb?.Invoke(key);
        }
        else
        {
            DeckBuilderBridge.CompleteTwoStep(_pickerID, _pendingCard, OutcomeAccepted);
            _pendingCard = null;
        }
    }

    private static void OnCancel()
    {
        KLog.Section("Keybind cancelled");

        if (_fromSearch)
        {
            KLog.Line("Search sub-flow cancel — delegating restore to SearchCardPatches.");
            var cb = _searchOnCancel;
            ClearSearchCallbacks();
            _pendingCard = null;
            cb?.Invoke();
        }
        else
        {
            KLog.Line("Direct pick cancel — reporting Cancelled to TwoStepCardFlow.");
            DeckBuilderBridge.CompleteTwoStep(_pickerID, _pendingCard, OutcomeCancelled);
            _pendingCard = null;
        }
    }

    private static void ClearSearchCallbacks()
    {
        _fromSearch = false;
        _searchOnBound = null;
        _searchOnCancel = null;
    }
}
