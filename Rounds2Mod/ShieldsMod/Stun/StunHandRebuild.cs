using System;
using System.Collections.Generic;
using ShieldsMod.Resistance;

namespace ShieldsMod.Stun;

internal static class StunHandRebuild
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
        ResistanceTierLogic.ComputeFromHand(cards, _levelByCardName, exclude);

    internal static void RebuildForPlayer(int playerID)
    {
        StunResistanceManager.instance?.RebuildForPlayer(playerID);
    }
}
