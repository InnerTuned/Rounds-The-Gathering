using InfoOverhaul.Delta;
using ShieldsMod.Resistance;
using UnboundLib.Cards;

namespace ShieldsMod.Cards;

internal static class UpgradeResistanceCardRegistrar
{
    internal static void RegisterAll()
    {
        SLog.Section("UpgradeResistanceCardRegistrar — RegisterAll");

        CustomCard.BuildCard<UpgradeResistanceCard>(OnUpgradeBuilt);
        CustomCard.BuildCard<PoisonResistanceMark>(OnPoisonMarkerBuilt);
        CustomCard.BuildCard<StunResistanceMark>(OnStunMarkerBuilt);
        CustomCard.BuildCard<LifestealResistanceMark>(OnLifestealMarkerBuilt);
    }

    private static void OnUpgradeBuilt(CardInfo ci)
    {
        ResistanceCardCatalog.UpgradeResistanceCard = ci;
        SLog.Line($"Registered Upgrade Resistance card: cardName='{ci.cardName}', gameObject.name='{ci.gameObject.name}'");

        CardDeltaRegistry.Register(ci.cardName, (_, _) => new[]
        {
            StatDeltaLine.Note("Opens a modal to choose Poison, Stun, or Lifesteal resistance. Stays in your deck after use.")
        });

        ResistanceDeckVisibility.LogExcludedCards();
    }

    private static void OnPoisonMarkerBuilt(CardInfo ci)
    {
        ResistanceCardCatalog.RegisterMarkerCard(ResistanceType.Poison, ci);
        ResistanceDraftFilter.Exclude(ci.cardName);
        RegisterMarkerDeltas(ci, ResistanceType.Poison);
        SLog.Line($"Registered poison marker '{ci.cardName}'.");
    }

    private static void OnStunMarkerBuilt(CardInfo ci)
    {
        ResistanceCardCatalog.RegisterMarkerCard(ResistanceType.Stun, ci);
        ResistanceDraftFilter.Exclude(ci.cardName);
        RegisterMarkerDeltas(ci, ResistanceType.Stun);
        SLog.Line($"Registered stun marker '{ci.cardName}'.");
    }

    private static void OnLifestealMarkerBuilt(CardInfo ci)
    {
        ResistanceCardCatalog.RegisterMarkerCard(ResistanceType.Lifesteal, ci);
        ResistanceDraftFilter.Exclude(ci.cardName);
        RegisterMarkerDeltas(ci, ResistanceType.Lifesteal);
        SLog.Line($"Registered lifesteal marker '{ci.cardName}'.");
    }

    private static void RegisterMarkerDeltas(CardInfo card, ResistanceType type)
    {
        if (card == null)
            return;

        string statLabel = ResistanceCardCatalog.GetStatLabel(type);
        CardDeltaRegistry.RegisterSimple(card.cardName, (player, addingCard) =>
            ResistanceDeltaHelper.ComputeTypeUpgradeDelta(player, type, addingCard));
        CardDeltaRegistry.RegisterRemovalSimple(card.cardName, (player, cardInfo) =>
            ResistanceDeltaHelper.ComputeRemovalDelta(
                player,
                cardInfo,
                statLabel,
                ResistanceCardCatalog.GetReductionPercent(type),
                ResistanceCardCatalog.GetComputeFromHand(type)));
    }
}
