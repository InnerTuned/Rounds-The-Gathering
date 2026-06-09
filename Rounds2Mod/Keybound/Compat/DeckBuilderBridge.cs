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
    private static bool _resolved;

    internal static bool IsAvailable
    {
        get { Resolve(); return _consumeCard != null; }
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
        }
        catch (Exception ex)
        {
            KLog.Warn($"DeckBuilder bridge resolve failed: {ex.Message}");
        }
    }
}
