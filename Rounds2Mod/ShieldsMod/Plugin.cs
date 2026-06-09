using System.IO;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ShieldsMod.Cards;
using ShieldsMod.Lifesteal;
using ShieldsMod.Poison;
using ShieldsMod.Shield;
using ShieldsMod.Stun;

namespace ShieldsMod;

[BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("root.rarity.lib", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("com.CrazyCoders.Rounds.RarityBundle", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("DeckBuilder", BepInDependency.DependencyFlags.SoftDependency)]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    /// <summary>Directory the plugin DLL lives in (set once in Awake, used by CardArtLoader).</summary>
    internal static string PluginDirectory { get; private set; }

    private void Awake()
    {
        Logger = base.Logger;
        PluginDirectory = Path.GetDirectoryName(Info.Location) ?? "";
        SLog.Section("Plugin Awake");
        SLog.Line($"PluginDirectory = {PluginDirectory}");

        new Harmony(MyPluginInfo.PLUGIN_GUID).PatchAll();

        gameObject.AddComponent<ShieldManager>();
        gameObject.AddComponent<PoisonResistanceManager>();
        gameObject.AddComponent<StunResistanceManager>();
        gameObject.AddComponent<LifestealResistanceManager>();

        SLog.Line($"Plugin {MyPluginInfo.PLUGIN_GUID} loaded.");
    }

    private void Start()
    {
        ShieldCardRegistrar.RegisterAll();
        PoisonCardRegistrar.RegisterAll();
        StunCardRegistrar.RegisterAll();
        LifestealCardRegistrar.RegisterAll();
    }
}
