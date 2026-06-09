using System;

namespace Keybound.Core;

/// <summary>Runtime configuration for a keybound card effect.</summary>
internal sealed class KeyboundEffectDef
{
    public string CardName;
    public float InitialDelay;
    public float Cooldown;
    public Func<Player, bool> Activate;
}
