using HarmonyLib;
using InfoOverhaul.UI;

namespace InfoOverhaul.Patches;

[HarmonyPatch]
internal static class PickPhasePatches
{
    [HarmonyPatch(typeof(CardChoiceVisuals), nameof(CardChoiceVisuals.Show))]
    [HarmonyPrefix]
    static void CardChoiceVisuals_Show_Prefix(int pickerID, bool animateIn)
    {
        PickStatsController.instance?.BeginPickPhase(pickerID, "CardChoiceVisuals.Show");
    }

    [HarmonyPatch(typeof(CardChoice), nameof(CardChoice.StartPick))]
    [HarmonyPostfix]
    static void CardChoice_StartPick_Postfix(int picksToSet, int pickerIDToSet)
    {
        PickStatsController.instance?.BeginPickPhase(pickerIDToSet, "CardChoice.StartPick");
    }
}
