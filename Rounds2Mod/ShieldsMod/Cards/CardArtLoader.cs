using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace ShieldsMod.Cards;

/// <summary>
/// Loads card face PNGs and wraps them in a UI Image so they can render
/// inside the card prefab's Canvas/Front/Background/Art RectTransform.
/// The returned GameObject is a template parked off-screen at y=100;
/// CardVisuals clones and reparents it at localPosition zero.
/// </summary>
public static class CardArtLoader
{
    private static readonly Dictionary<string, GameObject> Cache = new Dictionary<string, GameObject>();

    public static string AssetsDirectory =>
        Path.Combine(Plugin.PluginDirectory, "assets");

    public static GameObject Load(string fileName)
    {
        SLog.Section($"CardArtLoader.Load — '{fileName}'");
        SLog.Line($"AssetsDirectory = {AssetsDirectory}");

        if (string.IsNullOrEmpty(fileName))
        {
            SLog.Warn("Load called with empty fileName — returning null.");
            return null;
        }

        if (Cache.TryGetValue(fileName, out GameObject cached) && cached != null)
        {
            SLog.Line("Returning cached art object.");
            return cached;
        }

        string path = Path.Combine(AssetsDirectory, fileName);
        SLog.Line($"Full path = {path}");
        SLog.Line($"File.Exists = {File.Exists(path)}");

        if (!File.Exists(path))
        {
            SLog.Warn($"Card art not found at: {path}");
            return null;
        }

        byte[] bytes = File.ReadAllBytes(path);
        SLog.Line($"Read {bytes.Length} bytes.");

        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        bool loaded = texture.LoadImage(bytes);
        SLog.Line($"Texture.LoadImage = {loaded}, size = {texture.width}x{texture.height}");

        if (!loaded)
        {
            SLog.Warn($"Failed to decode card art: {path}");
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

        // Create a single GameObject with RectTransform + Image directly on it.
        // This ensures proper UI layout when cloned into the card's "Art" hierarchy.
        // Parked off-screen so the template doesn't render in world space.
        var artGo = new GameObject($"ShieldsCardArt_{fileName}");
        Object.DontDestroyOnLoad(artGo);

        // Add RectTransform (replaces Transform) with fill-parent anchors
        var rt = artGo.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.localPosition = Vector3.up * 10000f; // Far off-screen

        var img = artGo.AddComponent<Image>();
        img.sprite = sprite;
        img.preserveAspect = true;
        img.raycastTarget = false;

        Cache[fileName] = artGo;
        SLog.Line($"Loaded card art '{fileName}' from {path}");
        return artGo;
    }
}
