using System;
using System.Collections.Generic;
using ShieldsMod.Cards;

namespace ShieldsMod.Resistance;

/// <summary>
/// Counts resistance upgrades in hand order and computes reduction for each type.
/// Supports legacy tier cards (I/II/III) and new marker cards from Upgrade Resistance.
/// </summary>
internal static class ResistanceHandLogic
{
    internal static int GetTierForUpgradeIndex(int upgradeIndex) =>
        upgradeIndex switch
        {
            0 => 1,
            1 => 2,
            2 => 3,
            _ => 1
        };

    internal static int CountUpgrades(IEnumerable<CardInfo> cards, ResistanceType type, CardInfo exclude = null)
    {
        if (cards == null)
            return 0;

        int count = 0;
        foreach (CardInfo card in cards)
        {
            if (card == null || ReferenceEquals(card, exclude))
                continue;

            if (ResistanceCardCatalog.IsUpgradeCard(card, type))
                count++;
        }

        return count;
    }

    internal static float ComputeReductionFromHand(
        IEnumerable<CardInfo> cards,
        ResistanceType type,
        CardInfo exclude = null)
    {
        if (cards == null)
            return 0f;

        float reduction = 0f;
        bool established = false;
        int sequenceIndex = 0;

        foreach (CardInfo card in cards)
        {
            if (card == null || ReferenceEquals(card, exclude))
                continue;

            if (!TryGetTierForCard(card, type, sequenceIndex, out int tier))
                continue;

            if (!established)
            {
                reduction = ResistanceTierLogic.GetFirstReduction(tier);
                established = true;
            }
            else
            {
                reduction = ResistanceTierLogic.ApplyStacking(reduction, tier, out _);
            }

            sequenceIndex++;
        }

        return established ? ResistanceTierLogic.ClampReduction(reduction) : 0f;
    }

    internal static int GetNextUpgradeTier(IEnumerable<CardInfo> cards, ResistanceType type, CardInfo exclude = null)
    {
        int count = CountUpgrades(cards, type, exclude);
        return GetTierForUpgradeIndex(count);
    }

    private static bool TryGetTierForCard(
        CardInfo card,
        ResistanceType type,
        int markerSequenceIndex,
        out int tier)
    {
        tier = 0;
        if (!ResistanceCardCatalog.IsUpgradeCard(card, type))
            return false;

        if (ResistanceCardCatalog.TryGetLegacyTier(card, type, out tier))
            return true;

        tier = GetTierForUpgradeIndex(markerSequenceIndex);
        return true;
    }
}
