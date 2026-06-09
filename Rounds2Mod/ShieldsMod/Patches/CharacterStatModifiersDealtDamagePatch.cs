using HarmonyLib;
using ShieldsMod.Lifesteal;
using UnityEngine;

namespace ShieldsMod.Patches;

[HarmonyPatch(typeof(CharacterStatModifiers), nameof(CharacterStatModifiers.DealtDamage))]
internal static class CharacterStatModifiersDealtDamagePatch
{
    [HarmonyPostfix]
    private static void Postfix(CharacterStatModifiers __instance, Vector2 damage, bool selfDamage, Player damagedPlayer)
    {
        if (selfDamage || damagedPlayer == null || __instance.lifeSteal == 0f)
            return;

        if (LifestealResistanceManager.instance == null)
            return;

        float reduction = LifestealResistanceManager.instance.GetReduction(damagedPlayer.playerID);
        if (reduction <= 0f)
            return;

        float rawHeal = damage.magnitude * __instance.lifeSteal;
        float actualHeal = LifestealResistanceManager.instance.ApplyResistance(
            damagedPlayer.playerID, rawHeal, out float resistedAmount);

        if (resistedAmount <= 0f)
            return;

        Player attacker = __instance.GetComponent<Player>();
        if (attacker?.data == null)
            return;

        attacker.data.health -= resistedAmount;
        attacker.data.health = Mathf.Clamp(attacker.data.health, 1f, attacker.data.maxHealth);

        SLog.Section("Lifesteal resistance");
        SLog.Line(
            $"Player {attacker?.playerID ?? -1} would've healed {rawHeal:F1} from life steal but healed {actualHeal:F1} instead (victim {damagedPlayer.playerID} resisted {resistedAmount:F1}).");
    }
}
