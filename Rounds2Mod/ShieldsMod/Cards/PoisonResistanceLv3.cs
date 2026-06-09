namespace ShieldsMod.Cards;

public class PoisonResistanceLv3 : PoisonResistanceBase
{
    protected override int Level => 3;
    protected override string ArtFileName => "poison_resistance_lv3.png";
    protected override CardInfo.Rarity CardRarity => RarityHelper.Epic;
}
