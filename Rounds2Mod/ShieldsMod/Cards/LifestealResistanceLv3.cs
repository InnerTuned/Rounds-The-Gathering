namespace ShieldsMod.Cards;

public class LifestealResistanceLv3 : LifestealResistanceBase
{
    protected override int Level => 3;
    protected override float TotalReduction => 0.75f;
    protected override string ArtFileName => "lifesteal_resistance_lv3.png";
    protected override CardInfo.Rarity CardRarity => CardInfo.Rarity.Rare;
}
