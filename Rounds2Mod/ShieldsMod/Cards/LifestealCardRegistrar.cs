using UnboundLib.Cards;

namespace ShieldsMod.Cards;

internal static class LifestealCardRegistrar
{
    internal static void RegisterAll()
    {
        SLog.Section("LifestealCardRegistrar — RegisterAll");

        CustomCard.BuildCard<LifestealResistanceLv3>(ci => RegisterLegacy(ci, 3));
        CustomCard.BuildCard<LifestealResistanceLv2>(ci => RegisterLegacy(ci, 2));
        CustomCard.BuildCard<LifestealResistanceLv1>(ci => RegisterLegacy(ci, 1));
    }

    private static void RegisterLegacy(CardInfo ci, int level)
    {
        switch (level)
        {
            case 1: LifestealCardRegistry.Lv1 = ci; break;
            case 2: LifestealCardRegistry.Lv2 = ci; break;
            case 3: LifestealCardRegistry.Lv3 = ci; break;
        }

        ResistanceDraftFilter.Exclude(ci.cardName);
        ResistanceCardCatalog.RegisterLegacyTier(ci.cardName, Resistance.ResistanceType.Lifesteal, level);
        SLog.Line($"Registered legacy Lifesteal Resistance LV{level}: {ci.cardName} (draft excluded)");

        if (level == 1)
            LifestealCardDeltaRegistrar.RegisterAll();
    }
}
