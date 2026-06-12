using DeckBuilder.GameIntegration;

namespace DeckBuilder.Data;

/// <summary>
/// Central deck-builder and runtime-deck rules for special card categories.
/// </summary>
internal static class DeckCardRules
{
    private const int SingleInstanceMax = 1;

    /// <summary>
    /// Max copies allowed in a custom deck. Cyclical and Keybound cards are always 1.
    /// </summary>
    public static int MaxCountFor(CardInfo card)
    {
        if (card == null)
            return SingleInstanceMax;

        if (IsSingleInstance(card))
            return SingleInstanceMax;

        return RarityDeckLimits.MaxCountFor(card);
    }

    /// <summary>
    /// Whether a picked card should be removed from the runtime deck.
    /// Cyclical cards recycle back into the draft pool.
    /// </summary>
    public static bool ShouldConsumeOnPick(CardInfo card)
    {
        if (card == null)
            return true;

        if (CyclicalCardRegistry.IsCyclical(card))
            return false;

        if (KeyboundCardBridge.IsKeybound(card))
            return false;

        return true;
    }

    public static bool IsSingleInstance(CardInfo card) =>
        card != null && (CyclicalCardRegistry.IsCyclical(card) || KeyboundCardBridge.IsKeybound(card));
}
