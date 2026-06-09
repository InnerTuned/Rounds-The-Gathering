using InfoOverhaul.Delta;
using ShieldsMod.Poison;

namespace ShieldsMod.Cards;

internal static class PoisonCardDeltaRegistrar
{
    internal static void RegisterAll()
    {
        SLog.Section("PoisonCardDeltaRegistrar — RegisterAll");

        RegisterTier(PoisonCardRegistry.Lv1, 1);
        RegisterTier(PoisonCardRegistry.Lv2, 2);
        RegisterTier(PoisonCardRegistry.Lv3, 3);
    }

    private static void RegisterTier(CardInfo card, int level)
    {
        if (card == null)
            return;

        PoisonHandRebuild.RegisterTier(card.cardName, level);
        CardDeltaRegistry.RegisterSimple(card.cardName, (player, _) =>
            ResistanceDeltaHelper.ComputeAddDelta(
                player, level, "Poison Resistance",
                id => PoisonResistanceManager.instance.GetReductionPercent(id)));
        CardDeltaRegistry.RegisterRemovalSimple(card.cardName, (player, cardInfo) =>
            ResistanceDeltaHelper.ComputeRemovalDelta(
                player, cardInfo, "Poison Resistance",
                id => PoisonResistanceManager.instance.GetReductionPercent(id),
                cards => PoisonHandRebuild.ComputeReductionFromHand(cards)));
        SLog.Line($"Registered poison delta preview for '{card.cardName}' (LV{level}).");
    }
}
