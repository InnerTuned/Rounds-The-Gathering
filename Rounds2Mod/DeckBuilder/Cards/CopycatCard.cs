using UnboundLib.Cards;
using UnityEngine;

namespace DeckBuilder.Cards
{
    /// <summary>
    /// When picked from the draft table: pause the pick phase, let the player select any
    /// card from their hand, add a free clone of it, then end the pick — without consuming
    /// a normal pick slot. Copycat itself is consumed from the runtime deck.
    /// </summary>
    public class CopycatCard : CustomCard
    {
        public const string CardDisplayName = "Copycat";

        protected override string GetTitle() => CardDisplayName;

        protected override string GetDescription() =>
            "Duplicate any card from any player's hand. Copycat is consumed on confirm; your deck pick is skipped.";

        protected override CardInfoStat[] GetStats() => System.Array.Empty<CardInfoStat>();

        protected override CardInfo.Rarity GetRarity() => CardInfo.Rarity.Rare;

        protected override GameObject GetCardArt() => CardArtLoader.Load("Copycat.png");

        protected override CardThemeColor.CardThemeColorType GetTheme() =>
            CardThemeColor.CardThemeColorType.EvilPurple;

        public override string GetModName() => "DeckBuilder";

        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data,
            HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            // Effect handled entirely by SpecialCardPatches.ApplyStats_Prefix,
            // which intercepts before ApplyStats runs and starts the Copycat flow.
            // OnAddCard fires anyway (Harmony postfixes always run) but is intentionally empty.
        }
    }
}
