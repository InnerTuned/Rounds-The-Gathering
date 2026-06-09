using System.Collections.Generic;
using Keybound.Effects;

namespace Keybound.Core;

internal static class KeyboundCardRegistry
{
    private static readonly Dictionary<string, KeyboundEffectDef> Effects = new();

    internal static void Register(KeyboundEffectDef def)
    {
        Effects[def.CardName] = def;
        KLog.Line($"Registered keybound effect '{def.CardName}' (delay={def.InitialDelay}s, cd={def.Cooldown}s).");
    }

    internal static bool IsKeybound(string cardName) =>
        !string.IsNullOrEmpty(cardName) && Effects.ContainsKey(cardName);

    internal static bool TryGet(string cardName, out KeyboundEffectDef def) =>
        Effects.TryGetValue(cardName, out def);

    internal static void RegisterBuiltInEffects()
    {
        Register(TeleportEffect.Def);
    }
}
