using System;
using System.Collections.Generic;
using System.Linq;
using ShieldsMod.Resistance;
using UnityEngine;

namespace ShieldsMod.Cards;

internal static class ResistanceDeltaHelper
{
    internal static IReadOnlyList<(string label, string before, string after)> ComputeAddDelta(
        Player player,
        int level,
        string statLabel,
        Func<int, float> getReductionPercent)
    {
        if (player == null || getReductionPercent == null)
            return Array.Empty<(string, string, string)>();

        float beforePct = getReductionPercent(player.playerID);
        float before = beforePct / 100f;
        float afterPct = ResistanceTierLogic.ApplyCard(before, level, out float? healthMult) * 100f;

        var lines = new List<(string, string, string)>
        {
            (statLabel, $"{beforePct:F0}%", $"{afterPct:F0}%")
        };

        if (healthMult.HasValue && player.data != null)
        {
            float hpBefore = player.data.maxHealth;
            float hpAfter = hpBefore * healthMult.Value;
            lines.Add(("HP", $"{hpBefore:F0}", $"{hpAfter:F0}"));
        }

        return lines;
    }

    internal static IReadOnlyList<(string label, string before, string after)> ComputeRemovalDelta(
        Player player,
        CardInfo cardToRemove,
        string statLabel,
        Func<int, float> getReductionPercent,
        Func<IEnumerable<CardInfo>, float> computeFromHand)
    {
        if (player == null || cardToRemove == null || getReductionPercent == null || computeFromHand == null)
            return Array.Empty<(string, string, string)>();

        float before = getReductionPercent(player.playerID);
        var survivors = player.data?.currentCards?
            .Where(c => c != null && c.name != cardToRemove.name) ?? Enumerable.Empty<CardInfo>();
        float after = computeFromHand(survivors) * 100f;

        if (Mathf.Approximately(before, after))
            return Array.Empty<(string, string, string)>();

        return new[]
        {
            (statLabel, $"{before:F0}%", $"{after:F0}%")
        };
    }
}
