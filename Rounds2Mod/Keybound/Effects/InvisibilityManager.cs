using System.Collections.Generic;
using UnityEngine;

namespace Keybound.Effects;

/// <summary>Tracks active invisibility per player and applies/restores visuals.</summary>
internal sealed class InvisibilityManager : MonoBehaviour
{
    public static InvisibilityManager instance;

    private const float AlphaScale = 0.2f;
    internal static readonly Color InvisibleColor = new Color(1f, 1f, 1f, AlphaScale);

    private readonly Dictionary<int, ActiveInvisibility> _active = new();

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    internal static bool IsActive(int playerID) =>
        instance != null && instance._active.ContainsKey(playerID);

    internal static bool IsActive(Player player) =>
        player != null && IsActive(player.playerID);

    internal void Activate(Player player, float duration)
    {
        if (player == null || duration <= 0f)
            return;

        int id = player.playerID;
        if (_active.TryGetValue(id, out ActiveInvisibility existing))
            existing.Restore();

        var state = new ActiveInvisibility(player);
        state.CaptureAndApply();
        state.EndsAt = Time.time + duration;
        _active[id] = state;

        KLog.Line($"Invisibility activated for player {id} ({duration:F1}s).");
    }

    private void Update()
    {
        if (_active.Count == 0)
            return;

        float now = Time.time;
        var expired = new List<int>();

        foreach (var kvp in _active)
        {
            if (now >= kvp.Value.EndsAt)
                expired.Add(kvp.Key);
            else
                kvp.Value.Refresh();
        }

        foreach (int id in expired)
            EndInvisibility(id);
    }

    internal void ClearAll()
    {
        foreach (int id in new List<int>(_active.Keys))
            EndInvisibility(id);
    }

    private void EndInvisibility(int playerID)
    {
        if (!_active.TryGetValue(playerID, out ActiveInvisibility state))
            return;

        state.Restore();
        _active.Remove(playerID);
        KLog.Line($"Invisibility ended for player {playerID}.");
    }

    private sealed class ActiveInvisibility
    {
        private readonly Player _player;

        private readonly List<SetTeamColorState> _teamColors = new();
        private readonly List<ParticleState> _particles = new();
        private readonly List<SpriteState> _sprites = new();
        private readonly List<LineState> _lines = new();
        private readonly List<MeshState> _meshes = new();
        private readonly List<ColliderState> _colliders = new();

        private Color _originalColorMax;
        private Color _originalColorMin;
        private bool _isSimpleSkin;

        internal float EndsAt;

        internal ActiveInvisibility(Player player) => _player = player;

        internal void CaptureAndApply()
        {
            if (_player == null)
                return;

            Transform root = _player.transform.root;

            CaptureOriginalColors();
            CaptureTeamColors(root);
            CaptureParticles();
            CaptureRenderers(root);
            CaptureColliders(root);

            ApplyInvisibleColors();
        }

        private void CaptureOriginalColors()
        {
            var skinHandler = _player.gameObject.GetComponentInChildren<PlayerSkinHandler>();
            _isSimpleSkin = skinHandler != null && skinHandler.simpleSkin;

            if (_isSimpleSkin)
            {
                var layer = _player.gameObject.GetComponentInChildren<SetPlayerSpriteLayer>();
                if (layer != null)
                {
                    var masks = layer.transform.root.GetComponentsInChildren<SpriteMask>();
                    if (masks.Length > 0)
                    {
                        var sr = masks[0].GetComponent<SpriteRenderer>();
                        if (sr != null)
                        {
                            _originalColorMax = sr.color;
                            _originalColorMin = sr.color;
                        }
                    }
                }
            }
            else
            {
                var particles = _player.gameObject.GetComponentsInChildren<PlayerSkinParticle>();
                if (particles.Length > 0)
                {
                    var partField = typeof(PlayerSkinParticle).GetField("part",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (partField != null)
                    {
                        var ps = partField.GetValue(particles[0]) as ParticleSystem;
                        if (ps != null)
                        {
                            var startColor = ps.main.startColor;
                            _originalColorMax = startColor.colorMax;
                            _originalColorMin = startColor.colorMin;
                        }
                    }
                }
            }
        }

        private void CaptureTeamColors(Transform root)
        {
            foreach (var stc in root.GetComponentsInChildren<SetTeamColor>(true))
                _teamColors.Add(new SetTeamColorState(stc));
        }

        private void CaptureParticles()
        {
            var particles = _player.gameObject.GetComponentsInChildren<PlayerSkinParticle>();
            var partField = typeof(PlayerSkinParticle).GetField("part",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (partField == null)
                return;

            foreach (var psp in particles)
            {
                var ps = partField.GetValue(psp) as ParticleSystem;
                if (ps != null)
                {
                    var main = ps.main;
                    var startColor = main.startColor;
                    _particles.Add(new ParticleState(ps, startColor.colorMin, startColor.colorMax));
                }
            }
        }

        private void CaptureRenderers(Transform root)
        {
            foreach (var sr in root.GetComponentsInChildren<SpriteRenderer>(true))
                _sprites.Add(new SpriteState(sr, sr.color));

            foreach (var lr in root.GetComponentsInChildren<LineRenderer>(true))
                _lines.Add(new LineState(lr, lr.startColor, lr.endColor));

            foreach (var mr in root.GetComponentsInChildren<MeshRenderer>(true))
            {
                if (mr.material != null)
                    _meshes.Add(new MeshState(mr, mr.material.color));
            }
        }

        private void CaptureColliders(Transform root)
        {
            foreach (var col in root.GetComponentsInChildren<Collider2D>(true))
            {
                if (col.isTrigger)
                {
                    _colliders.Add(new ColliderState(col, col.enabled));
                    col.enabled = false;
                }
            }
        }

        private void ApplyInvisibleColors()
        {
            var invisibleSkin = new PlayerSkin
            {
                color = InvisibleColor,
                backgroundColor = InvisibleColor,
                winText = InvisibleColor,
                particleEffect = InvisibleColor
            };

            foreach (var stc in _teamColors)
            {
                if (stc.TeamColor != null)
                    stc.TeamColor.Set(invisibleSkin);
            }

            foreach (var p in _particles)
            {
                if (p.System == null)
                    continue;

                var main = p.System.main;
                var startColor = main.startColor;
                startColor.colorMin = InvisibleColor;
                startColor.colorMax = InvisibleColor;
                main.startColor = startColor;
            }

            foreach (var s in _sprites)
            {
                if (s.Renderer != null)
                    s.Renderer.color = InvisibleColor;
            }

            foreach (var l in _lines)
            {
                if (l.Renderer != null)
                {
                    l.Renderer.startColor = InvisibleColor;
                    l.Renderer.endColor = InvisibleColor;
                }
            }

            foreach (var m in _meshes)
            {
                if (m.Renderer?.material != null)
                    m.Renderer.material.color = InvisibleColor;
            }
        }

        internal void Refresh()
        {
            ApplyInvisibleColors();
        }

        internal void Restore()
        {
            var originalSkin = new PlayerSkin
            {
                color = _originalColorMax,
                backgroundColor = _originalColorMax,
                winText = _originalColorMax,
                particleEffect = _originalColorMax
            };

            foreach (var stc in _teamColors)
            {
                if (stc.TeamColor != null)
                    stc.TeamColor.Set(originalSkin);
            }

            foreach (var p in _particles)
            {
                if (p.System == null)
                    continue;

                var main = p.System.main;
                var startColor = main.startColor;
                startColor.colorMin = p.OriginalMin;
                startColor.colorMax = p.OriginalMax;
                main.startColor = startColor;
            }

            foreach (var s in _sprites)
            {
                if (s.Renderer != null)
                    s.Renderer.color = s.Original;
            }

            foreach (var l in _lines)
            {
                if (l.Renderer != null)
                {
                    l.Renderer.startColor = l.OriginalStart;
                    l.Renderer.endColor = l.OriginalEnd;
                }
            }

            foreach (var m in _meshes)
            {
                if (m.Renderer?.material != null)
                    m.Renderer.material.color = m.Original;
            }

            foreach (var c in _colliders)
            {
                if (c.Collider != null)
                    c.Collider.enabled = c.WasEnabled;
            }
        }
    }

    private readonly struct SetTeamColorState
    {
        internal readonly SetTeamColor TeamColor;

        internal SetTeamColorState(SetTeamColor teamColor) => TeamColor = teamColor;
    }

    private readonly struct ParticleState
    {
        internal readonly ParticleSystem System;
        internal readonly Color OriginalMin;
        internal readonly Color OriginalMax;

        internal ParticleState(ParticleSystem system, Color originalMin, Color originalMax)
        {
            System = system;
            OriginalMin = originalMin;
            OriginalMax = originalMax;
        }
    }

    private readonly struct SpriteState
    {
        internal readonly SpriteRenderer Renderer;
        internal readonly Color Original;

        internal SpriteState(SpriteRenderer renderer, Color original)
        {
            Renderer = renderer;
            Original = original;
        }
    }

    private readonly struct LineState
    {
        internal readonly LineRenderer Renderer;
        internal readonly Color OriginalStart;
        internal readonly Color OriginalEnd;

        internal LineState(LineRenderer renderer, Color originalStart, Color originalEnd)
        {
            Renderer = renderer;
            OriginalStart = originalStart;
            OriginalEnd = originalEnd;
        }
    }

    private readonly struct MeshState
    {
        internal readonly MeshRenderer Renderer;
        internal readonly Color Original;

        internal MeshState(MeshRenderer renderer, Color original)
        {
            Renderer = renderer;
            Original = original;
        }
    }

    private readonly struct ColliderState
    {
        internal readonly Collider2D Collider;
        internal readonly bool WasEnabled;

        internal ColliderState(Collider2D collider, bool wasEnabled)
        {
            Collider = collider;
            WasEnabled = wasEnabled;
        }
    }
}
