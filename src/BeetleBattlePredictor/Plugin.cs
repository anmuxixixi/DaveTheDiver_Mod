using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;

namespace BeetleBattlePredictor;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class Plugin : BasePlugin
{
    public const string PluginGuid = "cn.codex.davethediver.beetlebattlepredictor";
    public const string PluginName = "Beetle Battle Predictor";
    public const string PluginVersion = "1.0.8";

    internal static ManualLogSource ModLog { get; private set; } = null!;
    internal static ConfigEntry<bool> IsEnabled { get; private set; } = null!;
    internal static ConfigEntry<float> PositionX { get; private set; } = null!;
    internal static ConfigEntry<float> PositionY { get; private set; } = null!;
    internal static ConfigEntry<int> FontSize { get; private set; } = null!;

    public override void Load()
    {
        ModLog = Log;
        IsEnabled = Config.Bind("General", "Enabled", false, "Runtime overlay state. Every game launch starts disabled; press F8 to enable.");
        PositionX = Config.Bind("Overlay", "X", 28f, "Overlay X position in pixels.");
        PositionY = Config.Bind("Overlay", "Y", 340f, "Overlay Y position in pixels.");
        FontSize = Config.Bind("Overlay", "FontSize", 25, "Main overlay font size.");

        // F8 is a session toggle: never surprise the player with the overlay at startup.
        IsEnabled.Value = false;

        // Migrate earlier defaults while preserving custom user positions.
        if (Math.Abs(PositionX.Value - 24f) < 0.1f) PositionX.Value = 28f;
        if (Math.Abs(PositionY.Value - 120f) < 0.1f || Math.Abs(PositionY.Value - 250f) < 0.1f)
        {
            PositionY.Value = 340f;
        }
        if (FontSize.Value is 24 or 28) FontSize.Value = 25;

        AddComponent<PredictorOverlay>();
        Log.LogInfo($"{PluginName} {PluginVersion} loaded. F8 toggles the overlay.");
    }
}
