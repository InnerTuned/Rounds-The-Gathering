using System.Collections.Generic;
using Keybound.Core;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Keybound.UI;

/// <summary>
/// Bottom-right effect stack — one horizontal row per player, stacked vertically
/// (mirrors the top-left card bar layout without a background panel).
/// </summary>
public class EffectStackOverlay : MonoBehaviour
{
    public static EffectStackOverlay instance;

    private const float RowHeight = 35f;
    private const float RowSpacing = 4f;
    private const float SlotSize = 35f;

    private Canvas _canvas;
    private RectTransform _container;
    private readonly Dictionary<int, PlayerEffectRow> _playerRows = new();
    private GameObject _hoverCard;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        BuildUI();
        _canvas.gameObject.SetActive(false);
    }

    private void BuildUI()
    {
        _canvas = UIHelper.CreateFullscreenCanvas("KB_EffectStack", sortOrder: 45);

        // Transparent container — bottom-right, no background.
        _container = UIHelper.CreatePanel(_canvas.transform, "EffectContainer",
            new Vector2(0.55f, 0.02f), new Vector2(0.98f, 0.30f));
        var vLayout = _container.gameObject.AddComponent<VerticalLayoutGroup>();
        vLayout.childAlignment = TextAnchor.LowerRight;
        vLayout.childControlWidth = false;
        vLayout.childControlHeight = false;
        vLayout.childForceExpandWidth = false;
        vLayout.childForceExpandHeight = false;
        vLayout.spacing = RowSpacing;
        vLayout.padding = new RectOffset(0, 4, 4, 0);
    }

    public void RefreshAll()
    {
        if (EffectStackManager.instance == null)
        {
            HideAll();
            return;
        }

        bool anyVisible = false;
        var activePlayers = new HashSet<int>();

        foreach (var kvp in EffectStackManager.instance.GetAllBindings())
        {
            activePlayers.Add(kvp.Key);
            GetOrCreateRow(kvp.Key).Refresh(kvp.Value);
            anyVisible = true;
        }

        // Hide rows for players with no bindings.
        foreach (var kvp in _playerRows)
        {
            if (!activePlayers.Contains(kvp.Key))
                kvp.Value.Root.SetActive(false);
        }

        if (!anyVisible)
            HideAll();
        else
            _canvas.gameObject.SetActive(true);
    }

    public void Refresh(int playerID) => RefreshAll();

    public void HideAll()
    {
        ClearHover();
        foreach (var row in _playerRows.Values)
            row.Root.SetActive(false);
        if (_canvas != null)
            _canvas.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_canvas == null || !_canvas.gameObject.activeSelf) return;
        if (!IsGameActive())
            HideAll();
    }

    private PlayerEffectRow GetOrCreateRow(int playerID)
    {
        if (_playerRows.TryGetValue(playerID, out var row))
            return row;

        row = PlayerEffectRow.Create(_container, playerID, OnSlotHover, OnSlotHoverExit);
        _playerRows[playerID] = row;
        return row;
    }

    private void OnSlotHover(KeyboundBinding binding, Vector3 worldPos)
    {
        ClearHover();
        if (CardChoice.instance == null) return;

        CardInfo template = binding.Card ?? FindCardTemplate(binding.CardName);
        if (template == null) return;

        _hoverCard = CardChoice.instance.AddCardVisual(template, worldPos);
        if (_hoverCard == null) return;

        foreach (var col in _hoverCard.transform.root.GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        var cardCanvas = _hoverCard.GetComponentInChildren<Canvas>();
        if (cardCanvas != null) cardCanvas.sortingLayerName = "MostFront";

        var keyLabel = UIHelper.CreateText(_hoverCard.transform, "KeyLabel",
            $"Bound to {KeybindModalUI.FormatKey(binding.Key)}",
            fontSize: 18, alignment: TextAlignmentOptions.Bottom,
            color: new Color(0.75f, 0.9f, 1f));
        var rt = keyLabel.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 0f);
        rt.anchorMax = new Vector2(1f, 0.18f);
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }

    private void OnSlotHoverExit() => ClearHover();

    private void ClearHover()
    {
        if (_hoverCard != null)
        {
            Destroy(_hoverCard);
            _hoverCard = null;
        }
    }

    private static CardInfo FindCardTemplate(string cardName)
    {
        if (CardChoice.instance?.cards == null) return null;
        foreach (var c in CardChoice.instance.cards)
        {
            if (c != null && c.cardName == cardName) return c;
        }
        return null;
    }

    private static bool IsGameActive()
    {
        if (GameManager.instance == null) return false;
        if (!GameManager.instance.isPlaying) return false;
        if (PlayerManager.instance?.players == null || PlayerManager.instance.players.Count == 0)
            return false;
        return true;
    }

    // ── Per-player horizontal row ─────────────────────────────────────────────

    private sealed class PlayerEffectRow
    {
        internal GameObject Root;
        private RectTransform _slotRow;
        private readonly List<EffectSlotView> _slots = new();

        internal static PlayerEffectRow Create(Transform parent, int playerID,
            System.Action<KeyboundBinding, Vector3> onHover,
            System.Action onHoverExit)
        {
            var go = new GameObject($"EffectRow_P{playerID}");
            go.transform.SetParent(parent, false);

            var rt = go.AddComponent<RectTransform>();
            var le = go.AddComponent<LayoutElement>();
            le.preferredHeight = RowHeight;
            le.minHeight = RowHeight;

            var hLayout = go.AddComponent<HorizontalLayoutGroup>();
            hLayout.spacing = 6f;
            hLayout.childAlignment = TextAnchor.MiddleRight;
            hLayout.childControlWidth = false;
            hLayout.childControlHeight = false;
            hLayout.childForceExpandWidth = false;
            hLayout.childForceExpandHeight = false;

            var row = new PlayerEffectRow
            {
                Root = go,
                _slotRow = go.GetComponent<RectTransform>(),
                _onHover = onHover,
                _onHoverExit = onHoverExit
            };
            return row;
        }

        private System.Action<KeyboundBinding, Vector3> _onHover;
        private System.Action _onHoverExit;

        internal void Refresh(IReadOnlyList<KeyboundBinding> bindings)
        {
            Root.SetActive(true);
            EnsureSlotCount(bindings.Count);
            for (int i = 0; i < bindings.Count; i++)
                _slots[i].Bind(bindings[i]);
            for (int i = bindings.Count; i < _slots.Count; i++)
                _slots[i].Root.SetActive(false);
        }

        private void EnsureSlotCount(int count)
        {
            while (_slots.Count < count)
            {
                var slot = EffectSlotView.Create(_slotRow, _onHover, _onHoverExit);
                _slots.Add(slot);
            }
        }
    }

    // ── Individual effect slot ────────────────────────────────────────────────

    private sealed class EffectSlotView : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler
    {
        internal GameObject Root;
        private Image _bg;
        private TextMeshProUGUI _label;

        private static readonly Color CooldownRed  = new Color(0.55f, 0.12f, 0.12f, 0.95f);
        private static readonly Color ReadyGreen   = new Color(0.12f, 0.55f, 0.18f, 0.95f);
        private static readonly Color BorderRed    = new Color(0.95f, 0.20f, 0.20f, 1f);
        private static readonly Color BorderGreen  = new Color(0.2f, 1f, 0.2f, 1f);  // Bright neon green
        private KeyboundBinding _binding;
        private System.Action<KeyboundBinding, Vector3> _onHover;
        private System.Action _onHoverExit;

        internal static EffectSlotView Create(Transform parent,
            System.Action<KeyboundBinding, Vector3> onHover,
            System.Action onHoverExit)
        {
            var go = new GameObject("EffectSlot");
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(SlotSize, SlotSize);

            var le = go.AddComponent<LayoutElement>();
            le.preferredWidth = SlotSize;
            le.preferredHeight = SlotSize;

            var view = go.AddComponent<EffectSlotView>();
            view._onHover = onHover;
            view._onHoverExit = onHoverExit;
            view.Root = go;
            view.Build();
            return view;
        }

        private void Build()
        {
            _bg = gameObject.AddComponent<Image>();
            _bg.color = CooldownRed;
            var outline = _bg.gameObject.AddComponent<Outline>();
            outline.effectColor = BorderRed;
            outline.effectDistance = new Vector2(3f, -3f);  // Thicker border

            _label = UIHelper.CreateText(transform, "Abbr", "—",
                fontSize: 12, alignment: TextAlignmentOptions.Center);
        }

        internal void Bind(KeyboundBinding binding)
        {
            _binding = binding;
            _label.text = Abbreviate(binding.CardName);
            RefreshVisual();
        }

        private void Update()
        {
            if (_binding != null) RefreshVisual();
        }

        private void RefreshVisual()
        {
            // Background still fades red → green as the cooldown counts down.
            float progress = 1f - _binding.CooldownFill();
            _bg.color = Color.Lerp(CooldownRed, ReadyGreen, progress);

            // Border is binary: bright red while on cooldown, bright green the instant it's ready.
            var outline = _bg.GetComponent<Outline>() ?? _bg.gameObject.AddComponent<Outline>();
            outline.effectColor = _binding.IsReady() ? BorderGreen : BorderRed;
            outline.effectDistance = new Vector2(3f, -3f);  // Thicker border
        }

        public void OnPointerEnter(PointerEventData _)
        {
            if (_binding == null) return;
            _onHover?.Invoke(_binding, transform.position);
        }

        public void OnPointerExit(PointerEventData _) => _onHoverExit?.Invoke();

        private static string Abbreviate(string name)
        {
            if (string.IsNullOrEmpty(name)) return "?";
            if (name.Length == 1) return name.ToUpper();
            return char.ToUpper(name[0]) + name.Substring(1, 1).ToLower();
        }
    }
}
