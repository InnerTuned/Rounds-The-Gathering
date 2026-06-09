namespace ShieldsMod.Cards;

public class UpgradeShieldLv4 : UpgradeShieldHealthBase
{
    protected override int Level => 4;
    protected override float Multiplier => 2.00f;
    protected override string ArtFileName => "lv4_artwork.png";
    protected override CardInfo.Rarity CardRarity => CardInfo.Rarity.Uncommon;
}
