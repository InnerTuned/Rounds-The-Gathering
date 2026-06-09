using System.Collections.Generic;
using InfoOverhaul.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InfoOverhaul.UI;

/// <summary>Fully opaque dual-column stats overlay shown during card picks.</summary>
internal class StatsPopup : MonoBehaviour
{
    private readonly List<TextMeshProUGUI> _leftLines = new List<TextMeshProUGUI>();
    private readonly List<TextMeshProUGUI> _rightLines = new List<TextMeshProUGUI>();
    private RectTransform _root;
    private Player _player;

    public static StatsPopup Create(Transform parent)
    {
        var go = new GameObject("IO_StatsPopup");
        go.transform.SetParent(parent, false);
        var popup = go.AddComponent<StatsPopup>();
        popup.Build();
        go.SetActive(false);
        return popup;
    }

    private void Build()
    {
        _root = gameObject.AddComponent<RectTransform>();
        _root.anchorMin = Vector2.zero;
        _root.anchorMax = Vector2.one;
        _root.offsetMin = _root.offsetMax = Vector2.zero;

        var backdrop = new GameObject("Backdrop");
        backdrop.transform.SetParent(transform, false);
        var backdropRt = backdrop.AddComponent<RectTransform>();
        backdropRt.anchorMin = Vector2.zero;
        backdropRt.anchorMax = Vector2.one;
        backdropRt.offsetMin = backdropRt.offsetMax = Vector2.zero;
        var backdropImg = backdrop.AddComponent<Image>();
        backdropImg.color = new Color(0.02f, 0.02f, 0.05f, 1f);

        var panel = UIHelper.CreatePanel(transform, "Panel",
            new Vector2(0.12f, 0.08f), new Vector2(0.88f, 0.92f),
            bg: new Color(0.08f, 0.09f, 0.14f, 1f));

        var titleRt = UIHelper.CreatePanel(panel, "Title",
            new Vector2(0f, 0.9f), Vector2.one);
        UIHelper.CreateText(titleRt, "TitleText", "Player Stats", fontSize: 36);

        var closeBtn = UIHelper.AnchorButton(panel, "CloseBtn", "Close",
            new Vector2(0.82f, 0.91f), new Vector2(0.98f, 0.99f),
            new Color(0.45f, 0.12f, 0.12f), 22);
        closeBtn.onClick.AddListener(Hide);

        var columns = UIHelper.CreatePanel(panel, "Columns",
            new Vector2(0.04f, 0.04f), new Vector2(0.96f, 0.88f));

        var leftCol = UIHelper.CreatePanel(columns, "LeftColumn",
            new Vector2(0f, 0f), new Vector2(0.48f, 1f));
        var rightCol = UIHelper.CreatePanel(columns, "RightColumn",
            new Vector2(0.52f, 0f), Vector2.one);

        BuildColumn(leftCol, _leftLines, 12);
        BuildColumn(rightCol, _rightLines, 12);
    }

    private static void BuildColumn(RectTransform parent, List<TextMeshProUGUI> sink, int lineCount)
    {
        float rowHeight = 1f / lineCount;
        for (int i = 0; i < lineCount; i++)
        {
            float top = 1f - i * rowHeight;
            float bottom = top - rowHeight;
            var row = UIHelper.CreatePanel(parent, $"Line{i}",
                new Vector2(0f, bottom), new Vector2(1f, top));
            var text = UIHelper.CreateText(row, "Text", "—", fontSize: 22,
                alignment: TextAlignmentOptions.MidlineLeft,
                color: new Color(0.92f, 0.94f, 1f));
            sink.Add(text);
        }
    }

    public void Show(Player player)
    {
        _player = player;
        gameObject.SetActive(true);
        Refresh();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        _player = null;
    }

    public bool IsVisible => gameObject.activeSelf;

    private void Update()
    {
        if (!IsVisible || _player == null)
            return;

        Refresh();
    }

    private void Refresh()
    {
        var stats = StatsReader.Collect(_player);
        ApplyColumn(_leftLines, stats, 0);
        ApplyColumn(_rightLines, stats, _leftLines.Count);
    }

    private static void ApplyColumn(List<TextMeshProUGUI> labels, List<StatsReader.StatLine> stats, int offset)
    {
        for (int i = 0; i < labels.Count; i++)
        {
            int idx = offset + i;
            labels[i].text = idx < stats.Count ? stats[idx].Text : string.Empty;
        }
    }
}
