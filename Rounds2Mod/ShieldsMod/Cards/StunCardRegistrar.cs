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
        StunCardDeltaRegistrar.RegisterAll();
    }
}
