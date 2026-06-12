namespace DeckBuilder.GameIntegration;

/// <summary>Enabled logging for the unified two-step card flow (debugging cancel/confirm paths).</summary>
internal static class TwoStepLog
{
    private const string Sep = "=====================";
    private const string Tag = "[DeckBuilder:TwoStep]";

    public static void Section(string title)
    {
        Plugin.Logger.LogInfo(Sep);
        Plugin.Logger.LogInfo($"{Tag} {title}");
        Plugin.Logger.LogInfo(Sep);
    }

    public static void Line(string message) => Plugin.Logger.LogInfo($"{Tag} {message}");

    public static void Warn(string message) => Plugin.Logger.LogWarning($"{Tag} {message}");

    public static void Error(string message) => Plugin.Logger.LogError($"{Tag} {message}");
}
