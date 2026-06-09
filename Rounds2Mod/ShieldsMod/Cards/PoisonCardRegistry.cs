namespace ShieldsMod.Cards;

internal static class PoisonCardRegistry
{
    internal static CardInfo Lv1;
    internal static CardInfo Lv2;
    internal static CardInfo Lv3;

    internal static void WireLevelChain()
    {
        SetNext<PoisonResistanceLv1>(Lv1, Lv2);
        SetNext<PoisonResistanceLv2>(Lv2, Lv3);
        SLog.Line("Poison card level chain wired (LV1 -> LV3).");
    }

    private static void SetNext<T>(CardInfo from, CardInfo to) where T : PoisonResistanceBase
    {
        if (from == null)
            return;

        T card = from.GetComponent<T>();
        if (card != null)
            card.NextLevelCard = to;
    }
}
