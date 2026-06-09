namespace ShieldsMod.Cards;

public class StunResistanceLv1 : StunResistanceBase
{
    protected override int Level => 1;
    protected override float TotalReduction => 0.25f;
    protected override string ArtFileName => "stun_resistance_lv1.png";
    protected override CardInfo.Rarity CardRarity => CardInfo.Rarity.Common;
}
