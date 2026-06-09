using UnboundLib.Cards;
using UnityEngine;
using ShieldsMod.Lifesteal;

namespace ShieldsMod.Cards;

public abstract class LifestealResistanceBase : CustomCard
{
    protected abstract int Level { get; }
    protected abstract string ArtFileName { get; }
    protected abstract CardInfo.Rarity CardRarity { get; }

    protected override string GetTitle() => $"Lifesteal Resistance {Roman(Level)}";

    protected override string GetDescription() =>
        ResistanceCardText.Description(Level, "lifesteal");

    protected override CardInfoStat[] GetStats() => new[]
    {
        new CardInfoStat
        {
            positive = true,
            amount = ResistanceCardText.StatAmount(Level),
            stat = "Life Steal Resist",
            simepleAmount = CardInfoStat.SimpleAmount.notAssigned
        }
    };

    protected override CardInfo.Rarity GetRarity() => CardRarity;

    protected override GameObject GetCardArt() => CardArtLoader.Load(ArtFileName);

    protected override CardThemeColor.CardThemeColorType GetTheme() =>
        CardThemeColor.CardThemeColorType.EvilPurple;

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

        string cardName = cardInfo != null ? cardInfo.cardName : null;
        ResistanceCardOnAdd.Apply(
            player,
            data,
            Level,
            cardName,
            cards => LifestealHandRebuild.ComputeReductionFromHand(cards),
            id => LifestealResistanceManager.instance?.RebuildForPlayer(id));

        SLog.Section($"LifestealResistance LV{Level} — OnAddCard");
        SLog.Line($"player={player.playerID} reduction={LifestealResistanceManager.instance?.GetReductionPercent(player.playerID):F0}%");
    }

    private static string Roman(int level) => level switch
    {
        1 => "I",
        2 => "II",
        3 => "III",
        _ => level.ToString()
    };
}
