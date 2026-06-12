using System;
using System.Collections.Generic;
using ShieldsMod.Resistance;

namespace ShieldsMod.Lifesteal;

/// <summary>Rebuilds lifesteal resistance from cards in the player's hand.</summary>
internal static class LifestealHandRebuild
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
        ResistanceHandLogic.ComputeReductionFromHand(cards, ResistanceType.Lifesteal, exclude);

    internal static void RebuildForPlayer(int playerID)
    {
        LifestealResistanceManager.instance?.RebuildForPlayer(playerID);
    }
}
