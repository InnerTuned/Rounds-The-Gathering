using System;
using Keybound.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Keybound.UI;

/// <summary>Modal shown when a [Keybound] card is drafted — binds effect to a key.</summary>
public class KeybindModalUI : MonoBehaviour
{
    public static KeybindModalUI instance;

    // Unity KeyCode integer values (UnityEngine.CoreModule) — avoid compile-time KeyCode type.
    private const int KeyA = 97;
    private const int KeyZ = 122;
    private const int KeyAlpha0 = 48;
    private const int KeyAlpha9 = 57;

    private Canvas _canvas;
    private TextMeshProUGUI _messageText;
    private Button _bindButton;
    private Image _bindImage;
    private TextMeshProUGUI _bindLabel;

    private Action<int> _onConfirm;
    private Action _onCancel;
    private int _pickerID;
    private CardInfo _card;
    private int? _pendingKey;

    private static readonly Color PanelBg = new Color(0.04f, 0.06f, 0.12f, 1f);
    private static readonly Color CancelBg = new Color(0.40f, 0.10f, 0.10f, 0.95f);
    private static readonly Color BindReadyBg = new Color(0.10f, 0.45f, 0.12f, 0.95f);
    private static readonly Color BindGrayBg = new Color(0.18f, 0.18f, 0.18f, 0.55f);

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        BuildUI();
        _canvas.gameObject.SetActive(false);
    }

    private void BuildUI()
    {
        _canvas = UIHelper.CreateFullscreenCanvas("KB_KeybindModal", sortOrder: 190);

        UIHelper.CreatePanel(_canvas.transform, "Dim",
            Vector2.zero, Vector2.one, new Color(0f, 0f, 0f, 0.55f));

        var panel = UIHelper.CreatePanel(_canvas.transform, "Panel",
            new Vector2(0.25f, 0.35f), new Vector2(0.75f, 0.65f), PanelBg);

        UIHelper.CreateText(panel, "Title", "Bind Key",
            fontSize: 28, alignment: TextAlignmentOptions.Top,
            color: new Color(0.9f, 0.93f, 1f));

        _messageText = UIHelper.CreateText(panel, "Message",
            "Press a key to bind this effect.",
            fontSize: 22, alignment: TextAlignmentOptions.Midline,
            color: new Color(0.88f, 0.92f, 1f));
        var msgRt = _messageText.GetComponent<RectTransform>();
        msgRt.anchorMin = new Vector2(0.05f, 0.35f);
        msgRt.anchorMax = new Vector2(0.95f, 0.75f);
        msgRt.offsetMin = msgRt.offsetMax = Vector2.zero;

        var cancelBtn = UIHelper.CreateButton(panel, "CancelBtn", "[Cancel]",
            new Vector2(0.08f, 0.10f), new Vector2(0.42f, 0.28f),
            fontSize: 20, bgColor: CancelBg);
        cancelBtn.onClick.AddListener(OnCancelClicked);

        var bindGo = new GameObject("BindBtn");
        bindGo.transform.SetParent(panel, false);
        var bindRt = bindGo.AddComponent<RectTransform>();
        bindRt.anchorMin = new Vector2(0.58f, 0.10f);
        bindRt.anchorMax = new Vector2(0.92f, 0.28f);
        bindRt.offsetMin = bindRt.offsetMax = Vector2.zero;

        _bindImage = bindGo.AddComponent<Image>();
        _bindImage.color = BindGrayBg;
        _bindButton = bindGo.AddComponent<Button>();
        _bindButton.targetGraphic = _bindImage;
        _bindButton.interactable = false;
        _bindButton.onClick.AddListener(OnBindClicked);

        var bindLabelGo = new GameObject("Label");
        bindLabelGo.transform.SetParent(bindGo.transform, false);
        var bindLabelRt = bindLabelGo.AddComponent<RectTransform>();
        bindLabelRt.anchorMin = Vector2.zero;
        bindLabelRt.anchorMax = Vector2.one;
        bindLabelRt.offsetMin = bindLabelRt.offsetMax = Vector2.zero;
        _bindLabel = bindLabelGo.AddComponent<TextMeshProUGUI>();
        _bindLabel.text = "[Bind Key]";
        _bindLabel.fontSize = 20;
        _bindLabel.alignment = TextAlignmentOptions.Center;
        _bindLabel.color = new Color(0.45f, 0.45f, 0.45f);
    }

    public void Show(int pickerID, CardInfo card, Action<int> onConfirm, Action onCancel)
    {
        _pickerID = pickerID;
        _card = card;
        _onConfirm = onConfirm;
        _onCancel = onCancel;
        _pendingKey = null;

        _messageText.text = $"Press a key to bind <b>{card.cardName}</b>.";
        RefreshBindButton();
        _canvas.gameObject.SetActive(true);
        KLog.Line($"Keybind modal shown for '{card.cardName}' (picker={pickerID}).");
    }

    public void Hide()
    {
        _canvas.gameObject.SetActive(false);
        _onConfirm = null;
        _onCancel = null;
        _card = null;
        _pendingKey = null;
    }

    private void Update()
    {
        if (!_canvas.gameObject.activeSelf || _card == null) return;

        if (InputCompat.GetKeyDown(InputCompat.KeyEscape))
        {
            OnCancelClicked();
            return;
        }

        foreach (int key in GetBindableKeys())
        {
            if (!InputCompat.GetKeyDown(key)) continue;
            if (EffectStackManager.instance?.IsKeyTaken(_pickerID, key) == true)
            {
                _messageText.text = $"<color=#ff8888>{FormatKey(key)} is already bound.</color>";
                _pendingKey = null;
                RefreshBindButton();
                return;
            }

            _pendingKey = key;
            _messageText.text = $"Bind <b>{_card.cardName}</b> to <b>{FormatKey(key)}</b>?";
            RefreshBindButton();
            return;
        }
    }

    private void RefreshBindButton()
    {
        bool ready = _pendingKey.HasValue;
        _bindButton.interactable = ready;
        _bindImage.color = ready ? BindReadyBg : BindGrayBg;
        _bindLabel.color = ready ? Color.white : new Color(0.45f, 0.45f, 0.45f);
    }

    private void OnBindClicked()
    {
        if (!_pendingKey.HasValue) return;
        int key = _pendingKey.Value;
        var cb = _onConfirm;
        Hide();
        cb?.Invoke(key);
    }

    private void OnCancelClicked()
    {
        var cb = _onCancel;
        Hide();
        cb?.Invoke();
    }

    private static int[] GetBindableKeys()
    {
        var keys = new int[36];
        int i = 0;
        for (int c = KeyA; c <= KeyZ; c++)
            keys[i++] = c;
        for (int c = KeyAlpha0; c <= KeyAlpha9; c++)
            keys[i++] = c;
        return keys;
    }

    internal static string FormatKey(int key)
    {
        if (key >= KeyAlpha0 && key <= KeyAlpha9)
            return (key - KeyAlpha0).ToString();
        if (key >= KeyA && key <= KeyZ)
            return ((char)key).ToString().ToUpper();
        return key.ToString();
    }
}
