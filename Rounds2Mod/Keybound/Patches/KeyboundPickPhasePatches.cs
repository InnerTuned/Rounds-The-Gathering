using HarmonyLib;
using Keybound.Core;

namespace Keybound.Patches;

/// <summary>Pauses keybound timers during card picks and resets them when combat resumes.</summary>
[HarmonyPatch]
internal static class KeyboundPickPhasePatches
{
    [HarmonyPatch(typeof(CardChoice), nameof(CardChoice.StartPick))]
    [HarmonyPostfix]
    static void StartPick_Postfix()
    {
        EffectStackManager.instance?.OnPickPhaseBegin();
    }

    [HarmonyPatch(typeof(CardChoice), "RPCA_DonePicking")]
    [HarmonyPostfix]
    static void DonePicking_Postfix()
    {
        EffectStackManager.instance?.OnPickPhaseEnd();
    }
}
