namespace ShieldsMod.Cards;

public class UpgradeShieldLv5 : UpgradeShieldHealthBase
{
    protected override int Level => 5;
    protected override float FirstShieldMax => 1000f;
    protected override float Multiplier => 3.50f;
    protected override string ArtFileName => "lv5_artwork.png";
    protected override CardInfo.Rarity CardRarity => RarityHelper.Legendary;
}
