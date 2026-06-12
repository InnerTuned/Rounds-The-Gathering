using System;
using System.Collections.Generic;
using System.Text;
using InfoOverhaul.Delta;
using ShieldsMod.Cards;
using ShieldsMod.Resistance;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ShieldsMod.UI;

/// <summary>
/// Modal for choosing which resistance type to upgrade after picking Upgrade Resistance.
/// </summary>
public class ResistanceSelectModalUI : MonoBehaviour
{
    public static ResistanceSelectModalUI instance;

    private static readonly Color PanelBg = new Color(0.04f, 0.06f, 0.12f, 1f);
    private static readonly Color CancelBg = new Color(0.40f, 0.10f, 0.10f, 1f);
    private static readonly Color SelectReadyBg = new Color(0.10f, 0.45f, 0.12f, 1f);
    private static readonly Color SelectGrayBg = new Color(0.18f, 0.18f, 0.18f, 1f);
    private static readonly Color RowIdleBg = new Color(0.10f, 0.10f, 0.15f, 1f);
    private static readonly Color RowHoverBg = new Color(0.25f, 0.55f, 1.00f, 1f);
    private static readonly Color RowSelectedBg = new Color(0.15f, 0.75f, 0.25f, 1f);

    private Canvas _canvas;
    private TextMeshProUGUI _titleText;
    private TextMeshProUGUI _deltaPreviewText;
    private Transform _rowParent;
    private Button _selectButton;
    private Image _selectImage;
    private TextMeshProUGUI _selectLabel;

    private int _pickerID;
    private Player _picker;
    private ResistanceType? _selectedType;
    private readonly List<ResistanceOptionRow> _rows = new();
    private Action<ResistanceType> _onConfirm;
    private Action _onCancel;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        BuildUI();
        _canvas.gameObject.SetActive(false);
    }

    private void BuildUI()
    {
        _canvas = UIHelper.CreateFullscreenCanvas("SM_ResistanceSelectModal", sortOrder: 186);

        UIHelper.CreatePanel(_canvas.transform, "Dim",
            Vector2.zero, Vector2.one, bg: new Color(0f, 0f, 0f, 0.65f));

        var panel = UIHelper.CreatePanel(_canvas.transform, "Panel",
            new Vector2(0.12f, 0.22f), new Vector2(0.88f, 0.78f), bg: PanelBg);

        _titleText = UIHelper.CreateText(panel, "Title", "Upgrade Resistance",
            fontSize: 28, alignment: TextAlignmentOptions.Top,
            color: new Color(0.9f, 0.93f, 1f));

        var deltaPanel = UIHelper.CreatePanel(panel, "DeltaPanel",
            new Vector2(0.03f, 0.82f), new Vector2(0.97f, 0.95f),
            bg: new Color(0.03f, 0.04f, 0.08f, 1f));

        _deltaPreviewText = UIHelper.CreateText(deltaPanel, "DeltaText", "",
            fontSize: 18, alignment: TextAlignmentOptions.MidlineLeft,
            color: new Color(0.88f, 0.92f, 1f));
        _deltaPreviewText.richText = true;

        var scrollGo = new GameObject("Scroll");
        scrollGo.transform.SetParent(panel, false);
        var scrollRt = scrollGo.AddComponent<RectTransform>();
        scrollRt.anchorMin = new Vector2(0.03f, 0.18f);
        scrollRt.anchorMax = new Vector2(0.97f, 0.80f);
        scrollRt.offsetMin = scrollRt.offsetMax = Vector2.zero;

        var scrollImg = scrollGo.AddComponent<Image>();
        scrollImg.color = new Color(0.06f, 0.07f, 0.11f, 1f);

        var scroll = scrollGo.AddComponent<ScrollRect>();
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Clamped;

        var viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollGo.transform, false);
        var viewportRt = viewport.AddComponent<RectTransform>();
        viewportRt.anchorMin = Vector2.zero;
        viewportRt.anchorMax = Vector2.one;
        viewportRt.offsetMin = viewportRt.offsetMax = Vector2.zero;
        viewport.AddComponent<RectMask2D>();
        viewport.AddComponent<Image>().color = Color.clear;
        scroll.viewport = viewportRt;

        var content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        var contentRt = content.AddComponent<RectTransform>();
        contentRt.anchorMin = new Vector2(0f, 1f);
        contentRt.anchorMax = new Vector2(1f, 1f);
        contentRt.pivot = new Vector2(0.5f, 1f);
        contentRt.offsetMin = contentRt.offsetMax = Vector2.zero;

        var layout = content.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.spacing = 8f;
        layout.padding = new RectOffset(8, 8, 8, 8);
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        var fitter = content.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scroll.content = contentRt;
        _rowParent = content.transform;

        var cancelBtn = UIHelper.CreateButton(panel, "CancelBtn", "[Cancel]",
            Vector2.zero, Vector2.zero, fontSize: 20, bgColor: CancelBg);
        var cancelRt = cancelBtn.GetComponent<RectTransform>();
        cancelRt.anchorMin = new Vector2(0.03f, 0.04f);
        cancelRt.anchorMax = new Vector2(0.30f, 0.14f);
        cancelRt.offsetMin = cancelRt.offsetMax = Vector2.zero;
        cancelBtn.onClick.AddListener(OnCancelClicked);

        var selGo = new GameObject("SelectBtn");
        selGo.transform.SetParent(panel, false);
        var selRt = selGo.AddComponent<RectTransform>();
        selRt.anchorMin = new Vector2(0.70f, 0.04f);
        selRt.anchorMax = new Vector2(0.97f, 0.14f);
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

    public void Show(int pickerID, Action<ResistanceType> onConfirm, Action onCancel)
    {
        _pickerID = pickerID;
        _picker = PlayerManager.instance?.players?.Find(p => p.playerID == pickerID);
        _onConfirm = onConfirm;
        _onCancel = onCancel;
        _selectedType = null;
        _titleText.text = "Upgrade Resistance — choose a type";
        _deltaPreviewText.text = "<i>Hover or select a resistance to preview stat changes.</i>";

        RebuildRows();
        RefreshSelectButton();
        _canvas.gameObject.SetActive(true);
        SLog.Section("ResistanceSelectModal — Show");
        SLog.Line($"pickerID={pickerID}, rows={_rows.Count}");
    }

    public void Hide()
    {
        _canvas.gameObject.SetActive(false);
        ClearRows();
        _onConfirm = null;
        _onCancel = null;
        _selectedType = null;
    }

    private void RebuildRows()
    {
        ClearRows();
        if (_picker == null)
            return;

        foreach (ResistanceType type in Enum.GetValues(typeof(ResistanceType)))
            _rows.Add(CreateRow(type));
    }

    private ResistanceOptionRow CreateRow(ResistanceType type)
    {
        var go = new GameObject($"Row_{type}");
        go.transform.SetParent(_rowParent, false);

        var layout = go.AddComponent<LayoutElement>();
        layout.minHeight = 56f;
        layout.preferredHeight = 56f;

        var img = go.AddComponent<Image>();
        img.color = RowIdleBg;

        var btn = go.AddComponent<Button>();
        btn.targetGraphic = img;

        var titleGo = new GameObject("Title");
        titleGo.transform.SetParent(go.transform, false);
        var titleRt = titleGo.AddComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0.02f, 0f);
        titleRt.anchorMax = new Vector2(0.38f, 1f);
        titleRt.offsetMin = titleRt.offsetMax = Vector2.zero;
        var title = titleGo.AddComponent<TextMeshProUGUI>();
        title.fontSize = 20;
        title.alignment = TextAlignmentOptions.MidlineLeft;
        title.color = Color.white;

        var deltaGo = new GameObject("InlineDelta");
        deltaGo.transform.SetParent(go.transform, false);
        var deltaRt = deltaGo.AddComponent<RectTransform>();
        deltaRt.anchorMin = new Vector2(0.40f, 0f);
        deltaRt.anchorMax = new Vector2(0.98f, 1f);
        deltaRt.offsetMin = deltaRt.offsetMax = Vector2.zero;
        var delta = deltaGo.AddComponent<TextMeshProUGUI>();
        delta.fontSize = 17;
        delta.alignment = TextAlignmentOptions.MidlineLeft;
        delta.color = new Color(0.85f, 0.90f, 1f);
        delta.richText = true;

        int level = ResistanceHandLogic.CountUpgrades(_picker.data?.currentCards, type);
        title.text = $"{ResistanceCardCatalog.GetDisplayName(type)} (LV {level})";
        delta.text = FormatInlineDelta(type);

        var row = go.AddComponent<ResistanceOptionRow>();
        row.Init(type, btn, img, title, delta, this);
        btn.onClick.AddListener(() => SelectType(type));
        return row;
    }

    private string FormatInlineDelta(ResistanceType type)
    {
        if (_picker == null)
            return "";

        var tuples = ResistanceDeltaHelper.ComputeTypeUpgradeDelta(_picker, type);
        if (tuples.Count == 0)
            return "No change";

        var sb = new StringBuilder();
        for (int i = 0; i < tuples.Count; i++)
        {
            if (i > 0) sb.Append("   ");
            sb.Append(ToRichText(tuples[i]));
        }

        return sb.ToString();
    }

    private void SelectType(ResistanceType type)
    {
        _selectedType = type;
        foreach (var row in _rows)
            row.SetSelected(row.Type == type);
        UpdateDeltaPreview(type);
        RefreshSelectButton();
        SLog.Line($"Resistance type selected: {type}");
    }

    internal void PreviewType(ResistanceType type)
    {
        if (_selectedType.HasValue)
            return;
        UpdateDeltaPreview(type);
    }

    internal void ClearPreview()
    {
        if (_selectedType.HasValue)
            return;
        _deltaPreviewText.text = "<i>Hover or select a resistance to preview stat changes.</i>";
    }

    private void UpdateDeltaPreview(ResistanceType type)
    {
        if (_picker == null)
        {
            _deltaPreviewText.text = "";
            return;
        }

        var tuples = ResistanceDeltaHelper.ComputeTypeUpgradeDelta(_picker, type);
        if (tuples.Count == 0)
        {
            _deltaPreviewText.text = $"<i>No stat changes from upgrading {ResistanceCardCatalog.GetDisplayName(type)}.</i>";
            return;
        }

        var sb = new StringBuilder();
        sb.Append($"<b>If selecting {ResistanceCardCatalog.GetDisplayName(type)}:</b>  ");
        for (int i = 0; i < tuples.Count; i++)
        {
            if (i > 0) sb.Append("  |  ");
            sb.Append(ToRichText(tuples[i]));
        }

        _deltaPreviewText.text = sb.ToString();
    }

    private static string ToRichText((string label, string before, string after) tuple)
    {
        var line = new StatDeltaLine(tuple.label, tuple.before, tuple.after);
        return line.FormatRichText();
    }

    private void RefreshSelectButton()
    {
        bool ready = _selectedType.HasValue;
        _selectButton.interactable = ready;
        _selectImage.color = ready ? SelectReadyBg : SelectGrayBg;
        _selectLabel.color = ready ? Color.white : new Color(0.45f, 0.45f, 0.45f);
    }

    private void OnSelectClicked()
    {
        if (!_selectedType.HasValue)
            return;

        var confirm = _onConfirm;
        var type = _selectedType.Value;
        Hide();
        confirm?.Invoke(type);
    }

    private void OnCancelClicked()
    {
        var cancel = _onCancel;
        Hide();
        cancel?.Invoke();
    }

    private void ClearRows()
    {
        foreach (var row in _rows)
        {
            if (row != null)
                Destroy(row.gameObject);
        }
        _rows.Clear();
    }

    private class ResistanceOptionRow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        internal ResistanceType Type;
        private ResistanceSelectModalUI _owner;
        private Image _bg;
        private bool _selected;

        internal void Init(ResistanceType type, Button btn, Image bg,
            TextMeshProUGUI title, TextMeshProUGUI inlineDelta, ResistanceSelectModalUI owner)
        {
            Type = type;
            _owner = owner;
            _bg = bg;
        }

        internal void SetSelected(bool selected)
        {
            _selected = selected;
            _bg.color = selected ? RowSelectedBg : RowIdleBg;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_selected)
                _bg.color = RowHoverBg;
            _owner?.PreviewType(Type);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_selected)
                _bg.color = RowIdleBg;
            _owner?.ClearPreview();
        }
    }
}
