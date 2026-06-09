using InfoOverhaul.Delta;
using UnboundLib.Utils;

namespace InfoOverhaul.Compat;

/// <summary>
/// Tier-2 previews for MFM cards: auto stat deltas (Tier 1) plus effect notes.
/// Add card names and notes here as we validate each card in-game.
/// </summary>
internal static class MfmTier2Compat
{
    internal static void Init()
    {
        CardManager.AddAllCardsCallback(_ => RegisterEffectNotes());
    }

    private static void RegisterEffectNotes()
    {
        IOLog.Section("MfmTier2Compat — RegisterEffectNotes");

        // Example — uncomment and verify in-game when testing MFM:
        // CardDeltaTier2.RegisterEffectNotes("Thunder Core", "+ Arc lightning on hit");
        // CardDeltaTier2.RegisterEffectNotes("Bounce Split", "+ Splits bullets on bounce");

        IOLog.Line("MfmTier2Compat ready (no notes registered yet — add per card as validated).");
    }
}

