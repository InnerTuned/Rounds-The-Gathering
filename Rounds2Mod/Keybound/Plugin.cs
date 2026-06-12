using System.IO;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Keybound.Cards;
using Keybound.Core;
using Keybound.Effects;
using Keybound.UI;
using UnityEngine;

namespace Keybound;

[BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("root.rarity.lib", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("com.CrazyCoders.Rounds.RarityBundle", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("DeckBuilder", BepInDependency.DependencyFlags.SoftDependency)]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    internal static string PluginDirectory { get; private set; }

    private void Awake()
    {
        Logger = base.Logger;
        PluginDirectory = Path.GetDirectoryName(Info.Location) ?? "";
        KLog.Section("Plugin Awake");
        KLog.Line($"PluginDirectory = {PluginDirectory}");

        new Harmony(MyPluginInfo.PLUGIN_GUID).PatchAll();

        gameObject.AddComponent<EffectStackManager>();
        gameObject.AddComponent<InvisibilityManager>();

        var uiRoot = new GameObject("KB_UIRoot");
        DontDestroyOnLoad(uiRoot);
        uiRoot.AddComponent<KeybindModalUI>();
        uiRoot.AddComponent<EffectStackOverlay>();

        KLog.Line($"Plugin {MyPluginInfo.PLUGIN_GUID} loaded.");
    }

    private void Start()
    {
        KeyboundCardRegistrar.RegisterAll();
    }
}
