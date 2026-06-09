namespace ShieldsMod.Cards;

public class StunResistanceLv3 : StunResistanceBase
{
    protected override int Level => 3;
    protected override string ArtFileName => "stun_resistance_lv3.png";
    protected override CardInfo.Rarity CardRarity => RarityHelper.Epic;
}
