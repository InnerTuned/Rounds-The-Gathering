using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Keybound.Compat;

/// <summary>Reflection bridge to DeckBuilder deck consumption (soft dependency).</summary>
internal static class DeckBuilderBridge
{
    private static Type _deckManagerType;
    private static MethodInfo _consumeCard;
    private static MethodInfo _getRemaining;
    private static Type _networkSyncType;
    private static MethodInfo _broadcastRemaining;
    private static Type _twoStepType;
    private static MethodInfo _registerExternal;
    private static MethodInfo _completeExternal;
    private static bool _resolved;

    internal static bool IsAvailable
    {
        get { Resolve(); return _consumeCard != null; }
    }

    /// <summary>True if DeckBuilder's two-step flow engine is present.</summary>
    internal static bool TwoStepAvailable
    {
        get { Resolve(); return _registerExternal != null && _completeExternal != null; }
    }

    /// <summary>
    /// Registers a keybound-card handler with DeckBuilder's TwoStepCardFlow so the
    /// shared ApplyStats patch launches the keybind flow for any keybound card.
    /// </summary>
    internal static bool RegisterTwoStep(Func<CardInfo, bool> predicate, Action<int, CardInfo> starter)
    {
        Resolve();
        if (_registerExternal == null)
        {
            KLog.Warn("DeckBuilder TwoStepCardFlow not available — keybound pick interception disabled.");
            return false;
        }

        try
        {
            _registerExternal.Invoke(null, new object[] { predicate, starter });
            KLog.Line("Registered keybound two-step handler with DeckBuilder.");
            return true;
        }
        catch (Exception ex)
        {
            KLog.Warn($"RegisterTwoStep failed: {ex.Message}");
            return false;
        }
    }

    /// <summary>Reports a keybind flow's outcome to DeckBuilder's TwoStepCardFlow resolver.</summary>
    internal static void CompleteTwoStep(int pickerID, CardInfo card, int outcomeCode)
    {
        Resolve();
        if (_completeExternal == null)
        {
            KLog.Warn("DeckBuilder TwoStepCardFlow.CompleteExternal not available.");
            return;
        }

        try
        {
            KLog.Line($"CompleteTwoStep invoking DeckBuilder: picker={pickerID}, card='{card?.cardName ?? "null"}', outcomeCode={outcomeCode}");
            _completeExternal.Invoke(null, new object[] { pickerID, card, outcomeCode });
            KLog.Line("CompleteTwoStep invoke returned.");
        }
        catch (Exception ex)
        {
            KLog.Warn($"CompleteTwoStep failed: {ex.Message}");
        }
    }

    internal static void ConsumeCard(int playerID, CardInfo card)
    {
        Resolve();
        if (_consumeCard == null || card == null) return;

        try
        {
            _consumeCard.Invoke(null, new object[] { playerID, card });
            KLog.Line($"Consumed '{card.cardName}' from runtime deck (player {playerID}).");
            BroadcastRemaining(playerID);
        }
        catch (Exception ex)
        {
            KLog.Warn($"DeckBuilder ConsumeCard failed: {ex.Message}");
        }
    }

    private static void BroadcastRemaining(int playerID)
    {
        if (_broadcastRemaining == null || _getRemaining == null) return;

        try
        {
            int remaining = (int)_getRemaining.Invoke(null, new object[] { playerID });
            if (remaining >= 0)
                _broadcastRemaining.Invoke(null, new object[] { playerID, remaining });
        }
        catch (Exception ex)
        {
            KLog.Warn($"DeckBuilder broadcast failed: {ex.Message}");
        }
    }

    private static void Resolve()
    {
        if (_resolved) return;
        _resolved = true;

        try
        {
            var asm = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "DeckBuilder");
            if (asm == null)
            {
                KLog.Line("DeckBuilder not installed — runtime deck consumption skipped.");
                return;
            }

            _deckManagerType = asm.GetType("DeckBuilder.Data.DeckManager");
            if (_deckManagerType == null) return;

            _consumeCard = _deckManagerType.GetMethod("ConsumeCard",
                BindingFlags.Public | BindingFlags.Static,
                null, new[] { typeof(int), typeof(CardInfo) }, null);
            _getRemaining = _deckManagerType.GetMethod("GetRemainingCount",
                BindingFlags.Public | BindingFlags.Static,
                null, new[] { typeof(int) }, null);

            _networkSyncType = asm.GetType("DeckBuilder.Networking.DeckNetworkSync");
            _broadcastRemaining = _networkSyncType?.GetMethod("BroadcastRemainingCount",
                BindingFlags.Public | BindingFlags.Static,
                null, new[] { typeof(int), typeof(int) }, null);

            _twoStepType = asm.GetType("DeckBuilder.GameIntegration.TwoStepCardFlow");
            if (_twoStepType != null)
            {
                _registerExternal = _twoStepType.GetMethod("RegisterExternal",
                    BindingFlags.Public | BindingFlags.Static,
                    null, new[] { typeof(object), typeof(object) }, null);
                _completeExternal = _twoStepType.GetMethod("CompleteExternal",
                    BindingFlags.Public | BindingFlags.Static,
                    null, new[] { typeof(int), typeof(CardInfo), typeof(int) }, null);
            }
        }
        catch (Exception ex)
        {
            KLog.Warn($"DeckBuilder bridge resolve failed: {ex.Message}");
        }
    }
}
