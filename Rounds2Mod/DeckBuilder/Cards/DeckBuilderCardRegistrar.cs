using UnboundLib.Cards;

namespace DeckBuilder.Cards
{
    internal static class DeckBuilderCardRegistrar
    {
        internal static void RegisterAll()
        {
            RTGLog.Section("DeckBuilderCardRegistrar — RegisterAll");

            CopycatLog.Section("Register card");
            CustomCard.BuildCard<CopycatCard>(ci => CopycatLog.Line($"Built '{ci.cardName}' (object='{ci.gameObject.name}')."));

            SwapLog.Section("Register card");
            CustomCard.BuildCard<SwapCard>(ci => SwapLog.Line($"Built '{ci.cardName}' (object='{ci.gameObject.name}')."));

            SearchCardLog.Section("Register cards");
            CustomCard.BuildCard<RareSearchCard>(ci =>
                SearchCardLog.Line($"Built '{ci.cardName}' (object='{ci.gameObject.name}')."));
            CustomCard.BuildCard<LegendarySearchCard>(ci =>
                SearchCardLog.Line($"Built '{ci.cardName}' (object='{ci.gameObject.name}')."));

            RTGLog.Line("Registered Copycat, Swap, Rare Search, Legendary Search.");
        }
    }
}
