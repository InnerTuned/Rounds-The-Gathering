namespace ShieldsMod.Cards;

public class StunResistanceLv2 : StunResistanceBase
{
    protected override int Level => 2;
    protected override float TotalReduction => 0.50f;
    protected override string ArtFileName => "stun_resistance_lv2.png";
    protected override CardInfo.Rarity CardRarity => CardInfo.Rarity.Rare;
}
