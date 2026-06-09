using Keybound.Effects;
using UnboundLib.Cards;
using UnityEngine;

namespace Keybound.Cards;

/// <summary>
/// [Keybound] Rare card — press the bound key to teleport to the cursor.
/// Available 10s after the round starts; 15s cooldown after each use.
/// </summary>
public class TeleportCard : CustomCard
{
    public const string CardDisplayName = TeleportEffect.CardName;

    protected override string GetTitle() => CardDisplayName;

    protected override string GetDescription() =>
        "[Keybound] Press your bound key to teleport to your cursor.\n" +
        "Becomes available 10s after the round starts. Cooldown: 15s.";

    protected override CardInfoStat[] GetStats() => System.Array.Empty<CardInfoStat>();

    protected override CardInfo.Rarity GetRarity() => CardInfo.Rarity.Rare;

    protected override GameObject GetCardArt() => CardArtLoader.Load("Teleport.png");

    protected override CardThemeColor.CardThemeColorType GetTheme() =>
        CardThemeColor.CardThemeColorType.DefensiveBlue;

    public override string GetModName() => "Keybound";

    public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data,
        HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
    {
        // Keybound cards are handled by KeyboundPickPatches — never applied via normal pick flow.
    }
}
