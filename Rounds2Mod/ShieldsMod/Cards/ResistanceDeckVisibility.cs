namespace ShieldsMod.Cards;

/// <summary>
/// Public API for DeckBuilder (via reflection) to hide internal/legacy resistance cards.
/// </summary>
public static class ResistanceDeckVisibility
{
    private static bool _loggedOnce;

    /// <summary>
    /// True for legacy tier cards and internal marker cards that must not appear
    /// in custom decks or the DeckBuilder card browser.
    /// </summary>
    public static bool IsHiddenFromDecks(string cardName)
    {
        bool hidden = ResistanceDraftFilter.IsExcluded(cardName);

        if (!_loggedOnce && !string.IsNullOrEmpty(cardName) &&
            (cardName.Contains("Resistance") || cardName.Contains("resistance")))
        {
            SLog.Line($"ResistanceDeckVisibility.IsHiddenFromDecks('{cardName}') => {hidden}");
        }

        return hidden;
    }

    public static void LogExcludedCards()
    {
        _loggedOnce = true;
        SLog.Section("ResistanceDeckVisibility — Excluded Cards");
        ResistanceDraftFilter.LogAll();
    }
}
