using UnboundLib.Cards;
using UnityEngine;

namespace DeckBuilder.Cards;

/// <summary>
/// Search your deck for a Rare-or-lower card and add it to your hand.
/// </summary>
public class RareSearchCard : CustomCard
{
    public const string CardDisplayName = "Rare Search";

    protected override string GetTitle() => CardDisplayName;

    protected override string GetDescription() =>
        "Search your deck for a Rare or lower card and add it to your hand. Rare Search is consumed on confirm.";

    protected override CardInfoStat[] GetStats() => System.Array.Empty<CardInfoStat>();

    protected override CardInfo.Rarity GetRarity() => CardInfo.Rarity.Rare;

    protected override GameObject GetCardArt() => CardArtLoader.Load("RareSearch.png");

    protected override CardThemeColor.CardThemeColorType GetTheme() =>
        CardThemeColor.CardThemeColorType.DefensiveBlue;

    public override string GetModName() => "DeckBuilder";

    public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data,
        HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
    {
        // Handled by SearchCardPatches.
    }
}
