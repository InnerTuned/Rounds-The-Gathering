using System.Collections;
using System.Reflection;
using BepInEx.Logging;
using DeckBuilder.Data;
using HarmonyLib;
using UnityEngine;

namespace DeckBuilder.GameIntegration;

/// <summary>Restores the draft card-selection screen after a modal flow is cancelled.</summary>
internal static class PickPhaseRestoreHelper
{
    private static ManualLogSource Log => Plugin.Logger;
    private const string Tag = "[Restore]";

    private static readonly FieldInfo s_isPlayingField =
        AccessTools.Field(typeof(CardChoice), "isPlaying");
    private static readonly FieldInfo s_picksField =
        AccessTools.Field(typeof(CardChoice), "picks");
    private static readonly FieldInfo s_spawnedCardsField =
        AccessTools.Field(typeof(CardChoice), "spawnedCards");
    private static readonly FieldInfo s_isShowingField =
        AccessTools.Field(typeof(CardChoiceVisuals), "isShowinig");
    private static readonly FieldInfo s_currentlySelectedField =
        AccessTools.Field(typeof(CardChoice), "currentlySelectedCard");
    private static readonly MethodInfo s_setCurrentSelected =
        AccessTools.Method(typeof(CardChoiceVisuals), "SetCurrentSelected");

    internal static void RestoreDraftHand(int pickerID, CardInfo interceptedCard = null)
    {
        Log.LogInfo($"{Tag} === RestoreDraftHand START === pickerID={pickerID}, interceptedCard='{interceptedCard?.cardName ?? "null"}'");

        CardChoice cc = CardChoice.instance;
        if (cc == null)
        {
            Log.LogWarning($"{Tag} CardChoice.instance is null — abort.");
            return;
        }

        Log.LogInfo($"{Tag} BEFORE: pickrID={cc.pickrID}, IsPicking={cc.IsPicking}, isPlaying={s_isPlayingField?.GetValue(cc)}");

        // Dump spawned cards state
        var spawnedBefore = s_spawnedCardsField?.GetValue(cc) as IList;
        Log.LogInfo($"{Tag} spawnedCards field resolved: {s_spawnedCardsField != null}, list={spawnedBefore != null}, count={spawnedBefore?.Count ?? -1}");
        if (spawnedBefore != null)
        {
            for (int i = 0; i < spawnedBefore.Count; i++)
            {
                var go = spawnedBefore[i] as GameObject;
                if (go == null)
                {
                    Log.LogInfo($"{Tag}   spawnedCards[{i}] = NULL");
                    continue;
                }
                var cardInfo = go.GetComponent<CardInfo>();
                var stats = go.GetComponentInChildren<ApplyCardStats>();
                var doneField = AccessTools.Field(typeof(ApplyCardStats), "done");
                bool? done = stats != null && doneField != null ? (bool?)doneField.GetValue(stats) : null;
                Log.LogInfo($"{Tag}   spawnedCards[{i}] = '{cardInfo?.cardName ?? go.name}', active={go.activeSelf}, done={done}");
            }
        }

        // CardChoiceVisuals state
        if (CardChoiceVisuals.instance != null && s_isShowingField != null)
            Log.LogInfo($"{Tag} CardChoiceVisuals.isShowinig = {s_isShowingField.GetValue(CardChoiceVisuals.instance)}");

        cc.StopAllCoroutines();
        Log.LogInfo($"{Tag} Stopped CardChoice coroutines.");

        cc.pickrID = pickerID;
        Log.LogInfo($"{Tag} Set pickrID = {pickerID}");

        if (s_isPlayingField != null)
        {
            s_isPlayingField.SetValue(cc, false);
            Log.LogInfo($"{Tag} Set isPlaying = false");
        }

        cc.IsPicking = true;
        Log.LogInfo($"{Tag} Set IsPicking = true");

        if (interceptedCard != null)
        {
            ResetApplyStatsDone(interceptedCard);
            Log.LogInfo($"{Tag} Reset done flag on intercepted card '{interceptedCard.cardName}'");
        }

        int resetCount = ResetAllSpawnedDoneFlags(cc);
        Log.LogInfo($"{Tag} Reset done flags on {resetCount} spawned card(s).");

        ResetCardSelection(cc);

        Log.LogInfo($"{Tag} AFTER: pickrID={cc.pickrID}, IsPicking={cc.IsPicking}, isPlaying={s_isPlayingField?.GetValue(cc)}");
        Log.LogInfo($"{Tag} === RestoreDraftHand END ===");
    }

    /// <summary>
    /// Tears down the current draft hand and spawns a fresh one so the player can pick a
    /// replacement card. Used for the <see cref="TwoStepOutcome.ActionAcceptedContinue"/>
    /// outcome (e.g. Swap, after deleting a card).
    /// </summary>
    internal static void RedrawDraftHand(int pickerID)
    {
        Log.LogInfo($"{Tag} === RedrawDraftHand START === pickerID={pickerID}");

        CardChoice cc = CardChoice.instance;
        if (cc == null)
        {
            Log.LogWarning($"{Tag} CardChoice.instance is null — cannot redraw.");
            return;
        }

        cc.StopAllCoroutines();
        CardDelete.CardDeleteManager.CleanupSpawnedDraftCards(cc);
        s_isPlayingField?.SetValue(cc, false);

        if (DeckManager.RuntimeDecks.TryGetValue(pickerID, out var rt) && rt.Count > 0)
        {
            cc.cards = rt.ToArray();
            Log.LogInfo($"{Tag} Set CardChoice.cards to runtime deck ({rt.Count} cards).");
        }
        else if (DeckPickPatch.VanillaPool != null)
        {
            cc.cards = DeckPickPatch.VanillaPool;
            Log.LogInfo($"{Tag} Set CardChoice.cards to vanilla pool ({DeckPickPatch.VanillaPool.Length} cards).");
        }
        else
        {
            Log.LogWarning($"{Tag} No runtime deck or vanilla pool available.");
        }

        cc.IsPicking = true;
        s_picksField?.SetValue(cc, 1);
        cc.Pick(null, false);

        Log.LogInfo($"{Tag} === RedrawDraftHand END === new draft hand spawned.");
    }

    private static void ResetApplyStatsDone(CardInfo card)
    {
        var applyStats = card.GetComponentInChildren<ApplyCardStats>();
        if (applyStats == null)
        {
            Log.LogWarning($"{Tag} No ApplyCardStats on '{card.cardName}'");
            return;
        }

        var doneField = AccessTools.Field(typeof(ApplyCardStats), "done");
        doneField?.SetValue(applyStats, false);
    }

    private static int ResetAllSpawnedDoneFlags(CardChoice cc)
    {
        if (s_spawnedCardsField?.GetValue(cc) is not IList spawned)
        {
            Log.LogWarning($"{Tag} spawnedCards is null or not IList");
            return 0;
        }

        int count = 0;
        var doneField = AccessTools.Field(typeof(ApplyCardStats), "done");
        foreach (var obj in spawned)
        {
            if (obj is not GameObject go || go == null)
                continue;

            var stats = go.GetComponentInChildren<ApplyCardStats>();
            if (stats != null)
            {
                doneField?.SetValue(stats, false);
                count++;
            }
        }
        return count;
    }

    private static void ResetCardSelection(CardChoice cc)
    {
        s_currentlySelectedField?.SetValue(cc, 0);
        Log.LogInfo($"{Tag} Set currentlySelectedCard = 0");

        if (CardChoiceVisuals.instance != null && s_setCurrentSelected != null)
        {
            s_setCurrentSelected.Invoke(CardChoiceVisuals.instance, new object[] { 0 });
            Log.LogInfo($"{Tag} Called CardChoiceVisuals.SetCurrentSelected(0)");
        }
        else
        {
            Log.LogWarning($"{Tag} Cannot call SetCurrentSelected: instance={CardChoiceVisuals.instance != null}, method={s_setCurrentSelected != null}");
        }
    }
}
