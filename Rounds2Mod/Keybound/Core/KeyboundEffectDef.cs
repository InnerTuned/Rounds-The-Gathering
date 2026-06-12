using System;

namespace Keybound.Core;

/// <summary>Runtime configuration for a keybound card effect.</summary>
internal sealed class KeyboundEffectDef
{
    public string CardName;
    public float InitialDelay;
    /// <summary>Active time after activation (0 = instant).</summary>
    public float Duration;
    public float Cooldown;
    public Func<Player, bool> Activate;
}
