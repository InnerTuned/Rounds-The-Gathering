using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShieldsMod.Shield;

/// <summary>Rebuilds shield max from shield-upgrade cards still in the player's hand.</summary>
internal static class ShieldHandRebuild
{
    private readonly struct ShieldTier
    {
        public readonly float FirstShieldMax;
        public readonly float StackMultiplier;

        public ShieldTier(float firstShieldMax, float stackMultiplier)
        {
            FirstShieldMax = firstShieldMax;
            StackMultiplier = stackMultiplier;
        }
    }

    private static Dictionary<string, ShieldTier> _tierByCardName;

    internal static void RegisterTier(CardInfo card, float firstShieldMax, float stackMultiplier)
    {
        if (card == null || string.IsNullOrEmpty(card.cardName))
            return;

        _tierByCardName ??= new Dictionary<string, ShieldTier>(StringComparer.Ordinal);
        _tierByCardName[card.cardName] = new ShieldTier(firstShieldMax, stackMultiplier);
    }

    internal static float ComputeMaxFromHandExcluding(IEnumerable<CardInfo> cards, CardInfo exclude) =>
        ComputeMaxFromCards(cards, exclude);

    internal static void RebuildForPlayer(int playerID)
    {
        if (ShieldManager.instance == null || PlayerManager.instance == null)
            return;

        Player player = PlayerManager.instance.players.Find(p => p.playerID == playerID);
        if (player?.data?.currentCards == null)
            return;

        ShieldState state = ShieldManager.instance.GetOrCreateShield(playerID);
        float oldMax = state.Max;
        float max = ComputeMaxFromCards(player.data.currentCards);

        if (max <= 0f)
        {
            state.Max = 0f;
            state.Current = 0f;
        }
        else
        {
            state.Max = max;
            state.Current = Mathf.Min(state.Current, state.Max);
        }

        ShieldManager.instance.RefreshVisual(playerID);

        SLog.Line($"ShieldHandRebuild player={playerID} max {oldMax:F0} -> {state.Max:F0} current={state.Current:F0}");
    }

    private static float ComputeMaxFromCards(IEnumerable<CardInfo> cards, CardInfo exclude = null)
    {
        if (_tierByCardName == null || cards == null)
            return 0f;

        float max = 0f;
        bool established = false;

        foreach (CardInfo card in cards)
        {
            if (card == null || (exclude != null && card.name == exclude.name))
                continue;

            string name = card.cardName;
            if (string.IsNullOrEmpty(name) && card.sourceCard != null)
                name = card.sourceCard.cardName;

            if (string.IsNullOrEmpty(name) || !_tierByCardName.TryGetValue(name, out ShieldTier tier))
                continue;

            if (!established)
            {
                max = tier.FirstShieldMax;
                established = true;
            }
            else
            {
                max *= tier.StackMultiplier;
            }
        }

        return established ? max : 0f;
    }
}
