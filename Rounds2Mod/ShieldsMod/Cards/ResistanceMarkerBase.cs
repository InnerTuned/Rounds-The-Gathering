using ShieldsMod.Resistance;
using UnboundLib.Cards;
using UnityEngine;

namespace ShieldsMod.Cards;

/// <summary>Internal hand marker applied when Upgrade Resistance confirms a type.</summary>
public abstract class ResistanceMarkerBase : CustomCard
{
    protected abstract ResistanceType Type { get; }

    protected override string GetTitle() => ResistanceCardCatalog.GetDisplayName(Type);

    protected override string GetDescription() =>
        $"Tracks your {ResistanceCardCatalog.GetDisplayName(Type).ToLower()} upgrades.";

    protected override CardInfoStat[] GetStats() => System.Array.Empty<CardInfoStat>();

    protected override CardInfo.Rarity GetRarity() => CardInfo.Rarity.Common;

    protected override GameObject GetCardArt() => null;

    protected override CardThemeColor.CardThemeColorType GetTheme() =>
        CardThemeColor.CardThemeColorType.DefensiveBlue;

    public override string GetModName() => "ShieldsMod";

    public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats,
        CharacterStatModifiers statModifiers, Block block)
    {
        cardInfo.allowMultiple = true;
    }

    public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data,
        HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
    {
        if (player == null)
            return;

        ResistanceCardOnAdd.ApplyForType(player, data, Type, cardInfo);
        SLog.Section($"{Type} marker — OnAddCard");
        SLog.Line($"player={player.playerID} handCount={player.data?.currentCards?.Count ?? 0}");
    }
}

public class PoisonResistanceMark : ResistanceMarkerBase
{
    protected override ResistanceType Type => ResistanceType.Poison;
}

public class StunResistanceMark : ResistanceMarkerBase
{
    protected override ResistanceType Type => ResistanceType.Stun;
}

public class LifestealResistanceMark : ResistanceMarkerBase
{
    protected override ResistanceType Type => ResistanceType.Lifesteal;
}
