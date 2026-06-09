using System.Collections.Generic;
using System.Globalization;

namespace InfoOverhaul.Delta;

/// <summary>One stat change line for card-pick preview.</summary>
public readonly struct StatDeltaLine
{
    private const string IncreaseColor = "#66FF88";
    private const string DecreaseColor = "#FF6666";
    private const string NeutralColor = "#EBF0FF";

    /// <summary>
    /// Stats where a lower numeric value is better for the player.
    /// An increase in these values is shown in red; a decrease in green.
    /// </summary>
    private static readonly HashSet<string> LowerIsBetterStats = new HashSet<string>
    {
        "Block CD",
        "Player Size",
        "Attack SPD",
        "Reload Time",
        "Bullet Gravity",
    };

    public readonly string Label;
    public readonly string Before;
    public readonly string After;
    public readonly bool IsNote;

    public StatDeltaLine(string label, string before, string after, bool isNote = false)
    {
        Label = label;
        Before = before;
        After = after;
        IsNote = isNote;
    }

    /// <summary>Effect-only line for Tier-2 cards (stats + custom mechanics).</summary>
    public static StatDeltaLine Note(string text) => new StatDeltaLine(text, null, null, isNote: true);

    /// <summary>Swaps before/after for card-removal previews (add deltas shown in reverse).</summary>
    public StatDeltaLine Inverted =>
        IsNote ? this : new StatDeltaLine(Label, After, Before);

    public string Format() =>
        IsNote ? Label : $"{Label}: {Before} --> {After}";

    /// <summary>Rich-text format for TMP — after-value is green/red by numeric change, respecting per-stat polarity.</summary>
    public string FormatRichText()
    {
        if (IsNote)
            return Label;

        if (!TryParseStatValue(Before, out float beforeVal) || !TryParseStatValue(After, out float afterVal))
            return Format();

        bool lowerIsBetter = LowerIsBetterStats.Contains(Label);
        bool numericallyHigher = afterVal > beforeVal;
        bool isImprovement = lowerIsBetter ? !numericallyHigher : numericallyHigher;

        string afterColor = afterVal == beforeVal ? NeutralColor
            : isImprovement ? IncreaseColor
            : DecreaseColor;

        return $"{Label}: {Before} --> <color={afterColor}>{After}</color>";
    }

    private static bool TryParseStatValue(string text, out float value)
    {
        value = 0f;
        if (string.IsNullOrEmpty(text))
            return false;

        int end = 0;
        while (end < text.Length && (char.IsDigit(text[end]) || text[end] == '.' || text[end] == '-'))
            end++;

        if (end == 0)
            return false;

        return float.TryParse(text.Substring(0, end), NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }
}
