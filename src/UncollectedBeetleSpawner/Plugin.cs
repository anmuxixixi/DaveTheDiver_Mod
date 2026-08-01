using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace UncollectedBeetleSpawner;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class Plugin : BasePlugin
{
    public const string PluginGuid = "cn.codex.davethediver.uncollectedbeetlespawner";
    public const string PluginName = "Uncollected Beetle Spawner";
    public const string PluginVersion = "1.0.10";

    internal static ManualLogSource ModLog { get; private set; } = null!;

    public override void Load()
    {
        ModLog = Log;
        GameObjectSelectorPatch.ValidateInteropBindings();
        Harmony.CreateAndPatchAll(typeof(GameObjectSelectorPatch), PluginGuid);
        Harmony.CreateAndPatchAll(typeof(JungleSpawnerPatch), PluginGuid);
        Log.LogInfo($"{PluginName} {PluginVersion} loaded.");
    }
}
