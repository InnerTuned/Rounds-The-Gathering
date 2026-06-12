using Keybound.Effects;
using UnboundLib.Cards;
using UnityEngine;

namespace Keybound.Cards;

/// <summary>
/// [Keybound] Rare card — become briefly invisible; bullets pass through you and your shield.
/// </summary>
public class InvisibilityCard : CustomCard
{
    public const string CardDisplayName = InvisibilityEffect.CardName;

    protected override string GetTitle() => CardDisplayName;

    protected override string GetDescription() =>
        "[Keybound] Press your bound key to turn ghostly white and fade to 20% opacity.\n" +
        "Bullets pass through you and your shield while active.\n" +
        "Available 10s after the round starts. Lasts 7s. Cooldown: 20s.";

    protected override CardInfoStat[] GetStats() => System.Array.Empty<CardInfoStat>();

    protected override CardInfo.Rarity GetRarity() => CardInfo.Rarity.Rare;

    protected override GameObject GetCardArt() => CardArtLoader.Load("Invisibility.png");

    protected override CardThemeColor.CardThemeColorType GetTheme() =>
        CardThemeColor.CardThemeColorType.DefensiveBlue;

    public override string GetModName() => "Keybound";

    public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data,
        HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
    {
        // Handled by KeyboundPickPatches.
    }
}
