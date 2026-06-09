using System;
using System.Collections.Generic;
using InfoOverhaul.Delta;
using ShieldsMod.Lifesteal;

namespace ShieldsMod.Cards;

internal static class LifestealCardDeltaRegistrar
{
    internal static void RegisterAll()
    {
        SLog.Section("LifestealCardDeltaRegistrar — RegisterAll");

        RegisterTier(LifestealCardRegistry.Lv1, 0.25f);
        RegisterTier(LifestealCardRegistry.Lv2, 0.50f);
        RegisterTier(LifestealCardRegistry.Lv3, 0.75f);
    }

    private static void RegisterTier(CardInfo card, float totalReduction)
    {
        if (card == null)
            return;

        LifestealHandRebuild.RegisterTier(card.cardName, totalReduction);
        CardDeltaRegistry.RegisterSimple(card.cardName, (player, _) => ComputeDelta(player, totalReduction));
        SLog.Line($"Registered lifesteal delta preview for '{card.cardName}' ({totalReduction * 100f:F0}%).");
    }

    private static IReadOnlyList<(string label, string before, string after)> ComputeDelta(
        Player player, float tierReduction)
    {
        if (player == null || LifestealResistanceManager.instance == null)
            return Array.Empty<(string, string, string)>();

        float before = LifestealResistanceManager.instance.GetReductionPercent(player.playerID);
        float after = tierReduction * 100f;

        return new[]
        {
            ("Lifesteal Resistance", $"{before:F0}%", $"{after:F0}%")
        };
    }
}
