namespace ShieldsMod.Cards;

public class UpgradeShieldLv2 : UpgradeShieldHealthBase
{
    protected override int Level => 2;
    protected override float FirstShieldMax => 150f;
    protected override float Multiplier => 1.50f;
    protected override string ArtFileName => "lv2_artwork.png";
    protected override CardInfo.Rarity CardRarity => RarityHelper.Scarce;
}
