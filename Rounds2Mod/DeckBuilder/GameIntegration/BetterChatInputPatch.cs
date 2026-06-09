using System.Reflection;
using HarmonyLib;

namespace DeckBuilder.GameIntegration;

/// <summary>
/// BetterChat.Update() crashes when typing into UI fields. Skip it while text input is focused.
/// </summary>
[HarmonyPatch]
internal static class BetterChatInputPatch
{
    static MethodBase TargetMethod()
    {
        var type = AccessTools.TypeByName("BetterChat.BetterChat");
        return type == null ? null : AccessTools.Method(type, "Update");
    }

    static bool Prepare() => TargetMethod() != null;

    [HarmonyPrefix]
    static bool Prefix()
    {
        if (GameManager.lockInput)
            return false;

        return !UiInputFocusHelper.IsTextInputFocused();
    }
}
