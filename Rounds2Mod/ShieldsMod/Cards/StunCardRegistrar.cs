using UnboundLib.Cards;

namespace ShieldsMod.Cards;

internal static class StunCardRegistrar
{
    internal static void RegisterAll()
    {
        SLog.Section("StunCardRegistrar — RegisterAll");

        CustomCard.BuildCard<StunResistanceLv3>(ci => RegisterLegacy(ci, 3));
        CustomCard.BuildCard<StunResistanceLv2>(ci => RegisterLegacy(ci, 2));
        CustomCard.BuildCard<StunResistanceLv1>(ci => RegisterLegacy(ci, 1));
    }

    private static void RegisterLegacy(CardInfo ci, int level)
    {
        switch (level)
        {
            case 1: StunCardRegistry.Lv1 = ci; break;
            case 2: StunCardRegistry.Lv2 = ci; break;
            case 3: StunCardRegistry.Lv3 = ci; break;
        }

        ResistanceDraftFilter.Exclude(ci.cardName);
        ResistanceCardCatalog.RegisterLegacyTier(ci.cardName, Resistance.ResistanceType.Stun, level);
        SLog.Line($"Registered legacy Stun Resistance LV{level}: {ci.cardName} (draft excluded)");

        if (level == 1)
            StunCardDeltaRegistrar.RegisterAll();
    }
}
