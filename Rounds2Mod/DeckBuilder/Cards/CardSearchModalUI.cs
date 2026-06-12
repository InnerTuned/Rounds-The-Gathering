using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DeckBuilder.UI;
using InfoOverhaul.Delta;
using RarityLib.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DeckBuilder.Cards;

/// <summary>
/// Reusable in-game deck search modal — opaque, centered, with search, pagination,
/// and Cancel / Select buttons. Returns the chosen card or null on cancel.
/// </summary>
public class CardSearchModalUI : MonoBehaviour
{
    public static CardSearchModalUI instance;

    private const int CardsPerPage = 15;
    private const int MinSearchLength = 3;
    private const float CardCellWidth = 220f;
    private const float CardCellHeight = 300f;

    private static readonly Color PanelBg = new Color(0.04f, 0.06f, 0.12f, 1f);
    private static readonly Color CancelBg = new Color(0.40f, 0.10f, 0.10f, 1f);
    private static readonly Color SelectReadyBg = new Color(0.10f, 0.45f, 0.12f, 1f);
    private static readonly Color SelectGrayBg = new Color(0.18f, 0.18f, 0.18f, 1f);
    private static readonly Color TileIdleBg = new Color(0.10f, 0.10f, 0.15f, 1f);
    private static readonly Color TileHoverBg = new Color(0.25f, 0.55f, 1.00f, 1f);
    private static readonly Color TileSelectedBg = new Color(0.15f, 0.75f, 0.25f, 1f);
    private static readonly Color PageIdleColor = new Color(0.15f, 0.15f, 0.22f, 1f);
    private static readonly Color PageSelectedColor = Color.white;

    private Canvas _canvas;
    private TextMeshProUGUI _titleText;
    private TextMeshProUGUI _messageText;
    private TextMeshProUGUI _deltaPreviewText;
    private TMP_InputField _searchField;
    private Transform _cardGridContent;
    private Transform _pageButtonParent;
    private Button _selectButton;
    private Image _selectImage;
    private TextMeshProUGUI _selectLabel;

    private List<CardInfo> _allCards = new();
    private List<CardInfo> _filteredCards = new();
    private readonly List<CardSearchTile> _tiles = new();
    private readonly List<Button> _pageButtons = new();

    private int _pickerID;
    private int _currentPage;
    private string _searchQuery = "";
    private CardInfo _selectedCard;
    private Action<CardInfo> _onConfirm;
    private Action _onCancel;
    private Coroutine _refreshVisualsCoroutine;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        BuildUI();
        _canvas.gameObject.SetActive(false);
    }

    private void BuildUI()
    {
        _canvas = UIHelper.CreateFullscreenCanvas("RTG_CardSearchModal", sortOrder: 185);

        UIHelper.CreatePanel(_canvas.transform, "Dim",
            Vector2.zero, Vector2.one, bg: new Color(0f, 0f, 0f, 0.65f));

        var panel = UIHelper.CreatePanel(_canvas.transform, "Panel",
            new Vector2(0.08f, 0.10f), new Vector2(0.92f, 0.90f), bg: PanelBg);

        _titleText = UIHelper.CreateText(panel, "Title", "Search Deck",
            fontSize: 28, alignment: TextAlignmentOptions.Top,
            color: new Color(0.9f, 0.93f, 1f));

        _searchField = UIHelper.CreateInputField(panel, "SearchField", "Search cards (3+ chars)...",
            Vector2.zero, Vector2.zero, fontSize: 20);
        var searchRt = _searchField.GetComponent<RectTransform>();
        searchRt.anchorMin = new Vector2(0.03f, 0.90f);
        searchRt.anchorMax = new Vector2(0.45f, 0.97f);
        searchRt.offsetMin = searchRt.offsetMax = Vector2.zero;
        _searchField.onValueChanged.AddListener(OnSearchChanged);

        _messageText = UIHelper.CreateText(panel, "Message", "",
            fontSize: 20, alignment: TextAlignmentOptions.MidlineLeft,
            color: new Color(0.88f, 0.92f, 1f));
        var msgRt = _messageText.GetComponent<RectTransform>();
        msgRt.anchorMin = new Vector2(0.47f, 0.90f);
        msgRt.anchorMax = new Vector2(0.97f, 0.97f);
        msgRt.offsetMin = msgRt.offsetMax = Vector2.zero;

        var deltaPanel = UIHelper.CreatePanel(panel, "DeltaPanel",
            new Vector2(0.03f, 0.82f), new Vector2(0.97f, 0.89f),
            bg: new Color(0.03f, 0.04f, 0.08f, 1f));
        _deltaPreviewText = UIHelper.CreateText(deltaPanel, "DeltaText", "",
            fontSize: 17, alignment: TextAlignmentOptions.MidlineLeft,
            color: new Color(0.88f, 0.92f, 1f));
        _deltaPreviewText.richText = true;

        var listPanel = UIHelper.CreatePanel(panel, "ListPanel",
            new Vector2(0.03f, 0.16f), new Vector2(0.97f, 0.81f),
            bg: new Color(0.05f, 0.06f, 0.10f, 1f));
        BuildCardGridScroll(listPanel);

        var pageBar = UIHelper.CreatePanel(panel, "PageBar",
            new Vector2(0.03f, 0.09f), new Vector2(0.97f, 0.15f),
            bg: new Color(0.08f, 0.08f, 0.12f, 1f));
        BuildPageBar(pageBar);

        var cancelBtn = UIHelper.CreateButton(panel, "CancelBtn", "[Cancel]",
            Vector2.zero, Vector2.zero, fontSize: 20, bgColor: CancelBg);
        var cancelRt = cancelBtn.GetComponent<RectTransform>();
        cancelRt.anchorMin = new Vector2(0.03f, 0.02f);
        cancelRt.anchorMax = new Vector2(0.30f, 0.08f);
        cancelRt.offsetMin = cancelRt.offsetMax = Vector2.zero;
        cancelBtn.onClick.AddListener(OnCancelClicked);

        var selGo = new GameObject("SelectBtn");
        selGo.transform.SetParent(panel, false);
        var selRt = selGo.AddComponent<RectTransform>();
        selRt.anchorMin = new Vector2(0.70f, 0.02f);
        selRt.anchorMax = new Vector2(0.97f, 0.08f);
        selRt.offsetMin = selRt.offsetMax = Vector2.zero;

        _selectImage = selGo.AddComponent<Image>();
        _selectImage.color = SelectGrayBg;
        _selectButton = selGo.AddComponent<Button>();
        _selectButton.targetGraphic = _selectImage;
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
    }

    private void BuildCardGridScroll(RectTransform listPanel)
    {
        var scrollGo = new GameObject("CardScroll");
        scrollGo.transform.SetParent(listPanel, false);
        var scrollRt = scrollGo.AddComponent<RectTransform>();
        scrollRt.anchorMin = Vector2.zero;
        scrollRt.anchorMax = Vector2.one;
        scrollRt.offsetMin = scrollRt.offsetMax = Vector2.zero;

        var scroll = scrollGo.AddComponent<ScrollRect>();
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.scrollSensitivity = 80f;

        var vpGo = new GameObject("Viewport");
        vpGo.transform.SetParent(scrollGo.transform, false);
        var vpRt = vpGo.AddComponent<RectTransform>();
        vpRt.anchorMin = Vector2.zero;
        vpRt.anchorMax = Vector2.one;
        vpRt.offsetMin = vpRt.offsetMax = Vector2.zero;
        vpGo.AddComponent<Image>().color = new Color(1f, 1f, 1f, 0.01f);
        vpGo.AddComponent<Mask>().showMaskGraphic = false;
        scroll.viewport = vpRt;

        var contentGo = new GameObject("Content");
        contentGo.transform.SetParent(vpGo.transform, false);
        var contentRt = contentGo.AddComponent<RectTransform>();
        contentRt.anchorMin = new Vector2(0f, 1f);
        contentRt.anchorMax = new Vector2(1f, 1f);
        contentRt.pivot = new Vector2(0f, 1f);
        contentRt.anchoredPosition = Vector2.zero;
        contentRt.sizeDelta = Vector2.zero;

        var glg = contentGo.AddComponent<GridLayoutGroup>();
        glg.cellSize = new Vector2(CardCellWidth, CardCellHeight);
        glg.spacing = new Vector2(10f, 10f);
        glg.padding = new RectOffset(10, 10, 10, 10);
        glg.startCorner = GridLayoutGroup.Corner.UpperLeft;
        glg.startAxis = GridLayoutGroup.Axis.Horizontal;
        glg.childAlignment = TextAnchor.UpperLeft;
        glg.constraint = GridLayoutGroup.Constraint.Flexible;

        var csf = contentGo.AddComponent<ContentSizeFitter>();
        csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scroll.content = contentRt;
        _cardGridContent = contentRt;
    }

    private void BuildPageBar(RectTransform pageBar)
    {
        var pageContentGo = new GameObject("PageContent");
        pageContentGo.transform.SetParent(pageBar, false);
        var pageContentRt = pageContentGo.AddComponent<RectTransform>();
        pageContentRt.anchorMin = Vector2.zero;
        pageContentRt.anchorMax = Vector2.one;
        pageContentRt.offsetMin = new Vector2(8f, 4f);
        pageContentRt.offsetMax = new Vector2(-8f, -4f);
        _pageButtonParent = pageContentRt;

        var pageHlg = pageContentGo.AddComponent<HorizontalLayoutGroup>();
        pageHlg.childControlWidth = true;
        pageHlg.childControlHeight = true;
        pageHlg.childForceExpandWidth = false;
        pageHlg.childForceExpandHeight = true;
        pageHlg.spacing = 6f;
        pageHlg.childAlignment = TextAnchor.MiddleLeft;
    }

    public void Show(int pickerID, string title, string message, IReadOnlyList<CardInfo> cards,
        Action<CardInfo> onConfirm, Action onCancel)
    {
        _pickerID = pickerID;
        _onConfirm = onConfirm;
        _onCancel = onCancel;
        _selectedCard = null;
        _currentPage = 0;
        _searchQuery = "";
        _allCards = new List<CardInfo>(cards ?? Array.Empty<CardInfo>());
        _filteredCards = new List<CardInfo>(_allCards);

        _titleText.text = title;
        _messageText.text = message;
        _deltaPreviewText.text = "";
        if (_searchField != null)
            _searchField.SetTextWithoutNotify("");

        if (_allCards.Count == 0)
            _deltaPreviewText.text = "<i>No cards available — press Cancel to return to the draft.</i>";

        RefreshSelectButton();
        RebuildPageButtons();
        ShowPage(0);
        _canvas.gameObject.SetActive(true);

        SearchCardLog.Section("Search modal shown");
        SearchCardLog.Line($"title='{title}', cards={_allCards.Count}, pickerID={pickerID}");
    }

    public void Hide()
    {
        if (_refreshVisualsCoroutine != null)
        {
            StopCoroutine(_refreshVisualsCoroutine);
            _refreshVisualsCoroutine = null;
        }

        _canvas.gameObject.SetActive(false);
        ClearTiles();
        _onConfirm = null;
        _onCancel = null;
        _selectedCard = null;
        _allCards.Clear();
        _filteredCards.Clear();
    }

    private void OnSearchChanged(string query)
    {
        _searchQuery = query?.Trim() ?? "";
        _currentPage = 0;
        _selectedCard = null;
        _deltaPreviewText.text = "";

        if (_searchQuery.Length >= MinSearchLength)
            _filteredCards = FilterCards(_searchQuery);
        else
            _filteredCards = new List<CardInfo>(_allCards);

        RefreshSelectButton();
        RebuildPageButtons();
        ShowPage(0);
    }

    private List<CardInfo> FilterCards(string query)
    {
        var results = new List<CardInfo>();
        string lower = query.ToLowerInvariant();

        foreach (var card in _allCards)
        {
            if (card == null)
                continue;

            bool titleMatch = !string.IsNullOrEmpty(card.cardName)
                && card.cardName.ToLowerInvariant().Contains(lower);
            bool descMatch = !string.IsNullOrEmpty(card.cardDestription)
                && card.cardDestription.ToLowerInvariant().Contains(lower);

            if (titleMatch || descMatch)
                results.Add(card);
        }

        return results;
    }

    private void ShowPage(int pageIndex)
    {
        _currentPage = Mathf.Max(0, pageIndex);
        ClearTiles();

        int start = _currentPage * CardsPerPage;
        int end = Mathf.Min(start + CardsPerPage, _filteredCards.Count);

        for (int i = start; i < end; i++)
        {
            var card = _filteredCards[i];
            var tile = CardSearchTile.Create(_cardGridContent, card, OnTileClicked, OnTileHovered, OnTileHoverExit);
            _tiles.Add(tile);
        }

        UpdatePageButtonStyles();
        ScheduleVisualRefresh();
    }

    private void ScheduleVisualRefresh()
    {
        if (_refreshVisualsCoroutine != null)
            StopCoroutine(_refreshVisualsCoroutine);

        _refreshVisualsCoroutine = StartCoroutine(RefreshTileVisualsNextFrame());
    }

    private IEnumerator RefreshTileVisualsNextFrame()
    {
        yield return null;

        foreach (var tile in _tiles)
        {
            if (tile?.Root == null || tile.Card == null)
                continue;

            var visual = tile.Root.transform.Find("CardVisual")?.gameObject;
            if (visual != null)
                CardVisualHelper.FinalizeStaticCardVisual(visual, tile.Card);
        }

        Canvas.ForceUpdateCanvases();
        _refreshVisualsCoroutine = null;
    }

    private void ClearTiles()
    {
        foreach (var tile in _tiles)
        {
            if (tile?.Root != null)
                Destroy(tile.Root);
        }
        _tiles.Clear();
    }

    private int GetPageCount()
    {
        if (_filteredCards.Count == 0)
            return 1;
        return Mathf.CeilToInt(_filteredCards.Count / (float)CardsPerPage);
    }

    private void RebuildPageButtons()
    {
        foreach (Transform child in _pageButtonParent)
            Destroy(child.gameObject);
        _pageButtons.Clear();

        int pageCount = GetPageCount();
        for (int i = 0; i < pageCount; i++)
        {
            int pageIndex = i;
            var btn = CreatePageButton(i + 1);
            btn.onClick.AddListener(() => ShowPage(pageIndex));
            _pageButtons.Add(btn);
        }

        UpdatePageButtonStyles();
    }

    private Button CreatePageButton(int pageNumber)
    {
        var go = new GameObject($"Page_{pageNumber}");
        go.transform.SetParent(_pageButtonParent, false);

        var img = go.AddComponent<Image>();
        img.color = PageIdleColor;

        var btn = go.AddComponent<Button>();
        btn.targetGraphic = img;

        var le = go.AddComponent<LayoutElement>();
        le.preferredWidth = 40f;
        le.minWidth = 40f;

        var labelGo = new GameObject("Label");
        labelGo.transform.SetParent(go.transform, false);
        var labelRt = labelGo.AddComponent<RectTransform>();
        labelRt.anchorMin = Vector2.zero;
        labelRt.anchorMax = Vector2.one;
        labelRt.offsetMin = labelRt.offsetMax = Vector2.zero;

        var tmp = labelGo.AddComponent<TextMeshProUGUI>();
        tmp.text = pageNumber.ToString();
        tmp.fontSize = 18;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.raycastTarget = false;

        return btn;
    }

    private void UpdatePageButtonStyles()
    {
        for (int i = 0; i < _pageButtons.Count; i++)
        {
            var btn = _pageButtons[i];
            if (btn == null)
                continue;

            bool selected = i == _currentPage;
            var img = btn.GetComponent<Image>();
            if (img != null)
                img.color = selected ? PageSelectedColor : PageIdleColor;

            var label = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
                label.color = selected ? new Color(0.1f, 0.1f, 0.15f) : Color.white;
        }
    }

    private void OnTileClicked(CardInfo card)
    {
        _selectedCard = card;
        foreach (var tile in _tiles)
            tile.SetSelected(tile.Card == card);

        UpdateDeltaPreview(card);
        RefreshSelectButton();
        SearchCardLog.Line($"Card selected: '{card?.cardName ?? "null"}'.");
    }

    private void OnTileHovered(CardInfo card)
    {
        if (_selectedCard != null)
            return;

        UpdateDeltaPreview(card);
    }

    private void OnTileHoverExit()
    {
        if (_selectedCard != null)
            return;

        _deltaPreviewText.text = "";
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

        if (!CardDeltaRegistry.TryGetDeltas(picker, card, out var deltas)
            || deltas == null
            || deltas.Count == 0)
        {
            _deltaPreviewText.text = $"<i>No stat changes from adding {card.cardName}</i>";
            return;
        }

        var sb = new StringBuilder();
        sb.Append($"<b>If adding {card.cardName}:</b>  ");
        for (int i = 0; i < deltas.Count && i < 6; i++)
        {
            if (i > 0) sb.Append("  |  ");
            sb.Append(deltas[i].FormatRichText());
        }
        if (deltas.Count > 6)
            sb.Append($"  ... +{deltas.Count - 6} more");

        _deltaPreviewText.text = sb.ToString();
    }

    private void RefreshSelectButton()
    {
        bool ready = _selectedCard != null;
        _selectButton.interactable = ready;
        _selectImage.color = ready ? SelectReadyBg : SelectGrayBg;
        _selectLabel.color = ready ? Color.white : new Color(0.45f, 0.45f, 0.45f);
    }

    private void OnSelectClicked()
    {
        if (_selectedCard == null)
            return;

        var card = _selectedCard;
        var cb = _onConfirm;
        Hide();
        cb?.Invoke(card);
    }

    private void OnCancelClicked()
    {
        var cb = _onCancel;
        Hide();
        cb?.Invoke();
    }

    private sealed class CardSearchTile : MonoBehaviour
    {
        internal GameObject Root;
        internal CardInfo Card;
        private Image _bg;
        private bool _selected;

        internal static CardSearchTile Create(Transform parent, CardInfo card,
            Action<CardInfo> onClick, Action<CardInfo> onHover, Action onHoverExit)
        {
            var go = new GameObject($"SearchCard_{card.cardName}");
            go.transform.SetParent(parent, false);

            var le = go.AddComponent<LayoutElement>();
            le.preferredWidth = CardCellWidth;
            le.preferredHeight = CardCellHeight;

            var tile = go.AddComponent<CardSearchTile>();
            tile.Root = go;
            tile.Card = card;
            tile.Build(card);
            go.AddComponent<CardSearchTileBehaviour>().Init(tile, onClick, onHover, onHoverExit);
            return tile;
        }

        private void Build(CardInfo card)
        {
            _bg = gameObject.AddComponent<Image>();
            _bg.color = TileIdleBg;
            _bg.raycastTarget = true;

            CardVisualHelper.SetupCardVisual(card, gameObject);

            var rarityLabelGo = new GameObject("RarityLabel");
            rarityLabelGo.transform.SetParent(transform, false);
            var rarityLabelRt = rarityLabelGo.AddComponent<RectTransform>();
            rarityLabelRt.anchorMin = new Vector2(0f, 0f);
            rarityLabelRt.anchorMax = new Vector2(1f, 0.12f);
            rarityLabelRt.offsetMin = rarityLabelRt.offsetMax = Vector2.zero;

            var rarityBg = rarityLabelGo.AddComponent<Image>();
            rarityBg.color = new Color(0f, 0f, 0f, 0.7f);
            rarityBg.raycastTarget = false;

            string rarityName;
            try
            {
                rarityName = RarityUtils.GetRarityData(card.rarity).name;
            }
            catch
            {
                rarityName = card.rarity.ToString();
            }

            var rarityTextGo = new GameObject("RarityText");
            rarityTextGo.transform.SetParent(rarityLabelGo.transform, false);
            var rarityTextRt = rarityTextGo.AddComponent<RectTransform>();
            rarityTextRt.anchorMin = Vector2.zero;
            rarityTextRt.anchorMax = Vector2.one;
            rarityTextRt.offsetMin = rarityTextRt.offsetMax = Vector2.zero;

            var rarityTmp = rarityTextGo.AddComponent<TextMeshProUGUI>();
            rarityTmp.text = $"{card.cardName} ({rarityName})";
            rarityTmp.fontSize = 11;
            rarityTmp.alignment = TextAlignmentOptions.Center;
            rarityTmp.color = RarityColor(card.rarity);
            rarityTmp.enableWordWrapping = false;
            rarityTmp.overflowMode = TextOverflowModes.Ellipsis;
            rarityTmp.raycastTarget = false;
        }

        internal void SetSelected(bool selected)
        {
            _selected = selected;
            if (_bg != null)
                _bg.color = selected ? TileSelectedBg : TileIdleBg;
        }

        internal void SetHovered(bool hovered)
        {
            if (_selected || _bg == null)
                return;

            _bg.color = hovered ? TileHoverBg : TileIdleBg;
        }

        private static Color RarityColor(CardInfo.Rarity rarity)
        {
            try
            {
                return RarityUtils.GetRarityData(rarity).color;
            }
            catch
            {
                return Color.white;
            }
        }
    }

    private sealed class CardSearchTileBehaviour : MonoBehaviour,
        IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private CardSearchTile _tile;
        private Action<CardInfo> _onClick;
        private Action<CardInfo> _onHover;
        private Action _onHoverExit;

        internal void Init(CardSearchTile tile, Action<CardInfo> onClick,
            Action<CardInfo> onHover, Action onHoverExit)
        {
            _tile = tile;
            _onClick = onClick;
            _onHover = onHover;
            _onHoverExit = onHoverExit;
        }

        public void OnPointerClick(PointerEventData _) => _onClick?.Invoke(_tile.Card);
        public void OnPointerEnter(PointerEventData _)
        {
            _tile.SetHovered(true);
            _onHover?.Invoke(_tile.Card);
        }

        public void OnPointerExit(PointerEventData _)
        {
            _tile.SetHovered(false);
            _onHoverExit?.Invoke();
        }
    }
}
