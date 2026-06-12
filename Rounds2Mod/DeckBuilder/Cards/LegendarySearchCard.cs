using UnboundLib.Cards;
using UnityEngine;

namespace DeckBuilder.Cards;

/// <summary>
/// Search your deck for any remaining card and add it to your hand.
/// </summary>
public class LegendarySearchCard : CustomCard
{
    public const string CardDisplayName = "Legendary Search";

    protected override string GetTitle() => CardDisplayName;

    protected override string GetDescription() =>
        "Search your deck for any card and add it to your hand. Legendary Search is consumed on confirm.";

    protected override CardInfoStat[] GetStats() => System.Array.Empty<CardInfoStat>();

    protected override CardInfo.Rarity GetRarity() => RarityHelper.Legendary;

    protected override GameObject GetCardArt() => CardArtLoader.Load("LegendarySearch.png");

    protected override CardThemeColor.CardThemeColorType GetTheme() =>
        CardThemeColor.CardThemeColorType.EvilPurple;

    public override string GetModName() => "DeckBuilder";

    public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data,
        HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
    {
        // Handled by SearchCardPatches.
    }
}
