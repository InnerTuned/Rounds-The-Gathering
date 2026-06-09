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

        PoisonCardRegistry.WireLevelChain();
        RegisterPrerequisites();
        PoisonCardDeltaRegistrar.RegisterAll();
    }

    private static void RegisterPrerequisites()
    {
        SLog.Section("PoisonCardRegistrar — RegisterPrerequisites");

        if (!RtgPrerequisiteBridge.IsAvailable)
        {
            SLog.Warn("DeckBuilder not available — poison cards will NOT be unlock-gated.");
            return;
        }

        TryRegister(PoisonCardRegistry.Lv2, PoisonCardRegistry.Lv1);
        TryRegister(PoisonCardRegistry.Lv3, PoisonCardRegistry.Lv2);
    }

    private static void TryRegister(CardInfo card, CardInfo required)
    {
        if (card == null || required == null)
            return;

        bool ok = RtgPrerequisiteBridge.Register(card.cardName, required.cardName);
        SLog.Line($"Prereq: \"{card.cardName}\" requires \"{required.cardName}\" — registered={ok}");
    }
}
