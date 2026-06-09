using System.Collections;
using HarmonyLib;
using InfoOverhaul.Patches;
using UnboundLib.GameModes;
using UnityEngine;

namespace InfoOverhaul.UI;

/// <summary>Shows a bottom-left "Show Stats" button during card picks (always local-player stats).</summary>
public class PickStatsController : MonoBehaviour
{
    public static PickStatsController instance;

    public static bool isPickPhase;
    public static int currentPickerID = -1;

    private Canvas _canvas;
    private StatsPopup _popup;
    private GameObject _showStatsBtnGo;

    private void Awake()
    {
        instance = this;
        BuildUI();
        GameModeManager.AddHook(GameModeHooks.HookPlayerPickEnd, OnPlayerPickEnd);
        GameModeManager.AddHook(GameModeHooks.HookPickEnd, OnPickSequenceEnd);
        IOLog.Section("PickStatsController initialized");
    }

    private void BuildUI()
    {
        _canvas = UIHelper.CreateFullscreenCanvas("IO_PickStatsCanvas", sortOrder: 160);
        var root = _canvas.GetComponent<RectTransform>();

        var showBtn = UIHelper.AnchorButton(root, "ShowStatsBtn", "Show Stats",
            new Vector2(0.02f, 0.02f), new Vector2(0.16f, 0.08f),
            new Color(0.18f, 0.35f, 0.55f, 1f), 24);
        showBtn.onClick.AddListener(TogglePopup);
        _showStatsBtnGo = showBtn.gameObject;

        _popup = StatsPopup.Create(root);
        _canvas.gameObject.SetActive(false);
    }

    public void BeginPickPhase(int pickerID, string source)
    {
        if (pickerID < 0)
            return;

        if (GetLocalHumanPlayer() == null)
        {
            HideAll();
            IOLog.Line($"Pick phase for player {pickerID} (source={source}) — no local human, UI hidden.");
            return;
        }

        currentPickerID = pickerID;
        isPickPhase = true;

        _canvas.gameObject.SetActive(true);
        _showStatsBtnGo.SetActive(true);
        CardDeltaPreviewOverlay.instance?.OnPickSequenceStarted();
        IOLog.Line($"Pick phase active — picker={pickerID}, showing local stats (source={source}).");
    }

    public void EndPickPhase(string source)
    {
        IOLog.Line($"Pick phase ended (source={source}), was picker={currentPickerID}");
        isPickPhase = false;
        currentPickerID = -1;
        CardDeltaPreviewOverlay.instance?.OnPickSequenceEnded();
        HideAll();
    }

    private void HideAll()
    {
        _popup?.Hide();
        if (_canvas != null)
            _canvas.gameObject.SetActive(false);
    }

    private void TogglePopup()
    {
        var player = GetLocalHumanPlayer();
        if (player == null)
            return;

        if (_popup.IsVisible)
            _popup.Hide();
        else
            _popup.Show(player);
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

    private IEnumerator OnPlayerPickEnd(IGameModeHandler gm)
    {
        IOLog.Line($"Single player pick ended (picker={currentPickerID}); keeping stats UI up until pick sequence ends.");
        yield break;
    }

    private IEnumerator OnPickSequenceEnd(IGameModeHandler gm)
    {
        IOLog.Line("Pick sequence ended — auto-closing stats window.");
        EndPickPhase("HookPickEnd");
        yield break;
    }
}
