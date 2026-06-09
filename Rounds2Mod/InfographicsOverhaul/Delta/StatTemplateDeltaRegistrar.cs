using UnboundLib.Utils;

namespace InfoOverhaul.Delta;

/// <summary>
/// Auto-registers any card whose prefab has standard stat templates (Tier 1).
/// Includes vanilla cards and CustomCard mods like MFM Calibrate / Ammo +N series.
/// </summary>
internal static class StatTemplateDeltaRegistrar
{
    internal static void Init()
    {
        CardManager.AddAllCardsCallback(RegisterAll);
        IOLog.Line("StatTemplateDeltaRegistrar — waiting for CardManager.FirstTimeStart.");
    }

    private static void RegisterAll(CardInfo[] cards)
    {
        IOLog.Section("StatTemplateDeltaRegistrar — RegisterAll");
        int count = 0;
        int skippedExplicit = 0;

        foreach (var card in cards)
        {
            if (card == null || string.IsNullOrEmpty(card.cardName))
                continue;

            if (CardDeltaRegistry.IsRegistered(card.cardName))
            {
                skippedExplicit++;
                continue;
            }

            var root = card.sourceCard != null ? card.sourceCard.gameObject : card.gameObject;
            var cardGun = root.GetComponent<Gun>();
            var cardPlayer = root.GetComponent<CharacterStatModifiers>();
            var cardBlock = root.GetComponentInChildren<Block>();

            if (!StatTemplateDeltaProvider.HasStatTemplate(cardGun, cardPlayer, cardBlock))
                continue;

            CardDeltaRegistry.Register(card.cardName, StatTemplateDeltaProvider.Compute);
            count++;
        }

        IOLog.Line($"Registered {count} stat-template cards (skipped {skippedExplicit} already registered).");
    }
}

