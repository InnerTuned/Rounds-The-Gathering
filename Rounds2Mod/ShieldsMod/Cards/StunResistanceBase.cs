using UnboundLib.Cards;
using UnityEngine;
using ShieldsMod.Stun;

namespace ShieldsMod.Cards;

public abstract class StunResistanceBase : CustomCard
{
    protected abstract int Level { get; }
    protected abstract float TotalReduction { get; }
    protected abstract string ArtFileName { get; }
    protected abstract CardInfo.Rarity CardRarity { get; }

    public CardInfo NextLevelCard;

    protected override string GetTitle() => $"Stun Resistance {Roman(Level)}";

    protected override string GetDescription() =>
        $"Decrease the duration of Stun on you by {PercentLabel(TotalReduction)}.";

    protected override CardInfoStat[] GetStats() => new[]
    {
        new CardInfoStat
        {
            positive = true,
            amount = PercentLabel(TotalReduction),
            stat = "Stun Resist",
            simepleAmount = CardInfoStat.SimpleAmount.notAssigned
        }
    };

    protected override CardInfo.Rarity GetRarity() => CardRarity;

    protected override GameObject GetCardArt() => CardArtLoader.Load(ArtFileName);

    protected override CardThemeColor.CardThemeColorType GetTheme() =>
        CardThemeColor.CardThemeColorType.ColdBlue;

    public override string GetModName() => "ShieldsMod";

    public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats,
        CharacterStatModifiers statModifiers, Block block)
    {
        cardInfo.allowMultiple = false;
    }

    public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data,
        HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
    {
        if (player == null)
            return;

        StunResistanceManager.instance?.RebuildForPlayer(player.playerID);

        SLog.Section($"StunResistance LV{Level} — OnAddCard");
        SLog.Line($"player={player.playerID} totalReduction={TotalReduction * 100f:F0}%");

        if (NextLevelCard != null)
            SLog.Line($"Next tier '{NextLevelCard.cardName}' becomes draftable now that LV{Level} is owned.");
    }

    private static string PercentLabel(float reduction) => $"{Mathf.RoundToInt(reduction * 100f)}%";

    private static string Roman(int level) => level switch
    {
        1 => "I",
        2 => "II",
        3 => "III",
        _ => level.ToString()
    };
}
