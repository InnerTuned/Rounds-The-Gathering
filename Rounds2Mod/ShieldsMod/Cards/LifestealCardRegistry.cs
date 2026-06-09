namespace ShieldsMod.Cards;

internal static class LifestealCardRegistry
{
    internal static CardInfo Lv1;
    internal static CardInfo Lv2;
    internal static CardInfo Lv3;

    internal static void WireLevelChain()
    {
        SetNext<LifestealResistanceLv1>(Lv1, Lv2);
        SetNext<LifestealResistanceLv2>(Lv2, Lv3);
        SLog.Line("Lifesteal card level chain wired (LV1 -> LV3).");
    }

    private static void SetNext<T>(CardInfo from, CardInfo to) where T : LifestealResistanceBase
    {
        if (from == null)
            return;

        T card = from.GetComponent<T>();
        if (card != null)
            card.NextLevelCard = to;
    }
}
