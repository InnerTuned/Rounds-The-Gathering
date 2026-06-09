namespace ShieldsMod.Cards;

internal static class StunCardRegistry
{
    internal static CardInfo Lv1;
    internal static CardInfo Lv2;
    internal static CardInfo Lv3;

    internal static void WireLevelChain()
    {
        SetNext<StunResistanceLv1>(Lv1, Lv2);
        SetNext<StunResistanceLv2>(Lv2, Lv3);
        SLog.Line("Stun card level chain wired (LV1 -> LV3).");
    }

    private static void SetNext<T>(CardInfo from, CardInfo to) where T : StunResistanceBase
    {
        if (from == null)
            return;

        T card = from.GetComponent<T>();
        if (card != null)
            card.NextLevelCard = to;
    }
}
