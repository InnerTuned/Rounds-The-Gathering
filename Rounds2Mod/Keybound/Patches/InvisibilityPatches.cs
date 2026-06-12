using HarmonyLib;
using Keybound.Effects;
using UnityEngine;

namespace Keybound.Patches;

/// <summary>Blocks damage and projectile hits while a player is invisible.</summary>
[HarmonyPatch]
internal static class InvisibilityPatches
{
    [HarmonyPatch(typeof(HealthHandler), nameof(HealthHandler.DoDamage))]
    [HarmonyPrefix]
    static bool DoDamage_Prefix(HealthHandler __instance)
    {
        Player player = __instance?.GetComponent<Player>();
        if (player != null && InvisibilityManager.IsActive(player))
            return false;

        return true;
    }

    [HarmonyPatch(typeof(ProjectileHit), nameof(ProjectileHit.Hit))]
    [HarmonyPrefix]
    static bool ProjectileHit_Prefix(HitInfo hit)
    {
        if (hit.transform == null)
            return true;

        HealthHandler health = hit.transform.GetComponent<HealthHandler>()
            ?? hit.transform.GetComponentInParent<HealthHandler>();
        if (health == null)
            return true;

        Player player = health.GetComponent<Player>();
        if (player != null && InvisibilityManager.IsActive(player))
            return false;

        return true;
    }
}
