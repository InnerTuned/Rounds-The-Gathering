using HarmonyLib;

namespace DeckBuilder.CardDelete;

/// <summary>
/// Harmony patches that run at the correct time in the pick flow.
/// UnboundLib's HookPlayerPickStart fires before pickrID is assigned; these patches do not.
/// </summary>
[HarmonyPatch]
internal static class CardDeletePatches
{
    [HarmonyPatch(typeof(CardChoiceVisuals), nameof(CardChoiceVisuals.Show))]
    [HarmonyPrefix]
    private static void CardChoiceVisuals_Show_Prefix(int pickerID, bool animateIn)
    {
        CardDeleteManager.instance?.BeginPickPhase(pickerID, "CardChoiceVisuals.Show");
    }

    [HarmonyPatch(typeof(CardChoice), nameof(CardChoice.StartPick))]
    [HarmonyPostfix]
    private static void CardChoice_StartPick_Postfix(int picksToSet, int pickerIDToSet)
    {
        CardDeleteManager.instance?.BeginPickPhase(pickerIDToSet, "CardChoice.StartPick");
    }
}
