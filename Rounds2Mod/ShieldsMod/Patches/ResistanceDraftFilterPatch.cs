using HarmonyLib;
using ShieldsMod.Cards;

namespace ShieldsMod.Patches;

/// <summary>Prevents internal resistance marker cards and legacy tier cards from appearing in drafts.</summary>
[HarmonyPatch(typeof(ModdingUtils.Utils.Cards), "PlayerIsAllowedCard")]
internal static class ResistanceDraftFilterPatch
{
    [HarmonyPostfix]
    static void ExcludeInternalResistanceCards(CardInfo card, ref bool __result)
    {
        if (__result && ResistanceDraftFilter.IsExcluded(card?.cardName))
            __result = false;
    }
}
