using HarmonyLib;

namespace DeckBuilder.GameIntegration;

/// <summary>
/// Single, unified set of Harmony patches that drive every two-step card flow
/// (Copycat, Swap, Rare/Legendary Search, Keybound). Replaces the previously
/// duplicated patch sets in SpecialCardPatches / SearchCardPatches / KeyboundPickPatches.
///
/// Dispatch is data-driven via <see cref="TwoStepCardFlow"/>'s registry, so adding a new
/// two-step card requires only a registration — no new Harmony patches.
/// </summary>
[HarmonyPatch]
public static class TwoStepCardPatches
{
    /// <summary>
    /// Intercepts a drafted two-step card before its stats apply, launching the card's
    /// second step instead. Returning false blocks normal card application.
    /// </summary>
    [HarmonyPatch(typeof(ApplyCardStats), "ApplyStats")]
    [HarmonyPrefix]
    static bool ApplyStats_Prefix(ApplyCardStats __instance)
    {
        if (CardChoice.instance == null || !CardChoice.instance.IsPicking)
            return true;

        CardInfo card = __instance.GetComponentInParent<CardInfo>();
        if (card == null || !TwoStepCardFlow.IsTwoStep(card))
            return true;

        int pickerID = CardChoice.instance.pickrID;
        if (!IsLocalPicker(pickerID))
            return true;

        TwoStepLog.Line($"ApplyStats intercepted for two-step card '{card.cardName}' (picker={pickerID}).");
        bool started = TwoStepCardFlow.BeginFlow(pickerID, card);
        TwoStepLog.Line($"BeginFlow returned {started} — ApplyStats {(started ? "blocked" : "allowed")}.");
        return !started;
    }

    /// <summary>Suppresses the end-pick animation while any two-step second-step UI is open.</summary>
    [HarmonyPatch(typeof(CardChoice), "RPCA_DoEndPick")]
    [HarmonyPrefix]
    static bool RPCA_DoEndPick_Prefix()
    {
        if (!TwoStepCardFlow.IsFlowActive)
            return true;

        TwoStepLog.Line("RPCA_DoEndPick suppressed (second-step UI open).");
        return false;
    }

    /// <summary>Keeps a two-step card's own icon out of the card bar while its flow is active.</summary>
    [HarmonyPatch(typeof(CardBarHandler), "AddCard")]
    [HarmonyPrefix]
    static bool CardBarHandler_AddCard_Prefix(CardInfo card)
    {
        if (!TwoStepCardFlow.IsFlowActive || card == null)
            return true;

        if (TwoStepCardFlow.IsTwoStep(card))
        {
            TwoStepLog.Line($"Blocked card-bar add for two-step card '{card.cardName}'.");
            return false;
        }

        return true;
    }

    private static bool IsLocalPicker(int pickerID)
    {
        Player picker = PlayerManager.instance?.players?.Find(p => p.playerID == pickerID);
        if (picker == null)
            return false;

        var viewField = AccessTools.Field(typeof(CharacterData), "view");
        if (viewField == null)
            return true;

        object view = viewField.GetValue(picker.data);
        if (view == null)
            return true;

        var isMine = AccessTools.Property(view.GetType(), "IsMine");
        if (isMine == null)
            return true;

        return (bool)isMine.GetValue(view, null);
    }
}
