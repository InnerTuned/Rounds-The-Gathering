using System.Collections.Generic;
using UnboundLib;
using UnboundLib.Utils;
using UnboundLib.Networking;
using UnityEngine;
using DeckBuilder.Data;
using DeckBuilder.GameIntegration;

namespace DeckBuilder.Networking
{
    /// <summary>
    /// Handles multiplayer synchronization of deck pools and remaining card counts.
    /// In offline mode the RPCs fire locally without Photon.
    /// </summary>
    public static class DeckNetworkSync
    {
        // Pending pools received via RPC, keyed by playerID.
        // The DeckPickPatch reads from here if it's not the local player's own pool.
        public static readonly Dictionary<int, CardInfo[]> PendingPools =
            new Dictionary<int, CardInfo[]>();

        // ── Outbound ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Called on the local client just before a player's pick begins.
        /// Broadcasts the current deck pool to all other clients, then sets
        /// CardChoice.instance.cards locally.
        /// </summary>
        public static void SyncDeckAndSwapPool(Player player, int pickerID, CardInfo[] pool, string deckName)
        {
            RTGLog.Section($"DeckNetworkSync — SyncDeckAndSwapPool pickerID={pickerID} poolSize={pool.Length}");
            DeckManager.SetRuntimeDeckName(pickerID, deckName);

            string[] names = System.Array.ConvertAll(pool, ci => ci.gameObject.name);
            string deckJson = string.Join(",", names);
            RTGLog.Line($"RPC URPC_SyncDeck deck='{deckName}' payload length={deckJson.Length} chars, {names.Length} card(s).");

            NetworkingManager.RPC(typeof(DeckNetworkSync), nameof(URPC_SyncDeck),
                pickerID, deckJson, deckName ?? "");
        }

        /// <summary>Broadcast remaining deck count to all clients after a pick.</summary>
        public static void BroadcastRemainingCount(int playerID, int remaining)
        {
            RTGLog.Section($"DeckNetworkSync — BroadcastRemainingCount player={playerID}");
            RTGLog.Line($"remaining={remaining}");
            NetworkingManager.RPC(typeof(DeckNetworkSync), nameof(URPC_UpdateDeckCount),
                playerID, remaining);
        }

        // ── Inbound RPCs ──────────────────────────────────────────────────────────

        /// <summary>
        /// Received on ALL clients. Rebuilds the card pool from the name list and stores it
        /// in PendingPools so this client can swap CardChoice.cards at the right moment.
        /// On the sending client this also sets CardChoice.instance.cards directly.
        /// </summary>
        [UnboundRPC]
        public static void URPC_SyncDeck(int pickerID, string cardNamesCsv, string deckName)
        {
            RTGLog.Section($"DeckNetworkSync — URPC_SyncDeck pickerID={pickerID}");
            if (!string.IsNullOrEmpty(deckName))
                DeckManager.SetRuntimeDeckName(pickerID, deckName);

            var pool = new List<CardInfo>();
            foreach (string name in cardNamesCsv.Split(','))
            {
                if (string.IsNullOrWhiteSpace(name)) continue;
                CardInfo ci = CardManager.GetCardInfoWithName(name.Trim());
                if (ci != null)
                    pool.Add(ci);
                else
                    RTGLog.Warn($"URPC_SyncDeck: unknown card '{name}'");
            }

            PendingPools[pickerID] = pool.ToArray();

            // Also update the RuntimeDeck on remote clients so they track the pool size
            if (!DeckManager.RuntimeDecks.ContainsKey(pickerID))
                DeckManager.RuntimeDecks[pickerID] = pool;

            RTGLog.Line($"URPC_SyncDeck: stored pool of {pool.Count} cards for player {pickerID}.");

            // On every client, immediately swap CardChoice.cards so the spawned draft
            // cards are drawn from the deck pool
            if (CardChoice.instance != null)
            {
                CardChoice.instance.cards = pool.ToArray();
                RTGLog.Line($"URPC_SyncDeck: swapped CardChoice.cards on this client.");
            }
        }

        /// <summary>
        /// Received on ALL clients. Updates the DeckHUDOverlay with the remaining count.
        /// </summary>
        [UnboundRPC]
        public static void URPC_UpdateDeckCount(int playerID, int remaining)
        {
            RTGLog.Section($"DeckNetworkSync — URPC_UpdateDeckCount player={playerID}");
            RTGLog.Line($"remaining={remaining}");

            if (DeckManager.RuntimeDecks.TryGetValue(playerID, out var list))
            {
                int trimmed = 0;
                while (list.Count > remaining)
                {
                    list.RemoveAt(list.Count - 1);
                    trimmed++;
                }
                if (trimmed > 0)
                    RTGLog.Line($"Trimmed RuntimeDeck by {trimmed} to match broadcast count.");
            }
            else
            {
                RTGLog.Warn($"No RuntimeDeck for player {playerID} on this client.");
            }

            DeckHUDOverlay.OnDeckCountUpdated(playerID, remaining);
        }
    }
}
