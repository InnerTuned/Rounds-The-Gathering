using System;
using System.Collections.Generic;
using DeckBuilder.CardDelete;
using DeckBuilder.Data;
using DeckBuilder.Networking;

namespace DeckBuilder.GameIntegration;

/// <summary>
/// Result a two-step card's second step reports back to the flow controller.
/// </summary>
public enum TwoStepOutcome
{
    /// <summary>User backed out — restore the draft hand so they can pick again (no consume).</summary>
    Cancelled = 0,

    /// <summary>User committed — end the current pick turn.</summary>
    ActionAccepted = 1,

    /// <summary>User committed an action but should keep picking — redraw a fresh draft hand (e.g. Swap).</summary>
    ActionAcceptedContinue = 2,
}

/// <summary>Launches a two-step card's second step. Implemented per card type.</summary>
public delegate void TwoStepStarter(int pickerID, CardInfo card);

/// <summary>
/// Central controller for "two-step" cards — cards that, when drafted, require a
/// second selection/interaction before the pick is resolved (Copycat, Swap,
/// Rare/Legendary Search, Keybound). It owns the single shared flow flag, a registry
/// of which cards are two-step, and the one resolver every second-step UI calls when
/// the user finishes: <see cref="Complete"/>.
/// </summary>
public static class TwoStepCardFlow
{
    /// <summary>True while any two-step second-step UI is open. Replaces the old per-flow flags.</summary>
    public static bool IsFlowActive { get; private set; }

    private sealed class Registration
    {
        public Func<CardInfo, bool> Match;
        public TwoStepStarter Start;
    }

    private static readonly List<Registration> Registrations = new();

    // ── Registration ────────────────────────────────────────────────────────────

    /// <summary>Registers a two-step card identified by exact display name.</summary>
    public static void Register(string cardName, TwoStepStarter starter)
    {
        if (string.IsNullOrEmpty(cardName) || starter == null) return;
        Registrations.Add(new Registration
        {
            Match = c => c != null && c.cardName == cardName,
            Start = starter,
        });
        TwoStepLog.Line($"Registered two-step card '{cardName}' (total={Registrations.Count}).");
    }

    /// <summary>Registers a two-step handler matched by predicate (e.g. all keybound cards).</summary>
    public static void Register(Func<CardInfo, bool> predicate, TwoStepStarter starter)
    {
        if (predicate == null || starter == null) return;
        Registrations.Add(new Registration { Match = predicate, Start = starter });
        TwoStepLog.Line($"Registered predicate-based handler (total={Registrations.Count}).");
    }

    /// <summary>
    /// Cross-assembly registration entry. Other mods (Keybound) pass framework delegates
    /// as <see cref="object"/> through reflection; both sides share the closed generic types.
    /// </summary>
    public static void RegisterExternal(object predicate, object starter)
    {
        if (predicate is not Func<CardInfo, bool> p)
        {
            TwoStepLog.Warn("RegisterExternal: predicate has unexpected type.");
            return;
        }

        switch (starter)
        {
            case TwoStepStarter ts:
                Register(p, ts);
                break;
            case Action<int, CardInfo> act:
                Register(p, new TwoStepStarter(act));
                break;
            default:
                TwoStepLog.Warn("RegisterExternal: starter has unexpected type.");
                break;
        }
    }

    // ── Classification ──────────────────────────────────────────────────────────

    public static bool IsTwoStep(CardInfo card) => TryGetStarter(card, out _);

    public static bool TryGetStarter(CardInfo card, out TwoStepStarter starter)
    {
        starter = null;
        if (card == null) return false;

        foreach (var reg in Registrations)
        {
            if (reg.Match(card))
            {
                starter = reg.Start;
                return true;
            }
        }
        return false;
    }

    // ── Flow lifecycle ──────────────────────────────────────────────────────────

    /// <summary>
    /// Launches the second step for a two-step card. Called from the unified
    /// ApplyStats patch. Returns true if a flow was started (caller should block
    /// normal card application).
    /// </summary>
    internal static bool BeginFlow(int pickerID, CardInfo card)
    {
        if (!TryGetStarter(card, out var starter))
            return false;

        IsFlowActive = true;
        TwoStepLog.Section($"BeginFlow — '{card.cardName}'");
        TwoStepLog.Line($"pickerID={pickerID}, cardObject='{card.gameObject?.name ?? "null"}', IsFlowActive=true");

        try
        {
            starter(pickerID, card);
            TwoStepLog.Line($"Starter returned for '{card.cardName}'. IsFlowActive={IsFlowActive}");
        }
        catch (Exception ex)
        {
            TwoStepLog.Error($"Starter for '{card.cardName}' threw: {ex}");
            IsFlowActive = false;
            return false;
        }

        return true;
    }

    /// <summary>
    /// The single resolver every two-step second-step UI calls when the user finishes.
    /// Drives the pick-phase transition based on the reported outcome.
    /// </summary>
    public static void Complete(int pickerID, CardInfo card, TwoStepOutcome outcome)
    {
        TwoStepLog.Section($"Complete — outcome={outcome}");
        TwoStepLog.Line($"pickerID={pickerID}, card='{card?.cardName ?? "null"}', cardObject='{card?.gameObject?.name ?? "null"}'");
        TwoStepLog.Line($"IsFlowActive before clear: {IsFlowActive}");
        IsFlowActive = false;

        switch (outcome)
        {
            case TwoStepOutcome.ActionAccepted:
                TwoStepLog.Line("Branch: ActionAccepted -> EndPickPhase");
                CardDeleteManager.EndPickPhase();
                Broadcast(pickerID);
                break;

            case TwoStepOutcome.ActionAcceptedContinue:
                TwoStepLog.Line("Branch: ActionAcceptedContinue -> RedrawDraftHand");
                Broadcast(pickerID);
                PickPhaseRestoreHelper.RedrawDraftHand(pickerID);
                break;

            case TwoStepOutcome.Cancelled:
                TwoStepLog.Line("Branch: Cancelled -> RestoreDraftHand");
                PickPhaseRestoreHelper.RestoreDraftHand(pickerID, card);
                break;

            default:
                TwoStepLog.Warn($"Unknown outcome '{outcome}' — no action taken.");
                break;
        }

        LogPickStateAfterComplete();
    }

    /// <summary>Cross-assembly completion entry (Keybound calls this via reflection).</summary>
    public static void CompleteExternal(int pickerID, CardInfo card, int outcomeCode)
    {
        TwoStepLog.Line($"CompleteExternal called: pickerID={pickerID}, card='{card?.cardName ?? "null"}', outcomeCode={outcomeCode}");
        Complete(pickerID, card, (TwoStepOutcome)outcomeCode);
    }

    private static void LogPickStateAfterComplete()
    {
        CardChoice cc = CardChoice.instance;
        if (cc == null)
        {
            TwoStepLog.Warn("Post-complete: CardChoice.instance is null.");
            return;
        }

        TwoStepLog.Line($"Post-complete: pickrID={cc.pickrID}, IsPicking={cc.IsPicking}, IsFlowActive={IsFlowActive}");
    }

    private static void Broadcast(int pickerID)
    {
        int remaining = DeckManager.GetRemainingCount(pickerID);
        if (remaining >= 0)
            DeckNetworkSync.BroadcastRemainingCount(pickerID, remaining);
    }
}
