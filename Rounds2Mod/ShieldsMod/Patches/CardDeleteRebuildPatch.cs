using System.Reflection;
using HarmonyLib;
using ShieldsMod.Lifesteal;
using ShieldsMod.Poison;
using ShieldsMod.Shield;
using ShieldsMod.Stun;

namespace ShieldsMod.Patches;

/// <summary>
/// After the card-delete mod removes a card and reapplies survivors, recalculate shield max.
/// </summary>
[HarmonyPatch]
internal static class CardDeleteRebuildPatch
{
    private static MethodBase TargetMethod()
    {
        var type = AccessTools.TypeByName("DeckBuilder.CardDelete.CardDeleteManager");
        return type == null ? null : AccessTools.Method(type, "URPC_SyncDelete");
    }

    private static bool Prepare()
    {
        bool ok = TargetMethod() != null;
        if (!ok)
            SLog.Line("CardDeleteRebuildPatch — CardDeleteManager not found; delete hook skipped.");
        return ok;
    }

    private static void Postfix(int playerID)
    {
        ShieldHandRebuild.RebuildForPlayer(playerID);
        PoisonHandRebuild.RebuildForPlayer(playerID);
        StunHandRebuild.RebuildForPlayer(playerID);
        LifestealHandRebuild.RebuildForPlayer(playerID);
    }
}
