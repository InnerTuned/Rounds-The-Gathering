using Keybound.Core;
using UnityEngine;

namespace Keybound.Effects;

internal static class InvisibilityEffect
{
    internal const string CardName = "Invisibility";
    internal const float InitialDelay = 10f;
    internal const float Duration = 7f;
    internal const float Cooldown = 20f;

    internal static readonly KeyboundEffectDef Def = new()
    {
        CardName = CardName,
        InitialDelay = InitialDelay,
        Duration = Duration,
        Cooldown = Cooldown,
        Activate = TryActivate
    };

    private static bool TryActivate(Player player)
    {
        if (player == null)
            return false;

        if (InvisibilityManager.instance == null)
        {
            KLog.Warn("InvisibilityManager.instance is null.");
            return false;
        }

        InvisibilityManager.instance.Activate(player, Duration);
        return true;
    }
}
