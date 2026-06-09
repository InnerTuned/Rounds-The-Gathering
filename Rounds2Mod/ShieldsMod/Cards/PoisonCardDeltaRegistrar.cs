using System;
using System.Collections.Generic;
using InfoOverhaul.Delta;
using ShieldsMod.Poison;

namespace ShieldsMod.Cards;

internal static class PoisonCardDeltaRegistrar
{
    internal static void RegisterAll()
    {
        SLog.Section("PoisonCardDeltaRegistrar — RegisterAll");

        RegisterTier(PoisonCardRegistry.Lv1, 0.25f);
        RegisterTier(PoisonCardRegistry.Lv2, 0.50f);
        RegisterTier(PoisonCardRegistry.Lv3, 0.75f);
    }

    private static void RegisterTier(CardInfo card, float totalReduction)
    {
        if (card == null)
            return;

        PoisonHandRebuild.RegisterTier(card.cardName, totalReduction);
        CardDeltaRegistry.RegisterSimple(card.cardName, (player, _) => ComputeDelta(player, totalReduction));
        SLog.Line($"Registered poison delta preview for '{card.cardName}' ({totalReduction * 100f:F0}%).");
    }

    private static IReadOnlyList<(string label, string before, string after)> ComputeDelta(
        Player player, float tierReduction)
    {
        if (player == null || PoisonResistanceManager.instance == null)
            return Array.Empty<(string, string, string)>();

        float before = PoisonResistanceManager.instance.GetReductionPercent(player.playerID);
        float after = tierReduction * 100f;

        return new[]
        {
            ("Poison Resistance", $"{before:F0}%", $"{after:F0}%")
        };
    }
}
