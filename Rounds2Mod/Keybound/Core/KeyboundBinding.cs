using UnityEngine;

namespace Keybound.Core;

/// <summary>A keybound effect assigned to a player with cooldown tracking.</summary>
internal sealed class KeyboundBinding
{
    public int PlayerID;
    public string CardName;
    public CardInfo Card;
    public int Key;
    public KeyboundEffectDef Def;

    /// <summary>Time.time when the effect becomes usable (initial delay or cooldown).</summary>
    public float ReadyAt;

    /// <summary>Duration of the current lockout window (for overlay progress).</summary>
    public float LockoutDuration;

    public bool IsReady() => Time.time >= ReadyAt;

    /// <summary>0 = ready, 1 = fully locked out.</summary>
    public float CooldownFill()
    {
        if (IsReady()) return 0f;
        float remaining = ReadyAt - Time.time;
        if (LockoutDuration <= 0f) return 1f;
        return Mathf.Clamp01(remaining / LockoutDuration);
    }

    public void ArmInitialDelay()
    {
        LockoutDuration = Def.InitialDelay;
        ReadyAt = Time.time + Def.InitialDelay;
    }

    public void ArmCooldown()
    {
        LockoutDuration = Def.Cooldown;
        ReadyAt = Time.time + Def.Cooldown;
    }

    public void ArmAfterActivate()
    {
        float total = Mathf.Max(0f, Def.Duration) + Def.Cooldown;
        LockoutDuration = total;
        ReadyAt = Time.time + total;
    }
}
