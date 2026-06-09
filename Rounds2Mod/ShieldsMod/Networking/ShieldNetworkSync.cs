using UnityEngine;
using UnboundLib;
using UnboundLib.Networking;
using ShieldsMod.Shield;

namespace ShieldsMod.Networking;

/// <summary>Broadcasts shield HP when local authority applies owner-only damage (e.g. DoT).</summary>
public static class ShieldNetworkSync
{
    public static void BroadcastShieldHealth(int playerID, float current)
    {
        SLog.Line($"BroadcastShieldHealth player={playerID} current={current:F1}");
        NetworkingManager.RPC(typeof(ShieldNetworkSync), nameof(URPC_SetShieldHealth),
            playerID, current);
    }

    [UnboundRPC]
    public static void URPC_SetShieldHealth(int playerID, float current)
    {
        if (ShieldManager.instance == null)
            return;

        ShieldState state = ShieldManager.instance.GetOrCreateShield(playerID);
        state.Current = Mathf.Clamp(current, 0f, state.Max);
        ShieldManager.instance.RefreshVisual(playerID);
        SLog.Line($"URPC_SetShieldHealth player={playerID} current={state.Current:F1}");
    }
}
