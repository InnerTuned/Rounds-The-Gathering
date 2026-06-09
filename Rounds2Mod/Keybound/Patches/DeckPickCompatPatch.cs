using System.Reflection;
using HarmonyLib;
using Keybound.Core;

namespace Keybound.Patches;

/// <summary>
/// Prevents DeckBuilder's ApplyStats_Postfix from consuming keybound cards.
/// Harmony still runs postfixes when ApplyStats prefix returns false, so this guard is required.
/// </summary>
[HarmonyPatch]
internal static class DeckPickCompatPatch
{
    private static MethodBase TargetMethod()
    {
        var type = AccessTools.TypeByName("DeckBuilder.GameIntegration.DeckPickPatch");
        return type == null ? null : AccessTools.Method(type, "ApplyStats_Postfix");
    }

    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    static bool SuppressDeckConsume(ApplyCardStats __instance)
    {
        if (__instance == null) return true;

        CardInfo card = __instance.GetComponentInParent<CardInfo>();
        if (card == null) return true;

        // Keybound cards are consumed on bind-confirm, never by DeckPickPatch.
        if (!KeyboundCardRegistry.IsKeybound(card.cardName)) return true;

        KLog.Line($"Suppressed DeckPickPatch consume for keybound card '{card.cardName}'.");
        return false;
    }
}
