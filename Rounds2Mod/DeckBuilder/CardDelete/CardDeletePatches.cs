using HarmonyLib;

namespace DeckBuilder.CardDelete;

/// <summary>
/// Harmony patches that used to attach the card-bar delete buttons.
/// The delete mechanic is now handled by the "Changed Mind" card — see SpecialCardPatches.
/// Class is kept for reference but the [HarmonyPatch] attribute is removed so nothing runs.
/// </summary>
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
