using UnboundLib.Cards;

namespace ShieldsMod.Cards;

internal static class LifestealCardRegistrar
{
    internal static void RegisterAll()
    {
        SLog.Section("LifestealCardRegistrar — RegisterAll");

        CustomCard.BuildCard<LifestealResistanceLv3>(ci => LifestealCardRegistry.Lv3 = ci);
        CustomCard.BuildCard<LifestealResistanceLv2>(ci => LifestealCardRegistry.Lv2 = ci);
        CustomCard.BuildCard<LifestealResistanceLv1>(OnLv1Built);
    }

    private static void OnLv1Built(CardInfo ci)
    {
        LifestealCardRegistry.Lv1 = ci;
        SLog.Line($"Registered Lifesteal Resistance LV1: {ci.cardName}");
        LifestealCardDeltaRegistrar.RegisterAll();
    }
}
