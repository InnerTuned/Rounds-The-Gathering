using BepInEx.Logging;

namespace InfoOverhaul;

internal static class IOLog
{
    private static ManualLogSource _logger;

    internal static void Init(ManualLogSource logger) => _logger = logger;

    internal static void Section(string title) { /* _logger?.LogInfo($"── {title} ──"); */ }

    internal static void Line(string message) { /* _logger?.LogInfo(message); */ }

    internal static void Warn(string message) { /* _logger?.LogWarning(message); */ }

    internal static void Error(string message) { /* _logger?.LogError(message); */ }
}
