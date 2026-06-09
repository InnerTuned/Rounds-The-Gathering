using UnboundLib.Cards;

namespace ShieldsMod.Cards;

internal static class ShieldCardRegistrar
{
    internal static void RegisterAll()
    {
        SLog.Section("ShieldCardRegistrar — RegisterAll");

        // Register highest tier first; wire chain + prerequisites when LV1 completes (last).
        CustomCard.BuildCard<UpgradeShieldLv5>(OnLv5Built);
        CustomCard.BuildCard<UpgradeShieldLv4>(OnLv4Built);
        CustomCard.BuildCard<UpgradeShieldLv3>(OnLv3Built);
        CustomCard.BuildCard<UpgradeShieldLv2>(OnLv2Built);
        CustomCard.BuildCard<UpgradeShieldLv1>(OnLv1Built);
    }

    private static void OnLv5Built(CardInfo ci) { ShieldCardRegistry.Lv5 = ci; SLog.Line($"Registered LV5: {ci.cardName}"); }
    private static void OnLv4Built(CardInfo ci) { ShieldCardRegistry.Lv4 = ci; SLog.Line($"Registered LV4: {ci.cardName}"); }
    private static void OnLv3Built(CardInfo ci) { ShieldCardRegistry.Lv3 = ci; SLog.Line($"Registered LV3: {ci.cardName}"); }
    private static void OnLv2Built(CardInfo ci) { ShieldCardRegistry.Lv2 = ci; SLog.Line($"Registered LV2: {ci.cardName}"); }

    private static void OnLv1Built(CardInfo ci)
    {
        ShieldCardRegistry.Lv1 = ci;
        SLog.Line($"Registered LV1: {ci.cardName}");

        // All five cards are now registered. Wire internal references and declare the
        // unlock chain with DeckBuilder (per-player, inventory-based gating).
        ShieldCardRegistry.WireLevelChain();
        RegisterPrerequisites();
        ShieldCardDeltaRegistrar.RegisterAll();
    }

    /// <summary>
    /// Declares the unlock chain with DeckBuilder: each level requires the
    /// previous one to already be in the player's hand.
    /// </summary>
    private static void RegisterPrerequisites()
    {
        SLog.Section("ShieldCardRegistrar — RegisterPrerequisites");

        if (!RtgPrerequisiteBridge.IsAvailable)
        {
            SLog.Warn("DeckBuilder not available — shield cards will NOT be unlock-gated.");
            return;
        }

        TryRegister(ShieldCardRegistry.Lv2, ShieldCardRegistry.Lv1);
        TryRegister(ShieldCardRegistry.Lv3, ShieldCardRegistry.Lv2);
        TryRegister(ShieldCardRegistry.Lv4, ShieldCardRegistry.Lv3);
        TryRegister(ShieldCardRegistry.Lv5, ShieldCardRegistry.Lv4);
    }

    private static void TryRegister(CardInfo card, CardInfo required)
    {
        if (card == null || required == null)
        {
            SLog.Warn("Skipping prerequisite registration — a card reference was null.");
            return;
        }

        bool ok = RtgPrerequisiteBridge.Register(card.cardName, required.cardName);
        SLog.Line($"Prereq: \"{card.cardName}\" requires \"{required.cardName}\" — registered={ok}");
    }
}
