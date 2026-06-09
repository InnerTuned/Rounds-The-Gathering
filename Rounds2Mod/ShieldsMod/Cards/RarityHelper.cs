using RarityLib.Utils;

namespace ShieldsMod.Cards;

/// <summary>Resolves RarityBundle tier names after RarityLib has finalized.</summary>
internal static class RarityHelper
{
    public static CardInfo.Rarity Scarce => RarityUtils.GetRarity("Scarce");
    public static CardInfo.Rarity Epic => RarityUtils.GetRarity("Epic");
    public static CardInfo.Rarity Legendary => RarityUtils.GetRarity("Legendary");
}
