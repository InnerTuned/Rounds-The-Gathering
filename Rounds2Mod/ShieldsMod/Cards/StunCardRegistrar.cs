using UnboundLib.Cards;

namespace ShieldsMod.Cards;

internal static class StunCardRegistrar
{
    internal static void RegisterAll()
    {
        SLog.Section("StunCardRegistrar — RegisterAll");

        CustomCard.BuildCard<StunResistanceLv3>(ci => StunCardRegistry.Lv3 = ci);
        CustomCard.BuildCard<StunResistanceLv2>(ci => StunCardRegistry.Lv2 = ci);
        CustomCard.BuildCard<StunResistanceLv1>(OnLv1Built);
    }

    private static void OnLv1Built(CardInfo ci)
    {
        StunCardRegistry.Lv1 = ci;
        SLog.Line($"Registered Stun Resistance LV1: {ci.cardName}");

        StunCardRegistry.WireLevelChain();
        RegisterPrerequisites();
        StunCardDeltaRegistrar.RegisterAll();
    }

    private static void RegisterPrerequisites()
    {
        SLog.Section("StunCardRegistrar — RegisterPrerequisites");

        if (!RtgPrerequisiteBridge.IsAvailable)
        {
            SLog.Warn("DeckBuilder not available — stun cards will NOT be unlock-gated.");
            return;
        }

        TryRegister(StunCardRegistry.Lv2, StunCardRegistry.Lv1);
        TryRegister(StunCardRegistry.Lv3, StunCardRegistry.Lv2);
    }

    private static void TryRegister(CardInfo card, CardInfo required)
    {
        if (card == null || required == null)
            return;

        bool ok = RtgPrerequisiteBridge.Register(card.cardName, required.cardName);
        SLog.Line($"Prereq: \"{card.cardName}\" requires \"{required.cardName}\" — registered={ok}");
    }
}
