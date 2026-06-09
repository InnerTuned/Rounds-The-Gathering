namespace ShieldsMod.Cards;

internal static class ResistanceCardText
{
    internal static string Description(int level, string effectLabel) => level switch
    {
        1 => $"Sets {effectLabel} resistance to 25%, or +5% if you already have resistance.",
        2 => $"Sets {effectLabel} resistance to 50% if below 50%; otherwise +5% resistance and +10% max HP.",
        3 => $"Sets {effectLabel} resistance to 75% if below 75%; otherwise +5% resistance and +25% max HP.",
        _ => $"Improves {effectLabel} resistance."
    };

    internal static string StatAmount(int level) => level switch
    {
        1 => "25% / +5%",
        2 => "50% / +5%",
        3 => "75% / +5%",
        _ => "—"
    };
}
