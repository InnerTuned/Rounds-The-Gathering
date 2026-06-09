using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DeckBuilder.UI
{
    /// <summary>Helper utilities for creating Unity UI elements at runtime.</summary>
    internal static class UIHelper
    {
        // ── Canvas ────────────────────────────────────────────────────────────────

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

        // ── Containers ────────────────────────────────────────────────────────────

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

        public static RectTransform CreateStretchPanel(Transform parent, string name,
            float padLeft = 0, float padRight = 0, float padTop = 0, float padBottom = 0,
            Color? bg = null)
        {
            return CreatePanel(parent, name,
                Vector2.zero, Vector2.one,
                new Vector2(padLeft, padBottom), new Vector2(-padRight, -padTop),
                bg);
        }

        // ── Text ─────────────────────────────────────────────────────────────────

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

        // ── Button ────────────────────────────────────────────────────────────────

        public static Button CreateButton(Transform parent, string name, string label,
            Vector2 anchoredPos, Vector2 sizeDelta,
            int fontSize = 24, Color? bgColor = null, Color? textColor = null)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var rt = go.AddComponent<RectTransform>();
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;

            var img = go.AddComponent<Image>();
            img.color = bgColor ?? new Color(0.2f, 0.2f, 0.2f, 0.9f);

            var btn = go.AddComponent<Button>();
            var cb = new ColorBlock
            {
                normalColor = img.color,
                highlightedColor = img.color * 1.3f,
                pressedColor = img.color * 0.7f,
                disabledColor = new Color(0.3f, 0.3f, 0.3f, 0.5f),
                colorMultiplier = 1f,
                fadeDuration = 0.1f
            };
            btn.colors = cb;
            btn.targetGraphic = img;

            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(go.transform, false);
            var labelRt = labelGo.AddComponent<RectTransform>();
            labelRt.anchorMin = Vector2.zero;
            labelRt.anchorMax = Vector2.one;
            labelRt.offsetMin = Vector2.zero;
            labelRt.offsetMax = Vector2.zero;

            var tmp = labelGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = fontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = textColor ?? Color.white;

            return btn;
        }

        // ── Input Field ───────────────────────────────────────────────────────────

        public static TMP_InputField CreateInputField(Transform parent, string name,
            string placeholder, Vector2 anchoredPos, Vector2 sizeDelta,
            int fontSize = 22, TMP_InputField.ContentType contentType = TMP_InputField.ContentType.Standard)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var rt = go.AddComponent<RectTransform>();
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;

            var img = go.AddComponent<Image>();
            img.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

            // viewport
            var vpGo = new GameObject("Viewport");
            vpGo.transform.SetParent(go.transform, false);
            var vpRt = vpGo.AddComponent<RectTransform>();
            vpRt.anchorMin = Vector2.zero;
            vpRt.anchorMax = Vector2.one;
            vpRt.offsetMin = new Vector2(8, 1);
            vpRt.offsetMax = new Vector2(-8, -1);
            vpGo.AddComponent<RectMask2D>();

            // text
            var textGo = new GameObject("Text");
            textGo.transform.SetParent(vpGo.transform, false);
            var textRt = textGo.AddComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = Vector2.zero;
            textRt.offsetMax = Vector2.zero;
            var textTmp = textGo.AddComponent<TextMeshProUGUI>();
            textTmp.fontSize = fontSize;
            textTmp.color = Color.white;
            textTmp.alignment = TextAlignmentOptions.MidlineLeft;

            // placeholder
            var phGo = new GameObject("Placeholder");
            phGo.transform.SetParent(vpGo.transform, false);
            var phRt = phGo.AddComponent<RectTransform>();
            phRt.anchorMin = Vector2.zero;
            phRt.anchorMax = Vector2.one;
            phRt.offsetMin = Vector2.zero;
            phRt.offsetMax = Vector2.zero;
            var phTmp = phGo.AddComponent<TextMeshProUGUI>();
            phTmp.text = placeholder;
            phTmp.fontSize = fontSize;
            phTmp.color = new Color(0.5f, 0.5f, 0.5f);
            phTmp.fontStyle = FontStyles.Italic;
            phTmp.alignment = TextAlignmentOptions.MidlineLeft;

            var field = go.AddComponent<TMP_InputField>();
            field.textViewport = vpRt;
            field.textComponent = textTmp;
            field.placeholder = phTmp;
            field.contentType = contentType;
            field.targetGraphic = img;

            return field;
        }

        // ── Backdrop ──────────────────────────────────────────────────────────────

        public static Image CreateDimmedBackground(Transform parent)
        {
            var go = new GameObject("Backdrop");
            go.transform.SetParent(parent, false);
            go.transform.SetAsFirstSibling();

            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            var img = go.AddComponent<Image>();
            img.color = Color.black;
            return img;
        }
    }
}
