using System.Collections.Generic;

namespace DeckBuilder
{
    /// <summary>
    /// Public registry that lets mods declare card unlock prerequisites.
    /// A card is "unlocked" for a player only once they already own its required card.
    /// DeckBuilder evaluates these at each pick using the player's current
    /// inventory (player.data.currentCards), so unlocks are per-player and re-evaluated
    /// every draft — no fragile global enable/disable required.
    ///
    /// Mods register their chains on load, e.g.:
    ///   CardPrerequisiteRegistry.RegisterPrerequisite("Upgrade Shield Health II", "Upgrade Shield Health I");
    ///
    /// Keys/values are card display names (CardInfo.cardName).
    /// </summary>
    public static class CardPrerequisiteRegistry
    {
        // cardName -> required (prerequisite) cardName
        private static readonly Dictionary<string, string> _prerequisites =
            new Dictionary<string, string>();

        /// <summary>Declares that <paramref name="cardName"/> requires the player to already own <paramref name="requiredCardName"/>.</summary>
        public static void RegisterPrerequisite(string cardName, string requiredCardName)
        {
            if (string.IsNullOrEmpty(cardName) || string.IsNullOrEmpty(requiredCardName))
                return;
            _prerequisites[cardName] = requiredCardName;
        }

        /// <summary>True if a prerequisite has been registered for this card.</summary>
        public static bool HasPrerequisite(string cardName) =>
            !string.IsNullOrEmpty(cardName) && _prerequisites.ContainsKey(cardName);

        /// <summary>Returns the required card name for this card, or null if none.</summary>
        public static string GetPrerequisite(string cardName) =>
            (!string.IsNullOrEmpty(cardName) && _prerequisites.TryGetValue(cardName, out string req)) ? req : null;

        /// <summary>
        /// Returns true if the card has no prerequisite, or its prerequisite is present
        /// in <paramref name="ownedCardNames"/> (the player's current inventory).
        /// </summary>
        public static bool IsUnlocked(string cardName, ICollection<string> ownedCardNames)
        {
            string req = GetPrerequisite(cardName);
            if (string.IsNullOrEmpty(req))
                return true;
            return ownedCardNames != null && ownedCardNames.Contains(req);
        }

        /// <summary>Number of registered prerequisites (for diagnostics).</summary>
        public static int Count => _prerequisites.Count;
    }
}
