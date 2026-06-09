namespace ShieldsMod.Cards;

public class PoisonResistanceLv1 : PoisonResistanceBase
{
    protected override int Level => 1;
    protected override float TotalReduction => 0.25f;
    protected override string ArtFileName => "poison_resistance_lv1.png";
    protected override CardInfo.Rarity CardRarity => CardInfo.Rarity.Common;
}
