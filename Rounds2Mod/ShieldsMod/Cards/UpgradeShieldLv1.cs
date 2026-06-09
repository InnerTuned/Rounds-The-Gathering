namespace ShieldsMod.Cards;

public class UpgradeShieldLv1 : UpgradeShieldHealthBase
{
    protected override int Level => 1;
    protected override float Multiplier => 1.10f;
    protected override string ArtFileName => "lv1_artwork.png";
    protected override CardInfo.Rarity CardRarity => CardInfo.Rarity.Common;
}
