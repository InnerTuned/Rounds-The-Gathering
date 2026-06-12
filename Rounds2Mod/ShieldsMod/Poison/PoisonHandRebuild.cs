using System;
using System.Collections.Generic;
using ShieldsMod.Resistance;

namespace ShieldsMod.Poison;

/// <summary>Rebuilds poison resistance from cards in the player's hand.</summary>
internal static class PoisonHandRebuild
{
    private static Dictionary<string, int> _levelByCardName;

    internal static void RegisterTier(string cardName, int level)
    {
        if (string.IsNullOrEmpty(cardName) || level < 1 || level > 3)
            return;

        _levelByCardName ??= new Dictionary<string, int>(StringComparer.Ordinal);
        _levelByCardName[cardName] = level;
    }

    internal static float ComputeReductionFromHand(IEnumerable<CardInfo> cards, CardInfo exclude = null) =>
        ResistanceHandLogic.ComputeReductionFromHand(cards, ResistanceType.Poison, exclude);

    internal static void RebuildForPlayer(int playerID)
    {
        PoisonResistanceManager.instance?.RebuildForPlayer(playerID);
    }
}
