using RarityLib.Utils;

namespace DeckBuilder.Cards;

/// <summary>Resolves RarityBundle tier names after RarityLib has finalized.</summary>
internal static class RarityHelper
{
    public static CardInfo.Rarity Legendary => RarityUtils.GetRarity("Legendary");
}
