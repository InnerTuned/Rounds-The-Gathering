using RarityLib.Utils;

namespace DeckBuilder.Data;

/// <summary>Per-rarity copy limits for custom decks (uses RarityLib rarity names).</summary>
internal static class RarityDeckLimits
{
    private const int MaxThree = 3;
    private const int MaxTwo = 2;
    private const int MaxOne = 1;

    public static int MaxCountFor(CardInfo card)
    {
        if (card == null)
            return MaxOne;

        string name = RarityUtils.GetRarityData(card.rarity).name;
        switch (name)
        {
            case "Trinket":
            case "Common":
            case "Scarce":
                return MaxThree;
            case "Uncommon":
            case "Rare":
            case "Exotic":
                return MaxTwo;
            default:
                return MaxOne;
        }
    }
}
