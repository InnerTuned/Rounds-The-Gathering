using HarmonyLib;
using Keybound.Compat;
using Keybound.Core;
using Keybound.UI;
using UnityEngine;

namespace Keybound.Patches;

[HarmonyPatch]
internal static class KeyboundPickPatches
{
    internal static bool IsBindFlowActive;

    private static int _pickerID = -1;
    private static CardInfo _pendingCard;

    [HarmonyPatch(typeof(ApplyCardStats), "ApplyStats")]
    [HarmonyPrefix]
    static bool ApplyStats_Prefix(ApplyCardStats __instance)
    {
        if (CardChoice.instance == null || !CardChoice.instance.IsPicking) return true;

        CardInfo card = __instance.GetComponentInParent<CardInfo>();
        if (card == null || !KeyboundCardRegistry.IsKeybound(card.cardName)) return true;

        int pickerID = CardChoice.instance.pickrID;
        if (!IsLocalPicker(pickerID)) return true;

        KLog.Section($"Keybind flow started — '{card.cardName}'");
        StartBindFlow(pickerID, card);
        return false;
    }

    [HarmonyPatch(typeof(CardChoice), "RPCA_DoEndPick")]
    [HarmonyPrefix]
    static bool RPCA_DoEndPick_Prefix()
    {
        if (!IsBindFlowActive) return true;
        KLog.Line("RPCA_DoEndPick suppressed (keybind modal open).");
        return false;
    }

    [HarmonyPatch(typeof(CardBarHandler), "AddCard")]
    [HarmonyPrefix]
    static bool CardBarHandler_AddCard_Prefix(CardInfo card)
    {
        if (card == null) return true;
        if (!KeyboundCardRegistry.IsKeybound(card.cardName)) return true;

        KLog.Line($"Blocked card-bar add for keybound card '{card.cardName}'.");
        return false;
    }

    private static void StartBindFlow(int pickerID, CardInfo card)
    {
        _pickerID = pickerID;
        _pendingCard = card;
        IsBindFlowActive = true;

        KeybindModalUI.instance?.Show(pickerID, card,
            onConfirm: OnConfirm,
            onCancel: OnCancel);
    }

    private static void OnConfirm(int key)
    {
        KLog.Section($"Keybind confirmed — {_pendingCard?.cardName} -> {key}");
        IsBindFlowActive = false;

        if (_pendingCard != null)
        {
            EffectStackManager.instance?.AddBinding(_pickerID, _pendingCard, key);
            DeckBuilderBridge.ConsumeCard(_pickerID, _pendingCard);
        }

        PickPhaseHelper.EndPickPhase();
        _pendingCard = null;
    }

    private static void OnCancel()
    {
        KLog.Section("Keybind cancelled");
        IsBindFlowActive = false;
        PickPhaseHelper.RestorePickState(_pickerID, _pendingCard);
        _pendingCard = null;
    }

    private static bool IsLocalPicker(int pickerID)
    {
        Player picker = PlayerManager.instance?.players?.Find(p => p.playerID == pickerID);
        if (picker == null) return false;

        var viewField = AccessTools.Field(typeof(CharacterData), "view");
        object view = viewField?.GetValue(picker.data);
        if (view == null) return true;

        var isMine = AccessTools.Property(view.GetType(), "IsMine");
        if (isMine == null) return true;
        return (bool)isMine.GetValue(view, null);
    }
}
