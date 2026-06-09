using System;
using System.Collections.Generic;
using InfoOverhaul.Delta;
using ShieldsMod.Shield;

namespace ShieldsMod.Cards;

internal static class ShieldCardDeltaRegistrar
{
    internal static void RegisterAll()
    {
        SLog.Section("ShieldCardDeltaRegistrar — RegisterAll");

        RegisterTier(ShieldCardRegistry.Lv1, 1.10f);
        RegisterTier(ShieldCardRegistry.Lv2, 1.25f);
        RegisterTier(ShieldCardRegistry.Lv3, 1.50f);
        RegisterTier(ShieldCardRegistry.Lv4, 2.00f);
        RegisterTier(ShieldCardRegistry.Lv5, 2.65f);
    }

    private static void RegisterTier(CardInfo card, float multiplier)
    {
        if (card == null)
        {
            SLog.Warn("ShieldCardDeltaRegistrar — skipped null card reference.");
            return;
        }

        ShieldHandRebuild.RegisterTier(card, multiplier);
        CardDeltaRegistry.RegisterSimple(card.cardName, (player, _) => ComputeShieldDelta(player, multiplier));
        SLog.Line($"Registered stat-delta preview for '{card.cardName}' (x{multiplier:F2}).");
    }

    private static IReadOnlyList<(string label, string before, string after)> ComputeShieldDelta(
        Player player, float multiplier)
    {
        if (player == null || ShieldManager.instance == null)
            return Array.Empty<(string, string, string)>();

        ShieldState state = ShieldManager.instance.GetShield(player.playerID);
        float maxBefore = state.Max;
        float maxAfter = state.HasShield
            ? maxBefore * multiplier
            : ShieldState.DefaultMax * multiplier;

        return new[]
        {
            ("Shield Max", $"{maxBefore:F0}", $"{maxAfter:F0}")
        };
    }
}
