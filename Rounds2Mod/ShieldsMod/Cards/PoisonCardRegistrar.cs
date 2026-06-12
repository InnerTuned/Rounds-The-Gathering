using UnboundLib.Cards;

namespace ShieldsMod.Cards;

internal static class PoisonCardRegistrar
{
    internal static void RegisterAll()
    {
        SLog.Section("PoisonCardRegistrar — RegisterAll");

        CustomCard.BuildCard<PoisonResistanceLv3>(ci => RegisterLegacy(ci, 3));
        CustomCard.BuildCard<PoisonResistanceLv2>(ci => RegisterLegacy(ci, 2));
        CustomCard.BuildCard<PoisonResistanceLv1>(ci => RegisterLegacy(ci, 1));
    }

    private static void RegisterLegacy(CardInfo ci, int level)
    {
        switch (level)
        {
            case 1: PoisonCardRegistry.Lv1 = ci; break;
            case 2: PoisonCardRegistry.Lv2 = ci; break;
            case 3: PoisonCardRegistry.Lv3 = ci; break;
        }

        ResistanceDraftFilter.Exclude(ci.cardName);
        ResistanceCardCatalog.RegisterLegacyTier(ci.cardName, Resistance.ResistanceType.Poison, level);
        SLog.Line($"Registered legacy Poison Resistance LV{level}: {ci.cardName} (draft excluded)");

        if (level == 1)
            PoisonCardDeltaRegistrar.RegisterAll();
    }
}
