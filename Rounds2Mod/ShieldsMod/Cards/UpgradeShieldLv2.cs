namespace ShieldsMod.Cards;

public class UpgradeShieldLv2 : UpgradeShieldHealthBase
{
    protected override int Level => 2;
    protected override float Multiplier => 1.25f;
    protected override string ArtFileName => "lv2_artwork.png";
    protected override CardInfo.Rarity CardRarity => CardInfo.Rarity.Common;
}
