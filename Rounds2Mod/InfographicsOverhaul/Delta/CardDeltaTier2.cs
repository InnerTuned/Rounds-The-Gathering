using System;
using System.Collections.Generic;
using System.Linq;

namespace InfoOverhaul.Delta;

/// <summary>
/// Helpers for Tier-2 card previews: standard stats (auto) plus custom effect notes.
/// Compat mods call these from plugin startup after the target card pack has loaded.
/// </summary>
public static class CardDeltaTier2
{
    /// <summary>Registers static effect note lines shown after computed stat deltas.</summary>
    public static void RegisterEffectNotes(string cardName, params string[] notes)
    {
        if (notes == null || notes.Length == 0)
            return;

        CardDeltaRegistry.RegisterAddon(cardName, (_, __) =>
            notes.Select(StatDeltaLine.Note).ToList());
    }

    /// <summary>Registers dynamic effect note lines shown after computed stat deltas.</summary>
    public static void RegisterEffectNotes(string cardName, Func<Player, CardInfo, IReadOnlyList<string>> notes)
    {
        if (notes == null)
            return;

        CardDeltaRegistry.RegisterAddon(cardName, (player, card) =>
            (notes(player, card) ?? Array.Empty<string>()).Select(StatDeltaLine.Note).ToList());
    }

    /// <summary>
    /// Registers a full custom stat provider that replaces any auto-registered one.
    /// Use when auto stats are wrong but you still want numeric deltas.
    /// </summary>
    public static void RegisterOverrideSimple(string cardName,
        CardDeltaRegistry.SimpleDeltaProvider provider) =>
        CardDeltaRegistry.RegisterSimple(cardName, provider);
}

