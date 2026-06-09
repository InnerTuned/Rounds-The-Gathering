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

        LifestealCardRegistry.WireLevelChain();
        RegisterPrerequisites();
        LifestealCardDeltaRegistrar.RegisterAll();
    }

    private static void RegisterPrerequisites()
    {
        SLog.Section("LifestealCardRegistrar — RegisterPrerequisites");

        if (!RtgPrerequisiteBridge.IsAvailable)
        {
            SLog.Warn("DeckBuilder not available — lifesteal cards will NOT be unlock-gated.");
            return;
        }

        TryRegister(LifestealCardRegistry.Lv2, LifestealCardRegistry.Lv1);
        TryRegister(LifestealCardRegistry.Lv3, LifestealCardRegistry.Lv2);
    }

    private static void TryRegister(CardInfo card, CardInfo required)
    {
        if (card == null || required == null)
            return;

        bool ok = RtgPrerequisiteBridge.Register(card.cardName, required.cardName);
        SLog.Line($"Prereq: \"{card.cardName}\" requires \"{required.cardName}\" — registered={ok}");
    }
}
