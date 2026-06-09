using UnboundLib.Cards;
using UnboundLib.Utils;
using UnityEngine;
using ShieldsMod.Shield;

namespace ShieldsMod.Cards;

public abstract class UpgradeShieldHealthBase : CustomCard
{
    protected abstract int Level { get; }
    protected abstract float Multiplier { get; }
    protected abstract string ArtFileName { get; }
    protected abstract CardInfo.Rarity CardRarity { get; }

    /// <summary>CardInfo for the next tier; assigned during BuildCard registration.</summary>
    public CardInfo NextLevelCard;

    protected override string GetTitle() => $"Upgrade Shield Health {Roman(Level)}";

    protected override string GetDescription() =>
        $"Increases max shield health by {PercentLabel(Multiplier)}. Multiplies with other shield upgrades.";

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
            state.Max = ShieldState.DefaultMax * Multiplier;
            state.Current = state.Max;
        }
        else
        {
            state.Max *= Multiplier;
            state.Current = Mathf.Min(state.Current, state.Max);
        }

        ShieldManager.instance.RefreshVisual(playerID);

        SLog.Section($"UpgradeShieldHealth LV{Level} — OnAddCard");
        SLog.Line($"player={playerID} multiplier={Multiplier:F2} max {before:F1} -> {state.Max:F1}");

        // No manual enable needed: the next tier unlocks automatically because RTG
        // re-evaluates prerequisites against the player's hand each pick, and this
        // card is now in their hand.
        if (NextLevelCard != null)
            SLog.Line($"Next tier '{NextLevelCard.cardName}' becomes draftable now that LV{Level} is owned.");
        else
            SLog.Line("This is the final tier (no next level).");
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
