using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Keybound.UI;

internal static class UIHelper
{
    public static Canvas CreateFullscreenCanvas(string name, int sortOrder = 100)
    {
        var go = new GameObject(name);
        Object.DontDestroyOnLoad(go);

        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortOrder;

        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        go.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    public static RectTransform CreatePanel(Transform parent, string name,
        Vector2 anchorMin, Vector2 anchorMax, Color? bg = null)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        if (bg.HasValue)
        {
            var img = go.AddComponent<Image>();
            img.color = bg.Value;
        }
        return rt;
    }

    public static TextMeshProUGUI CreateText(Transform parent, string name, string text,
        int fontSize = 24, TextAlignmentOptions alignment = TextAlignmentOptions.Center,
        Color? color = null)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;

        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.color = color ?? Color.white;
        return tmp;
    }

    public static Button CreateButton(Transform parent, string name, string label,
        Vector2 anchorMin, Vector2 anchorMax,
        int fontSize = 20, Color? bgColor = null, Color? textColor = null)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = rt.offsetMax = Vector2.zero;

        var img = go.AddComponent<Image>();
        img.color = bgColor ?? new Color(0.2f, 0.2f, 0.2f, 0.9f);

        var btn = go.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.colors = new ColorBlock
        {
            normalColor = Color.white,
            highlightedColor = new Color(1.15f, 1.15f, 1.15f),
            pressedColor = new Color(0.8f, 0.8f, 0.8f),
            disabledColor = Color.white,
            colorMultiplier = 1f,
            fadeDuration = 0.1f
        };

        var labelGo = new GameObject("Label");
        labelGo.transform.SetParent(go.transform, false);
        var labelRt = labelGo.AddComponent<RectTransform>();
        labelRt.anchorMin = Vector2.zero;
        labelRt.anchorMax = Vector2.one;
        labelRt.offsetMin = labelRt.offsetMax = Vector2.zero;

        var tmp = labelGo.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = textColor ?? Color.white;

        return btn;
    }
}
