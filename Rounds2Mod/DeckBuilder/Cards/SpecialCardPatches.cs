using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
// Note: 'ModdingUtils.Utils.Cards' must be referenced by full name inside namespace
// DeckBuilder.Cards to avoid ambiguity with the DeckBuilder.Cards namespace itself.
using UnboundLib;
using UnboundLib.Networking;
using UnityEngine;
using DeckBuilder.CardDelete;
using DeckBuilder.Data;
using DeckBuilder.GameIntegration;
using DeckBuilder.Networking;

namespace DeckBuilder.Cards
{
    /// <summary>
    /// Harmony patches that intercept <see cref="ApplyCardStats.ApplyStats"/> for
    /// Copycat and Swap, blocking normal card-add and orchestrating the
    /// player-facing card-bar selection flow.
    /// </summary>
    [HarmonyPatch]
    public static class SpecialCardPatches
    {
        // ── Flags ─────────────────────────────────────────────────────────────────

        /// <summary>True while either Copycat or Swap selector UI is open.</summary>
        public static bool IsSpecialFlowActive;

        /// <summary>
        /// Set to true around AddCardToPlayer calls during Copycat confirm so that
        /// DeckPickPatch.ApplyStats_Postfix does not consume the cloned card from the
        /// runtime deck or clobber the pool.
        /// </summary>
        public static bool SuppressApplyStatsPostfix;

        // ── Per-flow state ─────────────────────────────────────────────────────────

        private static bool _isCopycatMode;
        private static int  _pickerID = -1;
        private static CardInfo _specialCard;
        private static Player   _pickerPlayer;

        // ── Reflection cache ──────────────────────────────────────────────────────

        private static readonly FieldInfo s_isPlayingField =
            AccessTools.Field(typeof(CardChoice), "isPlaying");
        private static readonly FieldInfo s_picksField =
            AccessTools.Field(typeof(CardChoice), "picks");
        private static readonly FieldInfo s_spawnedField =
            AccessTools.Field(typeof(CardChoice), "spawned");
        private static readonly MethodInfo s_rpca_assignCard =
            AccessTools.Method(typeof(ModdingUtils.Utils.Cards), "RPCA_AssignCard",
                new[] { typeof(string), typeof(int), typeof(bool), typeof(string), typeof(float), typeof(float), typeof(bool) });

        // ── Harmony: intercept ApplyStats for special cards ───────────────────────

        [HarmonyPatch(typeof(ApplyCardStats), "ApplyStats")]
        [HarmonyPrefix]
        static bool ApplyStats_Prefix(ApplyCardStats __instance)
        {
            if (CardChoice.instance == null || !CardChoice.instance.IsPicking) return true;

            CardInfo card = __instance.GetComponentInParent<CardInfo>();
            if (card == null) return true;

            int pickerID = CardChoice.instance.pickrID;
            if (!IsLocalPicker(pickerID)) return true;

            if (card.cardName == CopycatCard.CardDisplayName)
            {
                CopycatLog.Section("ApplyStats intercepted");
                CopycatLog.Line($"pickerID={pickerID}, cardObject='{card.gameObject.name}'");
                StartFlow(pickerID, card, isCopycat: true);
                return false;
            }

            if (card.cardName == SwapCard.CardDisplayName)
            {
                SwapLog.Section("ApplyStats intercepted");
                SwapLog.Line($"pickerID={pickerID}, cardObject='{card.gameObject.name}'");
                StartFlow(pickerID, card, isCopycat: false);
                return false;
            }

            return true;
        }

        // ── Harmony: block end-pick animation while UI is open ────────────────────

        [HarmonyPatch(typeof(CardChoice), "RPCA_DoEndPick")]
        [HarmonyPrefix]
        static bool RPCA_DoEndPick_Prefix()
        {
            if (!IsSpecialFlowActive) return true;
            LogLine(_isCopycatMode, "RPCA_DoEndPick suppressed (selector UI open).");
            return false;
        }

        // ── Harmony: prevent special card icons appearing in the card bar ─────────

        [HarmonyPatch(typeof(CardBarHandler), "AddCard")]
        [HarmonyPrefix]
        static bool CardBarHandler_AddCard_Prefix(CardInfo card)
        {
            if (!IsSpecialFlowActive) return true;
            if (card == null) return true;
            if (card.cardName == CopycatCard.CardDisplayName ||
                card.cardName == SwapCard.CardDisplayName)
            {
                if (card.cardName == CopycatCard.CardDisplayName)
                    CopycatLog.Line($"Blocked card-bar add for '{card.cardName}'.");
                else
                    SwapLog.Line($"Blocked card-bar add for '{card.cardName}'.");
                return false;
            }
            return true;
        }

        // ── Flow entry ────────────────────────────────────────────────────────────

        private static void StartFlow(int pickerID, CardInfo specialCard, bool isCopycat)
        {
            _isCopycatMode = isCopycat;
            _pickerID      = pickerID;
            _specialCard   = specialCard;
            _pickerPlayer  = PlayerManager.instance?.players?.Find(p => p.playerID == pickerID);

            LogSection(isCopycat, "StartFlow");
            LogLine(isCopycat, $"pickerID={pickerID}, specialCard='{specialCard.cardName}', object='{specialCard.gameObject.name}'");
            LogLine(isCopycat, $"pickerHandCount={_pickerPlayer?.data?.currentCards?.Count ?? 0}");

            // Guard: check if there are any cards to select
            bool hasSelectableCards = false;
            if (isCopycat)
            {
                // Copycat can copy from ANY player — check if any player has cards
                foreach (var p in PlayerManager.instance?.players ?? new List<Player>())
                {
                    int count = p?.data?.currentCards?.Count ?? 0;
                    LogLine(true, $"Player {p?.playerID ?? -1} handCount={count}");
                    if (count > 0)
                    {
                        hasSelectableCards = true;
                    }
                }
            }
            else
            {
                // Swap can only delete from own hand
                hasSelectableCards = _pickerPlayer?.data?.currentCards != null
                    && _pickerPlayer.data.currentCards.Count > 0;
            }

            if (!hasSelectableCards)
                LogWarn(isCopycat, "No selectable cards — showing selector UI; [Select] stays disabled until Cancel.");

            IsSpecialFlowActive = true;
            string msg = isCopycat ? "Select any card to duplicate" : "Select any card to delete";
            var mode = isCopycat ? CardSelectMode.CopyFromAny : CardSelectMode.DeleteFromOwn;
            LogLine(isCopycat, $"Opening selector UI (mode={mode}, message='{msg}').");
            CardBarSelectorUI.instance?.Show(pickerID, msg, mode,
                onConfirm: OnConfirm,
                onCancel:  OnCancel);
        }

        // ── Confirm / Cancel ──────────────────────────────────────────────────────

        private static void OnConfirm(CardInfo selectedCard)
        {
            LogSection(_isCopycatMode, "Confirm");
            LogLine(_isCopycatMode, $"selectedCard='{selectedCard?.cardName ?? "null"}', object='{selectedCard?.gameObject.name ?? "null"}'");
            IsSpecialFlowActive = false;
            if (_isCopycatMode)
                DoCopycatConfirm(selectedCard);
            else
                DoSwapConfirm(selectedCard);
        }

        private static void OnCancel()
        {
            LogSection(_isCopycatMode, "Cancel");
            IsSpecialFlowActive = false;

            CardChoice cc = CardChoice.instance;
            if (cc == null)
            {
                LogWarn(_isCopycatMode, "CardChoice.instance is null — cannot restore state.");
                return;
            }

            // Stop the IDoEndPick coroutine that started when the card was clicked.
            // This prevents NullReferenceExceptions and ensures clean state.
            cc.StopAllCoroutines();
            LogLine(_isCopycatMode, "Stopped CardChoice coroutines.");

            // Restore pickrID so the player can select cards again.
            // When the card was clicked, pickrID was set to -1 which disables DoPlayerSelect().
            cc.pickrID = _pickerID;
            LogLine(_isCopycatMode, $"Restored CardChoice.pickrID = {_pickerID}");

            // Reset the isPlaying field so picks can proceed.
            if (s_isPlayingField != null)
            {
                s_isPlayingField.SetValue(cc, false);
                LogLine(_isCopycatMode, "Reset CardChoice.isPlaying = false");
            }

            // Ensure IsPicking is true so Update() processes clicks.
            cc.IsPicking = true;
            LogLine(_isCopycatMode, "Ensured CardChoice.IsPicking = true");

            // Reset the "done" flag on the special card so it can be picked again.
            // ApplyCardStats.Pick() sets done=true before calling ApplyStats().
            if (_specialCard != null)
            {
                var applyStats = _specialCard.GetComponentInChildren<ApplyCardStats>();
                if (applyStats != null)
                {
                    var doneField = AccessTools.Field(typeof(ApplyCardStats), "done");
                    doneField?.SetValue(applyStats, false);
                    LogLine(_isCopycatMode, "Reset ApplyCardStats.done on the special card.");
                }
            }

            // Reset "done" on ALL spawned draft cards to ensure they're pickable.
            var spawned = s_spawnedField?.GetValue(cc) as System.Collections.IList;
            if (spawned != null)
            {
                int resetCount = 0;
                foreach (var obj in spawned)
                {
                    var go = obj as GameObject;
                    if (go == null) continue;
                    var stats = go.GetComponentInChildren<ApplyCardStats>();
                    if (stats == null) continue;
                    var doneField = AccessTools.Field(typeof(ApplyCardStats), "done");
                    if (doneField != null && (bool)doneField.GetValue(stats))
                    {
                        doneField.SetValue(stats, false);
                        resetCount++;
                    }
                }
                if (resetCount > 0)
                    LogLine(_isCopycatMode, $"Reset ApplyCardStats.done on {resetCount} spawned card(s).");
            }

            LogLine(_isCopycatMode, "Selector closed — resuming current draft hand (no consume, no redraw).");
        }

        // ── Copycat confirm: clone selected card, end pick phase ──────────────────

        private static void DoCopycatConfirm(CardInfo selectedCard)
        {
            CopycatLog.Section("Confirm — clone card");
            if (_pickerPlayer != null && selectedCard != null)
            {
                try
                {
                    CopycatLog.Line($"Cloning '{selectedCard.cardName}' (object='{selectedCard.gameObject.name}') for player {_pickerID}.");
                    // Suppress the DeckPickPatch postfix so AddCardToPlayer doesn't
                    // try to consume the clone from the runtime deck or restore the pool.
                    SuppressApplyStatsPostfix = true;
                    ModdingUtils.Utils.Cards.instance.AddCardToPlayer(_pickerPlayer, selectedCard,
                        reassign: true, twoLetterCode: "", forceDisplay: 0f, forceDisplayDelay: 0f, addToCardBar: true);
                    SuppressApplyStatsPostfix = false;
                    CopycatLog.Line($"Clone applied. pickerHandCount={_pickerPlayer.data.currentCards?.Count ?? 0}");
                }
                catch (Exception ex)
                {
                    SuppressApplyStatsPostfix = false;
                    CopycatLog.Error($"AddCardToPlayer failed: {ex}");
                }
            }
            else
            {
                CopycatLog.Warn("Confirm called with null player or selected card.");
            }

            ConsumeSpecialCard();
            CopycatLog.Line("Ending pick phase.");
            CardDeleteManager.EndPickPhase();
            BroadcastRemainingCount();
        }

        // ── Swap confirm: delete selected card, then redraw ───────────────────────

        private static void DoSwapConfirm(CardInfo selectedCard)
        {
            SwapLog.Section("Confirm — delete card");
            if (selectedCard != null)
            {
                SwapLog.Line($"Deleting '{selectedCard.cardName}' (object='{selectedCard.gameObject.name}') for player {_pickerID}.");
                NetworkingManager.RPC(typeof(SpecialCardPatches), nameof(URPC_DeleteCard),
                    _pickerID, selectedCard.gameObject.name);
            }
            else
            {
                SwapLog.Warn("Confirm called with null selected card.");
            }

            ConsumeSpecialCard();
            RedrawHand();
        }

        // ── Networked delete (all clients) ────────────────────────────────────────

        [UnboundRPC]
        public static void URPC_DeleteCard(int playerID, string cardObjectName)
        {
            SwapLog.Section("URPC_DeleteCard (all clients)");
            SwapLog.Line($"playerID={playerID}, deleteCardObject='{cardObjectName}'");

            Player player = PlayerManager.instance?.players?.Find(p => p.playerID == playerID);
            if (player == null)
            {
                SwapLog.Error($"Player {playerID} not found.");
                return;
            }

            // Build survivor list (exclude the card being deleted)
            var survivors = new List<string>();
            foreach (var c in player.data.currentCards)
            {
                if (c == null) continue;
                string n = c.gameObject.name;
                if (n == cardObjectName || n == cardObjectName + "(Clone)") continue;
                survivors.Add(n);
            }

            SwapLog.Line($"currentCards before reset: {player.data.currentCards?.Count ?? 0}");
            SwapLog.Line($"Survivors to reapply ({survivors.Count}): [{string.Join(", ", survivors)}]");

            try
            {
                ModdingUtils.Utils.Cards.RPCA_FullReset(playerID);
                SwapLog.Line("RPCA_FullReset completed.");
            }
            catch (Exception ex) { SwapLog.Error($"RPCA_FullReset failed: {ex.Message}"); }

            try
            {
                ModdingUtils.Utils.Cards.RPCA_ClearCardBar(playerID);
                SwapLog.Line("RPCA_ClearCardBar completed.");
            }
            catch (Exception ex) { SwapLog.Error($"RPCA_ClearCardBar failed: {ex.Message}"); }

            if (s_rpca_assignCard != null)
            {
                foreach (string name in survivors)
                {
                    try
                    {
                        s_rpca_assignCard.Invoke(null,
                            new object[] { name, playerID, true, "", 0f, 0f, true });
                        SwapLog.Line($"Reapplied '{name}'.");
                    }
                    catch (Exception ex)
                    {
                        SwapLog.Error($"RPCA_AssignCard '{name}' failed: {ex.Message}");
                    }
                }
            }
            else
            {
                SwapLog.Error("RPCA_AssignCard not found via reflection.");
            }

            SwapLog.Line($"currentCards after reapply: {player.data.currentCards?.Count ?? 0}");
        }

        // ── Shared helpers ────────────────────────────────────────────────────────

        private static void ConsumeSpecialCard()
        {
            if (_specialCard == null) return;

            LogSection(_isCopycatMode, "Consume special card");
            int before = DeckManager.GetRemainingCount(_pickerID);
            LogLine(_isCopycatMode, $"Consuming '{_specialCard.cardName}' (object='{_specialCard.gameObject.name}') from runtime deck.");
            DeckManager.ConsumeCard(_pickerID, _specialCard);
            int after = DeckManager.GetRemainingCount(_pickerID);
            LogLine(_isCopycatMode, $"Runtime deck remaining: {before} -> {after}");
        }

        /// <summary>
        /// Tears down the current draft hand and spawns a fresh one so the player can
        /// pick a replacement card (used by Changed Mind confirm and both cancels).
        /// </summary>
        private static void RedrawHand()
        {
            LogSection(_isCopycatMode, "RedrawHand");

            CardChoice cc = CardChoice.instance;
            if (cc == null)
            {
                LogError(_isCopycatMode, "CardChoice.instance is null — cannot redraw.");
                return;
            }

            cc.StopAllCoroutines();
            CardDeleteManager.CleanupSpawnedDraftCards(cc);
            if (s_isPlayingField != null) s_isPlayingField.SetValue(cc, false);

            // Refresh pool: runtime deck (minus consumed special card), or vanilla.
            if (DeckManager.RuntimeDecks.TryGetValue(_pickerID, out var rt) && rt.Count > 0)
            {
                cc.cards = rt.ToArray();
                LogLine(_isCopycatMode, $"Set CardChoice.cards to runtime deck ({rt.Count} cards).");
            }
            else if (DeckPickPatch.VanillaPool != null)
            {
                cc.cards = DeckPickPatch.VanillaPool;
                LogLine(_isCopycatMode, $"Set CardChoice.cards to vanilla pool ({DeckPickPatch.VanillaPool.Length} cards).");
            }
            else
            {
                LogWarn(_isCopycatMode, "No runtime deck or vanilla pool available.");
            }

            // Re-arm picks and spawn a new draft hand.
            cc.IsPicking = true;
            if (s_picksField != null) s_picksField.SetValue(cc, 1);
            cc.Pick(null, false);
            LogLine(_isCopycatMode, "New draft hand spawned.");

            BroadcastRemainingCount();
        }

        private static void BroadcastRemainingCount()
        {
            int remaining = DeckManager.GetRemainingCount(_pickerID);
            if (remaining >= 0)
            {
                LogLine(_isCopycatMode, $"Broadcasting remaining deck count: {remaining}");
                DeckNetworkSync.BroadcastRemainingCount(_pickerID, remaining);
            }
        }

        private static void LogSection(bool isCopycat, string title)
        {
            if (isCopycat) CopycatLog.Section(title);
            else SwapLog.Section(title);
        }

        private static void LogLine(bool isCopycat, string message)
        {
            if (isCopycat) CopycatLog.Line(message);
            else SwapLog.Line(message);
        }

        private static void LogWarn(bool isCopycat, string message)
        {
            if (isCopycat) CopycatLog.Warn(message);
            else SwapLog.Warn(message);
        }

        private static void LogError(bool isCopycat, string message)
        {
            if (isCopycat) CopycatLog.Error(message);
            else SwapLog.Error(message);
        }

        // ── Local-player check ────────────────────────────────────────────────────

        private static bool IsLocalPicker(int pickerID)
        {
            Player picker = PlayerManager.instance?.players?.Find(p => p.playerID == pickerID);
            if (picker == null) return false;

            var viewField = AccessTools.Field(typeof(CharacterData), "view");
            if (viewField == null) return true;

            object view = viewField.GetValue(picker.data);
            if (view == null) return true;

            var isMine = AccessTools.Property(view.GetType(), "IsMine");
            if (isMine == null) return true;

            return (bool)isMine.GetValue(view, null);
        }
    }
}
