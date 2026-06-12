using System;
using System.Collections.Generic;
using DeckBuilder.Data;
using RarityLib.Utils;
using UnityEngine;

namespace DeckBuilder.Cards;

/// <summary>
/// Builds the searchable card pool for in-game deck search cards.
/// </summary>
internal static class CardSearchPool
{
    /// <summary>
    /// Returns unique, alphabetically sorted cards still in the player's runtime deck
    /// that are unlocked and optionally capped at Rare rarity.
    /// </summary>
    private static readonly HashSet<string> ExcludedCardNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        CopycatCard.CardDisplayName,
        SwapCard.CardDisplayName,
        RareSearchCard.CardDisplayName,
        LegendarySearchCard.CardDisplayName
    };

    public static List<CardInfo> BuildSearchableCards(int playerID, bool capAtRare, string excludeCardName = null)
    {
        var results = new List<CardInfo>();
        if (!DeckManager.RuntimeDecks.TryGetValue(playerID, out var runtimeDeck)
            || runtimeDeck == null
            || runtimeDeck.Count == 0)
        {
            SearchCardLog.Line($"No runtime deck for player {playerID}.");
            return results;
        }

        var ownedNames = GetOwnedCardNames(playerID);
        var seenNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var card in runtimeDeck)
        {
            if (card == null || string.IsNullOrEmpty(card.cardName))
                continue;

            if (!CardPrerequisiteRegistry.IsUnlocked(card.cardName, ownedNames))
                continue;

            if (capAtRare && !IsRareOrLess(card))
                continue;

            if (ExcludedCardNames.Contains(card.cardName))
                continue;

            if (!string.IsNullOrEmpty(excludeCardName)
                && string.Equals(card.cardName, excludeCardName, StringComparison.OrdinalIgnoreCase))
                continue;

            if (!seenNames.Add(card.cardName))
                continue;

            results.Add(card);
        }

        SortAlphabetically(results);
        SearchCardLog.Line(
            $"Built searchable pool for player {playerID}: {results.Count} card(s), capAtRare={capAtRare}.");
        return results;
    }

    /// <summary>
    /// A card is Rare or lower when its relative rarity is at least as common as Rare.
    /// </summary>
    public static bool IsRareOrLess(CardInfo card)
    {
        if (card == null)
            return false;

        try
        {
            float rareThreshold = RarityUtils.GetRarityData(CardInfo.Rarity.Rare).relativeRarity;
            float cardRelative = RarityUtils.GetRarityData(card.rarity).relativeRarity;
            return cardRelative >= rareThreshold;
        }
        catch (Exception ex)
        {
            SearchCardLog.Warn($"Rarity compare failed for '{card.cardName}': {ex.Message}");
            return (int)card.rarity <= (int)CardInfo.Rarity.Rare;
        }
    }

    private static HashSet<string> GetOwnedCardNames(int playerID)
    {
        var owned = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        Player player = PlayerManager.instance?.players?.Find(p => p.playerID == playerID);
        if (player?.data?.currentCards == null)
            return owned;

        foreach (var card in player.data.currentCards)
        {
            if (card != null && !string.IsNullOrEmpty(card.cardName))
                owned.Add(card.cardName);
        }

        return owned;
    }

    private static void SortAlphabetically(List<CardInfo> cards)
    {
        cards.Sort((a, b) => string.Compare(
            a?.cardName, b?.cardName, StringComparison.OrdinalIgnoreCase));
    }
}
