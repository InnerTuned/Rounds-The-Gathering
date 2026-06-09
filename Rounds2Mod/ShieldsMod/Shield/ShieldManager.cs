using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnboundLib;
using UnboundLib.GameModes;

namespace ShieldsMod.Shield;

public class ShieldManager : MonoBehaviour
{
    public static ShieldManager instance;

    private readonly Dictionary<int, ShieldState> _shields = new Dictionary<int, ShieldState>();
    private readonly Dictionary<int, ShieldVisual> _visuals = new Dictionary<int, ShieldVisual>();
    private readonly Dictionary<int, ShieldParryTrigger> _parryTriggers = new Dictionary<int, ShieldParryTrigger>();

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);

        SLog.Section("ShieldManager — Awake");
        GameModeManager.AddHook(GameModeHooks.HookBattleStart, OnBattleStart);
        GameModeManager.AddHook(GameModeHooks.HookPointEnd, OnPointEnd);
        GameModeManager.AddHook(GameModeHooks.HookGameStart, OnGameStart);
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    private static IEnumerator OnBattleStart(IGameModeHandler gm)
    {
        instance?.ResetAllForBattle();
        yield break;
    }

    private static IEnumerator OnPointEnd(IGameModeHandler gm)
    {
        instance?.CleanupOrphanedAttachments();
        yield break;
    }

    private static IEnumerator OnGameStart(IGameModeHandler gm)
    {
        instance?.CleanupOrphanedAttachments();
        yield break;
    }

    public void ResetAllForBattle()
    {
        SLog.Section("ShieldManager — ResetAllForBattle");

        if (PlayerManager.instance == null)
        {
            SLog.Warn("PlayerManager.instance is null.");
            return;
        }

        foreach (Player player in PlayerManager.instance.players)
        {
            if (player == null)
                continue;

            ShieldHandRebuild.RebuildForPlayer(player.playerID);
            ShieldState state = GetOrCreateShield(player.playerID);
            if (state.HasShield)
                state.ResetToFull();
            else
                state.Current = 0f;

            EnsureAttachments(player, state);
            RefreshVisual(player.playerID);
            SLog.Line($"player={player.playerID} shield={state.Current}/{state.Max}");
        }
    }

    public ShieldState GetOrCreateShield(int playerID)
    {
        if (!_shields.TryGetValue(playerID, out ShieldState state))
        {
            state = new ShieldState();
            _shields[playerID] = state;
        }
        return state;
    }

    public ShieldState GetShield(int playerID) => GetOrCreateShield(playerID);

    /// <summary>
    /// Full-absorb: while shield is active, entire hit is negated.
    /// Larger hits zero the shield; smaller hits reduce it.
    /// </summary>
    public bool TryAbsorbFull(int playerID, float damageAmount, out bool depleted)
    {
        depleted = false;
        ShieldState state = GetOrCreateShield(playerID);

        if (!state.IsActive || damageAmount <= 0f)
            return false;

        if (damageAmount >= state.Current)
        {
            state.Current = 0f;
            depleted = true;
        }
        else
        {
            state.Current -= damageAmount;
        }

        RefreshVisual(playerID);
        return true;
    }

    public void RefreshVisual(int playerID)
    {
        if (_visuals.TryGetValue(playerID, out ShieldVisual visual) && visual != null)
            visual.Refresh();

        if (_parryTriggers.TryGetValue(playerID, out ShieldParryTrigger parry) && parry != null)
            parry.UpdateEnabled();
    }

    private void EnsureAttachments(Player player, ShieldState state)
    {
        int id = player.playerID;

        if (!_visuals.TryGetValue(id, out ShieldVisual visual) || visual == null)
        {
            var visualGo = new GameObject($"ShieldVisual_P{id}");
            visualGo.transform.SetParent(player.transform, false);
            visual = visualGo.AddComponent<ShieldVisual>();
            visual.Init(player, state);
            _visuals[id] = visual;
        }
        else
        {
            visual.SetState(state);
            visual.Refresh();
        }

        if (!_parryTriggers.TryGetValue(id, out ShieldParryTrigger parry) || parry == null)
        {
            var parryGo = new GameObject($"ShieldParry_P{id}");
            parryGo.transform.SetParent(player.transform, false);
            parry = parryGo.AddComponent<ShieldParryTrigger>();
            float radius = ShieldVisual.GetBodyRadius(player) * ShieldVisual.SizeMultiplier;
            parry.Init(player, state, radius);
            _parryTriggers[id] = parry;
        }
        else
        {
            parry.SetState(state);
            parry.UpdateEnabled();
        }
    }

    private void CleanupOrphanedAttachments()
    {
        var deadVisualKeys = new List<int>();
        foreach (var kv in _visuals)
        {
            if (kv.Value == null)
                deadVisualKeys.Add(kv.Key);
        }
        foreach (int key in deadVisualKeys)
            _visuals.Remove(key);

        var deadParryKeys = new List<int>();
        foreach (var kv in _parryTriggers)
        {
            if (kv.Value == null)
                deadParryKeys.Add(kv.Key);
        }
        foreach (int key in deadParryKeys)
            _parryTriggers.Remove(key);
    }
}
