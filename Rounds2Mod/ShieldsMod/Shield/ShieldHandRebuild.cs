using System;
using System.Collections.Generic;
using ShieldsMod.Cards;
using UnityEngine;

namespace ShieldsMod.Shield;

/// <summary>Rebuilds shield max from shield-upgrade cards still in the player's hand.</summary>
internal static class ShieldHandRebuild
{
    private static Dictionary<string, float> _multiplierByCardName;

    internal static void RegisterTier(CardInfo card, float multiplier)
    {
        if (card == null || string.IsNullOrEmpty(card.cardName))
            return;

        _multiplierByCardName ??= new Dictionary<string, float>(StringComparer.Ordinal);
        _multiplierByCardName[card.cardName] = multiplier;
    }

    internal static void RebuildForPlayer(int playerID)
    {
        if (ShieldManager.instance == null || PlayerManager.instance == null)
            return;

        Player player = PlayerManager.instance.players.Find(p => p.playerID == playerID);
        if (player?.data?.currentCards == null)
            return;

        float max = ShieldState.DefaultMax;
        int tiersFound = 0;

        if (_multiplierByCardName != null)
        {
            foreach (CardInfo card in player.data.currentCards)
            {
                if (card == null)
                    continue;

                string name = card.cardName;
                if (string.IsNullOrEmpty(name) && card.sourceCard != null)
                    name = card.sourceCard.cardName;

                if (!string.IsNullOrEmpty(name) && _multiplierByCardName.TryGetValue(name, out float mult))
                {
                    max *= mult;
                    tiersFound++;
                }
            }
        }

        ShieldState state = ShieldManager.instance.GetOrCreateShield(playerID);
        float oldMax = state.Max;

        if (tiersFound == 0)
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

        SLog.Line($"ShieldHandRebuild player={playerID} tiers={tiersFound} max {oldMax:F0} -> {state.Max:F0} current={state.Current:F0}");
    }
}
