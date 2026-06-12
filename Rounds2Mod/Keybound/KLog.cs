using BepInEx.Logging;

namespace Keybound;

internal static class KLog
{
    private static ManualLogSource Logger => Plugin.Logger;

    internal static void Section(string title)
    {
        // Logger?.LogInfo("=====================");
        // Logger?.LogInfo($"[Keybound] {title}");
        // Logger?.LogInfo("=====================");
    }

    internal static void Line(string message) { /* Logger?.LogInfo($"[Keybound] {message}"); */ }
    internal static void Warn(string message) { /* Logger?.LogWarning($"[Keybound] {message}"); */ }
    internal static void Error(string message) { /* Logger?.LogError($"[Keybound] {message}"); */ }
}
