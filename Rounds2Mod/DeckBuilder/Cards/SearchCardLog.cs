namespace DeckBuilder.Cards;

internal static class SearchCardLog
{
    private const string Sep = "=====================";
    private const string Tag = "[DeckBuilder:Search]";

    public static void Section(string title)
    {
        // Plugin.Logger.LogInfo(Sep);
        // Plugin.Logger.LogInfo($"{Tag} {title}");
        // Plugin.Logger.LogInfo(Sep);
    }

    public static void Line(string message) { /* Plugin.Logger.LogInfo($"{Tag} {message}"); */ }

    public static void Warn(string message) { /* Plugin.Logger.LogWarning($"{Tag} {message}"); */ }

    public static void Error(string message) { /* Plugin.Logger.LogError($"{Tag} {message}"); */ }
}
