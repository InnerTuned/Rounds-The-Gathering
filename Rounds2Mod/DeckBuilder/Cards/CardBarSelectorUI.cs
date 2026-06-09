using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using HarmonyLib;
using InfoOverhaul.Delta;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DeckBuilder.UI;

namespace DeckBuilder.Cards
{
    /// <summary>
    /// Selection mode determines behavior and delta preview type.
    /// </summary>
    public enum CardSelectMode
    {
        /// <summary>Copycat: copy from any player, show add-delta on hover.</summary>
        CopyFromAny,
        /// <summary>Swap: delete from own hand, show removal-delta on hover.</summary>
        DeleteFromOwn
    }

    /// <summary>
    /// Singleton overlay shown when Copycat or Swap is activated.
    /// Presents a message and [Cancel] / [Select] buttons.
    /// Attaches <see cref="CardBarSelectButton"/> to card bar slots.
    /// </summary>
    public class CardBarSelectorUI : MonoBehaviour
    {
        public static CardBarSelectorUI instance;

        private Canvas _canvas;
        private TextMeshProUGUI _messageText;
        private TextMeshProUGUI _deltaPreviewText;
        private Button _selectButton;
        private Image _selectImage;
        private TextMeshProUGUI _selectLabel;

        private Action<CardInfo> _onConfirm;
        private Action _onCancel;
        private CardInfo _selectedCard;
        private readonly List<CardBarSelectButton> _slotButtons = new List<CardBarSelectButton>();

        private CardSelectMode _mode;
        private int _pickerID;

        internal static readonly Color HoverColor    = new Color(0.25f, 0.55f, 1.00f, 1f);
        internal static readonly Color SelectedColor = new Color(0.15f, 0.75f, 0.25f, 1f);

        private static readonly Color PanelBg       = new Color(0.04f, 0.06f, 0.12f, 0.95f);
        private static readonly Color CancelBg       = new Color(0.40f, 0.10f, 0.10f, 0.95f);
        private static readonly Color SelectReadyBg  = new Color(0.10f, 0.45f, 0.12f, 0.95f);
        private static readonly Color SelectGrayBg   = new Color(0.18f, 0.18f, 0.18f, 0.55f);

        private static readonly FieldInfo s_cardBarButtonCard =
            AccessTools.Field(typeof(CardBarButton), "card");

        private void Awake()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            BuildUI();
            _canvas.gameObject.SetActive(false);
        }

        // ── Build ─────────────────────────────────────────────────────────────────

        private void BuildUI()
        {
            _canvas = UIHelper.CreateFullscreenCanvas("RTG_CardSelectorUI", sortOrder: 180);

            // Main action panel (bottom)
            var panel = UIHelper.CreatePanel(_canvas.transform, "SelectorPanel",
                new Vector2(0.10f, 0.09f), new Vector2(0.90f, 0.19f),
                bg: PanelBg);

            _messageText = UIHelper.CreateText(panel, "Message", "",
                fontSize: 22, alignment: TextAlignmentOptions.MidlineLeft,
                color: new Color(0.90f, 0.93f, 1f));
            var msgRt = _messageText.GetComponent<RectTransform>();
            msgRt.anchorMin = new Vector2(0.02f, 0f);
            msgRt.anchorMax = new Vector2(0.60f, 1f);
            msgRt.offsetMin = msgRt.offsetMax = Vector2.zero;

            var cancelBtn = UIHelper.CreateButton(panel, "CancelBtn", "[Cancel]",
                Vector2.zero, Vector2.zero, fontSize: 20, bgColor: CancelBg);
            var cancelRt = cancelBtn.GetComponent<RectTransform>();
            cancelRt.anchorMin = new Vector2(0.62f, 0.12f);
            cancelRt.anchorMax = new Vector2(0.79f, 0.88f);
            cancelRt.offsetMin = cancelRt.offsetMax = Vector2.zero;
            cancelBtn.onClick.AddListener(OnCancelClicked);

            // Select button built manually so we can keep references to image + label
            var selGo = new GameObject("SelectBtn");
            selGo.transform.SetParent(panel, false);
            var selRt = selGo.AddComponent<RectTransform>();
            selRt.anchorMin = new Vector2(0.81f, 0.12f);
            selRt.anchorMax = new Vector2(0.98f, 0.88f);
            selRt.offsetMin = selRt.offsetMax = Vector2.zero;

            _selectImage = selGo.AddComponent<Image>();
            _selectImage.color = SelectGrayBg;

            _selectButton = selGo.AddComponent<Button>();
            _selectButton.targetGraphic = _selectImage;
            _selectButton.colors = new ColorBlock
            {
                normalColor      = Color.white,
                highlightedColor = new Color(1.15f, 1.15f, 1.15f),
                pressedColor     = new Color(0.80f, 0.80f, 0.80f),
                disabledColor    = Color.white,
                colorMultiplier  = 1f,
                fadeDuration     = 0.1f
            };
            _selectButton.interactable = false;
            _selectButton.onClick.AddListener(OnSelectClicked);

            var selLabelGo = new GameObject("Label");
            selLabelGo.transform.SetParent(selGo.transform, false);
            var selLabelRt = selLabelGo.AddComponent<RectTransform>();
            selLabelRt.anchorMin = Vector2.zero;
            selLabelRt.anchorMax = Vector2.one;
            selLabelRt.offsetMin = selLabelRt.offsetMax = Vector2.zero;
            _selectLabel = selLabelGo.AddComponent<TextMeshProUGUI>();
            _selectLabel.text = "[Select]";
            _selectLabel.fontSize = 20;
            _selectLabel.alignment = TextAlignmentOptions.Center;
            _selectLabel.color = new Color(0.45f, 0.45f, 0.45f);

            // Delta preview panel (above action panel)
            var deltaPanel = UIHelper.CreatePanel(_canvas.transform, "DeltaPanel",
                new Vector2(0.10f, 0.20f), new Vector2(0.90f, 0.32f),
                bg: new Color(0.03f, 0.04f, 0.08f, 0.90f));

            _deltaPreviewText = UIHelper.CreateText(deltaPanel, "DeltaText", "",
                fontSize: 18, alignment: TextAlignmentOptions.MidlineLeft,
                color: new Color(0.88f, 0.92f, 1f));
            _deltaPreviewText.richText = true;
            var deltaRt = _deltaPreviewText.GetComponent<RectTransform>();
            deltaRt.anchorMin = new Vector2(0.02f, 0.05f);
            deltaRt.anchorMax = new Vector2(0.98f, 0.95f);
            deltaRt.offsetMin = deltaRt.offsetMax = Vector2.zero;
        }

        // ── Public API ────────────────────────────────────────────────────────────

        public void Show(int pickerID, string message, CardSelectMode mode,
            Action<CardInfo> onConfirm, Action onCancel)
        {
            _mode        = mode;
            _pickerID    = pickerID;
            _onConfirm   = onConfirm;
            _onCancel    = onCancel;
            _selectedCard = null;
            _messageText.text = message;
            _deltaPreviewText.text = "";
            RefreshSelectButton();
            AttachSlotButtons(pickerID, mode);
            _canvas.gameObject.SetActive(true);

            LogSection("Selector UI shown");
            LogLine($"pickerID={pickerID}, mode={mode}, message='{message}', slots={_slotButtons.Count}");
        }

        public void Hide()
        {
            LogLine($"Selector UI hidden (hadSelection={_selectedCard != null}).");
            _canvas.gameObject.SetActive(false);
            DetachSlotButtons();
            _onConfirm    = null;
            _onCancel     = null;
            _selectedCard = null;
        }

        // Called by CardBarSelectButton when a slot is clicked
        internal void NotifySlotClicked(CardBarSelectButton clickedBtn, CardInfo card)
        {
            foreach (var b in _slotButtons)
                b.SetSelected(b == clickedBtn);
            _selectedCard = card;
            RefreshSelectButton();
            UpdateDeltaPreview(card);
            LogLine($"Slot selected: '{card?.cardName ?? "null"}' (object='{card?.gameObject.name ?? "null"}').");
        }

        // Called by CardBarSelectButton on hover
        internal void NotifySlotHovered(CardInfo card)
        {
            if (_selectedCard != null) return; // Don't change preview if something is selected
            UpdateDeltaPreview(card);
        }

        // Called by CardBarSelectButton on hover exit
        internal void NotifySlotHoverExit()
        {
            if (_selectedCard != null) return;
            _deltaPreviewText.text = "";
        }

        // ── Private helpers ───────────────────────────────────────────────────────

        private void RefreshSelectButton()
        {
            bool ready = _selectedCard != null;
            _selectButton.interactable = ready;
            _selectImage.color = ready ? SelectReadyBg : SelectGrayBg;
            _selectLabel.color = ready ? Color.white : new Color(0.45f, 0.45f, 0.45f);
        }

        private void UpdateDeltaPreview(CardInfo card)
        {
            if (card == null)
            {
                _deltaPreviewText.text = "";
                return;
            }

            Player picker = PlayerManager.instance?.players?.Find(p => p.playerID == _pickerID);
            if (picker == null)
            {
                _deltaPreviewText.text = "";
                return;
            }

            IReadOnlyList<StatDeltaLine> deltas = null;
            bool hasDeltas = false;

            if (_mode == CardSelectMode.CopyFromAny)
            {
                // Copycat: show what happens if we ADD this card
                hasDeltas = CardDeltaRegistry.TryGetDeltas(picker, card, out deltas);
            }
            else
            {
                // Swap: show what happens if we REMOVE this card
                hasDeltas = CardDeltaRegistry.TryGetRemovalDeltas(picker, card, out deltas);
            }

            if (!hasDeltas || deltas == null || deltas.Count == 0)
            {
                string action = _mode == CardSelectMode.CopyFromAny ? "copying" : "removing";
                _deltaPreviewText.text = $"<i>No stat changes from {action} {card.cardName}</i>";
                return;
            }

            var sb = new StringBuilder();
            string header = _mode == CardSelectMode.CopyFromAny
                ? $"If copying {card.cardName}:"
                : $"If removing {card.cardName}:";
            sb.Append($"<b>{header}</b>  ");

            for (int i = 0; i < deltas.Count && i < 6; i++)
            {
                if (i > 0) sb.Append("  |  ");
                sb.Append(deltas[i].FormatRichText());
            }
            if (deltas.Count > 6)
                sb.Append($"  ... +{deltas.Count - 6} more");

            _deltaPreviewText.text = sb.ToString();
        }

        private void OnCancelClicked()
        {
            LogLine("[Cancel] button clicked.");
            var cb = _onCancel;
            Hide();
            cb?.Invoke();
        }

        private void OnSelectClicked()
        {
            if (_selectedCard == null)
            {
                LogWarn("[Select] clicked with no card selected.");
                return;
            }

            LogLine($"[Select] button clicked for '{_selectedCard.cardName}'.");
            var card = _selectedCard;
            var cb   = _onConfirm;
            Hide();
            cb?.Invoke(card);
        }

        private void AttachSlotButtons(int pickerID, CardSelectMode mode)
        {
            DetachSlotButtons();

            if (CardBarHandler.instance == null)
            {
                LogWarn("AttachSlotButtons: CardBarHandler.instance is null.");
                return;
            }

            CardBar[] bars = Traverse.Create(CardBarHandler.instance)
                .Field("cardBars").GetValue<CardBar[]>();

            if (bars == null)
            {
                LogWarn("AttachSlotButtons: cardBars array is null.");
                return;
            }

            int beforeCount = _slotButtons.Count;
            if (mode == CardSelectMode.CopyFromAny)
            {
                // Copycat: attach to ALL players' card bars
                for (int i = 0; i < bars.Length; i++)
                {
                    if (bars[i] == null) continue;
                    int attached = AttachToBar(bars[i]);
                    LogLine($"Attached {attached} slot(s) on player {i} bar.");
                }
            }
            else
            {
                // Swap: attach only to the picker's card bar
                if (pickerID < 0 || pickerID >= bars.Length)
                {
                    LogWarn($"AttachSlotButtons: invalid pickerID={pickerID}, bars={bars.Length}");
                    return;
                }

                int attached = AttachToBar(bars[pickerID]);
                LogLine($"Attached {attached} slot(s) on picker bar (player {pickerID}).");
            }

            LogLine($"Total selectable slots: {_slotButtons.Count} (was {beforeCount}).");
        }

        private int AttachToBar(CardBar bar)
        {
            int attached = 0;
            foreach (Transform child in bar.transform)
            {
                if (!child.gameObject.activeSelf) continue;
                var barBtn = child.GetComponent<CardBarButton>();
                if (barBtn == null) continue;
                var card = s_cardBarButtonCard?.GetValue(barBtn) as CardInfo;
                if (card == null) continue;

                var slotBtn = child.gameObject.GetComponent<CardBarSelectButton>()
                           ?? child.gameObject.AddComponent<CardBarSelectButton>();
                slotBtn.Init(this, card);
                _slotButtons.Add(slotBtn);
                attached++;
            }

            return attached;
        }

        private void DetachSlotButtons()
        {
            int count = _slotButtons.Count;
            foreach (var b in _slotButtons)
                if (b != null) Destroy(b);
            _slotButtons.Clear();
            if (count > 0)
                LogLine($"Detached {count} slot button(s).");
        }

        private bool IsCopycatMode => _mode == CardSelectMode.CopyFromAny;

        private void LogSection(string title)
        {
            if (IsCopycatMode) CopycatLog.Section(title);
            else SwapLog.Section(title);
        }

        private void LogLine(string message)
        {
            if (IsCopycatMode) CopycatLog.Line(message);
            else SwapLog.Line(message);
        }

        private void LogWarn(string message)
        {
            if (IsCopycatMode) CopycatLog.Warn(message);
            else SwapLog.Warn(message);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Attached to each live card-bar slot while the selector UI is open.
    /// Highlights on hover / selected state and routes clicks to the parent UI.
    /// </summary>
    public class CardBarSelectButton : MonoBehaviour,
        IPointerClickHandler,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        private CardBarSelectorUI _ui;
        private CardInfo          _card;
        private Image             _icon;
        private Color             _originalColor;
        private bool              _isSelected;

        internal void Init(CardBarSelectorUI ui, CardInfo card)
        {
            _ui   = ui;
            _card = card;
            _icon = GetComponentInChildren<Image>();
            if (_icon != null) _originalColor = _icon.color;
        }

        internal void SetSelected(bool selected)
        {
            _isSelected = selected;
            ApplyColor();
        }

        private void ApplyColor()
        {
            if (_icon == null) return;
            _icon.color = _isSelected ? CardBarSelectorUI.SelectedColor : _originalColor;
        }

        public void OnPointerEnter(PointerEventData _)
        {
            if (_icon == null || _isSelected) return;
            _icon.color = CardBarSelectorUI.HoverColor;
            _ui?.NotifySlotHovered(_card);
        }

        public void OnPointerExit(PointerEventData _)
        {
            if (_icon == null) return;
            ApplyColor();
            _ui?.NotifySlotHoverExit();
        }

        public void OnPointerClick(PointerEventData _)
        {
            if (_ui == null || _card == null) return;
            _ui.NotifySlotClicked(this, _card);
        }

        private void OnDestroy()
        {
            if (_icon != null) _icon.color = _originalColor;
        }
    }
}
