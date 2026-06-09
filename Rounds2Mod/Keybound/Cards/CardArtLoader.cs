using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Keybound.Cards;

/// <summary>
/// Loads card face PNGs from Keybound/assets and wraps them in a UI Image
/// for the card prefab's Art slot.
/// Template is parked off-screen so it doesn't render in world space.
/// </summary>
internal static class CardArtLoader
{
    private static readonly Dictionary<string, GameObject> Cache = new Dictionary<string, GameObject>();

    private static string AssetsDirectory =>
        Path.Combine(Plugin.PluginDirectory, "assets");

    internal static GameObject Load(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return null;

        if (Cache.TryGetValue(fileName, out GameObject cached) && cached != null)
        {
            KLog.Line($"CardArtLoader: Returning cached art for '{fileName}'.");
            return cached;
        }

        string path = Path.Combine(AssetsDirectory, fileName);
        KLog.Line($"CardArtLoader: Loading '{fileName}' from {path}");

        if (!File.Exists(path))
        {
            KLog.Warn($"CardArtLoader: Card art not found: {path}");
            return null;
        }

        byte[] bytes = File.ReadAllBytes(path);
        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!texture.LoadImage(bytes))
        {
            KLog.Warn($"CardArtLoader: Failed to decode card art: {path}");
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

        var artGo = new GameObject($"KeyboundCardArt_{fileName}");
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
        KLog.Line($"CardArtLoader: Loaded {texture.width}x{texture.height} from {path}");
        return artGo;
    }
}
