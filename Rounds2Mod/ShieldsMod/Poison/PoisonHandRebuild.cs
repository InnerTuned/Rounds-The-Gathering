using System;
using System.Collections.Generic;

namespace ShieldsMod.Poison;

/// <summary>Maps poison-resistance card names to total reduction tiers.</summary>
internal static class PoisonHandRebuild
{
    private static Dictionary<string, float> _reductionByCardName;

    internal static void RegisterTier(string cardName, float totalReduction)
    {
        if (string.IsNullOrEmpty(cardName))
            return;

        _reductionByCardName ??= new Dictionary<string, float>(StringComparer.Ordinal);
        _reductionByCardName[cardName] = totalReduction;
    }

    internal static float ComputeReductionFromHand(IEnumerable<CardInfo> cards)
    {
        float best = 0f;
        if (_reductionByCardName == null || cards == null)
            return best;

        foreach (CardInfo card in cards)
        {
            if (card == null)
                continue;

            string name = card.cardName;
            if (string.IsNullOrEmpty(name) && card.sourceCard != null)
                name = card.sourceCard.cardName;

            if (!string.IsNullOrEmpty(name) && _reductionByCardName.TryGetValue(name, out float reduction))
                best = Math.Max(best, reduction);
        }

        return best;
    }

    internal static void RebuildForPlayer(int playerID)
    {
        PoisonResistanceManager.instance?.RebuildForPlayer(playerID);
    }
}
