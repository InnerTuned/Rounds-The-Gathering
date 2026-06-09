namespace ShieldsMod.Cards;

public class UpgradeShieldLv3 : UpgradeShieldHealthBase
{
    protected override int Level => 3;
    protected override float Multiplier => 1.50f;
    protected override string ArtFileName => "lv3_artwork.png";
    protected override CardInfo.Rarity CardRarity => CardInfo.Rarity.Uncommon;
}
