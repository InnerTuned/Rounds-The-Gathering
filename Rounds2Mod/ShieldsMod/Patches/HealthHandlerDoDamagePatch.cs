using HarmonyLib;
using UnityEngine;
using ShieldsMod.Shield;
using ShieldsMod.Networking;
using ShieldsMod.Poison;

namespace ShieldsMod.Patches;

[HarmonyPatch(typeof(HealthHandler), nameof(HealthHandler.DoDamage))]
internal static class HealthHandlerDoDamagePatch
{
    [HarmonyPrefix]
    private static bool Prefix(
        HealthHandler __instance,
        ref Vector2 damage,
        Vector2 position,
        Color blinkColor,
        GameObject damagingWeapon,
        Player damagingPlayer,
        bool healthRemoval,
        bool lethal,
        bool ignoreBlock)
    {
        if (damage == Vector2.zero)
            return true;

        Player victim = __instance.GetComponent<Player>();
        if (victim == null)
            return true;

        float rawAmount = damage.magnitude;
        float amount = rawAmount;
        bool isPoison = PoisonDamageDetector.IsPoisonDamage(blinkColor, damagingWeapon);

        if (isPoison && PoisonResistanceManager.instance != null)
        {
            float reduction = PoisonResistanceManager.instance.GetReduction(victim.playerID);
            if (reduction > 0f)
            {
                amount = PoisonResistanceManager.instance.ApplyResistance(victim.playerID, rawAmount, out _);
                if (amount > 0f)
                    damage = damage.normalized * amount;
                else
                    damage = Vector2.zero;
            }
        }

        if (amount <= 0f)
            return false;

        if (ShieldManager.instance == null)
        {
            if (isPoison && rawAmount > amount)
                LogPoisonPlayerDamage(victim.playerID, rawAmount, amount);
            return true;
        }

        if (!ShieldManager.instance.TryAbsorbFull(victim.playerID, amount, out bool depleted))
        {
            if (isPoison && rawAmount > amount)
                LogPoisonPlayerDamage(victim.playerID, rawAmount, amount);
            return true;
        }

        if (isPoison && rawAmount > amount)
            LogPoisonShieldDamage(victim.playerID, rawAmount, amount, depleted);

        damage = Vector2.zero;

        if (victim.data?.view != null && victim.data.view.IsMine)
            ShieldNetworkSync.BroadcastShieldHealth(victim.playerID, ShieldManager.instance.GetShield(victim.playerID).Current);

        return false;
    }

    private static void LogPoisonPlayerDamage(int playerID, float rawAmount, float actualAmount)
    {
        SLog.Section("Poison resistance — player");
        SLog.Line($"Player {playerID} would've taken {rawAmount:F1} poison damage but took {actualAmount:F1} instead.");
    }

    private static void LogPoisonShieldDamage(int playerID, float rawAmount, float actualAmount, bool depleted)
    {
        SLog.Section("Poison resistance — shield");
        SLog.Line($"Player {playerID}'s shield would've taken {rawAmount:F1} poison damage but took {actualAmount:F1} instead (depleted={depleted}).");
    }
}
