using System;
using System.Collections.Generic;
using InfoOverhaul.Delta;
using ShieldsMod.Shield;
using UnityEngine;

namespace ShieldsMod.Cards;

internal static class ShieldCardDeltaRegistrar
{
    internal static void RegisterAll()
    {
        SLog.Section("ShieldCardDeltaRegistrar — RegisterAll");

        RegisterTier(ShieldCardRegistry.Lv1, 100f, 1.20f);
        RegisterTier(ShieldCardRegistry.Lv2, 150f, 1.50f);
        RegisterTier(ShieldCardRegistry.Lv3, 250f, 2.00f);
        RegisterTier(ShieldCardRegistry.Lv4, 500f, 3.00f);
        RegisterTier(ShieldCardRegistry.Lv5, 1000f, 3.50f);
    }

    private static void RegisterTier(CardInfo card, float firstShieldMax, float stackMultiplier)
    {
        if (card == null)
        {
            SLog.Warn("ShieldCardDeltaRegistrar — skipped null card reference.");
            return;
        }

        ShieldHandRebuild.RegisterTier(card, firstShieldMax, stackMultiplier);
        CardDeltaRegistry.RegisterSimple(card.cardName, (player, _) =>
            ComputeShieldDelta(player, firstShieldMax, stackMultiplier));
        CardDeltaRegistry.RegisterRemovalSimple(card.cardName, (player, cardInfo) =>
            ComputeShieldRemoval(player, cardInfo));
        SLog.Line($"Registered stat-delta preview for '{card.cardName}' (first={firstShieldMax:F0}, x{stackMultiplier:F2}).");
    }

    private static IReadOnlyList<(string label, string before, string after)> ComputeShieldDelta(
        Player player, float firstShieldMax, float stackMultiplier)
    {
        if (player == null || ShieldManager.instance == null)
            return Array.Empty<(string, string, string)>();

        ShieldState state = ShieldManager.instance.GetShield(player.playerID);
        float maxBefore = state.Max;
        float maxAfter = state.HasShield
            ? maxBefore * stackMultiplier
            : firstShieldMax;

        return new[]
        {
            ("Shield Max", $"{maxBefore:F0}", $"{maxAfter:F0}")
        };
    }

    private static IReadOnlyList<(string label, string before, string after)> ComputeShieldRemoval(
        Player player, CardInfo cardToRemove)
    {
        if (player == null || ShieldManager.instance == null || cardToRemove == null)
            return Array.Empty<(string, string, string)>();

        ShieldState state = ShieldManager.instance.GetShield(player.playerID);
        float maxBefore = state.Max;
        float maxAfter = ShieldHandRebuild.ComputeMaxFromHandExcluding(
            player.data?.currentCards, cardToRemove);

        if (Mathf.Approximately(maxBefore, maxAfter))
            return Array.Empty<(string, string, string)>();

        string afterText = maxAfter > 0f ? $"{maxAfter:F0}" : "None";
        return new[]
        {
            ("Shield Health", $"{maxBefore:F0}", afterText)
        };
    }
}
