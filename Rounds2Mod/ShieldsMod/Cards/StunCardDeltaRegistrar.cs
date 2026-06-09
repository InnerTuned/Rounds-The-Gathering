using System;
using System.Collections.Generic;
using InfoOverhaul.Delta;
using ShieldsMod.Stun;

namespace ShieldsMod.Cards;

internal static class StunCardDeltaRegistrar
{
    internal static void RegisterAll()
    {
        SLog.Section("StunCardDeltaRegistrar — RegisterAll");

        RegisterTier(StunCardRegistry.Lv1, 0.25f);
        RegisterTier(StunCardRegistry.Lv2, 0.50f);
        RegisterTier(StunCardRegistry.Lv3, 0.75f);
    }

    private static void RegisterTier(CardInfo card, float totalReduction)
    {
        if (card == null)
            return;

        StunHandRebuild.RegisterTier(card.cardName, totalReduction);
        CardDeltaRegistry.RegisterSimple(card.cardName, (player, _) => ComputeDelta(player, totalReduction));
        SLog.Line($"Registered stun delta preview for '{card.cardName}' ({totalReduction * 100f:F0}%).");
    }

    private static IReadOnlyList<(string label, string before, string after)> ComputeDelta(
        Player player, float tierReduction)
    {
        if (player == null || StunResistanceManager.instance == null)
            return Array.Empty<(string, string, string)>();

        float before = StunResistanceManager.instance.GetReductionPercent(player.playerID);
        float after = tierReduction * 100f;

        return new[]
        {
            ("Stun Resistance", $"{before:F0}%", $"{after:F0}%")
        };
    }
}
