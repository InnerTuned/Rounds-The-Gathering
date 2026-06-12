using System;
using System.Collections.Generic;

namespace DeckBuilder.Data;

/// <summary>
/// Cards registered here stay in the runtime deck after being picked (like Upgrade Resistance).
/// </summary>
public static class CyclicalCardRegistry
{
    private static readonly HashSet<string> Names = new(StringComparer.OrdinalIgnoreCase);

    public static void Register(string cardName)
    {
        if (!string.IsNullOrEmpty(cardName))
            Names.Add(cardName);
    }

    public static bool IsCyclical(string cardName) =>
        !string.IsNullOrEmpty(cardName) && Names.Contains(cardName);

    public static bool IsCyclical(CardInfo card) =>
        card != null && IsCyclical(card.cardName);
}
