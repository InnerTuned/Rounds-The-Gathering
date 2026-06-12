using System.IO;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using DeckBuilder.CardDelete;
using DeckBuilder.Cards;
using DeckBuilder.Data;
using DeckBuilder.UI;
using DeckBuilder.GameIntegration;

namespace DeckBuilder;

[BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("root.rarity.lib", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("com.CrazyCoders.Rounds.RarityBundle", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("InfoOverhaul", BepInDependency.DependencyFlags.SoftDependency)]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    /// <summary>Directory the plugin DLL lives in (used by CardArtLoader).</summary>
    internal static string PluginDirectory { get; private set; }

    private void Awake()
    {
        Logger = base.Logger;
        PluginDirectory = Path.GetDirectoryName(Info.Location) ?? "";
        RTGLog.Section("Plugin Awake");

        new Harmony(MyPluginInfo.PLUGIN_GUID).PatchAll();

        gameObject.AddComponent<DeckManager>();
        gameObject.AddComponent<CardDeleteManager>();

        // HUD overlay (lives for the whole game session)
        gameObject.AddComponent<DeckHUDOverlay>();

        // UI screens — each manages its own canvas via DontDestroyOnLoad
        var uiRoot = new GameObject("RTG_UIRoot");
        DontDestroyOnLoad(uiRoot);
        uiRoot.AddComponent<DeckSelectorScreen>();
        uiRoot.AddComponent<CreateDeckScreen>();
        uiRoot.AddComponent<DeckEditorScreen>();
        uiRoot.AddComponent<DeckBuilderUiInputLock>();
        uiRoot.AddComponent<CardBarSelectorUI>();
        uiRoot.AddComponent<CardSearchModalUI>();

        RTGLog.Line($"Plugin {MyPluginInfo.PLUGIN_GUID} loaded.");
    }

    private void Start()
    {
        DeckBuilderCardRegistrar.RegisterAll();
    }
}
