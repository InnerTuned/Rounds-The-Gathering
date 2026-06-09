using HarmonyLib;
using ShieldsMod.Stun;
using UnityEngine;

namespace ShieldsMod.Patches;

[HarmonyPatch(typeof(StunHandler), nameof(StunHandler.AddStun))]
internal static class StunHandlerAddStunPatch
{
    [HarmonyPrefix]
    private static bool Prefix(StunHandler __instance, ref float f)
    {
        if (f <= 0f || StunResistanceManager.instance == null)
            return true;

        Player player = __instance.GetComponent<Player>();
        if (player == null)
            return true;

        float reduction = StunResistanceManager.instance.GetReduction(player.playerID);
        if (reduction <= 0f)
            return true;

        float rawDuration = f;
        f = StunResistanceManager.instance.ApplyResistance(player.playerID, rawDuration, out _);

        if (rawDuration > f)
        {
            SLog.Section("Stun resistance");
            SLog.Line($"Player {player.playerID} would've been stunned for {rawDuration:F2}s but duration was reduced to {f:F2}s.");
        }

        if (f <= 0f)
            return false;

        return true;
    }
}
