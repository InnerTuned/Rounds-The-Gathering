using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnboundLib;
using UnboundLib.Utils;
using DeckBuilder.Data;

namespace DeckBuilder.UI
{
    /// <summary>
    /// Screen 3 — full card editor.
    /// Mirrors the visual layout of ToggleCardsMenuHandler with per-card [-] # [+] controls.
    /// </summary>
    public class DeckEditorScreen : MonoBehaviour
    {
        public static DeckEditorScreen instance;

        // ── State ─────────────────────────────────────────────────────────────────

        private DeckData _deck;
        private string _currentCategory;

        // ── UI roots ──────────────────────────────────────────────────────────────

        private Canvas _canvas;
        private TextMeshProUGUI _viewingText;
        private TextMeshProUGUI _deckCountText;
        private Transform _cardGridContent;
        private Transform _categoryButtonParent;
        private ScrollRect _cardScrollRect;

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
            _canvas.gameObject.SetActive(true);
            BuildCategoryButtons();
            ShowCategory(CardManager.categories.Count > 0 ? CardManager.categories[0] : "");
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

            _viewingText = UIHelper.CreateText(topBar, "ViewingText", "Viewing: —",
                fontSize: 22, alignment: TextAlignmentOptions.MidlineLeft);
            var vtRt = _viewingText.GetComponent<RectTransform>();
            vtRt.anchorMin = new Vector2(0.01f, 0f);
            vtRt.anchorMax = new Vector2(0.35f, 1f);
            vtRt.offsetMin = vtRt.offsetMax = Vector2.zero;

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

            // ── Right panel: card grid ────────────────────────────────────────────

            var rightPanel = UIHelper.CreatePanel(outer, "RightPanel",
                new Vector2(0.19f, 0f), new Vector2(1f, 0.92f),
                bg: new Color(0.05f, 0.05f, 0.08f, 1f)); // Dark background so we can see where it is

            var cardScrollGo = BuildScrollRect(rightPanel, "CardScroll",
                out _cardGridContent);
            var cardSrRt = cardScrollGo.GetComponent<RectTransform>();
            cardSrRt.anchorMin = Vector2.zero;
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
            out Transform content)
        {
            // Scroll rect container
            var srGo = new GameObject(name);
            srGo.transform.SetParent(parent, false);
            var srRt = srGo.AddComponent<RectTransform>();
            srRt.anchorMin = Vector2.zero;
            srRt.anchorMax = Vector2.one;
            srRt.offsetMin = srRt.offsetMax = Vector2.zero;

            var sr = srGo.AddComponent<ScrollRect>();
            sr.horizontal = false;
            sr.vertical = true;
            sr.movementType = ScrollRect.MovementType.Clamped;
            sr.scrollSensitivity = 80f; // Faster scrolling for better UX

            // Viewport with mask
            var vpGo = new GameObject("Viewport");
            vpGo.transform.SetParent(srGo.transform, false);
            var vpRt = vpGo.AddComponent<RectTransform>();
            vpRt.anchorMin = Vector2.zero;
            vpRt.anchorMax = Vector2.one;
            vpRt.offsetMin = vpRt.offsetMax = Vector2.zero;
            vpRt.pivot = new Vector2(0, 1);
            // Use Image + Mask instead of RectMask2D for better compatibility
            var vpImg = vpGo.AddComponent<Image>();
            vpImg.color = new Color(1, 1, 1, 0.01f); // Nearly invisible but needed for Mask
            var vpMask = vpGo.AddComponent<Mask>();
            vpMask.showMaskGraphic = false;
            sr.viewport = vpRt;

            // Content that grows vertically
            var contentGo = new GameObject("Content");
            contentGo.transform.SetParent(vpGo.transform, false);
            var contentRt = contentGo.AddComponent<RectTransform>();
            contentRt.anchorMin = new Vector2(0, 1);
            contentRt.anchorMax = new Vector2(1, 1);
            contentRt.pivot = new Vector2(0, 1);
            contentRt.sizeDelta = new Vector2(0, 0);
            contentRt.anchoredPosition = Vector2.zero;

            var csf = contentGo.AddComponent<ContentSizeFitter>();
            csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

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

            RTGLog.Line($"CardManager.categories.Count = {CardManager.categories.Count}");
            if (CardManager.categories.Count == 0)
            {
                RTGLog.Warn("No categories found in CardManager!");
            }

            foreach (string cat in CardManager.categories)
            {
                RTGLog.Line($"  Adding category button: '{cat}'");
                string captured = cat;
                var btn = UIHelper.CreateButton(_categoryButtonParent, $"Cat_{cat}", cat,
                    Vector2.zero, new Vector2(0, 44), fontSize: 18,
                    bgColor: new Color(0.15f, 0.15f, 0.22f));
                var rt = btn.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0f, 0f);
                rt.anchorMax = new Vector2(1f, 0f);
                rt.sizeDelta = new Vector2(0, 44);
                btn.onClick.AddListener(() => ShowCategory(captured));
            }
            RTGLog.Line($"Built {CardManager.categories.Count} category button(s).");
        }

        // ── Card grid ─────────────────────────────────────────────────────────────

        private void ShowCategory(string category)
        {
            RTGLog.Section($"DeckEditorScreen — ShowCategory '{category}'");
            _currentCategory = category;
            _viewingText.text = $"Viewing: {category}";

            // Use DestroyImmediate so the GridLayoutGroup sees an empty parent
            // before new cards are added — Destroy is deferred and causes cards
            // from the previous category to still count toward layout positions.
            foreach (var row in _cardRows)
                if (row != null) DestroyImmediate(row);
            _cardRows.Clear();

            string[] cardNames = CardManager.GetCardsInCategory(category);
            RTGLog.Line($"GetCardsInCategory returned {cardNames?.Length ?? 0} card name(s).");

            if (cardNames == null || cardNames.Length == 0)
            {
                RTGLog.Warn($"No cards found for category '{category}'.");
                UpdateDeckCountDisplay();
                return;
            }

            // Get CardInfo for all cards and sort by rarity (Common → Uncommon → Rare → ...)
            var cardsWithInfo = new List<CardInfo>();
            foreach (string cardName in cardNames)
            {
                CardInfo ci = CardManager.GetCardInfoWithName(cardName);
                if (ci != null)
                    cardsWithInfo.Add(ci);
            }
            cardsWithInfo.Sort((a, b) => a.rarity.CompareTo(b.rarity));
            RTGLog.Line($"Sorted {cardsWithInfo.Count} cards by rarity.");

            int built = 0;
            int skipped = 0;
            foreach (CardInfo ci in cardsWithInfo)
            {
                if (ci == null)
                {
                    skipped++;
                    continue;
                }
                BuildCardRow(ci);
                built++;
            }

            RTGLog.Line($"Built {built} card row(s), skipped {skipped}. Deck total={_deck?.TotalCount ?? 0}.");

            // Force layout rebuild
            Canvas.ForceUpdateCanvases();
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(_cardGridContent as RectTransform);

            // Log diagnostic info about scroll rect and cards
            RTGLog.Line($"=== Card Layout Diagnostics ===");
            RTGLog.Line($"CardGridContent: childCount={_cardGridContent.childCount}");
            
            if (_cardScrollRect != null)
            {
                var srRt = _cardScrollRect.GetComponent<RectTransform>();
                RTGLog.Line($"ScrollRect: rect={srRt.rect}, active={_cardScrollRect.gameObject.activeInHierarchy}");
                if (_cardScrollRect.viewport != null)
                {
                    RTGLog.Line($"Viewport: rect={_cardScrollRect.viewport.rect}");
                }
                if (_cardScrollRect.content != null)
                {
                    RTGLog.Line($"Content: rect={_cardScrollRect.content.rect}, sizeDelta={_cardScrollRect.content.sizeDelta}");
                }
            }
            
            if (_cardRows.Count > 0)
            {
                for (int i = 0; i < Mathf.Min(3, _cardRows.Count); i++)
                {
                    var cardRt = _cardRows[i].GetComponent<RectTransform>();
                    var cardImg = _cardRows[i].GetComponent<Image>();
                    RTGLog.Line($"Card[{i}] '{_cardRows[i].name}': localPos={cardRt.localPosition}, anchoredPos={cardRt.anchoredPosition}, sizeDelta={cardRt.sizeDelta}, rect={cardRt.rect}, imgColor={cardImg?.color}, active={_cardRows[i].activeInHierarchy}");
                }
            }
            else
            {
                RTGLog.Warn("No card rows created!");
            }

            UpdateDeckCountDisplay();
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
            SetupCardVisual(cardInfo, rowGo);

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

        /// <summary>
        /// Instantiates the actual card prefab as a visual, similar to ToggleCardsMenuHandler.
        /// </summary>
        private void SetupCardVisual(CardInfo cardInfo, GameObject parent)
        {
            RTGLog.Line($"SetupCardVisual '{cardInfo.cardName}': cardArt={(cardInfo.cardArt != null ? cardInfo.cardArt.name : "NULL")}");

            // Instantiate the card prefab
            GameObject cardObject = Instantiate(cardInfo.gameObject, parent.transform);
            cardObject.name = "CardVisual";
            cardObject.SetActive(true);

            // Remove unnecessary parts
            var back = FindChildByName(cardObject, "Back");
            if (back != null) Destroy(back);

            var damagable = FindChildByName(cardObject, "Damagable");
            if (damagable != null) Destroy(damagable);

            // Remove particle systems (cause lag in menus)
            var particles = FindChildByName(cardObject, "UI_ParticleSystem");
            if (particles != null) Destroy(particles);

            // Disable block front
            var blockFront = FindChildByName(cardObject, "BlockFront");
            if (blockFront != null) blockFront.SetActive(false);

            // Make all canvas groups visible
            foreach (var cg in cardObject.GetComponentsInChildren<CanvasGroup>(true))
            {
                cg.alpha = 1;
            }

            // Set up card visuals
            foreach (var cv in cardObject.GetComponentsInChildren<CardVisuals>(true))
            {
                cv.firstValueToSet = true;
            }

            // Disable animations on the card for static menu display
            foreach (var anim in cardObject.GetComponentsInChildren<Animator>(true))
            {
                anim.enabled = false;
            }
            foreach (var curveAnim in cardObject.GetComponentsInChildren<CurveAnimation>(true))
            {
                curveAnim.enabled = false;
            }

            // Scale and position the card to fit in our container
            var cardRt = cardObject.GetOrAddComponent<RectTransform>();
            cardRt.localScale = Vector3.one * 15f; // Larger scale to fill the 220x300 cell
            cardRt.anchorMin = new Vector2(0.5f, 0.5f);
            cardRt.anchorMax = new Vector2(0.5f, 0.5f);
            cardRt.pivot = new Vector2(0.5f, 0.5f);
            cardRt.anchoredPosition = new Vector2(0, 10f); // Slight upward offset for controls

            // Disable raycasting on the card visual so clicks go to the parent
            foreach (var graphic in cardObject.GetComponentsInChildren<Graphic>(true))
            {
                graphic.raycastTarget = false;
            }

            // Manually place card art so we don't depend on CardVisuals.Start()
            // which requires CardChoice.instance (not available in menu scenes).
            if (cardInfo.cardArt != null)
            {
                var artTransform = FindChildByName(cardObject, "Art");
                RTGLog.Line($"SetupCardVisual '{cardInfo.cardName}': Art transform={(artTransform != null ? artTransform.name : "NOT FOUND")}");
                if (artTransform != null)
                {
                    var clone = Instantiate(cardInfo.cardArt, artTransform.transform);
                    clone.transform.SetAsFirstSibling();

                    // Reset RectTransform to fill parent (card art templates may have off-screen positioning)
                    var cloneRt = clone.GetComponent<RectTransform>();
                    if (cloneRt != null)
                    {
                        cloneRt.anchorMin = Vector2.zero;
                        cloneRt.anchorMax = Vector2.one;
                        cloneRt.offsetMin = Vector2.zero;
                        cloneRt.offsetMax = Vector2.zero;
                        cloneRt.localPosition = Vector3.zero;
                        cloneRt.localScale = Vector3.one;
                        RTGLog.Line($"SetupCardVisual '{cardInfo.cardName}': Art RectTransform reset to fill parent.");
                    }
                    else
                    {
                        clone.transform.localPosition = Vector3.zero;
                        clone.transform.localScale = Vector3.one;
                    }
                    RTGLog.Line($"SetupCardVisual '{cardInfo.cardName}': Art placed successfully.");
                }
                else
                {
                    RTGLog.Warn($"SetupCardVisual '{cardInfo.cardName}': 'Art' child not found — cannot place card art.");
                }
            }
            else
            {
                RTGLog.Line($"SetupCardVisual '{cardInfo.cardName}': no cardArt set, skipping art placement.");
            }
        }

        private static GameObject FindChildByName(GameObject parent, string name)
        {
            foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == name) return child.gameObject;
            }
            return null;
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
            _deckCountText.text = $"Cards in Deck: {total} / {_deck.maxSize}";
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
