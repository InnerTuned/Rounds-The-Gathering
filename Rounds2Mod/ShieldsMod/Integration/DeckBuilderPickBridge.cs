using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace ShieldsMod.Integration;

/// <summary>Optional DeckBuilder hooks for ending picks and consuming runtime-deck cards.</summary>
internal static class DeckBuilderPickBridge
{
    private static Type s_cardDeleteManagerType;
    private static MethodInfo s_endPickPhase;
    private static Type s_deckManagerType;
    private static MethodInfo s_consumeCard;
    private static MethodInfo s_getRemainingCount;
    private static Type s_deckNetworkSyncType;
    private static MethodInfo s_broadcastRemainingCount;
    private static Type s_specialCardPatchesType;
    private static FieldInfo s_suppressApplyStatsPostfix;

    private static bool _cardDeleteResolved;
    private static bool _deckManagerResolved;
    private static bool _deckNetworkResolved;

    private static MethodInfo s_rpcaDonePicking;
    private static FieldInfo s_isPlayingField;
    private static FieldInfo s_picksField;
    private static FieldInfo s_spawnedCardsField;

    internal static bool IsDeckBuilderLoaded
    {
        get
        {
            ResolveCardDeleteManager();
            return s_endPickPhase != null;
        }
    }

    internal static IDisposable SuppressDeckPickPostfix()
    {
        if (!TrySetSuppressFlag(true))
            return EmptyDisposable.Instance;

        return new SuppressScope();
    }

    internal static void SetSuppressDeckPickPostfix(bool value) => TrySetSuppressFlag(value);

    internal static void EndPickPhase()
    {
        if (TryInvokeEndPickPhase())
            return;

        FallbackEndPickPhase();
    }

    internal static void ConsumeRuntimeCard(int playerID, CardInfo card)
    {
        if (card == null)
            return;

        if (TryInvokeConsumeCard(playerID, card))
        {
            BroadcastRemainingCount(playerID);
            return;
        }

        SLog.Line($"DeckBuilder not loaded — skipped runtime consume for '{card.cardName}'.");
    }

    private static bool TryInvokeEndPickPhase()
    {
        ResolveCardDeleteManager();
        if (s_endPickPhase == null)
            return false;

        try
        {
            s_endPickPhase.Invoke(null, null);
            return true;
        }
        catch (Exception ex)
        {
            SLog.Error($"DeckBuilder EndPickPhase failed: {ex.Message}");
            return false;
        }
    }

    private static bool TryInvokeConsumeCard(int playerID, CardInfo card)
    {
        ResolveDeckManager();
        if (s_consumeCard == null)
            return false;

        try
        {
            s_consumeCard.Invoke(null, new object[] { playerID, card });
            return true;
        }
        catch (Exception ex)
        {
            SLog.Error($"DeckBuilder ConsumeCard failed: {ex.Message}");
            return false;
        }
    }

    private static void BroadcastRemainingCount(int playerID)
    {
        ResolveDeckNetworkSync();
        ResolveDeckManager();
        if (s_broadcastRemainingCount == null || s_getRemainingCount == null)
            return;

        try
        {
            int remaining = (int)s_getRemainingCount.Invoke(null, new object[] { playerID });
            if (remaining >= 0)
                s_broadcastRemainingCount.Invoke(null, new object[] { playerID, remaining });
        }
        catch (Exception ex)
        {
            SLog.Warn($"BroadcastRemainingCount failed: {ex.Message}");
        }
    }

    private static void FallbackEndPickPhase()
    {
        CardChoice cc = CardChoice.instance;
        if (cc == null)
        {
            SLog.Error("CardChoice.instance is null — cannot end pick.");
            return;
        }

        cc.StopAllCoroutines();
        CacheCardChoiceFields();

        s_isPlayingField?.SetValue(cc, false);
        s_picksField?.SetValue(cc, 0);

        CleanupSpawnedDraftCards(cc);

        if (s_rpcaDonePicking != null)
        {
            try
            {
                s_rpcaDonePicking.Invoke(cc, null);
                return;
            }
            catch (Exception ex)
            {
                SLog.Warn($"RPCA_DonePicking failed: {ex.Message}");
            }
        }

        cc.IsPicking = false;
    }

    private static void CleanupSpawnedDraftCards(CardChoice cc)
    {
        CacheCardChoiceFields();
        var spawned = s_spawnedCardsField?.GetValue(cc) as System.Collections.Generic.List<GameObject>;
        if (spawned == null)
            return;

        foreach (GameObject go in spawned.ToArray())
        {
            if (go == null)
                continue;

            try
            {
                CardVisuals visuals = go.GetComponentInChildren<CardVisuals>();
                visuals?.Leave();
            }
            catch
            {
                // ignored
            }

            UnityEngine.Object.Destroy(go);
        }

        spawned.Clear();
    }

    private static void CacheCardChoiceFields()
    {
        if (s_isPlayingField != null)
            return;

        s_isPlayingField = AccessTools.Field(typeof(CardChoice), "isPlaying");
        s_picksField = AccessTools.Field(typeof(CardChoice), "picks");
        s_spawnedCardsField = AccessTools.Field(typeof(CardChoice), "spawnedCards");
        s_rpcaDonePicking = AccessTools.Method(typeof(CardChoice), "RPCA_DonePicking");
    }

    private static void ResolveCardDeleteManager()
    {
        if (_cardDeleteResolved)
            return;

        _cardDeleteResolved = true;
        s_cardDeleteManagerType = AccessTools.TypeByName("DeckBuilder.CardDelete.CardDeleteManager");
        if (s_cardDeleteManagerType == null)
            return;

        s_endPickPhase = AccessTools.Method(s_cardDeleteManagerType, "EndPickPhase");
    }

    private static void ResolveDeckManager()
    {
        if (_deckManagerResolved)
            return;

        _deckManagerResolved = true;
        s_deckManagerType = AccessTools.TypeByName("DeckBuilder.Data.DeckManager");
        if (s_deckManagerType == null)
            return;

        s_consumeCard = AccessTools.Method(s_deckManagerType, "ConsumeCard");
        s_getRemainingCount = AccessTools.Method(s_deckManagerType, "GetRemainingCount");
    }

    private static void ResolveDeckNetworkSync()
    {
        if (_deckNetworkResolved)
            return;

        _deckNetworkResolved = true;
        s_deckNetworkSyncType = AccessTools.TypeByName("DeckBuilder.Networking.DeckNetworkSync");
        if (s_deckNetworkSyncType == null)
            return;

        s_broadcastRemainingCount = AccessTools.Method(s_deckNetworkSyncType, "BroadcastRemainingCount");
    }

    private static bool TrySetSuppressFlag(bool value)
    {
        if (s_suppressApplyStatsPostfix == null)
        {
            s_specialCardPatchesType = AccessTools.TypeByName("DeckBuilder.Cards.SpecialCardPatches");
            if (s_specialCardPatchesType == null)
                return false;

            s_suppressApplyStatsPostfix =
                AccessTools.Field(s_specialCardPatchesType, "SuppressApplyStatsPostfix");
        }

        if (s_suppressApplyStatsPostfix == null)
            return false;

        s_suppressApplyStatsPostfix.SetValue(null, value);
        return true;
    }

    private sealed class EmptyDisposable : IDisposable
    {
        internal static readonly EmptyDisposable Instance = new();
        public void Dispose() { }
    }

    private sealed class SuppressScope : IDisposable
    {
        public void Dispose() => TrySetSuppressFlag(false);
    }
}
