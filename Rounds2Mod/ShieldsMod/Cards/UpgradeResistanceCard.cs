using UnboundLib.Cards;
using UnityEngine;

namespace ShieldsMod.Cards;

/// <summary>
/// Opens a modal to choose which resistance type to upgrade (poison, stun, lifesteal).
/// </summary>
public class UpgradeResistanceCard : CustomCard
{
    public const string CardDisplayName = "Upgrade Resistance";

    protected override string GetTitle() => CardDisplayName;

    protected override string GetDescription() =>
        "Choose a resistance type to upgrade. Each pick follows the usual tier and stacking rules. Returns to your deck after use.";

    protected override CardInfoStat[] GetStats() => new[]
    {
        new CardInfoStat
        {
            positive = true,
            amount = "Poison / Stun / Lifesteal",
            stat = "Resistance",
            simepleAmount = CardInfoStat.SimpleAmount.notAssigned
        }
    };

    protected override CardInfo.Rarity GetRarity() => CardInfo.Rarity.Uncommon;

    protected override GameObject GetCardArt() => CardArtLoader.Load("poison_resistance_lv3.png");

    protected override CardThemeColor.CardThemeColorType GetTheme() =>
        CardThemeColor.CardThemeColorType.EvilPurple;

    public override string GetModName() => "ShieldsMod";

    public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats,
        CharacterStatModifiers statModifiers, Block block)
    {
        cardInfo.allowMultiple = false;
    }

    public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data,
        HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
    {
        // Handled by ResistanceCardPatches.
    }
}
