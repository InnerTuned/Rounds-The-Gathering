using System;
using System.Collections.Generic;

namespace ShieldsMod.Cards;

/// <summary>Cards excluded from draft pools (markers and legacy tier cards).</summary>
internal static class ResistanceDraftFilter
{
    private static readonly HashSet<string> ExcludedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    internal static void Exclude(string cardName)
    {
        if (!string.IsNullOrEmpty(cardName))
        {
            ExcludedNames.Add(cardName);
            SLog.Line($"ResistanceDraftFilter.Exclude: '{cardName}'");
        }
    }

    internal static bool IsExcluded(string cardName) =>
        !string.IsNullOrEmpty(cardName) && ExcludedNames.Contains(cardName);

    internal static void LogAll()
    {
        SLog.Line($"Total excluded: {ExcludedNames.Count}");
        foreach (var name in ExcludedNames)
            SLog.Line($"  - '{name}'");
    }
}
