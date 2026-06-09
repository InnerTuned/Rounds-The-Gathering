using InfoOverhaul.Delta;
using ShieldsMod.Stun;

namespace ShieldsMod.Cards;

internal static class StunCardDeltaRegistrar
{
    internal static void RegisterAll()
    {
        SLog.Section("StunCardDeltaRegistrar — RegisterAll");

        RegisterTier(StunCardRegistry.Lv1, 1);
        RegisterTier(StunCardRegistry.Lv2, 2);
        RegisterTier(StunCardRegistry.Lv3, 3);
    }

    private static void RegisterTier(CardInfo card, int level)
    {
        if (card == null)
            return;

        StunHandRebuild.RegisterTier(card.cardName, level);
        CardDeltaRegistry.RegisterSimple(card.cardName, (player, _) =>
            ResistanceDeltaHelper.ComputeAddDelta(
                player, level, "Stun Resistance",
                id => StunResistanceManager.instance.GetReductionPercent(id)));
        CardDeltaRegistry.RegisterRemovalSimple(card.cardName, (player, cardInfo) =>
            ResistanceDeltaHelper.ComputeRemovalDelta(
                player, cardInfo, "Stun Resistance",
                id => StunResistanceManager.instance.GetReductionPercent(id),
                cards => StunHandRebuild.ComputeReductionFromHand(cards)));
        SLog.Line($"Registered stun delta preview for '{card.cardName}' (LV{level}).");
    }
}
