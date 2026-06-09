using HarmonyLib;
using UnboundLib.Utils.UI;
using UnityEngine;
using DeckBuilder.UI;

namespace DeckBuilder.GameIntegration
{
    [HarmonyPatch]
    internal static class DeckEntryPatch
    {
        /// <summary>
        /// Intercepts ToggleCardsMenuHandler.SetActive when the card menu is being opened
        /// and shows our DeckSelectorScreen instead.
        /// </summary>
        [HarmonyPatch(typeof(ToggleCardsMenuHandler), nameof(ToggleCardsMenuHandler.SetActive),
            new[] { typeof(Transform), typeof(bool) })]
        [HarmonyPrefix]
        static bool Prefix(Transform trans, bool active)
        {
            // Only intercept when opening (active=true) and it's the card menu canvas itself.
            if (!active) return true;
            if (ToggleCardsMenuHandler.cardMenuCanvas == null) return true;
            if (trans != ToggleCardsMenuHandler.cardMenuCanvas.transform) return true;

            RTGLog.Section("DeckEntryPatch — intercepting Toggle Cards open");

            DeckSelectorScreen selectorScreen = DeckSelectorScreen.instance;
            if (selectorScreen == null)
            {
                RTGLog.Warn("DeckSelectorScreen.instance is null — falling back to original menu.");
                return true;
            }

            selectorScreen.Show();
            RTGLog.Line("Original Toggle Cards menu suppressed.");
            return false;
        }
    }
}
