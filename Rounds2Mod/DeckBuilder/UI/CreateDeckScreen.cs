using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DeckBuilder.Data;

namespace DeckBuilder.UI
{
    /// <summary>Screen for naming a new deck and setting its max size.</summary>
    public class CreateDeckScreen : MonoBehaviour
    {
        public static CreateDeckScreen instance;

        public bool IsOpen => _canvas != null && _canvas.gameObject.activeInHierarchy;

        private Canvas _canvas;
        private TMP_InputField _nameField;
        private TMP_InputField _maxSizeField;
        private Button _createBtn;
        private TextMeshProUGUI _errorText;

        private void Awake()
        {
            instance = this;
            BuildUI();
        }

        // ── Build ─────────────────────────────────────────────────────────────────

        private void BuildUI()
        {
            _canvas = UIHelper.CreateFullscreenCanvas("RTG_CreateDeckScreen", sortOrder: 210);
            // Note: Do NOT parent this component to the canvas - all screens share a single uiRoot

            UIHelper.CreateDimmedBackground(_canvas.transform);

            // Taller, fully-opaque panel
            var panel = UIHelper.CreatePanel(_canvas.transform, "Panel",
                new Vector2(0.28f, 0.12f), new Vector2(0.72f, 0.88f),
                bg: new Color(0.08f, 0.08f, 0.12f, 1f));

            // Title
            var titleRt = UIHelper.CreatePanel(panel, "Title",
                new Vector2(0f, 0.88f), Vector2.one);
            UIHelper.CreateText(titleRt, "TitleText", "Create New Deck", fontSize: 34);

            // Deck Name label
            var nameLabelRt = UIHelper.CreatePanel(panel, "NameLabel",
                new Vector2(0.05f, 0.74f), new Vector2(0.95f, 0.82f));
            UIHelper.CreateText(nameLabelRt, "Lbl", "Deck Name", fontSize: 22,
                alignment: TextAlignmentOptions.MidlineLeft);

            // Deck Name input field — taller slot gives text room to breathe
            _nameField = UIHelper.CreateInputField(panel, "NameField",
                "Enter deck name...",
                Vector2.zero, Vector2.zero, fontSize: 22);
            var nfRt = _nameField.GetComponent<RectTransform>();
            nfRt.anchorMin = new Vector2(0.05f, 0.61f);
            nfRt.anchorMax = new Vector2(0.95f, 0.73f);
            nfRt.offsetMin = nfRt.offsetMax = Vector2.zero;
            _nameField.onValueChanged.AddListener(_ => Validate());

            // Max Deck Size label
            var sizeLabelRt = UIHelper.CreatePanel(panel, "SizeLabel",
                new Vector2(0.05f, 0.48f), new Vector2(0.95f, 0.57f));
            UIHelper.CreateText(sizeLabelRt, "Lbl", "Max Deck Size (default 50)", fontSize: 22,
                alignment: TextAlignmentOptions.MidlineLeft);

            // Max Deck Size input field
            _maxSizeField = UIHelper.CreateInputField(panel, "MaxSizeField",
                "50",
                Vector2.zero, Vector2.zero, fontSize: 22,
                contentType: TMP_InputField.ContentType.IntegerNumber);
            var mfRt = _maxSizeField.GetComponent<RectTransform>();
            mfRt.anchorMin = new Vector2(0.05f, 0.35f);
            mfRt.anchorMax = new Vector2(0.95f, 0.47f);
            mfRt.offsetMin = mfRt.offsetMax = Vector2.zero;
            _maxSizeField.text = "50";
            _maxSizeField.onValueChanged.AddListener(_ => Validate());

            // Validation error text
            var errRt = UIHelper.CreatePanel(panel, "ErrorPanel",
                new Vector2(0.05f, 0.26f), new Vector2(0.95f, 0.34f));
            _errorText = UIHelper.CreateText(errRt, "ErrorText", "", fontSize: 18,
                color: new Color(1f, 0.35f, 0.35f));

            // Create Deck! button
            _createBtn = AnchorButton(panel, "CreateBtn", "Create Deck!",
                new Vector2(0.12f, 0.12f), new Vector2(0.88f, 0.24f),
                new Color(0.15f, 0.5f, 0.15f), fontSize: 26);
            _createBtn.onClick.AddListener(OnCreateClicked);

            // Back button
            var backBtn = AnchorButton(panel, "BackBtn", "Back",
                new Vector2(0.05f, 0.02f), new Vector2(0.4f, 0.10f),
                new Color(0.3f, 0.3f, 0.3f), fontSize: 20);
            backBtn.onClick.AddListener(OnBackClicked);

            Validate();

            // Start hidden — will be shown via Show()
            _canvas.gameObject.SetActive(false);
        }

        /// <summary>Anchor-based button helper (no anchoredPosition required).</summary>
        private static Button AnchorButton(RectTransform parent, string name, string label,
            Vector2 anchorMin, Vector2 anchorMax, Color bgColor, int fontSize)
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
            btn.colors = new ColorBlock
            {
                normalColor     = bgColor,
                highlightedColor = bgColor * 1.3f,
                pressedColor    = bgColor * 0.7f,
                disabledColor   = new Color(0.25f, 0.25f, 0.25f, 0.6f),
                colorMultiplier = 1f,
                fadeDuration    = 0.1f
            };

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

        // ── Show / Hide ───────────────────────────────────────────────────────────

        public void Show()
        {
            RTGLog.Section("CreateDeckScreen — Show");
            _nameField.text = "";
            _maxSizeField.text = "50";
            _errorText.text = "";
            Validate();
            _canvas.gameObject.SetActive(true);
        }

        public void Hide()
        {
            RTGLog.Section("CreateDeckScreen — Hide");
            _canvas.gameObject.SetActive(false);
        }

        // ── Validation ────────────────────────────────────────────────────────────

        private void Validate()
        {
            string deckName = _nameField.text.Trim();
            bool nameOk = deckName.Length > 0;
            bool unique = DeckManager.instance?.AllDecks.Find(d => d.name == deckName) == null;
            bool sizeOk = int.TryParse(_maxSizeField.text, out int sz) && sz >= 1;

            string error = "";
            if (deckName.Length > 0 && !unique) error = "A deck with this name already exists.";
            else if (_maxSizeField.text.Length > 0 && !sizeOk) error = "Max size must be a number ≥ 1.";

            _errorText.text = error;
            _createBtn.interactable = nameOk && unique && sizeOk;
        }

        // ── Callbacks ─────────────────────────────────────────────────────────────

        private void OnCreateClicked()
        {
            string deckName = _nameField.text.Trim();
            int maxSize = int.TryParse(_maxSizeField.text, out int sz) ? sz : 50;

            RTGLog.Section($"CreateDeckScreen — Create Deck '{deckName}'");
            RTGLog.Line($"maxSize={maxSize}");

            DeckData newDeck = DeckManager.instance.CreateDeck(deckName, maxSize);
            Hide();
            DeckEditorScreen.instance?.Open(newDeck);
        }

        private void OnBackClicked()
        {
            RTGLog.Section("CreateDeckScreen — Back");
            Hide();
            DeckSelectorScreen.instance?.Show();
        }
    }
}
