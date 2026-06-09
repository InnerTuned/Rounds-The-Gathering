namespace ShieldsMod.Cards;

/// <summary>Holds CardInfo references while async BuildCard callbacks complete.</summary>
internal static class ShieldCardRegistry
{
    internal static CardInfo Lv1;
    internal static CardInfo Lv2;
    internal static CardInfo Lv3;
    internal static CardInfo Lv4;
    internal static CardInfo Lv5;

    internal static void WireLevelChain()
    {
        SetNext<UpgradeShieldLv1>(Lv1, Lv2);
        SetNext<UpgradeShieldLv2>(Lv2, Lv3);
        SetNext<UpgradeShieldLv3>(Lv3, Lv4);
        SetNext<UpgradeShieldLv4>(Lv4, Lv5);
        SLog.Line("Shield card level chain wired (LV1 -> LV5).");
    }

    private static void SetNext<T>(CardInfo from, CardInfo to) where T : UpgradeShieldHealthBase
    {
        if (from == null)
            return;

        T card = from.GetComponent<T>();
        if (card != null)
            card.NextLevelCard = to;
    }
}
