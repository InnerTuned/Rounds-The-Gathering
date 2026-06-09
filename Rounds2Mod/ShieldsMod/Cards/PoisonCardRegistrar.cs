using UnboundLib.Cards;

namespace ShieldsMod.Cards;

internal static class PoisonCardRegistrar
{
    internal static void RegisterAll()
    {
        SLog.Section("PoisonCardRegistrar — RegisterAll");

        CustomCard.BuildCard<PoisonResistanceLv3>(ci => PoisonCardRegistry.Lv3 = ci);
        CustomCard.BuildCard<PoisonResistanceLv2>(ci => PoisonCardRegistry.Lv2 = ci);
        CustomCard.BuildCard<PoisonResistanceLv1>(OnLv1Built);
    }

    private static void OnLv1Built(CardInfo ci)
    {
        PoisonCardRegistry.Lv1 = ci;
        SLog.Line($"Registered Poison Resistance LV1: {ci.cardName}");
        PoisonCardDeltaRegistrar.RegisterAll();
    }
}
