namespace ShieldsMod;

internal static class SLog
{
    private const string Sep = "=====================";

    public static void Section(string title)
    {
        Plugin.Logger.LogInfo(Sep);
        Plugin.Logger.LogInfo($"[ShieldsMod] {title}");
        Plugin.Logger.LogInfo(Sep);
    }

    public static void Line(string message) =>
        Plugin.Logger.LogInfo($"[ShieldsMod] {message}");

    public static void Warn(string message) =>
        Plugin.Logger.LogWarning($"[ShieldsMod] {message}");

    public static void Error(string message) =>
        Plugin.Logger.LogError($"[ShieldsMod] {message}");
}
