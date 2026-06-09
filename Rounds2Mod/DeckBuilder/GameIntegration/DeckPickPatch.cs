using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnboundLib;
using UnboundLib.Utils;
using DeckBuilder.Data;
using DeckBuilder.Networking;

namespace DeckBuilder.GameIntegration
{
    [HarmonyPatch]
    internal static class DeckPickPatch
    {
        // The permanent vanilla pool — captured the first time we see it.
        private static CardInfo[] _vanillaPool;

        // Exposed so SpecialCardPatches can restore the pool after a special-card flow.
        internal static CardInfo[] VanillaPool => _vanillaPool;

        // Saved pool to restore after a custom-deck pick.
        private static CardInfo[] _savedPool;

        // ── Capture vanilla pool + set correct pool BEFORE StartPick selects cards ──

        [HarmonyPatch(typeof(CardChoice), nameof(CardChoice.StartPick))]
        [HarmonyPrefix]
        static void StartPick_Prefix(int picksToSet, int pickerIDToSet)
        {
            int pickerID = pickerIDToSet;
            RTGLog.Section($"DeckPickPatch — StartPick_Prefix pickerID={pickerID}");

            // Capture the vanilla pool once, BEFORE any swap can corrupt it.
            if (CardChoice.instance?.cards != null &&
                (_vanillaPool == null || CardChoice.instance.cards.Length > _vanillaPool.Length))
            {
                _vanillaPool = CardChoice.instance.cards;
                RTGLog.Line($"Captured vanilla pool ({_vanillaPool.Length} cards).");
            }

            Player player = PlayerManager.instance?.players?.Find(p => p.playerID == pickerID);

            // Build the set of card names this player already owns (their current hand).
            var ownedNames = new HashSet<string>();
            if (player != null && player.data?.currentCards != null)
            {
                RTGLog.Line($"=== Player {pickerID} Current Hand ({player.data.currentCards.Count} cards) ===");
                for (int i = 0; i < player.data.currentCards.Count; i++)
                {
                    var card = player.data.currentCards[i];
                    string name = card?.cardName ?? "NULL";
                    RTGLog.Line($"  [{i + 1}] \"{name}\"");
                    if (card != null && !string.IsNullOrEmpty(card.cardName))
                        ownedNames.Add(card.cardName);
                }
            }
            else
            {
                RTGLog.Line($"Player {pickerID} has no cards in hand yet.");
            }
            if (player == null)
            {
                RTGLog.Warn($"No player for pickerID={pickerID}");
                return;
            }

            DeckData deck = DeckManager.GetActiveDeckForPlayer(player);

            if (deck == null)
            {
                // Vanilla / bot player — ensure the full vanilla pool is active
                if (_vanillaPool != null)
                {
                    CardChoice.instance.cards = _vanillaPool;
                    RTGLog.Line($"Player {pickerID} (vanilla) — set to vanilla pool ({_vanillaPool.Length} cards).");
                }
                _savedPool = null;
                DeckManager.SetRuntimeDeckName(pickerID, null);
                DeckHUDOverlay.ShowForPicker(pickerID);
                return;
            }

            // Custom deck player — build/get the RuntimeDeck pool
            RTGLog.Line($"Active deck='{deck.name}' entries={deck.cards.Count} total={deck.TotalCount}");
            _savedPool = _vanillaPool ?? CardChoice.instance.cards;

            CardInfo[] pool;
            if (DeckManager.RuntimeDecks.TryGetValue(pickerID, out List<CardInfo> runtimeList) && runtimeList.Count > 0)
            {
                pool = runtimeList.ToArray();
                RTGLog.Line($"Using RuntimeDeck ({pool.Length} cards).");
            }
            else
            {
                RTGLog.Line("RuntimeDeck empty — building from DeckData.");
                pool = DeckManager.BuildCardPool(deck);
                DeckManager.RuntimeDecks[pickerID] = new List<CardInfo>(pool);
                RTGLog.Line($"Initialized RuntimeDeck with {pool.Length} cards.");
            }

            if (pool.Length == 0)
            {
                RTGLog.Warn($"Player {pickerID} deck is empty — vanilla fallback.");
                if (_vanillaPool != null) CardChoice.instance.cards = _vanillaPool;
                _savedPool = null;
                DeckHUDOverlay.ShowForPicker(pickerID);
                return;
            }

            DeckManager.SetRuntimeDeckName(pickerID, deck.name);

            // Gate cards by per-player unlock prerequisites, based on what's in the
            // player's hand this pick. The RuntimeDeck keeps ALL cards (including locked
            // ones) so they become draftable as soon as their prerequisite is owned.
            CardInfo[] draftPool = FilterByPrerequisites(pool, ownedNames, pickerID);

            if (draftPool.Length == 0)
            {
                RTGLog.Warn($"All {pool.Length} deck cards are locked for player {pickerID} — using unfiltered pool as fallback.");
                draftPool = pool;
            }

            // Sync to all clients (in offline mode this runs synchronously and swaps
            // CardChoice.cards locally too, before Pick() spawns the draft cards).
            DeckNetworkSync.SyncDeckAndSwapPool(player, pickerID, draftPool, deck.name);

            // Ensure local pool is set even if the RPC path changes.
            CardChoice.instance.cards = draftPool;
            RTGLog.Line($"Set CardChoice.cards to {draftPool.Length} draftable card(s) (of {pool.Length} in deck) for player {pickerID}.");

            DeckHUDOverlay.ShowForPicker(pickerID);
        }

        /// <summary>
        /// Filters a deck pool down to the cards currently UNLOCKED for the player,
        /// using DeckBuilder's prerequisite registry and the player's owned cards.
        /// Locked cards (prerequisite not yet owned) are logged and excluded from this pick.
        /// </summary>
        private static CardInfo[] FilterByPrerequisites(CardInfo[] pool, HashSet<string> ownedNames, int pickerID)
        {
            var unlocked = new List<CardInfo>();
            var lockedNames = new List<string>();

            foreach (var ci in pool)
            {
                if (ci == null) continue;
                if (CardPrerequisiteRegistry.IsUnlocked(ci.cardName, ownedNames))
                {
                    unlocked.Add(ci);
                }
                else if (!lockedNames.Contains(ci.cardName))
                {
                    lockedNames.Add(ci.cardName);
                }
            }

            if (lockedNames.Count > 0)
            {
                RTGLog.Line($"=== Cards LOCKED this pick for player {pickerID} ({lockedNames.Count}) ===");
                foreach (var name in lockedNames)
                {
                    string req = CardPrerequisiteRegistry.GetPrerequisite(name);
                    RTGLog.Line($"  [LOCKED] \"{name}\" (requires \"{req}\")");
                }
            }
            else
            {
                RTGLog.Line($"No locked cards this pick for player {pickerID}.");
            }

            return unlocked.ToArray();
        }

        // ── Log cards drawn for pick ───────────────────────────────────────────────

        [HarmonyPatch(typeof(CardChoice), "SpawnUniqueCard")]
        [HarmonyPostfix]
        static void SpawnUniqueCard_Postfix(ref GameObject __result, int ___pickrID)
        {
            if (__result == null) return;
            
            var cardInfo = __result.GetComponent<CardInfo>();
            if (cardInfo != null)
            {
                RTGLog.Line($"[DRAWN] Player {___pickrID}: \"{cardInfo.cardName}\" ({cardInfo.rarity})");
            }
        }

        // ── Consume card + restore vanilla pool after pick ────────────────────────

        [HarmonyPatch(typeof(ApplyCardStats), "ApplyStats")]
        [HarmonyPostfix]
        static void ApplyStats_Postfix(ApplyCardStats __instance)
        {
            // Suppress during special-card flows (Copycat, Changed Mind) to avoid
            // incorrectly consuming cards or clobbering the pool mid-flow.
            if (DeckBuilder.Cards.SpecialCardPatches.SuppressApplyStatsPostfix) return;

            CardInfo card = __instance.GetComponentInParent<CardInfo>();
            if (card == null) return;

            int pickerID = CardChoice.instance != null ? CardChoice.instance.pickrID : -1;
            if (pickerID < 0) return;

            RTGLog.Section($"DeckPickPatch — ApplyStats_Postfix card='{card.gameObject.name}' pickerID={pickerID}");

            DeckManager.ConsumeCard(pickerID, card);

            // Restore the vanilla pool so the next player starts fresh
            if (_vanillaPool != null)
            {
                CardChoice.instance.cards = _vanillaPool;
                RTGLog.Line($"Restored vanilla pool ({_vanillaPool.Length} cards).");
            }
            else if (_savedPool != null)
            {
                CardChoice.instance.cards = _savedPool;
                RTGLog.Line($"Restored saved pool ({_savedPool.Length} cards).");
            }
            _savedPool = null;

            int remaining = DeckManager.GetRemainingCount(pickerID);
            if (remaining >= 0)
                DeckNetworkSync.BroadcastRemainingCount(pickerID, remaining);
        }
    }
}
