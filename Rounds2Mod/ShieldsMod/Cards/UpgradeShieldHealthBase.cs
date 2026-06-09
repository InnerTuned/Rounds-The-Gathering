using UnboundLib.Cards;
using UnboundLib.Utils;
using UnityEngine;
using ShieldsMod.Shield;

namespace ShieldsMod.Cards;

public abstract class UpgradeShieldHealthBase : CustomCard
{
    protected abstract int Level { get; }
    protected abstract float FirstShieldMax { get; }
    protected abstract float Multiplier { get; }
    protected abstract string ArtFileName { get; }
    protected abstract CardInfo.Rarity CardRarity { get; }

    protected override string GetTitle() => $"Upgrade Shield Health {Roman(Level)}";

    protected override string GetDescription() =>
        $"Grants or increases max shield health by {PercentLabel(Multiplier)}. First pickup sets a base shield if you have none.";

    protected override CardInfoStat[] GetStats() => new[]
    {
        new CardInfoStat
        {
            positive = true,
            amount = PercentLabel(Multiplier),
            stat = "Shield Max",
            simepleAmount = CardInfoStat.SimpleAmount.notAssigned
        }
    };

    protected override CardInfo.Rarity GetRarity() => CardRarity;

    protected override GameObject GetCardArt() => CardArtLoader.Load(ArtFileName);

    protected override CardThemeColor.CardThemeColorType GetTheme() =>
        CardThemeColor.CardThemeColorType.DefensiveBlue;

    public override string GetModName() => "ShieldsMod";

    public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats,
        CharacterStatModifiers statModifiers, Block block)
    {
        cardInfo.allowMultiple = false;
    }

    public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data,
        HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
    {
        if (player == null || ShieldManager.instance == null)
            return;

        int playerID = player.playerID;
        ShieldState state = ShieldManager.instance.GetOrCreateShield(playerID);
        float before = state.Max;

        if (!state.HasShield)
        {
            state.Max = FirstShieldMax;
            state.Current = state.Max;
        }
        else
        {
            state.Max *= Multiplier;
            state.Current = Mathf.Min(state.Current, state.Max);
        }

        ShieldManager.instance.RefreshVisual(playerID);

        SLog.Section($"UpgradeShieldHealth LV{Level} — OnAddCard");
        SLog.Line($"player={playerID} firstMax={FirstShieldMax:F0} multiplier={Multiplier:F2} max {before:F1} -> {state.Max:F1}");
    }

    private static string PercentLabel(float multiplier) =>
        $"+{Mathf.RoundToInt((multiplier - 1f) * 100f)}%";

    private static string Roman(int level) => level switch
    {
        1 => "I",
        2 => "II",
        3 => "III",
        4 => "IV",
        5 => "V",
        _ => level.ToString()
    };
}
