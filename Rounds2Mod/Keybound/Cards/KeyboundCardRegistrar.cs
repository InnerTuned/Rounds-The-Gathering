using Keybound.Core;
using Keybound.Effects;
using UnboundLib.Cards;

namespace Keybound.Cards;

internal static class KeyboundCardRegistrar
{
    internal static void RegisterAll()
    {
        KLog.Section("KeyboundCardRegistrar — RegisterAll");

        KeyboundCardRegistry.RegisterBuiltInEffects();

        CustomCard.BuildCard<TeleportCard>(ci =>
            KLog.Line($"Registered keybound card: {ci.cardName}"));

        CustomCard.BuildCard<InvisibilityCard>(ci =>
            KLog.Line($"Registered keybound card: {ci.cardName}"));
    }
}
