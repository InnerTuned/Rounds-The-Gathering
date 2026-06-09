using UnityEngine;

namespace ShieldsMod.Poison;

internal static class PoisonDamageDetector
{
    public static bool IsPoisonDamage(Color blinkColor, GameObject damagingWeapon)
    {
        if (damagingWeapon != null)
        {
            if (damagingWeapon.GetComponentInChildren<RayHitPoison>() != null)
                return true;

            Transform root = damagingWeapon.transform.root;
            if (root != null && root.GetComponentInChildren<RayHitPoison>() != null)
                return true;
        }

        return blinkColor.g > 0.35f && blinkColor.g >= blinkColor.r && blinkColor.g >= blinkColor.b;
    }
}
