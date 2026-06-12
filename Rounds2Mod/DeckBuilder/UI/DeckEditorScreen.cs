using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnboundLib;
using UnboundLib.Utils;
using DeckBuilder.Data;
using DeckBuilder.GameIntegration;

namespace DeckBuilder.UI
{
    /// <summary>
    /// Screen 3 — full card editor.
    /// Mirrors the visual layout of ToggleCardsMenuHandler with per-card [-] # [+] controls.
    /// </summary>
    public class DeckEditorScreen : MonoBehaviour
    {
        public static DeckEditorScreen instance;

        public bool IsOpen => _canvas != null && _canvas.gameObject.activeInHierarchy;

        private const string MyDeckCategory = "My Deck";
        private const string AllCardsCategory = "All Cards";
        private const string SearchResultsCategory = "Search Results";
        private const int CardsPerPage = 50;
        private const int MinSearchLength = 3;

        private static readonly Color PageIdleColor = new Color(0.15f, 0.15f, 0.22f);
        private static readonly Color PageSelectedColor = Color.white;
        private static readonly Color PageHoverColor = new Color(0.35f, 0.55f, 1f);
        private static readonly Color PagePressedColor = new Color(0.25f, 0.45f, 0.9f);
        private static readonly Color PageSelectedTextColor = new Color(0.1f, 0.1f, 0.15f);
        private static readonly Color PageIdleTextColor = Color.white;

        // ── State ─────────────────────────────────────────────────────────────────

        private DeckData _deck;
        private string _currentCategory;
        private int _currentPage;
        private string _searchQuery = "";
        private bool _isSearchActive;
        private List<CardInfo> _cachedCategoryCards = new List<CardInfo>();

        // ── UI roots ──────────────────────────────────────────────────────────────

        private Canvas _canvas;
        private TMP_InputField _searchField;
        private TextMeshProUGUI _deckCountText;
        private Transform _cardGridContent;
        private Transform _categoryButtonParent;
        private Transform _pageButtonParent;
        private ScrollRect _cardScrollRect;
        private readonly List<Button> _categoryButtons = new List<Button>();
        private readonly List<string> _categoryButtonNames = new List<string>();
        private readonly List<Button> _pageButtons = new List<Button>();

        // ── Modal ─────────────────────────────────────────────────────────────────

        private GameObject _maxSizeModal;
        private TMP_InputField _maxSizeModalField;
        private TextMeshProUGUI _maxSizeModalError;

        // ── Card row tracking ─────────────────────────────────────────────────────

        private readonly List<GameObject> _cardRows = new List<GameObject>();

        private void Awake()
        {
            instance = this;
            BuildUI();
        }

        // ── Open / Close ──────────────────────────────────────────────────────────

        public void Open(DeckData deck)
        {
            RTGLog.Section($"DeckEditorScreen — Open deck '{deck?.name}'");
            _deck = deck;
            ClearSearch();
            _canvas.gameObject.SetActive(true);
            BuildCategoryButtons();
            ShowCategory(AllCardsCategory);
        }

        private void OnDisable()
        {
            RTGLog.Section("DeckEditorScreen — OnDisable (close)");
            if (_deck == null)
            {
                RTGLog.Line("No deck loaded — nothing to save.");
                return;
            }
            if (_deck.TotalCount <= _deck.maxSize)
            {
                DeckManager.instance?.Save();
                RTGLog.Line($"Auto-saved '{_deck.name}' ({_deck.TotalCount}/{_deck.maxSize} cards).");
            }
            else
            {
                RTGLog.Warn($"Deck over limit ({_deck.TotalCount}/{_deck.maxSize}) — NOT saved.");
            }
        }

        // ── Build ─────────────────────────────────────────────────────────────────

        private void BuildUI()
        {
            _canvas = UIHelper.CreateFullscreenCanvas("RTG_DeckEditorScreen", sortOrder: 220);
            // Note: Do NOT parent this component to the canvas - all screens share a single uiRoot

            UIHelper.CreateDimmedBackground(_canvas.transform);

            // ── Main layout: left column + right content ──────────────────────────

            // Outer container
            var outer = UIHelper.CreateStretchPanel(_canvas.transform, "Outer",
                20, 20, 20, 20,
                new Color(0.06f, 0.06f, 0.09f, 0.97f));

            // ── Top bar ───────────────────────────────────────────────────────────

            var topBar = UIHelper.CreatePanel(outer, "TopBar",
                new Vector2(0f, 0.92f), Vector2.one,
                bg: new Color(0.1f, 0.1f, 0.15f, 0.95f));

            _searchField = UIHelper.CreateInputField(topBar, "SearchField", "Search cards (3+ chars)...",
                Vector2.zero, Vector2.zero, fontSize: 20);
            var searchRt = _searchField.GetComponent<RectTransform>();
            searchRt.anchorMin = new Vector2(0.01f, 0.15f);
            searchRt.anchorMax = new Vector2(0.35f, 0.85f);
            searchRt.offsetMin = searchRt.offsetMax = Vector2.zero;
            _searchField.onValueChanged.AddListener(OnSearchChanged);

            _deckCountText = UIHelper.CreateText(topBar, "DeckCountText", "Cards in Deck: 0",
                fontSize: 22, alignment: TextAlignmentOptions.MidlineLeft);
            var dcRt = _deckCountText.GetComponent<RectTransform>();
            dcRt.anchorMin = new Vector2(0.36f, 0f);
            dcRt.anchorMax = new Vector2(0.65f, 1f);
            dcRt.offsetMin = dcRt.offsetMax = Vector2.zero;

            // Change Max Size button
            var changeSizeBtn = UIHelper.CreateButton(topBar, "ChangeSizeBtn", "Change Max Size",
                Vector2.zero, Vector2.zero, fontSize: 18,
                bgColor: new Color(0.35f, 0.2f, 0.05f));
            var csRt = changeSizeBtn.GetComponent<RectTransform>();
            csRt.anchorMin = new Vector2(0.66f, 0.1f);
            csRt.anchorMax = new Vector2(0.85f, 0.9f);
            csRt.offsetMin = csRt.offsetMax = Vector2.zero;
            changeSizeBtn.onClick.AddListener(OpenMaxSizeModal);

            // Close button
            var closeBtn = UIHelper.CreateButton(topBar, "CloseBtn", "Close",
                Vector2.zero, Vector2.zero, fontSize: 18,
                bgColor: new Color(0.45f, 0.1f, 0.1f));
            var clRt = closeBtn.GetComponent<RectTransform>();
            clRt.anchorMin = new Vector2(0.87f, 0.1f);
            clRt.anchorMax = new Vector2(0.99f, 0.9f);
            clRt.offsetMin = clRt.offsetMax = Vector2.zero;
            closeBtn.onClick.AddListener(CloseScreen);

            // ── Left column: categories ───────────────────────────────────────────

            var leftPanel = UIHelper.CreatePanel(outer, "LeftPanel",
                Vector2.zero, new Vector2(0.18f, 0.92f),
                bg: new Color(0.08f, 0.08f, 0.12f, 0.9f));

            // Category scroll
            var catScrollGo = BuildScrollRect(leftPanel, "CategoryScroll",
                out _categoryButtonParent);
            var catSrRt = catScrollGo.GetComponent<RectTransform>();
            catSrRt.anchorMin = Vector2.zero;
            catSrRt.anchorMax = Vector2.one;
            catSrRt.offsetMin = catSrRt.offsetMax = Vector2.zero;
            _categoryButtonParent.GetComponent<RectTransform>().GetComponent<VerticalLayoutGroup>()?.GetType();
            var vlg = _categoryButtonParent.gameObject.AddComponent<VerticalLayoutGroup>();
            vlg.childControlHeight = false;
            vlg.childForceExpandHeight = false;
            vlg.spacing = 4;
            vlg.padding = new RectOffset(4, 4, 4, 4);

            // ── Right panel: card grid + pagination ───────────────────────────────

            var rightPanel = UIHelper.CreatePanel(outer, "RightPanel",
                new Vector2(0.19f, 0f), new Vector2(1f, 0.92f),
                bg: new Color(0.05f, 0.05f, 0.08f, 1f));

            var pageBar = UIHelper.CreatePanel(rightPanel, "PageBar",
                new Vector2(0f, 0f), new Vector2(1f, 0.07f),
                bg: new Color(0.08f, 0.08f, 0.12f, 0.95f));

            // Simple panel with HorizontalLayoutGroup (no ScrollRect - avoids raycast issues)
            var pageContentGo = new GameObject("PageContent");
            pageContentGo.transform.SetParent(pageBar.transform, false);
            var pageContentRt = pageContentGo.AddComponent<RectTransform>();
            pageContentRt.anchorMin = Vector2.zero;
            pageContentRt.anchorMax = Vector2.one;
            pageContentRt.pivot = new Vector2(0f, 0.5f);
            pageContentRt.offsetMin = new Vector2(8f, 4f);
            pageContentRt.offsetMax = new Vector2(-8f, -4f);
            _pageButtonParent = pageContentRt;

            var pageHlg = pageContentGo.AddComponent<HorizontalLayoutGroup>();
            pageHlg.childControlWidth = true;
            pageHlg.childControlHeight = true;
            pageHlg.childForceExpandWidth = false;
            pageHlg.childForceExpandHeight = true;
            pageHlg.spacing = 6;
            pageHlg.padding = new RectOffset(4, 4, 0, 0);
            pageHlg.childAlignment = TextAnchor.MiddleLeft;

            var cardScrollGo = BuildScrollRect(rightPanel, "CardScroll",
                out _cardGridContent);
            var cardSrRt = cardScrollGo.GetComponent<RectTransform>();
            cardSrRt.anchorMin = new Vector2(0f, 0.07f);
            cardSrRt.anchorMax = Vector2.one;
            cardSrRt.offsetMin = cardSrRt.offsetMax = Vector2.zero;

            var glg = _cardGridContent.gameObject.AddComponent<GridLayoutGroup>();
            glg.cellSize = new Vector2(220, 300);
            glg.spacing = new Vector2(10, 10);
            glg.padding = new RectOffset(10, 10, 10, 10);
            glg.startCorner = GridLayoutGroup.Corner.UpperLeft;
            glg.startAxis = GridLayoutGroup.Axis.Horizontal;
            glg.childAlignment = TextAnchor.UpperLeft;
            glg.constraint = GridLayoutGroup.Constraint.Flexible;

            // Store reference for logging
            _cardScrollRect = cardScrollGo.GetComponent<ScrollRect>();

            // ── Max size modal (hidden) ────────────────────────────────────────────

            BuildMaxSizeModal(outer);

            // Start hidden — will be shown via Open()
            _canvas.gameObject.SetActive(false);
        }

        // ── Scroll rect builder ───────────────────────────────────────────────────

        private static GameObject BuildScrollRect(Transform parent, string name,
            out Transform content, bool horizontal = false)
        {
            var srGo = new GameObject(name);
            srGo.transform.SetParent(parent, false);
            var srRt = srGo.AddComponent<RectTransform>();
            srRt.anchorMin = Vector2.zero;
            srRt.anchorMax = Vector2.one;
            srRt.offsetMin = srRt.offsetMax = Vector2.zero;

            var sr = srGo.AddComponent<ScrollRect>();
            sr.horizontal = horizontal;
            sr.vertical = !horizontal;
            sr.movementType = ScrollRect.MovementType.Clamped;
            sr.scrollSensitivity = 80f;

            var vpGo = new GameObject("Viewport");
            vpGo.transform.SetParent(srGo.transform, false);
            var vpRt = vpGo.AddComponent<RectTransform>();
            vpRt.anchorMin = Vector2.zero;
            vpRt.anchorMax = Vector2.one;
            vpRt.offsetMin = vpRt.offsetMax = Vector2.zero;
            vpRt.pivot = horizontal ? new Vector2(0f, 0.5f) : new Vector2(0f, 1f);
            var vpImg = vpGo.AddComponent<Image>();
            vpImg.color = new Color(1, 1, 1, 0.01f);
            var vpMask = vpGo.AddComponent<Mask>();
            vpMask.showMaskGraphic = false;
            sr.viewport = vpRt;

            var contentGo = new GameObject("Content");
            contentGo.transform.SetParent(vpGo.transform, false);
            var contentRt = contentGo.AddComponent<RectTransform>();
            if (horizontal)
            {
                contentRt.anchorMin = new Vector2(0f, 0f);
                contentRt.anchorMax = new Vector2(0f, 1f);
                contentRt.pivot = new Vector2(0f, 0.5f);
            }
            else
            {
                contentRt.anchorMin = new Vector2(0f, 1f);
                contentRt.anchorMax = new Vector2(1f, 1f);
                contentRt.pivot = new Vector2(0f, 1f);
            }
            contentRt.sizeDelta = Vector2.zero;
            contentRt.anchoredPosition = Vector2.zero;

            var csf = contentGo.AddComponent<ContentSizeFitter>();
            if (horizontal)
            {
                csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                csf.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            }
            else
            {
                csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }

            sr.content = contentRt;
            content = contentRt;
            return srGo;
        }

        // ── Category buttons ──────────────────────────────────────────────────────

        private void BuildCategoryButtons()
        {
            RTGLog.Section("DeckEditorScreen — BuildCategoryButtons");
            foreach (Transform child in _categoryButtonParent)
                Destroy(child.gameObject);
            _categoryButtons.Clear();
            _categoryButtonNames.Clear();

            RTGLog.Line($"CardManager.categories.Count = {CardManager.categories.Count}");
            if (CardManager.categories.Count == 0)
            {
                RTGLog.Warn("No categories found in CardManager!");
            }

            AddCategoryButton(MyDeckCategory);
            AddCategoryButton(AllCardsCategory);

            foreach (string cat in CardManager.categories)
                AddCategoryButton(cat);

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_categoryButtonParent as RectTransform);

            RTGLog.Line($"Built {_categoryButtons.Count} category button(s).");
        }

        private void AddCategoryButton(string cat)
        {
            RTGLog.Line($"  Adding category button: '{cat}'");
            string captured = cat;
            var btn = CreateCategoryButton(_categoryButtonParent, cat, () => ShowCategory(captured));
            _categoryButtons.Add(btn);
            _categoryButtonNames.Add(cat);
        }

        // ── Card grid ─────────────────────────────────────────────────────────────

        private void ShowCategory(string category)
        {
            RTGLog.Section($"DeckEditorScreen — ShowCategory '{category}'");

            if (_isSearchActive && category != SearchResultsCategory)
                ClearSearch();

            _isSearchActive = false;
            _currentCategory = category;
            _currentPage = 0;
            _cachedCategoryCards = GetCardsForCategory(category);
            RTGLog.Line($"Resolved {_cachedCategoryCards.Count} card(s) for category '{category}'.");

            UpdateCategoryButtonStyles();
            RebuildPageButtons();
            ShowPage(0);
        }

        private void ShowPage(int page)
        {
            int pageCount = GetPageCount();
            _currentPage = Mathf.Clamp(page, 0, Mathf.Max(0, pageCount - 1));

            ClearCardRows();

            if (_cachedCategoryCards.Count == 0)
            {
                RTGLog.Warn($"No cards found for category '{_currentCategory}'.");
                UpdatePageButtonStyles();
                UpdateDeckCountDisplay();
                return;
            }

            int start = _currentPage * CardsPerPage;
            int end = Mathf.Min(start + CardsPerPage, _cachedCategoryCards.Count);
            int built = 0;

            for (int i = start; i < end; i++)
            {
                CardInfo ci = _cachedCategoryCards[i];
                if (ci == null)
                    continue;
                BuildCardRow(ci);
                built++;
            }

            RTGLog.Line(
                $"Built {built} card row(s) for '{_currentCategory}' page {_currentPage + 1}/{pageCount}.");

            if (_cardScrollRect != null)
                _cardScrollRect.verticalNormalizedPosition = 1f;

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_cardGridContent as RectTransform);

            UpdatePageButtonStyles();
            UpdateDeckCountDisplay();
        }

        private void ClearCardRows()
        {
            foreach (var row in _cardRows)
                if (row != null)
                    DestroyImmediate(row);
            _cardRows.Clear();
        }

        private int GetPageCount() =>
            _cachedCategoryCards.Count == 0
                ? 1
                : Mathf.CeilToInt(_cachedCategoryCards.Count / (float)CardsPerPage);

        private void RebuildPageButtons()
        {
            foreach (Transform child in _pageButtonParent)
                Destroy(child.gameObject);
            _pageButtons.Clear();

            int pageCount = GetPageCount();
            for (int page = 0; page < pageCount; page++)
            {
                int capturedPage = page;
                string label = (page + 1).ToString();
                var btn = CreatePageButton(_pageButtonParent, label, capturedPage);
                _pageButtons.Add(btn);
            }

            // Force layout rebuild so raycast hit areas match visual positions
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_pageButtonParent as RectTransform);
        }

        private Button CreateCategoryButton(Transform parent, string label, Action onClick)
        {
            var go = new GameObject($"Cat_{label}");
            go.transform.SetParent(parent, false);

            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.sizeDelta = new Vector2(0f, 44f);

            var img = go.AddComponent<Image>();
            img.color = PageIdleColor;
            img.raycastTarget = true;

            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            btn.onClick.AddListener(() => onClick());

            var le = go.AddComponent<LayoutElement>();
            le.preferredHeight = 44f;
            le.minHeight = 44f;
            le.flexibleHeight = 0f;

            AddNavButtonLabel(go.transform, label, 18, TextAlignmentOptions.Center);
            return btn;
        }

        private Button CreatePageButton(Transform parent, string label, int pageIndex)
        {
            var go = new GameObject($"Page_{label}");
            go.transform.SetParent(parent, false);

            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 0f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 0.5f);
            rt.sizeDelta = new Vector2(40f, 0f);

            var img = go.AddComponent<Image>();
            img.color = PageIdleColor;
            img.raycastTarget = true;

            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            btn.onClick.AddListener(() => ShowPage(pageIndex));

            var le = go.AddComponent<LayoutElement>();
            le.preferredWidth = 40f;
            le.minWidth = 40f;
            le.flexibleWidth = 0f;

            AddNavButtonLabel(go.transform, label, 18, TextAlignmentOptions.Center);
            return btn;
        }

        private static void AddNavButtonLabel(Transform parent, string text, int fontSize,
            TextAlignmentOptions alignment)
        {
            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(parent, false);
            var labelRt = labelGo.AddComponent<RectTransform>();
            labelRt.anchorMin = Vector2.zero;
            labelRt.anchorMax = Vector2.one;
            labelRt.offsetMin = Vector2.zero;
            labelRt.offsetMax = Vector2.zero;

            var tmp = labelGo.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.alignment = alignment;
            tmp.color = PageIdleTextColor;
            tmp.raycastTarget = false;
        }

        private void UpdateCategoryButtonStyles()
        {
            for (int i = 0; i < _categoryButtons.Count; i++)
            {
                Button btn = _categoryButtons[i];
                if (btn == null)
                    continue;

                bool selected = !_isSearchActive
                    && i < _categoryButtonNames.Count
                    && _categoryButtonNames[i] == _currentCategory;
                ApplyNavButtonStyle(btn, selected);
            }
        }

        private void UpdatePageButtonStyles()
        {
            for (int i = 0; i < _pageButtons.Count; i++)
            {
                Button btn = _pageButtons[i];
                if (btn == null)
                    continue;

                bool selected = i == _currentPage;
                ApplyNavButtonStyle(btn, selected);
            }
        }

        private static void ApplyNavButtonStyle(Button btn, bool selected)
        {
            var img = btn.GetComponent<Image>();
            if (img == null)
                return;

            Color normal = selected ? PageSelectedColor : PageIdleColor;
            Color highlighted = selected ? PageSelectedColor : PageHoverColor;
            Color pressed = selected ? new Color(0.9f, 0.9f, 0.9f) : PagePressedColor;

            img.color = normal;
            btn.colors = new ColorBlock
            {
                normalColor = normal,
                highlightedColor = highlighted,
                pressedColor = pressed,
                disabledColor = new Color(0.3f, 0.3f, 0.3f, 0.5f),
                colorMultiplier = 1f,
                fadeDuration = 0.08f
            };

            var label = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
                label.color = selected ? PageSelectedTextColor : PageIdleTextColor;
        }

        private List<CardInfo> GetCardsForCategory(string category)
        {
            if (category == MyDeckCategory)
                return GetMyDeckCards();

            if (category == AllCardsCategory)
                return GetAllCardsAlphabetical();

            string[] cardNames = CardManager.GetCardsInCategory(category);
            if (cardNames == null || cardNames.Length == 0)
                return new List<CardInfo>();

            var cardsWithInfo = new List<CardInfo>(cardNames.Length);
            foreach (string cardName in cardNames)
            {
                CardInfo ci = CardManager.GetCardInfoWithName(cardName);
                if (ci != null && !ModCardVisibilityBridge.IsHidden(ci))
                    cardsWithInfo.Add(ci);
            }

            SortCardsAlphabeticalThenRarity(cardsWithInfo);
            return cardsWithInfo;
        }

        private List<CardInfo> GetAllCardsAlphabetical()
        {
            var cardsWithInfo = new List<CardInfo>(CardManager.cards.Count);
            int hiddenCount = 0;
            foreach (string cardName in CardManager.cards.Keys)
            {
                CardInfo ci = CardManager.GetCardInfoWithName(cardName);
                if (ci == null)
                    continue;

                if (ModCardVisibilityBridge.IsHidden(ci))
                {
                    hiddenCount++;
                    RTGLog.Line($"GetAllCardsAlphabetical: hiding '{ci.cardName}'");
                }
                else
                {
                    cardsWithInfo.Add(ci);
                }
            }

            RTGLog.Line($"GetAllCardsAlphabetical: {cardsWithInfo.Count} visible, {hiddenCount} hidden");
            SortCardsAlphabeticalThenRarity(cardsWithInfo);
            return cardsWithInfo;
        }

        private List<CardInfo> GetMyDeckCards()
        {
            var cardsWithInfo = new List<CardInfo>();
            if (_deck?.cards == null)
                return cardsWithInfo;

            foreach (CardEntry entry in _deck.cards)
            {
                if (entry == null || entry.count <= 0 || string.IsNullOrEmpty(entry.cardObjectName))
                    continue;

                CardInfo ci = CardManager.GetCardInfoWithName(entry.cardObjectName);
                if (ci != null)
                    cardsWithInfo.Add(ci);
            }

            SortCardsAlphabeticalThenRarity(cardsWithInfo);
            return cardsWithInfo;
        }

        private void RefreshMyDeckViewIfActive()
        {
            if (_currentCategory != MyDeckCategory)
                return;

            _cachedCategoryCards = GetMyDeckCards();
            int pageCount = GetPageCount();
            if (_currentPage >= pageCount)
                _currentPage = Mathf.Max(0, pageCount - 1);

            RebuildPageButtons();
            ShowPage(_currentPage);
        }

        // ── Search ────────────────────────────────────────────────────────────────

        private void OnSearchChanged(string query)
        {
            _searchQuery = query?.Trim() ?? "";

            if (_searchQuery.Length >= MinSearchLength)
            {
                _isSearchActive = true;
                _currentCategory = SearchResultsCategory;
                _currentPage = 0;
                _cachedCategoryCards = SearchCards(_searchQuery);

                RTGLog.Line($"Search '{_searchQuery}' found {_cachedCategoryCards.Count} card(s).");

                UpdateCategoryButtonStyles();
                RebuildPageButtons();
                ShowPage(0);
            }
            else if (_isSearchActive)
            {
                _isSearchActive = false;
                ShowCategory(AllCardsCategory);
            }
        }

        private List<CardInfo> SearchCards(string query)
        {
            var results = new List<CardInfo>();
            if (string.IsNullOrEmpty(query))
                return results;

            string lowerQuery = query.ToLowerInvariant();

            foreach (string cardName in CardManager.cards.Keys)
            {
                CardInfo ci = CardManager.GetCardInfoWithName(cardName);
                if (ci == null || ModCardVisibilityBridge.IsHidden(ci))
                    continue;

                bool matchesTitle = !string.IsNullOrEmpty(ci.cardName)
                    && ci.cardName.ToLowerInvariant().Contains(lowerQuery);

                bool matchesDescription = !string.IsNullOrEmpty(ci.cardDestription)
                    && ci.cardDestription.ToLowerInvariant().Contains(lowerQuery);

                if (matchesTitle || matchesDescription)
                    results.Add(ci);
            }

            SortCardsAlphabeticalThenRarity(results);
            return results;
        }

        private void ClearSearch()
        {
            if (_searchField != null)
                _searchField.text = "";

            _searchQuery = "";
            _isSearchActive = false;
        }

        /// <summary>Letter groups A→Z; within each letter, least rare first; ties broken by full name.</summary>
        private static void SortCardsAlphabeticalThenRarity(List<CardInfo> cards)
        {
            cards.Sort((a, b) =>
            {
                int letterCmp = CompareFirstLetter(a?.cardName, b?.cardName);
                if (letterCmp != 0)
                    return letterCmp;

                int rarityCmp = a.rarity.CompareTo(b.rarity);
                if (rarityCmp != 0)
                    return rarityCmp;

                return string.Compare(a.cardName, b.cardName, StringComparison.OrdinalIgnoreCase);
            });
        }

        private static int CompareFirstLetter(string a, string b)
        {
            char letterA = GetSortLetter(a);
            char letterB = GetSortLetter(b);
            return letterA.CompareTo(letterB);
        }

        private static char GetSortLetter(string name)
        {
            if (string.IsNullOrEmpty(name))
                return '\0';
            return char.ToUpperInvariant(name[0]);
        }

        private void BuildCardRow(CardInfo cardInfo)
        {
            string cardName = cardInfo.gameObject.name;
            CardEntry entry = GetOrCreateEntry(cardName);
            int maxCount = DeckManager.MaxCountForCard(cardInfo);

            // Container
            var rowGo = new GameObject($"Card_{cardName}");
            rowGo.transform.SetParent(_cardGridContent, false);
            var rowRt = rowGo.AddComponent<RectTransform>();
            rowRt.sizeDelta = new Vector2(220, 300);

            var le = rowGo.AddComponent<LayoutElement>();
            le.preferredWidth = 220;
            le.preferredHeight = 300;

            _cardRows.Add(rowGo);

            // Background for click detection
            var bgImg = rowGo.AddComponent<Image>();
            bgImg.color = new Color(0.1f, 0.1f, 0.15f, 1f);
            bgImg.raycastTarget = true;

            // ── Instantiate the actual card visual ──────────────────────────────
            CardVisualHelper.SetupCardVisual(cardInfo, rowGo);

            // ── Rarity label overlay at bottom (Image + TMP must be on separate objects) ──
            var rarityLabelGo = new GameObject("RarityLabel");
            rarityLabelGo.transform.SetParent(rowGo.transform, false);
            var rarityLabelRt = rarityLabelGo.AddComponent<RectTransform>();
            rarityLabelRt.anchorMin = new Vector2(0f, 0f);
            rarityLabelRt.anchorMax = new Vector2(1f, 0.12f);
            rarityLabelRt.offsetMin = rarityLabelRt.offsetMax = Vector2.zero;
            var rarityBg = rarityLabelGo.AddComponent<Image>();
            rarityBg.color = new Color(0, 0, 0, 0.7f);
            rarityBg.raycastTarget = false;

            var rarityTextGo = new GameObject("RarityText");
            rarityTextGo.transform.SetParent(rarityLabelGo.transform, false);
            var rarityTextRt = rarityTextGo.AddComponent<RectTransform>();
            rarityTextRt.anchorMin = Vector2.zero;
            rarityTextRt.anchorMax = Vector2.one;
            rarityTextRt.offsetMin = rarityTextRt.offsetMax = Vector2.zero;
            var rarityTmp = rarityTextGo.AddComponent<TextMeshProUGUI>();
            rarityTmp.text = $"{cardInfo.cardName} ({cardInfo.rarity})";
            rarityTmp.fontSize = 11;
            rarityTmp.alignment = TextAlignmentOptions.Center;
            rarityTmp.color = RarityColor(cardInfo.rarity);
            rarityTmp.enableWordWrapping = false;
            rarityTmp.overflowMode = TextOverflowModes.Ellipsis;
            rarityTmp.raycastTarget = false;

            // ── Controls container (visible when count > 0) ─────────────────────────
            var controlsGo = new GameObject("Controls");
            controlsGo.transform.SetParent(rowGo.transform, false);
            var controlsRt = controlsGo.AddComponent<RectTransform>();
            controlsRt.anchorMin = Vector2.zero;
            controlsRt.anchorMax = Vector2.one;
            controlsRt.offsetMin = controlsRt.offsetMax = Vector2.zero;
            controlsGo.SetActive(entry.count > 0);

            // Minus button - bottom LEFT corner, small square
            var minusBtnGo = new GameObject("MinusBtn");
            minusBtnGo.transform.SetParent(controlsGo.transform, false);
            var minusRt = minusBtnGo.AddComponent<RectTransform>();
            minusRt.anchorMin = new Vector2(0f, 0f);
            minusRt.anchorMax = new Vector2(0f, 0f);
            minusRt.pivot = new Vector2(0f, 0f);
            minusRt.anchoredPosition = new Vector2(4, 4);
            minusRt.sizeDelta = new Vector2(36, 36);
            var minusImg = minusBtnGo.AddComponent<Image>();
            minusImg.color = new Color(0.7f, 0.15f, 0.15f, 0.95f);
            var minusBtn = minusBtnGo.AddComponent<Button>();
            minusBtn.targetGraphic = minusImg;
            var minusTxt = new GameObject("Text").AddComponent<TextMeshProUGUI>();
            minusTxt.transform.SetParent(minusBtnGo.transform, false);
            var minusTxtRt = minusTxt.GetComponent<RectTransform>();
            minusTxtRt.anchorMin = Vector2.zero;
            minusTxtRt.anchorMax = Vector2.one;
            minusTxtRt.offsetMin = minusTxtRt.offsetMax = Vector2.zero;
            minusTxt.text = "-";
            minusTxt.raycastTarget = false;
            minusTxt.fontSize = 24;
            minusTxt.alignment = TextAlignmentOptions.Center;
            minusTxt.color = Color.white;

            // Count label - bottom CENTER
            var countLabelGo = new GameObject("CountLabel");
            countLabelGo.transform.SetParent(controlsGo.transform, false);
            var countRt = countLabelGo.AddComponent<RectTransform>();
            countRt.anchorMin = new Vector2(0.5f, 0f);
            countRt.anchorMax = new Vector2(0.5f, 0f);
            countRt.pivot = new Vector2(0.5f, 0f);
            countRt.anchoredPosition = new Vector2(0, 4);
            countRt.sizeDelta = new Vector2(40, 36);
            var countBg = countLabelGo.AddComponent<Image>();
            countBg.color = new Color(0, 0, 0, 0.8f);
            countBg.raycastTarget = false;

            var countTextGo = new GameObject("CountText");
            countTextGo.transform.SetParent(countLabelGo.transform, false);
            var countTextRt = countTextGo.AddComponent<RectTransform>();
            countTextRt.anchorMin = Vector2.zero;
            countTextRt.anchorMax = Vector2.one;
            countTextRt.offsetMin = countTextRt.offsetMax = Vector2.zero;
            var countTmp = countTextGo.AddComponent<TextMeshProUGUI>();
            countTmp.text = entry.count.ToString();
            countTmp.fontSize = 22;
            countTmp.alignment = TextAlignmentOptions.Center;
            countTmp.color = Color.white;
            countTmp.raycastTarget = false;

            // Plus button - bottom RIGHT corner, small square
            var plusBtnGo = new GameObject("PlusBtn");
            plusBtnGo.transform.SetParent(controlsGo.transform, false);
            var plusRt = plusBtnGo.AddComponent<RectTransform>();
            plusRt.anchorMin = new Vector2(1f, 0f);
            plusRt.anchorMax = new Vector2(1f, 0f);
            plusRt.pivot = new Vector2(1f, 0f);
            plusRt.anchoredPosition = new Vector2(-4, 4);
            plusRt.sizeDelta = new Vector2(36, 36);
            var plusImg = plusBtnGo.AddComponent<Image>();
            plusImg.color = new Color(0.15f, 0.6f, 0.15f, 0.95f);
            var plusBtn = plusBtnGo.AddComponent<Button>();
            plusBtn.targetGraphic = plusImg;
            var plusTxt = new GameObject("Text").AddComponent<TextMeshProUGUI>();
            plusTxt.transform.SetParent(plusBtnGo.transform, false);
            var plusTxtRt = plusTxt.GetComponent<RectTransform>();
            plusTxtRt.anchorMin = Vector2.zero;
            plusTxtRt.anchorMax = Vector2.one;
            plusTxtRt.offsetMin = plusTxtRt.offsetMax = Vector2.zero;
            plusTxt.text = "+";
            plusTxt.fontSize = 24;
            plusTxt.alignment = TextAlignmentOptions.Center;
            plusTxt.color = Color.white;
            plusTxt.raycastTarget = false;

            // Click on card itself toggles (adds 1 if count==0)
            var clickCatcher = rowGo.AddComponent<Button>();
            clickCatcher.targetGraphic = bgImg;
            clickCatcher.onClick.AddListener(() =>
            {
                if (entry.count == 0)
                {
                    entry.count = 1;
                    controlsGo.SetActive(true);
                    countTmp.text = "1";
                    bgImg.color = new Color(0.15f, 0.25f, 0.15f, 0.9f);
                    UpdateDeckCountDisplay();
                }
            });

            minusBtn.onClick.AddListener(() =>
            {
                if (entry.count <= 0) return;
                entry.count--;
                countTmp.text = entry.count.ToString();
                if (entry.count == 0)
                {
                    controlsGo.SetActive(false);
                    bgImg.color = new Color(0.1f, 0.1f, 0.14f, 0.85f);
                    UpdateDeckCountDisplay();
                    RefreshMyDeckViewIfActive();
                    return;
                }

                UpdateDeckCountDisplay();
            });

            plusBtn.onClick.AddListener(() =>
            {
                if (entry.count >= maxCount) return;
                entry.count++;
                countTmp.text = entry.count.ToString();
                UpdateDeckCountDisplay();
            });
        }

        private CardEntry GetOrCreateEntry(string cardObjectName)
        {
            var entry = _deck.cards.Find(e => e.cardObjectName == cardObjectName);
            if (entry == null)
            {
                entry = new CardEntry { cardObjectName = cardObjectName, count = 0 };
                _deck.cards.Add(entry);
            }
            return entry;
        }

        private void UpdateDeckCountDisplay()
        {
            if (_deck == null) return;
            int total = _deck.TotalCount;

            if (_isSearchActive)
            {
                _deckCountText.text = $"Found {_cachedCategoryCards.Count} cards — Deck: {total} / {_deck.maxSize}";
            }
            else
            {
                _deckCountText.text = $"Cards in Deck: {total} / {_deck.maxSize}";
            }

            _deckCountText.color = total > _deck.maxSize
                ? new Color(1f, 0.3f, 0.3f)
                : Color.white;
        }

        // ── Max Size Modal ────────────────────────────────────────────────────────

        private void BuildMaxSizeModal(Transform parent)
        {
            _maxSizeModal = new GameObject("MaxSizeModal");
            _maxSizeModal.transform.SetParent(parent, false);
            _maxSizeModal.SetActive(false);

            var rt = _maxSizeModal.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.3f, 0.35f);
            rt.anchorMax = new Vector2(0.7f, 0.65f);
            rt.offsetMin = rt.offsetMax = Vector2.zero;

            var bg = _maxSizeModal.AddComponent<Image>();
            bg.color = new Color(0.08f, 0.08f, 0.14f, 0.98f);

            UIHelper.CreateText(rt, "Title", "Change Max Deck Size", fontSize: 26);

            _maxSizeModalField = UIHelper.CreateInputField(rt, "SizeField",
                _deck?.maxSize.ToString() ?? "50",
                Vector2.zero, Vector2.zero,
                contentType: TMP_InputField.ContentType.IntegerNumber);
            var fRt = _maxSizeModalField.GetComponent<RectTransform>();
            fRt.anchorMin = new Vector2(0.1f, 0.38f);
            fRt.anchorMax = new Vector2(0.9f, 0.55f);
            fRt.offsetMin = fRt.offsetMax = Vector2.zero;
            _maxSizeModalField.onValueChanged.AddListener(_ => ValidateModal());

            _maxSizeModalError = UIHelper.CreateText(rt, "ModalError", "",
                fontSize: 17, color: new Color(1f, 0.35f, 0.35f));
            var eRt = _maxSizeModalError.GetComponent<RectTransform>();
            eRt.anchorMin = new Vector2(0.05f, 0.25f);
            eRt.anchorMax = new Vector2(0.95f, 0.37f);
            eRt.offsetMin = eRt.offsetMax = Vector2.zero;

            var confirmBtn = UIHelper.CreateButton(rt, "ConfirmBtn", "Confirm",
                Vector2.zero, Vector2.zero, fontSize: 22,
                bgColor: new Color(0.15f, 0.5f, 0.15f));
            var cRt = confirmBtn.GetComponent<RectTransform>();
            cRt.anchorMin = new Vector2(0.1f, 0.06f);
            cRt.anchorMax = new Vector2(0.55f, 0.23f);
            cRt.offsetMin = cRt.offsetMax = Vector2.zero;
            confirmBtn.onClick.AddListener(ConfirmMaxSize);

            var cancelBtn = UIHelper.CreateButton(rt, "CancelBtn", "Cancel",
                Vector2.zero, Vector2.zero, fontSize: 22,
                bgColor: new Color(0.45f, 0.1f, 0.1f));
            var canRt = cancelBtn.GetComponent<RectTransform>();
            canRt.anchorMin = new Vector2(0.57f, 0.06f);
            canRt.anchorMax = new Vector2(0.9f, 0.23f);
            canRt.offsetMin = canRt.offsetMax = Vector2.zero;
            cancelBtn.onClick.AddListener(() => _maxSizeModal.SetActive(false));
        }

        private void OpenMaxSizeModal()
        {
            if (_deck == null) return;
            RTGLog.Section($"DeckEditorScreen — Change Max Size (current={_deck.maxSize})");
            _maxSizeModalField.text = _deck.maxSize.ToString();
            _maxSizeModalError.text = "";
            _maxSizeModal.SetActive(true);
        }

        private void ValidateModal()
        {
            bool ok = int.TryParse(_maxSizeModalField.text, out int v) && v >= 1;
            _maxSizeModalError.text = ok ? "" : "Must be a number ≥ 1.";
        }

        private void ConfirmMaxSize()
        {
            if (!int.TryParse(_maxSizeModalField.text, out int v) || v < 1) return;
            RTGLog.Section($"DeckEditorScreen — Confirm Max Size {v}");
            _deck.maxSize = v;
            UpdateDeckCountDisplay();
            _maxSizeModal.SetActive(false);
            DeckManager.instance?.Save();
        }

        // ── Close ─────────────────────────────────────────────────────────────────

        private void CloseScreen()
        {
            RTGLog.Section("DeckEditorScreen — Close");
            _canvas.gameObject.SetActive(false); // OnDisable auto-saves
            DeckSelectorScreen.instance?.Show();
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private static Color RarityColor(CardInfo.Rarity rarity)
        {
            switch (rarity)
            {
                case CardInfo.Rarity.Common: return new Color(0.6f, 0.6f, 0.6f);
                case CardInfo.Rarity.Uncommon: return new Color(0.2f, 0.8f, 0.2f);
                case CardInfo.Rarity.Rare: return new Color(0.2f, 0.4f, 1f);
                default: return new Color(1f, 0.65f, 0f);
            }
        }
    }
}
