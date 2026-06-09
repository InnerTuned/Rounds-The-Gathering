using InfoOverhaul.Delta;
using ShieldsMod.Lifesteal;

namespace ShieldsMod.Cards;

internal static class LifestealCardDeltaRegistrar
{
    internal static void RegisterAll()
    {
        SLog.Section("LifestealCardDeltaRegistrar — RegisterAll");

        RegisterTier(LifestealCardRegistry.Lv1, 1);
        RegisterTier(LifestealCardRegistry.Lv2, 2);
        RegisterTier(LifestealCardRegistry.Lv3, 3);
    }

    private static void RegisterTier(CardInfo card, int level)
    {
        if (card == null)
            return;

        LifestealHandRebuild.RegisterTier(card.cardName, level);
        CardDeltaRegistry.RegisterSimple(card.cardName, (player, _) =>
            ResistanceDeltaHelper.ComputeAddDelta(
                player, level, "Lifesteal Resistance",
                id => LifestealResistanceManager.instance.GetReductionPercent(id)));
        CardDeltaRegistry.RegisterRemovalSimple(card.cardName, (player, cardInfo) =>
            ResistanceDeltaHelper.ComputeRemovalDelta(
                player, cardInfo, "Lifesteal Resistance",
                id => LifestealResistanceManager.instance.GetReductionPercent(id),
                cards => LifestealHandRebuild.ComputeReductionFromHand(cards)));
        SLog.Line($"Registered lifesteal delta preview for '{card.cardName}' (LV{level}).");
    }
}
