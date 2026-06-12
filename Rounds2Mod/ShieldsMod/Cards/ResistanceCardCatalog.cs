using System;
using System.Collections.Generic;
using ShieldsMod.Lifesteal;
using ShieldsMod.Poison;
using ShieldsMod.Resistance;
using ShieldsMod.Stun;

namespace ShieldsMod.Cards;

/// <summary>Maps card names to resistance types and legacy tiers.</summary>
internal static class ResistanceCardCatalog
{
    private static readonly Dictionary<string, (ResistanceType type, int legacyTier)> LegacyTiers =
        new Dictionary<string, (ResistanceType, int)>(StringComparer.Ordinal);

    internal static CardInfo UpgradeResistanceCard;
    internal static CardInfo PoisonMarkerCard;
    internal static CardInfo StunMarkerCard;
    internal static CardInfo LifestealMarkerCard;

    internal static void RegisterLegacyTier(string cardName, ResistanceType type, int tier)
    {
        if (string.IsNullOrEmpty(cardName) || tier < 1 || tier > 3)
            return;

        LegacyTiers[cardName] = (type, tier);
        switch (type)
        {
            case ResistanceType.Poison:
                PoisonHandRebuild.RegisterTier(cardName, tier);
                break;
            case ResistanceType.Stun:
                StunHandRebuild.RegisterTier(cardName, tier);
                break;
            case ResistanceType.Lifesteal:
                LifestealHandRebuild.RegisterTier(cardName, tier);
                break;
        }
    }

    internal static void RegisterMarkerCard(ResistanceType type, CardInfo card)
    {
        switch (type)
        {
            case ResistanceType.Poison:
                PoisonMarkerCard = card;
                break;
            case ResistanceType.Stun:
                StunMarkerCard = card;
                break;
            case ResistanceType.Lifesteal:
                LifestealMarkerCard = card;
                break;
        }
    }

    internal static CardInfo GetMarkerCard(ResistanceType type) => type switch
    {
        ResistanceType.Poison => PoisonMarkerCard,
        ResistanceType.Stun => StunMarkerCard,
        ResistanceType.Lifesteal => LifestealMarkerCard,
        _ => null
    };

    internal static bool IsUpgradeCard(CardInfo card, ResistanceType type)
    {
        if (card == null)
            return false;

        string name = card.cardName;
        if (string.IsNullOrEmpty(name))
            return false;

        if (GetMarkerCard(type)?.cardName == name)
            return true;

        return LegacyTiers.TryGetValue(name, out var entry) && entry.type == type;
    }

    internal static bool TryGetLegacyTier(CardInfo card, ResistanceType type, out int tier)
    {
        tier = 0;
        if (card == null)
            return false;

        string name = card.cardName;
        if (string.IsNullOrEmpty(name))
            return false;

        if (!LegacyTiers.TryGetValue(name, out var entry) || entry.type != type)
            return false;

        tier = entry.legacyTier;
        return true;
    }

    internal static string GetDisplayName(ResistanceType type) => type switch
    {
        ResistanceType.Poison => "Poison Resistance",
        ResistanceType.Stun => "Stun Resistance",
        ResistanceType.Lifesteal => "Lifesteal Resistance",
        _ => "Resistance"
    };

    internal static string GetStatLabel(ResistanceType type) => type switch
    {
        ResistanceType.Poison => "Poison Resistance",
        ResistanceType.Stun => "Stun Resistance",
        ResistanceType.Lifesteal => "Lifesteal Resistance",
        _ => "Resistance"
    };

    internal static Func<int, float> GetReductionPercent(ResistanceType type) => type switch
    {
        ResistanceType.Poison => id => PoisonResistanceManager.instance.GetReductionPercent(id),
        ResistanceType.Stun => id => StunResistanceManager.instance.GetReductionPercent(id),
        ResistanceType.Lifesteal => id => LifestealResistanceManager.instance.GetReductionPercent(id),
        _ => _ => 0f
    };

    internal static Action<int> GetRebuildAction(ResistanceType type) => type switch
    {
        ResistanceType.Poison => id => PoisonHandRebuild.RebuildForPlayer(id),
        ResistanceType.Stun => id => StunHandRebuild.RebuildForPlayer(id),
        ResistanceType.Lifesteal => id => LifestealHandRebuild.RebuildForPlayer(id),
        _ => _ => { }
    };

    internal static Func<IEnumerable<CardInfo>, float> GetComputeFromHand(ResistanceType type) => type switch
    {
        ResistanceType.Poison => cards => ResistanceHandLogic.ComputeReductionFromHand(cards, ResistanceType.Poison),
        ResistanceType.Stun => cards => ResistanceHandLogic.ComputeReductionFromHand(cards, ResistanceType.Stun),
        ResistanceType.Lifesteal => cards => ResistanceHandLogic.ComputeReductionFromHand(cards, ResistanceType.Lifesteal),
        _ => _ => 0f
    };
}
