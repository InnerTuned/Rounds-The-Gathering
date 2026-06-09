namespace DeckBuilder
{
    internal static class RTGLog
    {
        private const string Sep = "=====================";

        public static void Section(string title)
        {
            Plugin.Logger.LogInfo(Sep);
            Plugin.Logger.LogInfo($"[DeckBuilder] {title}");
            Plugin.Logger.LogInfo(Sep);
        }

        public static void Line(string message) =>
            Plugin.Logger.LogInfo($"[DeckBuilder] {message}");

        public static void Warn(string message) =>
            Plugin.Logger.LogWarning($"[DeckBuilder] {message}");

        public static void Error(string message) =>
            Plugin.Logger.LogError($"[DeckBuilder] {message}");
    }
}
