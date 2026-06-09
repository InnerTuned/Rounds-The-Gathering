using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DeckBuilder.Data;

namespace DeckBuilder.UI
{
    /// <summary>Screen B — shown instead of the original Toggle Cards menu.</summary>
    public class DeckSelectorScreen : MonoBehaviour
    {
        public static DeckSelectorScreen instance;

        private Canvas _canvas;
        private Button _createBtn;
        private Button _editBtn;
        private Button _deleteBtn;
        private TextMeshProUGUI _deleteBtnLabel;
        private TMP_Dropdown _deckDropdown;
        private TextMeshProUGUI _activeDeckLabel;

        private GameObject _deleteModal;
        private TextMeshProUGUI _deleteModalMessage;
        private string _pendingDeleteDeckName;

        private void Awake()
        {
            instance = this;
            BuildUI();
        }

        // ── Build ─────────────────────────────────────────────────────────────────

        private void BuildUI()
        {
            _canvas = UIHelper.CreateFullscreenCanvas("RTG_DeckSelectorScreen", sortOrder: 200);
            // Note: Do NOT parent this component to the canvas - all screens share a single uiRoot

            UIHelper.CreateDimmedBackground(_canvas.transform);

            // Centre panel — taller so items breathe
            var panel = UIHelper.CreatePanel(_canvas.transform, "Panel",
                new Vector2(0.3f, 0.15f), new Vector2(0.7f, 0.85f),
                bg: new Color(0.08f, 0.08f, 0.12f, 1f));

            // Title
            var titleRt = UIHelper.CreatePanel(panel, "Title",
                new Vector2(0f, 0.85f), Vector2.one);
            UIHelper.CreateText(titleRt, "TitleText", "Your Decks", fontSize: 38);

            // Create New Deck button
            _createBtn = AnchorButton(panel, "CreateBtn", "Create New Deck",
                new Vector2(0.08f, 0.74f), new Vector2(0.92f, 0.85f),
                bgColor: new Color(0.15f, 0.5f, 0.15f), fontSize: 26);
            _createBtn.onClick.AddListener(OnCreateClicked);

            // Edit Deck button
            _editBtn = AnchorButton(panel, "EditBtn", "Edit Deck",
                new Vector2(0.08f, 0.60f), new Vector2(0.92f, 0.72f),
                bgColor: new Color(0.2f, 0.3f, 0.55f), fontSize: 26);
            _editBtn.onClick.AddListener(OnEditClicked);

            // Deck dropdown — sits directly below Edit Deck
            _deckDropdown = BuildDropdown(panel,
                new Vector2(0.08f, 0.46f), new Vector2(0.92f, 0.58f));

            // Set Playable Deck + Delete Deck — side by side below the dropdown
            var setBtn = AnchorButton(panel, "SetBtn", "Set Playable Deck",
                new Vector2(0.08f, 0.31f), new Vector2(0.50f, 0.43f),
                bgColor: new Color(0.55f, 0.35f, 0.05f), fontSize: 22);
            setBtn.onClick.AddListener(OnSetPlayableDeckClicked);

            _deleteBtn = AnchorButton(panel, "DeleteBtn", "Delete Deck",
                new Vector2(0.52f, 0.31f), new Vector2(0.92f, 0.43f),
                bgColor: new Color(0.55f, 0.12f, 0.12f), fontSize: 22);
            _deleteBtn.onClick.AddListener(OnDeleteClicked);
            var deleteColors = _deleteBtn.colors;
            deleteColors.disabledColor = new Color(0.28f, 0.28f, 0.32f, 1f);
            _deleteBtn.colors = deleteColors;
            _deleteBtnLabel = _deleteBtn.transform.Find("Label")?.GetComponent<TextMeshProUGUI>();

            BuildDeleteModal(panel);

            // Active deck label — shows which deck is currently active for gameplay
            var activeLabelPanel = UIHelper.CreatePanel(panel, "ActiveDeckPanel",
                new Vector2(0.05f, 0.19f), new Vector2(0.95f, 0.29f),
                bg: new Color(0.05f, 0.05f, 0.08f, 0.8f));
            _activeDeckLabel = UIHelper.CreateText(activeLabelPanel, "ActiveDeckLabel",
                "Active: —", fontSize: 20, alignment: TextAlignmentOptions.Center);

            // Close button
            var closeBtn = AnchorButton(panel, "CloseBtn", "Close",
                new Vector2(0.2f, 0.06f), new Vector2(0.8f, 0.16f),
                bgColor: new Color(0.45f, 0.1f, 0.1f), fontSize: 22);
            closeBtn.onClick.AddListener(Hide);

            // Start hidden — will be shown via Show()
            _canvas.gameObject.SetActive(false);
        }

        /// <summary>Creates a button using anchor min/max instead of anchoredPosition.</summary>
        private static Button AnchorButton(RectTransform parent, string name, string label,
            Vector2 anchorMin, Vector2 anchorMax,
            Color bgColor, int fontSize)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = rt.offsetMax = Vector2.zero;

            var img = go.AddComponent<Image>();
            img.color = bgColor;

            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            var cb = new ColorBlock
            {
                normalColor    = bgColor,
                highlightedColor = bgColor * 1.3f,
                pressedColor   = bgColor * 0.7f,
                disabledColor  = new Color(0.25f, 0.25f, 0.25f, 0.6f),
                colorMultiplier = 1f,
                fadeDuration   = 0.1f
            };
            btn.colors = cb;

            var lGo = new GameObject("Label");
            lGo.transform.SetParent(go.transform, false);
            var lRt = lGo.AddComponent<RectTransform>();
            lRt.anchorMin = Vector2.zero;
            lRt.anchorMax = Vector2.one;
            lRt.offsetMin = lRt.offsetMax = Vector2.zero;
            var lTmp = lGo.AddComponent<TextMeshProUGUI>();
            lTmp.text = label;
            lTmp.fontSize = fontSize;
            lTmp.alignment = TextAlignmentOptions.Center;
            lTmp.color = Color.white;

            return btn;
        }

        private TMP_Dropdown BuildDropdown(RectTransform parent,
            Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject("DeckDropdown");
            go.transform.SetParent(parent, false);

            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = rt.offsetMax = Vector2.zero;

            var img = go.AddComponent<Image>();
            img.color = new Color(0.12f, 0.12f, 0.18f, 0.95f);

            var dd = go.AddComponent<TMP_Dropdown>();
            dd.targetGraphic = img;

            // label text
            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(go.transform, false);
            var labelRt = labelGo.AddComponent<RectTransform>();
            labelRt.anchorMin = Vector2.zero;
            labelRt.anchorMax = Vector2.one;
            labelRt.offsetMin = new Vector2(10, 4);
            labelRt.offsetMax = new Vector2(-30, -4);
            var labelTmp = labelGo.AddComponent<TextMeshProUGUI>();
            labelTmp.fontSize = 20;
            labelTmp.color = Color.white;
            labelTmp.alignment = TextAlignmentOptions.MidlineLeft;
            dd.captionText = labelTmp;

            // Arrow indicator on the right
            var arrowGo = new GameObject("Arrow");
            arrowGo.transform.SetParent(go.transform, false);
            var arrowRt = arrowGo.AddComponent<RectTransform>();
            arrowRt.anchorMin = new Vector2(1, 0);
            arrowRt.anchorMax = new Vector2(1, 1);
            arrowRt.pivot = new Vector2(1, 0.5f);
            arrowRt.sizeDelta = new Vector2(30, 0);
            arrowRt.anchoredPosition = new Vector2(-5, 0);
            var arrowTmp = arrowGo.AddComponent<TextMeshProUGUI>();
            arrowTmp.text = "v";
            arrowTmp.fontSize = 18;
            arrowTmp.color = Color.white;
            arrowTmp.alignment = TextAlignmentOptions.Center;

            // template — needs its own Canvas to appear on top
            var tmplGo = new GameObject("Template");
            tmplGo.transform.SetParent(go.transform, false);
            tmplGo.SetActive(false);

            var tmplRt = tmplGo.AddComponent<RectTransform>();
            // Anchored to the bottom of the dropdown, expanding downward
            tmplRt.anchorMin = new Vector2(0, 0);
            tmplRt.anchorMax = new Vector2(1, 0);
            tmplRt.pivot = new Vector2(0.5f, 1f);
            tmplRt.anchoredPosition = Vector2.zero;
            tmplRt.sizeDelta = new Vector2(0, 400f); // Tall enough for many items

            // Add Canvas to template so it renders on top
            var tmplCanvas = tmplGo.AddComponent<Canvas>();
            tmplCanvas.overrideSorting = true;
            tmplCanvas.sortingOrder = 500;
            tmplGo.AddComponent<GraphicRaycaster>();

            var tmplImg = tmplGo.AddComponent<Image>();
            tmplImg.color = new Color(0.08f, 0.08f, 0.12f, 1f);

            var scroll = tmplGo.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;

            // Viewport — fills the template area
            var vpGo = new GameObject("Viewport");
            vpGo.transform.SetParent(tmplGo.transform, false);
            var vpRt = vpGo.AddComponent<RectTransform>();
            vpRt.anchorMin = Vector2.zero;
            vpRt.anchorMax = Vector2.one;
            vpRt.offsetMin = new Vector2(2, 2);
            vpRt.offsetMax = new Vector2(-2, -2);
            vpRt.pivot = new Vector2(0, 1);
            var vpImg = vpGo.AddComponent<Image>();
            vpImg.color = new Color(0.08f, 0.08f, 0.12f, 1f);
            vpGo.AddComponent<Mask>().showMaskGraphic = false;
            scroll.viewport = vpRt;

            // Content — standard TMP_Dropdown setup
            var contentGo = new GameObject("Content");
            contentGo.transform.SetParent(vpGo.transform, false);
            var contentRt = contentGo.AddComponent<RectTransform>();
            contentRt.anchorMin = new Vector2(0, 1);
            contentRt.anchorMax = new Vector2(1, 1);
            contentRt.pivot = new Vector2(0.5f, 1);
            contentRt.anchoredPosition = Vector2.zero;
            contentRt.sizeDelta = new Vector2(0, 56f); // Will be resized by TMP_Dropdown
            scroll.content = contentRt;

            // Item template — TMP_Dropdown will clone this for each option
            var itemGo = new GameObject("Item");
            itemGo.transform.SetParent(contentGo.transform, false);
            var itemRt = itemGo.AddComponent<RectTransform>();
            itemRt.anchorMin = new Vector2(0, 0.5f);
            itemRt.anchorMax = new Vector2(1, 0.5f);
            itemRt.pivot = new Vector2(0.5f, 0.5f);
            itemRt.sizeDelta = new Vector2(0, 54f); // Height of each item

            var itemImg = itemGo.AddComponent<Image>();
            itemImg.color = new Color(0.15f, 0.15f, 0.20f, 1f);

            var itemToggle = itemGo.AddComponent<Toggle>();
            itemToggle.targetGraphic = itemImg;
            itemToggle.isOn = true;
            var toggleColors = new ColorBlock
            {
                normalColor = new Color(0.15f, 0.15f, 0.20f, 1f),
                highlightedColor = new Color(0.25f, 0.30f, 0.40f, 1f),
                pressedColor = new Color(0.30f, 0.35f, 0.50f, 1f),
                disabledColor = new Color(0.2f, 0.2f, 0.2f, 0.5f),
                colorMultiplier = 1f,
                fadeDuration = 0.1f
            };
            itemToggle.colors = toggleColors;

            // Item Background (selection highlight)
            var itemBgGo = new GameObject("Item Background");
            itemBgGo.transform.SetParent(itemGo.transform, false);
            var itemBgRt = itemBgGo.AddComponent<RectTransform>();
            itemBgRt.anchorMin = Vector2.zero;
            itemBgRt.anchorMax = Vector2.one;
            itemBgRt.offsetMin = Vector2.zero;
            itemBgRt.offsetMax = Vector2.zero;
            var itemBgImg = itemBgGo.AddComponent<Image>();
            itemBgImg.color = new Color(0.20f, 0.45f, 0.65f, 1f);
            itemToggle.graphic = itemBgImg;

            // Item Label — centered text
            var itemLabelGo = new GameObject("Item Label");
            itemLabelGo.transform.SetParent(itemGo.transform, false);
            var itemLabelRt = itemLabelGo.AddComponent<RectTransform>();
            itemLabelRt.anchorMin = Vector2.zero;
            itemLabelRt.anchorMax = Vector2.one;
            itemLabelRt.offsetMin = new Vector2(8, 4);
            itemLabelRt.offsetMax = new Vector2(-8, -4);
            var itemLabelTmp = itemLabelGo.AddComponent<TextMeshProUGUI>();
            itemLabelTmp.fontSize = 22;
            itemLabelTmp.color = Color.white;
            itemLabelTmp.alignment = TextAlignmentOptions.Center; // Center the text
            itemLabelTmp.raycastTarget = false;

            dd.itemText = itemLabelTmp;
            dd.template = tmplRt;

            dd.onValueChanged.AddListener(_ => UpdateDeleteButtonState());

            return dd;
        }

        private void BuildDeleteModal(RectTransform parent)
        {
            _deleteModal = new GameObject("DeleteDeckModal");
            _deleteModal.transform.SetParent(parent, false);
            _deleteModal.SetActive(false);

            // Full-panel opaque overlay — nothing shows through underneath
            var overlayRt = _deleteModal.AddComponent<RectTransform>();
            overlayRt.anchorMin = Vector2.zero;
            overlayRt.anchorMax = Vector2.one;
            overlayRt.offsetMin = overlayRt.offsetMax = Vector2.zero;

            var overlayBg = _deleteModal.AddComponent<Image>();
            overlayBg.color = new Color(0.06f, 0.06f, 0.09f, 1f);
            overlayBg.raycastTarget = true;

            // Dialog box on top of the overlay
            var dialogGo = new GameObject("Dialog");
            dialogGo.transform.SetParent(_deleteModal.transform, false);
            var dialogRt = dialogGo.AddComponent<RectTransform>();
            dialogRt.anchorMin = new Vector2(0.06f, 0.30f);
            dialogRt.anchorMax = new Vector2(0.94f, 0.70f);
            dialogRt.offsetMin = dialogRt.offsetMax = Vector2.zero;

            var dialogBg = dialogGo.AddComponent<Image>();
            dialogBg.color = new Color(0.08f, 0.08f, 0.14f, 1f);

            // Title — pinned to the top strip, red
            var titleTmp = UIHelper.CreateText(dialogRt, "Title", "Delete Deck?",
                fontSize: 26, alignment: TextAlignmentOptions.Center,
                color: new Color(0.85f, 0.2f, 0.2f));
            var titleRt = titleTmp.GetComponent<RectTransform>();
            titleRt.anchorMin = new Vector2(0f, 0.74f);
            titleRt.anchorMax = new Vector2(1f, 1f);
            titleRt.offsetMin = titleRt.offsetMax = Vector2.zero;

            // Message — middle band
            _deleteModalMessage = UIHelper.CreateText(dialogRt, "Message", "",
                fontSize: 19, alignment: TextAlignmentOptions.Center);
            var msgRt = _deleteModalMessage.GetComponent<RectTransform>();
            msgRt.anchorMin = new Vector2(0.06f, 0.32f);
            msgRt.anchorMax = new Vector2(0.94f, 0.74f);
            msgRt.offsetMin = msgRt.offsetMax = Vector2.zero;
            _deleteModalMessage.enableWordWrapping = true;

            var confirmBtn = UIHelper.CreateButton(dialogRt, "ConfirmBtn", "Delete",
                Vector2.zero, Vector2.zero, fontSize: 22,
                bgColor: new Color(0.55f, 0.12f, 0.12f));
            var cRt = confirmBtn.GetComponent<RectTransform>();
            cRt.anchorMin = new Vector2(0.08f, 0.08f);
            cRt.anchorMax = new Vector2(0.48f, 0.28f);
            cRt.offsetMin = cRt.offsetMax = Vector2.zero;
            confirmBtn.onClick.AddListener(ConfirmDeleteDeck);

            var cancelBtn = UIHelper.CreateButton(dialogRt, "CancelBtn", "Cancel",
                Vector2.zero, Vector2.zero, fontSize: 22,
                bgColor: new Color(0.25f, 0.25f, 0.30f));
            var canRt = cancelBtn.GetComponent<RectTransform>();
            canRt.anchorMin = new Vector2(0.52f, 0.08f);
            canRt.anchorMax = new Vector2(0.92f, 0.28f);
            canRt.offsetMin = canRt.offsetMax = Vector2.zero;
            cancelBtn.onClick.AddListener(CancelDeleteDeck);

            _deleteModal.transform.SetAsLastSibling();
        }

        // ── Show / Hide ───────────────────────────────────────────────────────────

        public void Show()
        {
            RTGLog.Section("DeckSelectorScreen — Show");
            RefreshDropdown();
            _canvas.gameObject.SetActive(true);
        }

        public void Hide()
        {
            RTGLog.Section("DeckSelectorScreen — Hide");
            CancelDeleteDeck();
            _canvas.gameObject.SetActive(false);
        }

        // ── Refresh ───────────────────────────────────────────────────────────────

        private void RefreshDropdown()
        {
            RTGLog.Line($"RefreshDropdown: DeckManager.instance is {(DeckManager.instance == null ? "NULL" : "valid")}");
            List<DeckData> decks = DeckManager.instance?.AllDecks ?? new List<DeckData>();
            RTGLog.Line($"RefreshDropdown: {decks.Count} custom deck(s).");

            _deckDropdown.ClearOptions();
            var options = new List<TMP_Dropdown.OptionData>();

            // Build ordered list: custom decks first, then Default Deck at the bottom
            if (decks.Count > 0)
            {
                foreach (var d in decks)
                {
                    options.Add(new TMP_Dropdown.OptionData(d.name));
                    RTGLog.Line($"  Added custom deck: '{d.name}'");
                }
            }

            // Always add Default Deck at the end
            options.Add(new TMP_Dropdown.OptionData(DeckManager.DefaultDeckName));
            RTGLog.Line($"  Added: '{DeckManager.DefaultDeckName}'");
            RTGLog.Line($"RefreshDropdown: total options = {options.Count}");

            _deckDropdown.AddOptions(options);

            // Edit button is always enabled
            _editBtn.interactable = true;

            // Set dropdown to active deck
            string activeName = DeckManager.instance?.ActiveDeckName ?? DeckManager.DefaultDeckName;
            RTGLog.Line($"RefreshDropdown: activeName = '{activeName}'");

            int selectedIdx = options.FindIndex(o => o.text == activeName);
            if (selectedIdx < 0)
                selectedIdx = options.Count - 1; // Default to "Default Deck" at the end
            _deckDropdown.value = selectedIdx;

            RTGLog.Line($"RefreshDropdown: selected '{options[selectedIdx].text}' (index {selectedIdx}), dropdown.options.Count={_deckDropdown.options.Count}");

            RefreshActiveDeckLabel();
            UpdateDeleteButtonState();
        }

        private void UpdateDeleteButtonState()
        {
            if (_deleteBtn == null) return;

            string selectedName = GetSelectedDeckName();
            bool canDelete = !string.IsNullOrEmpty(selectedName)
                && selectedName != DeckManager.DefaultDeckName
                && DeckManager.instance?.AllDecks.Exists(d => d.name == selectedName) == true;

            _deleteBtn.interactable = canDelete;

            if (_deleteBtnLabel != null)
            {
                _deleteBtnLabel.color = canDelete
                    ? Color.white
                    : new Color(0.45f, 0.45f, 0.48f, 1f);
            }
        }

        private string GetSelectedDeckName()
        {
            if (_deckDropdown == null || _deckDropdown.options.Count == 0)
                return null;

            int idx = _deckDropdown.value;
            if (idx < 0 || idx >= _deckDropdown.options.Count)
                return null;

            return _deckDropdown.options[idx].text;
        }

        private void RefreshActiveDeckLabel()
        {
            if (_activeDeckLabel == null) return;
            string activeName = DeckManager.instance?.ActiveDeckName ?? DeckManager.DefaultDeckName;
            if (activeName == DeckManager.DefaultDeckName)
            {
                _activeDeckLabel.text = "Playing with: Default Deck (all cards)";
                _activeDeckLabel.color = new Color(0.7f, 0.7f, 0.7f);
            }
            else
            {
                _activeDeckLabel.text = $"Playing with: {activeName}";
                _activeDeckLabel.color = new Color(0.4f, 0.9f, 0.4f);
            }
        }

        // ── Callbacks ─────────────────────────────────────────────────────────────

        private void OnCreateClicked()
        {
            RTGLog.Section("DeckSelectorScreen — Create clicked");
            Hide();
            CreateDeckScreen.instance?.Show();
        }

        private void OnSetPlayableDeckClicked()
        {
            RTGLog.Section("DeckSelectorScreen — Set Playable Deck clicked");
            if (DeckManager.instance == null) return;

            string selectedName = GetSelectedDeckName();
            if (string.IsNullOrEmpty(selectedName)) return;

            DeckManager.instance.SetActiveDeck(selectedName);
            RTGLog.Line($"Set active deck to '{selectedName}'.");
            RefreshActiveDeckLabel();
        }

        private void OnDeleteClicked()
        {
            RTGLog.Section("DeckSelectorScreen — Delete Deck clicked");
            if (DeckManager.instance == null) return;

            string selectedName = GetSelectedDeckName();
            if (string.IsNullOrEmpty(selectedName) || selectedName == DeckManager.DefaultDeckName)
            {
                RTGLog.Line("Cannot delete Default Deck.");
                return;
            }

            if (DeckManager.instance.AllDecks.Find(d => d.name == selectedName) == null)
            {
                RTGLog.Warn($"Deck '{selectedName}' not found — cannot delete.");
                return;
            }

            _pendingDeleteDeckName = selectedName;
            _deleteModalMessage.text =
                $"Delete deck \"{selectedName}\"?\n\nThis cannot be undone.";
            _deleteModal.transform.SetAsLastSibling();
            _deleteModal.SetActive(true);
        }

        private void ConfirmDeleteDeck()
        {
            RTGLog.Section("DeckSelectorScreen — Confirm Delete Deck");
            if (DeckManager.instance == null || string.IsNullOrEmpty(_pendingDeleteDeckName))
            {
                CancelDeleteDeck();
                return;
            }

            string deckName = _pendingDeleteDeckName;
            RTGLog.Line($"Deleting deck '{deckName}'.");
            DeckManager.instance.DeleteDeck(deckName);

            _pendingDeleteDeckName = null;
            _deleteModal.SetActive(false);

            RefreshDropdown();
        }

        private void CancelDeleteDeck()
        {
            RTGLog.Line("Delete deck cancelled.");
            _pendingDeleteDeckName = null;
            if (_deleteModal != null)
                _deleteModal.SetActive(false);
        }

        private void OnEditClicked()
        {
            RTGLog.Section("DeckSelectorScreen — Edit clicked");
            if (DeckManager.instance == null)
            {
                RTGLog.Warn("DeckManager.instance is null.");
                return;
            }

            string selectedName = GetSelectedDeckName();
            if (string.IsNullOrEmpty(selectedName)) return;

            // Handle "Default Deck" selection
            if (selectedName == DeckManager.DefaultDeckName)
            {
                RTGLog.Line("Selected Default Deck — setting as active (vanilla behavior).");
                DeckManager.instance.SetActiveDeck(DeckManager.DefaultDeckName);
                Hide();
                return;
            }

            // Find the custom deck
            List<DeckData> decks = DeckManager.instance.AllDecks;
            DeckData selectedDeck = decks.Find(d => d.name == selectedName);

            if (selectedDeck == null)
            {
                RTGLog.Warn($"Deck '{selectedName}' not found.");
                return;
            }

            DeckManager.instance.SetActiveDeck(selectedDeck.name);
            RTGLog.Line($"Opening editor for '{selectedDeck.name}'.");
            RTGLog.Line($"DeckEditorScreen.instance is {(DeckEditorScreen.instance == null ? "NULL" : "valid")}");
            Hide();
            if (DeckEditorScreen.instance != null)
            {
                DeckEditorScreen.instance.Open(selectedDeck);
            }
            else
            {
                RTGLog.Error("Cannot open editor — DeckEditorScreen.instance is null!");
            }
        }
    }
}
