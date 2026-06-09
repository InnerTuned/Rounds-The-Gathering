using System.Collections;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace Keybound.Core;

/// <summary>Ends the draft pick after a keybound card is confirmed.</summary>
internal static class PickPhaseHelper
{
    private static readonly FieldInfo s_isPlayingField =
        AccessTools.Field(typeof(CardChoice), "isPlaying");
    private static readonly FieldInfo s_picksField =
        AccessTools.Field(typeof(CardChoice), "picks");
    private static readonly FieldInfo s_spawnedCardsField =
        AccessTools.Field(typeof(CardChoice), "spawnedCards");
    private static readonly MethodInfo s_rpca_donePicking =
        AccessTools.Method(typeof(CardChoice), "RPCA_DonePicking");

    internal static void EndPickPhase()
    {
        KLog.Section("EndPickPhase");

        CardChoice cc = CardChoice.instance;
        if (cc == null)
        {
            KLog.Error("CardChoice.instance is null.");
            return;
        }

        cc.StopAllCoroutines();
        CleanupSpawnedDraftCards(cc);

        if (s_isPlayingField != null) s_isPlayingField.SetValue(cc, false);
        if (s_picksField != null) s_picksField.SetValue(cc, 0);

        if (s_rpca_donePicking == null)
        {
            cc.IsPicking = false;
        }
        else
        {
            try { s_rpca_donePicking.Invoke(cc, null); }
            catch (System.Exception ex)
            {
                KLog.Error($"RPCA_DonePicking failed: {ex.Message}");
                cc.IsPicking = false;
            }
        }

        KLog.Line($"Pick ended. IsPicking={cc.IsPicking}");
    }

    internal static void RestorePickState(int pickerID, CardInfo card)
    {
        CardChoice cc = CardChoice.instance;
        if (cc == null) return;

        cc.StopAllCoroutines();
        cc.pickrID = pickerID;
        if (s_isPlayingField != null) s_isPlayingField.SetValue(cc, false);
        cc.IsPicking = true;

        if (card != null)
        {
            var applyStats = card.GetComponentInChildren<ApplyCardStats>();
            var doneField = AccessTools.Field(typeof(ApplyCardStats), "done");
            doneField?.SetValue(applyStats, false);
        }

        ResetSpawnedDoneFlags(cc);
        KLog.Line($"Restored pick state for picker {pickerID}.");
    }

    private static void ResetSpawnedDoneFlags(CardChoice cc)
    {
        if (s_spawnedCardsField?.GetValue(cc) is not IList spawned) return;

        var doneField = AccessTools.Field(typeof(ApplyCardStats), "done");
        foreach (var obj in spawned)
        {
            if (obj is not GameObject go) continue;
            var stats = go.GetComponentInChildren<ApplyCardStats>();
            if (stats != null) doneField?.SetValue(stats, false);
        }
    }

    private static void CleanupSpawnedDraftCards(CardChoice cc)
    {
        if (s_spawnedCardsField?.GetValue(cc) is not IList spawned) return;

        for (int i = spawned.Count - 1; i >= 0; i--)
        {
            if (spawned[i] is GameObject go && go != null)
                Object.Destroy(go);
        }
        spawned.Clear();
    }
}
