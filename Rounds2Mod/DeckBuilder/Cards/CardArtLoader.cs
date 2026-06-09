using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace DeckBuilder.Cards;

/// <summary>
/// Loads card face PNGs from DeckBuilder/assets and wraps them in a UI Image
/// for the card prefab's Art slot.
/// </summary>
public static class CardArtLoader
{
    private static readonly Dictionary<string, GameObject> Cache = new Dictionary<string, GameObject>();

    public static string AssetsDirectory =>
        Path.Combine(Plugin.PluginDirectory, "assets");

    public static GameObject Load(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return null;

        if (Cache.TryGetValue(fileName, out GameObject cached) && cached != null)
        {
            LogArtLoad(fileName, "Returning cached art object.");
            return cached;
        }

        string path = Path.Combine(AssetsDirectory, fileName);
        LogArtLoad(fileName, $"Loading from {path}");
        if (!File.Exists(path))
        {
            LogArtWarn(fileName, $"Card art not found: {path}");
            return null;
        }

        byte[] bytes = File.ReadAllBytes(path);
        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!texture.LoadImage(bytes))
        {
            LogArtWarn(fileName, $"Failed to decode card art: {path}");
            Object.Destroy(texture);
            return null;
        }

        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        var sprite = Sprite.Create(
            texture,
            new Rect(0f, 0f, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100f);

        var artGo = new GameObject($"DeckBuilderCardArt_{fileName}");
        Object.DontDestroyOnLoad(artGo);

        var rt = artGo.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.localPosition = Vector3.up * 10000f;

        var img = artGo.AddComponent<Image>();
        img.sprite = sprite;
        img.preserveAspect = true;
        img.raycastTarget = false;

        Cache[fileName] = artGo;
        LogArtLoad(fileName, $"Loaded {texture.width}x{texture.height} from {path}");
        return artGo;
    }

    private static void LogArtLoad(string fileName, string message)
    {
        if (fileName == "Copycat.png") CopycatLog.Line(message);
        else if (fileName == "Swap.png") SwapLog.Line(message);
        else RTGLog.Line(message);
    }

    private static void LogArtWarn(string fileName, string message)
    {
        if (fileName == "Copycat.png") CopycatLog.Warn(message);
        else if (fileName == "Swap.png") SwapLog.Warn(message);
        else RTGLog.Warn(message);
    }
}
