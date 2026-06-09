using System.Collections;
using System.Collections.Generic;
using UnboundLib.GameModes;
using UnityEngine;

namespace ShieldsMod.Stun;

public class StunResistanceManager : MonoBehaviour
{
    public static StunResistanceManager instance;

    private readonly Dictionary<int, float> _reductionByPlayer = new Dictionary<int, float>();

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        GameModeManager.AddHook(GameModeHooks.HookBattleStart, OnBattleStart);
        SLog.Section("StunResistanceManager — Awake");
    }

    private static IEnumerator OnBattleStart(IGameModeHandler gm)
    {
        if (PlayerManager.instance == null)
            yield break;

        foreach (Player player in PlayerManager.instance.players)
        {
            if (player != null)
                instance?.RebuildForPlayer(player.playerID);
        }

        yield break;
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public float GetReduction(int playerID)
    {
        return _reductionByPlayer.TryGetValue(playerID, out float reduction) ? reduction : 0f;
    }

    public float GetReductionPercent(int playerID) => GetReduction(playerID) * 100f;

    public void RebuildForPlayer(int playerID)
    {
        if (PlayerManager.instance == null)
            return;

        Player player = PlayerManager.instance.players.Find(p => p.playerID == playerID);
        if (player?.data?.currentCards == null)
            return;

        float reduction = StunHandRebuild.ComputeReductionFromHand(player.data.currentCards);
        _reductionByPlayer[playerID] = reduction;
        SLog.Line($"StunResistance rebuild player={playerID} reduction={reduction * 100f:F0}%");
    }

    public float ApplyResistance(int playerID, float rawDuration, out float resistedAmount)
    {
        float reduction = GetReduction(playerID);
        resistedAmount = rawDuration * reduction;
        return rawDuration - resistedAmount;
    }
}
