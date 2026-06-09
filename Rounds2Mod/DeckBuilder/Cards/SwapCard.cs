using UnboundLib.Cards;
using UnityEngine;

namespace DeckBuilder.Cards
{
    /// <summary>
    /// When picked from the draft table: pause the pick phase, let the player select any
    /// card from their hand to permanently delete it, then deal a fresh pick hand.
    /// Swap itself is consumed from the runtime deck.
    /// Replaces the old card-bar delete-button mechanic.
    /// </summary>
    public class SwapCard : CustomCard
    {
        public const string CardDisplayName = "Swap";

        protected override string GetTitle() => CardDisplayName;

        protected override string GetDescription() =>
            "Delete any card from your hand, then draw a new card to replace it.";

        protected override CardInfoStat[] GetStats() => System.Array.Empty<CardInfoStat>();

        protected override CardInfo.Rarity GetRarity() => CardInfo.Rarity.Uncommon;

        protected override GameObject GetCardArt() => CardArtLoader.Load("Swap.png");

        protected override CardThemeColor.CardThemeColorType GetTheme() =>
            CardThemeColor.CardThemeColorType.DefensiveBlue;

        public override string GetModName() => "DeckBuilder";

        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data,
            HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            // Effect handled entirely by SpecialCardPatches.ApplyStats_Prefix.
            // OnAddCard is intentionally empty.
        }
    }
}
