using System.Collections;
using TMPro;
using UnityEngine;
using UnboundLib;
using UnboundLib.GameModes;
using DeckBuilder.Data;
using DeckBuilder.UI;

namespace DeckBuilder.GameIntegration
{
    /// <summary>
    /// Displays a "Current Deck: #" counter during the card pick phase.
    /// Visible on all clients; tracks the current picker's remaining deck count.
    /// </summary>
    public class DeckHUDOverlay : MonoBehaviour
    {
        public static DeckHUDOverlay instance;

        private Canvas _canvas;
        private TextMeshProUGUI _countText;

        // The playerID of the player whose pick is currently active.
        private int _currentPickerID = -1;

        private void Awake()
        {
            RTGLog.Section("DeckHUDOverlay — Awake");
            instance = this;
            DontDestroyOnLoad(gameObject);
            BuildHUD();
            SetVisible(false);

            GameModeManager.AddHook(GameModeHooks.HookPlayerPickStart, OnPickStart);
            GameModeManager.AddHook(GameModeHooks.HookPlayerPickEnd, OnPickEnd);
            RTGLog.Line("Registered HookPlayerPickStart / HookPlayerPickEnd.");
        }

        // ── Build ─────────────────────────────────────────────────────────────────

        private void BuildHUD()
        {
            _canvas = UIHelper.CreateFullscreenCanvas("RTG_DeckHUDOverlay", sortOrder: 50);

            // Bottom-center panel
            var panel = UIHelper.CreatePanel(_canvas.transform, "HUDPanel",
                new Vector2(0.35f, 0.02f), new Vector2(0.65f, 0.08f),
                bg: new Color(0f, 0f, 0f, 0.65f));

            _countText = UIHelper.CreateText(panel, "CountText",
                "Deck: —", fontSize: 22,
                alignment: TextAlignmentOptions.Center);
            var tRt = _countText.GetComponent<RectTransform>();
            tRt.anchorMin = Vector2.zero;
            tRt.anchorMax = Vector2.one;
            tRt.offsetMin = tRt.offsetMax = Vector2.zero;
        }

        // ── Game hooks ────────────────────────────────────────────────────────────

        private IEnumerator OnPickStart(IGameModeHandler gm)
        {
            // Note: This hook fires too early — the actual HUD display is triggered
            // from DeckPickPatch.Show_Prefix via ShowForPicker().
            yield break;
        }

        private IEnumerator OnPickEnd(IGameModeHandler gm)
        {
            RTGLog.Section("DeckHUDOverlay — Pick End");
            _currentPickerID = -1;
            SetVisible(false);
            yield break;
        }

        // ── Public API for Harmony patches ──────────────────────────────────────────

        /// <summary>Called from DeckPickPatch when we know the actual pickerID.</summary>
        public static void ShowForPicker(int pickerID)
        {
            if (instance == null) return;

            RTGLog.Section($"DeckHUDOverlay — ShowForPicker pickerID={pickerID}");

            instance._currentPickerID = pickerID;

            int remaining = DeckManager.GetRemainingCount(pickerID);
            string deckName = DeckManager.GetRuntimeDeckName(pickerID);
            RTGLog.Line($"Showing HUD. deck='{deckName ?? "(none)"}' remaining={remaining}");
            instance.RefreshDisplay(pickerID);
            instance.SetVisible(true);
        }

        /// <summary>Called when the pick phase ends to hide the HUD.</summary>
        public static void HideHUD()
        {
            if (instance == null) return;
            instance._currentPickerID = -1;
            instance.SetVisible(false);
        }

        // ── Static callback from network sync ─────────────────────────────────────

        /// <summary>Called on all clients when a player's remaining deck count changes.</summary>
        public static void OnDeckCountUpdated(int playerID, int remaining)
        {
            if (instance == null) return;
            if (instance._currentPickerID != playerID) return;
            RTGLog.Line($"DeckHUDOverlay — count update player={playerID} remaining={remaining}");
            instance.RefreshDisplay(playerID, remaining);
        }

        // ── Display update ────────────────────────────────────────────────────────

        private void RefreshDisplay(int playerID, int? overrideCount = null)
        {
            int count = overrideCount ?? DeckManager.GetRemainingCount(playerID);
            string deckName = DeckManager.GetRuntimeDeckName(playerID);

            if (count < 0)
            {
                _countText.text = string.IsNullOrEmpty(deckName)
                    ? "All cards"
                    : $"{deckName} — all cards";
                _countText.color = new Color(0.6f, 0.6f, 0.6f);
                return;
            }

            _countText.text = string.IsNullOrEmpty(deckName)
                ? $"Current Deck: {count}"
                : $"{deckName} — {count} cards";

            _countText.color = count <= 5
                ? new Color(1f, 0.4f, 0.4f)
                : Color.white;
        }

        private void SetVisible(bool visible)
        {
            if (_canvas != null)
                _canvas.gameObject.SetActive(visible);
        }

        // ── Update ────────────────────────────────────────────────────────────────

        private void Update()
        {
            // Keep display in sync every frame during pick phase (covers local-only play)
            if (_currentPickerID >= 0 && _canvas != null && _canvas.gameObject.activeSelf)
                RefreshDisplay(_currentPickerID);
        }
    }
}
