using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using InfoOverhaul.Compat;
using InfoOverhaul.Delta;
using InfoOverhaul.UI;
using UnityEngine;

namespace InfoOverhaul;

[BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private void Awake()
    {
        Logger = base.Logger;
        IOLog.Init(Logger);
        IOLog.Section("Plugin Awake");

        new Harmony(MyPluginInfo.PLUGIN_GUID).PatchAll();
        StatTemplateDeltaRegistrar.Init();
        MfmTier2Compat.Init();
        gameObject.AddComponent<PickStatsController>();
        gameObject.AddComponent<CardDeltaPreviewOverlay>();

        IOLog.Line($"Plugin {MyPluginInfo.PLUGIN_GUID} loaded.");
    }
}
