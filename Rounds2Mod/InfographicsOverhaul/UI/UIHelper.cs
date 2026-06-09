using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InfoOverhaul.UI;

internal static class UIHelper
{
    public static Canvas CreateFullscreenCanvas(string name, int sortOrder = 100)
    {
        var go = new GameObject(name);
        Object.DontDestroyOnLoad(go);
        go.SetActive(false);

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
        Vector2 anchorMin, Vector2 anchorMax,
        Vector2 offsetMin = default, Vector2 offsetMax = default,
        Color? bg = null)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;
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
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.color = color ?? Color.white;
        return tmp;
    }

    public static Button AnchorButton(RectTransform parent, string name, string label,
        Vector2 anchorMin, Vector2 anchorMax, Color bgColor, int fontSize)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);

        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = rt.offsetMax = Vector2.zero;

        var img = go.AddComponent<Image>();
        img.color = bgColor;

        var btn = go.AddComponent<Button>();
        btn.targetGraphic = img;
        var cb = new ColorBlock
        {
            normalColor = bgColor,
            highlightedColor = bgColor * 1.3f,
            pressedColor = bgColor * 0.7f,
            disabledColor = new Color(0.25f, 0.25f, 0.25f, 0.6f),
            colorMultiplier = 1f,
            fadeDuration = 0.1f
        };
        btn.colors = cb;

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
        tmp.color = Color.white;

        return btn;
    }
}
