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

    private const int KeyA = 97;
    private const int KeyZ = 122;
    private const int KeyAlpha0 = 48;
    private const int KeyAlpha9 = 57;

    private Canvas _canvas;
    private TextMeshProUGUI _messageText;
    private Image _bindImage;
    private TextMeshProUGUI _bindLabel;

    private RectTransform _cancelRect;
    private RectTransform _bindRect;
    private Camera _uiCamera;

    private Action<int> _onConfirm;
    private Action _onCancel;
    private int _pickerID;
    private CardInfo _card;
    private int? _pendingKey;
    private int _ignoreInputFrames;
    private bool _pressStartedInModal;

    private static readonly Color PanelBg = new Color(0.04f, 0.06f, 0.12f, 1f);
    private static readonly Color CancelBg = new Color(0.40f, 0.10f, 0.10f, 0.95f);
    private static readonly Color CancelHoverBg = new Color(0.55f, 0.15f, 0.15f, 0.95f);
    private static readonly Color BindReadyBg = new Color(0.10f, 0.45f, 0.12f, 0.95f);
    private static readonly Color BindHoverBg = new Color(0.15f, 0.60f, 0.18f, 0.95f);
    private static readonly Color BindGrayBg = new Color(0.18f, 0.18f, 0.18f, 0.55f);

    private Image _cancelImage;

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

        // Cancel — visual-only, no onClick listener. Polled from Update().
        var cancelGo = new GameObject("CancelBtn");
        cancelGo.transform.SetParent(panel, false);
        _cancelRect = cancelGo.AddComponent<RectTransform>();
        _cancelRect.anchorMin = new Vector2(0.08f, 0.10f);
        _cancelRect.anchorMax = new Vector2(0.42f, 0.28f);
        _cancelRect.offsetMin = _cancelRect.offsetMax = Vector2.zero;
        _cancelImage = cancelGo.AddComponent<Image>();
        _cancelImage.color = CancelBg;
        var cancelLabel = new GameObject("Label");
        cancelLabel.transform.SetParent(cancelGo.transform, false);
        var clRt = cancelLabel.AddComponent<RectTransform>();
        clRt.anchorMin = Vector2.zero; clRt.anchorMax = Vector2.one;
        clRt.offsetMin = clRt.offsetMax = Vector2.zero;
        var clTmp = cancelLabel.AddComponent<TextMeshProUGUI>();
        clTmp.text = "[Cancel]";
        clTmp.fontSize = 20;
        clTmp.alignment = TextAlignmentOptions.Center;
        clTmp.color = Color.white;

        // Bind — visual-only, no onClick listener. Polled from Update().
        var bindGo = new GameObject("BindBtn");
        bindGo.transform.SetParent(panel, false);
        _bindRect = bindGo.AddComponent<RectTransform>();
        _bindRect.anchorMin = new Vector2(0.58f, 0.10f);
        _bindRect.anchorMax = new Vector2(0.92f, 0.28f);
        _bindRect.offsetMin = _bindRect.offsetMax = Vector2.zero;
        _bindImage = bindGo.AddComponent<Image>();
        _bindImage.color = BindGrayBg;
        var bindLabelGo = new GameObject("Label");
        bindLabelGo.transform.SetParent(bindGo.transform, false);
        var blRt = bindLabelGo.AddComponent<RectTransform>();
        blRt.anchorMin = Vector2.zero; blRt.anchorMax = Vector2.one;
        blRt.offsetMin = blRt.offsetMax = Vector2.zero;
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
        _ignoreInputFrames = 3;
        _pressStartedInModal = false;
        RefreshBindVisual();

        _canvas.gameObject.SetActive(true);
        _uiCamera = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;
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

        if (_ignoreInputFrames > 0)
        {
            _ignoreInputFrames--;
            return;
        }

        // Keyboard: Escape cancels
        if (InputCompat.GetKeyDown(InputCompat.KeyEscape))
        {
            DoCancel("escape");
            return;
        }

        // Mouse: poll the click ourselves instead of relying on the EventSystem.
        // A click is only honored if BOTH its press and release happen while the
        // modal is open — this rejects the carried-over release from the click
        // that opened the modal (whose press happened before the modal existed).
        if (InputCompat.GetMouseButtonDown(0))
        {
            _pressStartedInModal = true;
            KLog.Line("Mouse down observed — pressStartedInModal=true.");
        }

        if (InputCompat.GetMouseButtonUp(0))
        {
            Vector3 mousePos = InputCompat.MousePosition;
            bool overCancel = RectTransformUtility.RectangleContainsScreenPoint(_cancelRect, mousePos, _uiCamera);
            bool overBind = _pendingKey.HasValue &&
                RectTransformUtility.RectangleContainsScreenPoint(_bindRect, mousePos, _uiCamera);

            if (!_pressStartedInModal)
            {
                if (overCancel || overBind)
                    KLog.Line($"Mouse up IGNORED (pressStartedInModal=false). overCancel={overCancel}, overBind={overBind}");
            }
            else
            {
                _pressStartedInModal = false;
                KLog.Line($"Mouse up accepted. overCancel={overCancel}, overBind={overBind}");

                if (overCancel)
                {
                    DoCancel("mouse");
                    return;
                }

                if (overBind)
                {
                    DoBind();
                    return;
                }
            }
        }

        // Hover feedback
        UpdateHoverVisuals();

        // Key binding detection
        foreach (int key in GetBindableKeys())
        {
            if (!InputCompat.GetKeyDown(key)) continue;
            if (EffectStackManager.instance?.IsKeyTaken(_pickerID, key) == true)
            {
                _messageText.text = $"<color=#ff8888>{FormatKey(key)} is already bound.</color>";
                _pendingKey = null;
                RefreshBindVisual();
                return;
            }

            _pendingKey = key;
            _messageText.text = $"Bind <b>{_card.cardName}</b> to <b>{FormatKey(key)}</b>?";
            RefreshBindVisual();
            return;
        }
    }

    private void DoCancel(string source)
    {
        KLog.Line($"Keybind cancel accepted (source={source}).");
        var cb = _onCancel;
        Hide();
        cb?.Invoke();
    }

    private void DoBind()
    {
        if (!_pendingKey.HasValue) return;
        int key = _pendingKey.Value;
        KLog.Line($"Keybind confirmed: key={FormatKey(key)}.");
        var cb = _onConfirm;
        Hide();
        cb?.Invoke(key);
    }

    private void UpdateHoverVisuals()
    {
        Vector3 mousePos = InputCompat.MousePosition;
        bool overCancel = RectTransformUtility.RectangleContainsScreenPoint(_cancelRect, mousePos, _uiCamera);
        _cancelImage.color = overCancel ? CancelHoverBg : CancelBg;

        if (_pendingKey.HasValue)
        {
            bool overBind = RectTransformUtility.RectangleContainsScreenPoint(_bindRect, mousePos, _uiCamera);
            _bindImage.color = overBind ? BindHoverBg : BindReadyBg;
        }
    }

    private void RefreshBindVisual()
    {
        bool ready = _pendingKey.HasValue;
        _bindImage.color = ready ? BindReadyBg : BindGrayBg;
        _bindLabel.color = ready ? Color.white : new Color(0.45f, 0.45f, 0.45f);
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
