namespace ShieldsMod.Cards;

public class UpgradeShieldLv5 : UpgradeShieldHealthBase
{
    protected override int Level => 5;
    protected override float Multiplier => 2.65f;
    protected override string ArtFileName => "lv5_artwork.png";
    protected override CardInfo.Rarity CardRarity => CardInfo.Rarity.Rare;
}
