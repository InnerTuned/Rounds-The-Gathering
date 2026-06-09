namespace ShieldsMod.Cards;

public class UpgradeShieldLv3 : UpgradeShieldHealthBase
{
    protected override int Level => 3;
    protected override float FirstShieldMax => 250f;
    protected override float Multiplier => 2.00f;
    protected override string ArtFileName => "lv3_artwork.png";
    protected override CardInfo.Rarity CardRarity => CardInfo.Rarity.Uncommon;
}
