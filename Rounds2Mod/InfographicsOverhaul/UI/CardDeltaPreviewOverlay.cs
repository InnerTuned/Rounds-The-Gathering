using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;  // StringBuilder used in SetColumns
using HarmonyLib;
using InfoOverhaul.Delta;
using TMPro;
using UnboundLib.GameModes;
using UnityEngine;

namespace InfoOverhaul.UI;

/// <summary>
/// Shows stat deltas for the currently highlighted draft card, above the RTG deck HUD.
/// Up to 3 stats per column; extra stats overflow into additional columns to the right.
/// </summary>
public class CardDeltaPreviewOverlay : MonoBehaviour
{
    public static CardDeltaPreviewOverlay instance;

    private static readonly FieldInfo SpawnedCardsField =
        AccessTools.Field(typeof(CardChoice), "spawnedCards");

    private static readonly FieldInfo SelectedCardField =
        AccessTools.Field(typeof(CardChoice), "currentlySelectedCard");

    private const int RowsPerCol = 3;
    private const int MaxCols = 5; // supports up to 15 stat lines

    private Canvas _canvas;
    private TextMeshProUGUI _headerText;
    private TextMeshProUGUI[] _colTexts;
    private bool _pickSequenceActive;

    private void Awake()
    {
        instance = this;
        BuildUI();
        GameModeManager.AddHook(GameModeHooks.HookPickEnd, OnPickSequenceEnd);
        IOLog.Section("CardDeltaPreviewOverlay initialized");
    }

    private void BuildUI()
    {
        _canvas = UIHelper.CreateFullscreenCanvas("IO_CardDeltaPreview", sortOrder: 55);

        var panel = UIHelper.CreatePanel(_canvas.transform, "DeltaPanel",
            new Vector2(0.18f, 0.085f), new Vector2(0.82f, 0.22f),
            bg: new Color(0.04f, 0.06f, 0.10f, 0.92f));

        // Header — full width, top strip
        var headerRt = UIHelper.CreatePanel(panel, "Header",
            new Vector2(0.01f, 0.72f), new Vector2(0.99f, 0.98f));
        _headerText = UIHelper.CreateText(headerRt, "HeaderText", "Card preview",
            fontSize: 20, alignment: TextAlignmentOptions.MidlineLeft,
            color: new Color(0.75f, 0.85f, 1f));

        // Pre-allocate column text objects; each occupies 1/MaxCols of the body area
        _colTexts = new TextMeshProUGUI[MaxCols];
        float colWidth = 1f / MaxCols;
        for (int c = 0; c < MaxCols; c++)
        {
            float xMin = c * colWidth + 0.01f;
            float xMax = (c + 1) * colWidth - 0.01f;
            var colRt = UIHelper.CreatePanel(panel, $"Col{c}",
                new Vector2(xMin, 0.02f), new Vector2(xMax, 0.70f));
            _colTexts[c] = UIHelper.CreateText(colRt, "Text", string.Empty,
                fontSize: 17, alignment: TextAlignmentOptions.TopLeft,
                color: new Color(0.92f, 0.94f, 1f));
            _colTexts[c].richText = true;
        }

        _canvas.gameObject.SetActive(false);
    }

    public void OnPickSequenceStarted()
    {
        if (GetLocalHumanPlayer() == null)
        {
            SetVisible(false);
            return;
        }

        _pickSequenceActive = true;
        SetVisible(true);
        Refresh();
    }

    public void OnPickSequenceEnded()
    {
        _pickSequenceActive = false;
        SetVisible(false);
    }

    private void Update()
    {
        if (!_pickSequenceActive || _canvas == null || !_canvas.gameObject.activeSelf)
            return;

        Refresh();
    }

    private void Refresh()
    {
        var player = GetLocalHumanPlayer();
        if (player == null)
        {
            SetVisible(false);
            return;
        }

        if (!TryGetHighlightedCard(out CardInfo card))
        {
            _headerText.text = "Card preview";
            SetColumns(null);
            _colTexts[0].text = "—";
            return;
        }

        _headerText.text = $"If selecting {card.cardName}:";

        if (!CardDeltaRegistry.TryGetDeltas(player, card, out IReadOnlyList<StatDeltaLine> deltas))
        {
            SetColumns(null);
            _colTexts[0].text = "N/A";
            return;
        }

        if (deltas.Count == 0)
        {
            SetColumns(null);
            _colTexts[0].text = "No stat changes";
            return;
        }

        SetColumns(deltas);
    }

    /// <summary>
    /// Populates columns with up to <see cref="RowsPerCol"/> lines each.
    /// Passing null clears all columns except col 0 (caller sets its text).
    /// </summary>
    private void SetColumns(IReadOnlyList<StatDeltaLine> deltas)
    {
        if (deltas == null)
        {
            for (int c = 0; c < MaxCols; c++)
            {
                _colTexts[c].text = string.Empty;
                _colTexts[c].gameObject.SetActive(c == 0);
            }
            return;
        }

        int colCount = Mathf.Clamp(Mathf.CeilToInt(deltas.Count / (float)RowsPerCol), 1, MaxCols);

        for (int c = 0; c < MaxCols; c++)
        {
            bool active = c < colCount;
            _colTexts[c].gameObject.SetActive(active);

            if (!active)
                continue;

            var sb = new StringBuilder();
            int start = c * RowsPerCol;
            int end = Mathf.Min(start + RowsPerCol, deltas.Count);
            for (int i = start; i < end; i++)
            {
                if (i > start) sb.AppendLine();
                sb.Append(deltas[i].FormatRichText());
            }
            _colTexts[c].text = sb.ToString();
        }
    }

    private static bool TryGetHighlightedCard(out CardInfo card)
    {
        card = null;
        var cc = CardChoice.instance;
        if (cc == null || !cc.IsPicking || SpawnedCardsField == null || SelectedCardField == null)
            return false;

        var spawned = SpawnedCardsField.GetValue(cc) as List<GameObject>;
        if (spawned == null || spawned.Count == 0)
            return false;

        int index = (int)SelectedCardField.GetValue(cc);
        index = Mathf.Clamp(index, 0, spawned.Count - 1);

        var go = spawned[index];
        if (go == null)
            return false;

        card = go.GetComponent<CardInfo>();
        if (card == null)
            card = go.GetComponentInChildren<CardInfo>();

        return card != null;
    }

    private static Player GetLocalHumanPlayer()
    {
        if (PlayerManager.instance == null)
            return null;

        foreach (var p in PlayerManager.instance.players)
        {
            if (p == null || p.GetComponent<PlayerAPI>()?.enabled == true)
                continue;

            object view = AccessTools.Field(typeof(CharacterData), "view").GetValue(p.data);
            if (view == null)
                return p;

            if ((bool)AccessTools.Property(view.GetType(), "IsMine").GetValue(view, null))
                return p;
        }

        return null;
    }

    private void SetVisible(bool visible)
    {
        if (_canvas != null)
            _canvas.gameObject.SetActive(visible);
    }

    private IEnumerator OnPickSequenceEnd(IGameModeHandler gm)
    {
        OnPickSequenceEnded();
        yield break;
    }
}
