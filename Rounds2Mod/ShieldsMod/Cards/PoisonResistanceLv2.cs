namespace ShieldsMod.Cards;

public class PoisonResistanceLv2 : PoisonResistanceBase
{
    protected override int Level => 2;
    protected override string ArtFileName => "poison_resistance_lv2.png";
    protected override CardInfo.Rarity CardRarity => CardInfo.Rarity.Rare;
}
