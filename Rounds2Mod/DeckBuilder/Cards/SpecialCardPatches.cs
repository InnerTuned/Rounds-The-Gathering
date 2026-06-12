using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
// Note: 'ModdingUtils.Utils.Cards' must be referenced by full name inside namespace
// DeckBuilder.Cards to avoid ambiguity with the DeckBuilder.Cards namespace itself.
using UnboundLib;
using UnboundLib.Networking;
using UnityEngine;
using DeckBuilder.Data;
using DeckBuilder.GameIntegration;

namespace DeckBuilder.Cards
{
    /// <summary>
    /// Handler for the Copycat and Swap two-step flows. Registers itself with
    /// <see cref="TwoStepCardFlow"/>; all Harmony plumbing lives in TwoStepCardPatches.
    /// </summary>
    public static class SpecialCardPatches
    {
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

        private static readonly MethodInfo s_rpca_assignCard =
            AccessTools.Method(typeof(ModdingUtils.Utils.Cards), "RPCA_AssignCard",
                new[] { typeof(string), typeof(int), typeof(bool), typeof(string), typeof(float), typeof(float), typeof(bool) });

        // ── Registration ────────────────────────────────────────────────────────────

        /// <summary>Registers Copycat and Swap as two-step cards. Call once at startup.</summary>
        public static void RegisterFlows()
        {
            TwoStepCardFlow.Register(CopycatCard.CardDisplayName,
                (pickerID, card) => StartFlow(pickerID, card, isCopycat: true));
            TwoStepCardFlow.Register(SwapCard.CardDisplayName,
                (pickerID, card) => StartFlow(pickerID, card, isCopycat: false));
        }

        // ── Flow entry ────────────────────────────────────────────────────────────

        private static void StartFlow(int pickerID, CardInfo specialCard, bool isCopycat)
        {
            _isCopycatMode = isCopycat;
            _pickerID      = pickerID;
            _specialCard   = specialCard;
            _pickerPlayer  = PlayerManager.instance?.players?.Find(p => p.playerID == pickerID);

            LogSection(isCopycat, "StartFlow");
            LogLine(isCopycat, $"pickerID={pickerID}, specialCard='{specialCard.cardName}'");

            string msg = isCopycat ? "Select any card to duplicate" : "Select any card to delete";
            var mode = isCopycat ? CardSelectMode.CopyFromAny : CardSelectMode.DeleteFromOwn;
            CardBarSelectorUI.instance?.Show(pickerID, msg, mode,
                onConfirm: OnConfirm,
                onCancel:  OnCancel);
        }

        // ── Confirm / Cancel ──────────────────────────────────────────────────────

        private static void OnConfirm(CardInfo selectedCard)
        {
            LogSection(_isCopycatMode, "Confirm");
            LogLine(_isCopycatMode, $"selectedCard='{selectedCard?.cardName ?? "null"}'");
            if (_isCopycatMode)
                DoCopycatConfirm(selectedCard);
            else
                DoSwapConfirm(selectedCard);
        }

        private static void OnCancel()
        {
            LogSection(_isCopycatMode, "Cancel");
            LogLine(_isCopycatMode, $"Reporting Cancelled to TwoStepCardFlow (picker={_pickerID}, card='{_specialCard?.cardName ?? "null"}').");
            TwoStepCardFlow.Complete(_pickerID, _specialCard, TwoStepOutcome.Cancelled);
        }

        // ── Copycat confirm: clone selected card, end pick phase ──────────────────

        private static void DoCopycatConfirm(CardInfo selectedCard)
        {
            CopycatLog.Section("Confirm — clone card");
            if (_pickerPlayer != null && selectedCard != null)
            {
                try
                {
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
            TwoStepCardFlow.Complete(_pickerID, _specialCard, TwoStepOutcome.ActionAccepted);
        }

        // ── Swap confirm: delete selected card, then redraw for a replacement pick ──

        private static void DoSwapConfirm(CardInfo selectedCard)
        {
            SwapLog.Section("Confirm — delete card");
            if (selectedCard != null)
            {
                int cardIndex = _pickerPlayer?.data?.currentCards?.IndexOf(selectedCard) ?? -1;
                SwapLog.Line($"Deleting '{selectedCard.cardName}' (index={cardIndex}) for player {_pickerID}.");
                NetworkingManager.RPC(typeof(SpecialCardPatches), nameof(URPC_DeleteCard),
                    _pickerID, cardIndex);
            }
            else
            {
                SwapLog.Warn("Confirm called with null selected card.");
            }

            ConsumeSpecialCard();
            TwoStepCardFlow.Complete(_pickerID, _specialCard, TwoStepOutcome.ActionAcceptedContinue);
        }

        // ── Networked delete (all clients) ────────────────────────────────────────

        [UnboundRPC]
        public static void URPC_DeleteCard(int playerID, int cardIndex)
        {
            SwapLog.Section("URPC_DeleteCard (all clients)");
            SwapLog.Line($"playerID={playerID}, deleteCardIndex={cardIndex}");

            Player player = PlayerManager.instance?.players?.Find(p => p.playerID == playerID);
            if (player == null)
            {
                SwapLog.Error($"Player {playerID} not found.");
                return;
            }

            if (cardIndex < 0 || cardIndex >= player.data.currentCards.Count)
            {
                SwapLog.Error($"Invalid cardIndex={cardIndex} (hand size={player.data.currentCards?.Count ?? 0}).");
                return;
            }

            // Build survivor list — exclude exactly one card by index (handles duplicate card names).
            var survivors = new List<string>();
            for (int i = 0; i < player.data.currentCards.Count; i++)
            {
                if (i == cardIndex) continue;
                var c = player.data.currentCards[i];
                if (c == null) continue;
                survivors.Add(c.gameObject.name);
            }

            try
            {
                ModdingUtils.Utils.Cards.RPCA_FullReset(playerID);
            }
            catch (Exception ex) { SwapLog.Error($"RPCA_FullReset failed: {ex.Message}"); }

            try
            {
                ModdingUtils.Utils.Cards.RPCA_ClearCardBar(playerID);
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

            int before = DeckManager.GetRemainingCount(_pickerID);
            DeckManager.ConsumeCard(_pickerID, _specialCard);
            int after = DeckManager.GetRemainingCount(_pickerID);
            LogLine(_isCopycatMode, $"Consumed '{_specialCard.cardName}'. Runtime deck: {before} -> {after}");
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
    }
}
