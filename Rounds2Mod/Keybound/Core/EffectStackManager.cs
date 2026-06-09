using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Keybound.UI;
using UnityEngine;

namespace Keybound.Core;

/// <summary>Tracks keybound bindings per player and handles key-press activation.</summary>
public class EffectStackManager : MonoBehaviour
{
    public static EffectStackManager instance;

    private readonly Dictionary<int, List<KeyboundBinding>> _bindings = new();

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    internal IReadOnlyList<KeyboundBinding> GetBindings(int playerID)
    {
        if (!_bindings.TryGetValue(playerID, out var list))
            return System.Array.Empty<KeyboundBinding>();
        return list;
    }

    internal IEnumerable<KeyValuePair<int, IReadOnlyList<KeyboundBinding>>> GetAllBindings()
    {
        foreach (var kvp in _bindings)
        {
            if (kvp.Value != null && kvp.Value.Count > 0)
                yield return new KeyValuePair<int, IReadOnlyList<KeyboundBinding>>(kvp.Key, kvp.Value);
        }
    }

    internal bool IsKeyTaken(int playerID, int key) =>
        GetBindings(playerID).Any(b => b.Key == key);

    internal void AddBinding(int playerID, CardInfo card, int key)
    {
        if (card == null || !KeyboundCardRegistry.TryGet(card.cardName, out var def))
        {
            KLog.Warn($"AddBinding failed — '{card?.cardName ?? "null"}' is not keybound.");
            return;
        }

        if (!_bindings.TryGetValue(playerID, out var list))
        {
            list = new List<KeyboundBinding>();
            _bindings[playerID] = list;
        }

        var binding = new KeyboundBinding
        {
            PlayerID = playerID,
            CardName = card.cardName,
            Card = card,
            Key = key,
            Def = def
        };
        binding.ArmInitialDelay();
        list.Add(binding);

        KLog.Line($"Bound '{card.cardName}' to {key} for player {playerID}.");
        EffectStackOverlay.instance?.RefreshAll();
    }

    internal void ClearAll()
    {
        _bindings.Clear();
        EffectStackOverlay.instance?.HideAll();
    }

    private bool _wasGameActive;

    private void Update()
    {
        bool active = IsGameActive();
        if (_wasGameActive && !active)
            ClearAll();
        _wasGameActive = active;

        if (!active) return;

        Player local = GetLocalHumanPlayer();
        if (local == null) return;

        foreach (var binding in GetBindings(local.playerID))
        {
            if (!binding.IsReady()) continue;
            if (!InputCompat.GetKeyDown(binding.Key)) continue;

            if (binding.Def.Activate(local))
                binding.ArmCooldown();
        }

        EffectStackOverlay.instance?.RefreshAll();
    }

    private static bool IsGameActive()
    {
        if (GameManager.instance == null) return false;
        if (!GameManager.instance.isPlaying) return false;
        if (PlayerManager.instance?.players == null || PlayerManager.instance.players.Count == 0)
            return false;
        return true;
    }

    internal static Player GetLocalHumanPlayer()
    {
        if (PlayerManager.instance == null) return null;

        foreach (var p in PlayerManager.instance.players)
        {
            if (p == null || p.GetComponent<PlayerAPI>()?.enabled == true)
                continue;

            var viewField = AccessTools.Field(typeof(CharacterData), "view");
            object view = viewField?.GetValue(p.data);
            if (view == null) return p;

            var isMine = AccessTools.Property(view.GetType(), "IsMine");
            if (isMine == null) return p;
            if ((bool)isMine.GetValue(view, null)) return p;
        }

        return null;
    }
}
