using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnboundLib;
using UnboundLib.Utils;
using DeckBuilder.GameIntegration;

namespace DeckBuilder.Data
{
    public class DeckManager : MonoBehaviour
    {
        public static DeckManager instance;

        private DeckCollection _collection = new DeckCollection();
        private string _savePath;

        // RuntimeDeck: per-player card pool for the current game session (not persisted).
        // Key = playerID, Value = list of CardInfo that shrinks as cards are picked.
        public static Dictionary<int, List<CardInfo>> RuntimeDecks { get; } = new Dictionary<int, List<CardInfo>>();

        // Display name for each player's active deck this match (synced in multiplayer).
        public static Dictionary<int, string> RuntimeDeckNames { get; } = new Dictionary<int, string>();

        public DeckData ActiveDeck
        {
            get
            {
                if (_collection.decks.Count == 0) return null;
                if (_collection.activeDeckName == DefaultDeckName) return null;
                var named = _collection.decks.FirstOrDefault(d => d.name == _collection.activeDeckName);
                return named ?? _collection.decks[0];
            }
        }

        /// <summary>The name of the currently active deck, or DefaultDeckName if using vanilla.</summary>
        public string ActiveDeckName
        {
            get => string.IsNullOrEmpty(_collection.activeDeckName) ? DefaultDeckName : _collection.activeDeckName;
        }

        public List<DeckData> AllDecks
        {
            get
            {
                RTGLog.Line($"AllDecks accessed: _collection is {(_collection == null ? "NULL" : "valid")}, decks.Count={_collection?.decks?.Count ?? -1}");
                return _collection?.decks ?? new List<DeckData>();
            }
        }

        private void Awake()
        {
            RTGLog.Section("DeckManager — Awake");
            if (instance != null && instance != this)
            {
                RTGLog.Warn("Duplicate DeckManager instance destroyed.");
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);

            _savePath = Path.Combine(BepInEx.Paths.ConfigPath, "RoundsTheGathering", "decks.json");
            Directory.CreateDirectory(Path.GetDirectoryName(_savePath));
            RTGLog.Line($"SavePath={_savePath}");
            Load();
            RTGLog.Line($"Ready. Decks={_collection.decks.Count} Active='{_collection.activeDeckName ?? "(none)"}'");
        }

        public void Load()
        {
            RTGLog.Section("DeckManager — Load");
            RTGLog.Line($"SavePath={_savePath}");
            RTGLog.Line($"File.Exists={File.Exists(_savePath)}");

            if (!File.Exists(_savePath))
            {
                _collection = new DeckCollection();
                RTGLog.Line($"No save file at {_savePath}");
                return;
            }
            try
            {
                string json = File.ReadAllText(_savePath);
                RTGLog.Line($"Read {json.Length} chars from {_savePath}");

                // Log a snippet of the JSON for debugging
                string snippet = json.Length > 300 ? json.Substring(0, 300) + "..." : json;
                RTGLog.Line($"JSON snippet: {snippet}");

                // Use manual JSON parsing (JsonUtility fails on nested arrays)
                _collection = DeckSaveMapper.FromJson(json);

                int totalCards = _collection.decks.Sum(d => d.TotalCount);
                RTGLog.Line($"Loaded {_collection.decks.Count} deck(s), {totalCards} card entries. Active='{_collection.activeDeckName}'");

                // Log each deck for debugging
                foreach (var deck in _collection.decks)
                {
                    RTGLog.Line($"  Deck '{deck.name}': maxSize={deck.maxSize}, cards={deck.cards.Count}, total={deck.TotalCount}");
                }

                if (_collection.decks.Count == 0 && json.Length > 50)
                    RTGLog.Warn("Save file has content but no decks parsed — check decks.json format.");
            }
            catch (System.Exception ex)
            {
                RTGLog.Error($"Load failed: {ex.Message}");
                RTGLog.Error($"Stack: {ex.StackTrace}");
                _collection = new DeckCollection();
            }
        }

        public void Save()
        {
            RTGLog.Section("DeckManager — Save");
            try
            {
                RTGLog.Line($"_collection has {_collection.decks.Count} deck(s), activeDeckName='{_collection.activeDeckName}'");
                
                // Use manual JSON serialization (JsonUtility fails on nested arrays)
                string json = DeckSaveMapper.ToJson(_collection);
                RTGLog.Line($"JSON length={json.Length}");
                
                // Log first 500 chars of JSON for debugging
                string snippet = json.Length > 500 ? json.Substring(0, 500) + "..." : json;
                RTGLog.Line($"JSON content: {snippet}");
                
                File.WriteAllText(_savePath, json);

                int totalCards = _collection.decks.Sum(d => d.TotalCount);
                RTGLog.Line($"Wrote {_collection.decks.Count} deck(s), {totalCards} card entries to {_savePath}");
            }
            catch (System.Exception ex)
            {
                RTGLog.Error($"Save failed: {ex.Message}");
                RTGLog.Error($"Stack: {ex.StackTrace}");
            }
        }

        public DeckData CreateDeck(string deckName, int maxSize)
        {
            RTGLog.Section($"DeckManager — CreateDeck '{deckName}'");
            var deck = new DeckData { name = deckName, maxSize = maxSize };
            _collection.decks.Add(deck);
            if (_collection.activeDeckName == null)
                _collection.activeDeckName = deckName;
            Save();
            RTGLog.Line($"Created maxSize={maxSize} active={_collection.activeDeckName}");
            return deck;
        }

        public void DeleteDeck(string deckName)
        {
            RTGLog.Section($"DeckManager — DeleteDeck '{deckName}'");
            _collection.decks.RemoveAll(d => d.name == deckName);
            if (_collection.activeDeckName == deckName)
                _collection.activeDeckName = _collection.decks.Count > 0 ? _collection.decks[0].name : null;
            Save();
            RTGLog.Line($"Remaining decks={_collection.decks.Count} active='{_collection.activeDeckName ?? "(none)"}'");
        }

        public void SetActiveDeck(string deckName)
        {
            RTGLog.Section($"DeckManager — SetActiveDeck '{deckName}'");
            _collection.activeDeckName = deckName;
            Save();
        }

        /// <summary>
        /// Name used to represent the "Default Deck" (all cards available).
        /// When this is active, the player uses vanilla card pool behavior.
        /// </summary>
        public const string DefaultDeckName = "Default Deck";

        /// <summary>Returns the active deck for a given player, or null for vanilla fallback.</summary>
        public static DeckData GetActiveDeckForPlayer(Player player)
        {
            if (instance == null) return null;

            // Only the local human player uses a custom deck
            // In single-player vs bots, only playerID 0 is human
            // In multiplayer, check PhotonView.IsMine
            if (!IsLocalHumanPlayer(player))
            {
                RTGLog.Line($"GetActiveDeckForPlayer: player {player.playerID} is not local human — vanilla fallback.");
                return null;
            }

            // Check if "Default Deck" is selected (vanilla behavior)
            if (string.IsNullOrEmpty(instance._collection.activeDeckName) ||
                instance._collection.activeDeckName == DefaultDeckName)
            {
                RTGLog.Line($"GetActiveDeckForPlayer: player {player.playerID} has Default Deck selected — vanilla fallback.");
                return null;
            }

            return instance.ActiveDeck;
        }

        private static bool IsLocalHumanPlayer(Player player)
        {
            if (player == null)
            {
                RTGLog.Line("IsLocalHumanPlayer: player is null, returning false");
                return false;
            }

            // Check if we're in offline/local mode (no room or not connected)
            bool isOffline = !Photon.Pun.PhotonNetwork.IsConnected || 
                             Photon.Pun.PhotonNetwork.CurrentRoom == null ||
                             Photon.Pun.PhotonNetwork.OfflineMode;

            if (isOffline)
            {
                // In offline mode (vs bots), only playerID 0 is the human player
                bool isHuman = player.playerID == 0;
                RTGLog.Line($"IsLocalHumanPlayer: OFFLINE mode, player {player.playerID}, isHuman={isHuman}");
                return isHuman;
            }

            // In online multiplayer, use PhotonView to check ownership
            var pv = player.GetComponent<Photon.Pun.PhotonView>();
            if (pv != null)
            {
                bool isMine = pv.IsMine;
                RTGLog.Line($"IsLocalHumanPlayer: ONLINE mode, player {player.playerID} has PhotonView, IsMine={isMine}");
                return isMine;
            }

            // Fallback: only playerID 0
            RTGLog.Line($"IsLocalHumanPlayer: Fallback, player {player.playerID}, using playerID==0");
            return player.playerID == 0;
        }

        /// <summary>
        /// Expands a DeckData into an array of CardInfo, duplicating entries per their count.
        /// Rarity cap: Trinket/Common/Scarce max 3, Uncommon/Rare/Exotic max 2, Epic+ max 1.
        /// Note: this builds the FULL pool including locked cards. Unlock gating is
        /// applied per-pick from the player's inventory (see DeckPickPatch), so locked
        /// cards remain in the RuntimeDeck and become draftable once their prerequisite
        /// is owned.
        /// </summary>
        public static CardInfo[] BuildCardPool(DeckData deck)
        {
            RTGLog.Section("BuildCardPool");
            var result = new List<CardInfo>();

            foreach (var entry in deck.cards)
            {
                CardInfo ci = CardManager.GetCardInfoWithName(entry.cardObjectName);
                if (ci == null)
                {
                    RTGLog.Warn($"BuildCardPool: unknown card '{entry.cardObjectName}', skipping.");
                    continue;
                }

                if (ModCardVisibilityBridge.IsHidden(ci))
                {
                    RTGLog.Line($"BuildCardPool: excluded hidden card '{ci.cardName}', skipping.");
                    continue;
                }

                int maxCount = RarityDeckLimits.MaxCountFor(ci);
                int clampedCount = Mathf.Clamp(entry.count, 0, maxCount);
                for (int i = 0; i < clampedCount; i++)
                    result.Add(ci);
            }

            RTGLog.Line($"BuildCardPool: deck='{deck.name}' pool={result.Count} cards.");
            return result.ToArray();
        }

        public static int MaxCountForCard(CardInfo card) => RarityDeckLimits.MaxCountFor(card);

        /// <summary>Rebuilds all RuntimeDecks from each player's active deck at game start.</summary>
        public static void InitRuntimeDecks()
        {
            RTGLog.Section("InitRuntimeDecks");
            RuntimeDecks.Clear();
            RuntimeDeckNames.Clear();
            foreach (var player in PlayerManager.instance.players)
            {
                DeckData deck = GetActiveDeckForPlayer(player);
                if (deck == null)
                {
                    RTGLog.Line($"Player {player.playerID} — no deck, vanilla fallback.");
                    continue;
                }
                CardInfo[] pool = BuildCardPool(deck);
                RuntimeDecks[player.playerID] = new List<CardInfo>(pool);
                SetRuntimeDeckName(player.playerID, deck.name);
                RTGLog.Line($"Player {player.playerID} — runtime deck '{deck.name}' built: {pool.Length} cards.");
            }
        }

        /// <summary>Removes one instance of cardInfo from the RuntimeDeck for playerID.</summary>
        public static void ConsumeCard(int playerID, CardInfo card)
        {
            RTGLog.Section($"DeckManager — ConsumeCard player={playerID}");
            if (!RuntimeDecks.ContainsKey(playerID))
            {
                RTGLog.Warn($"No RuntimeDeck for player {playerID}.");
                return;
            }
            var deck = RuntimeDecks[playerID];

            // Strip "(Clone)" suffix that Unity adds when instantiating
            string cardName = card.gameObject.name;
            if (cardName.EndsWith("(Clone)"))
                cardName = cardName.Substring(0, cardName.Length - 7);

            RTGLog.Line($"Looking for card '{cardName}' (original: '{card.gameObject.name}')");

            int idx = deck.FindIndex(c => {
                string deckCardName = c.gameObject.name;
                if (deckCardName.EndsWith("(Clone)"))
                    deckCardName = deckCardName.Substring(0, deckCardName.Length - 7);
                return deckCardName == cardName;
            });

            if (idx >= 0)
            {
                deck.RemoveAt(idx);
                RTGLog.Line($"Removed '{cardName}' — remaining={deck.Count}");
            }
            else
            {
                RTGLog.Warn($"Card '{cardName}' not found in RuntimeDeck (count={deck.Count}).");
                // Log first few cards in deck for debugging
                for (int i = 0; i < System.Math.Min(5, deck.Count); i++)
                {
                    RTGLog.Line($"  RuntimeDeck[{i}] = '{deck[i].gameObject.name}'");
                }
            }
        }

        public static int GetRemainingCount(int playerID)
        {
            if (!RuntimeDecks.ContainsKey(playerID)) return -1;
            return RuntimeDecks[playerID].Count;
        }

        public static void SetRuntimeDeckName(int playerID, string deckName)
        {
            if (string.IsNullOrEmpty(deckName))
                RuntimeDeckNames.Remove(playerID);
            else
                RuntimeDeckNames[playerID] = deckName;
        }

        public static string GetRuntimeDeckName(int playerID)
        {
            if (RuntimeDeckNames.TryGetValue(playerID, out string name))
                return name;
            return null;
        }
    }
}
