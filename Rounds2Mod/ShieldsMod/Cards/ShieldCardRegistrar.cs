using UnboundLib.Cards;

namespace ShieldsMod.Cards;

internal static class ShieldCardRegistrar
{
    internal static void RegisterAll()
    {
        SLog.Section("ShieldCardRegistrar — RegisterAll");

        CustomCard.BuildCard<UpgradeShieldLv5>(ci => ShieldCardRegistry.Lv5 = ci);
        CustomCard.BuildCard<UpgradeShieldLv4>(ci => ShieldCardRegistry.Lv4 = ci);
        CustomCard.BuildCard<UpgradeShieldLv3>(ci => ShieldCardRegistry.Lv3 = ci);
        CustomCard.BuildCard<UpgradeShieldLv2>(ci => ShieldCardRegistry.Lv2 = ci);
        CustomCard.BuildCard<UpgradeShieldLv1>(OnLv1Built);
    }

    private static void OnLv1Built(CardInfo ci)
    {
        ShieldCardRegistry.Lv1 = ci;
        SLog.Line($"Registered LV1: {ci.cardName}");
        ShieldCardDeltaRegistrar.RegisterAll();
    }
}
