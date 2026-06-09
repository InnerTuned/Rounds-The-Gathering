using HarmonyLib;
using Keybound.Core;
using UnityEngine;

namespace Keybound.Effects;

internal static class TeleportEffect
{
    internal const string CardName = "Teleport";
    internal const float InitialDelay = 10f;
    internal const float Cooldown = 15f;

    internal static readonly KeyboundEffectDef Def = new()
    {
        CardName = CardName,
        InitialDelay = InitialDelay,
        Cooldown = Cooldown,
        Activate = TryTeleport
    };

    private static bool TryTeleport(Player player)
    {
        if (player?.data == null || MainCam.instance?.cam == null)
            return false;

        Vector3 world = MainCam.instance.cam.ScreenToWorldPoint(InputCompat.MousePosition);
        world.z = 0f;

        var collision = player.GetComponentInParent<PlayerCollision>();
        collision?.IgnoreWallForFrames(2);

        player.transform.position = world;
        if (player.data.playerVel != null)
            Traverse.Create(player.data.playerVel).Field("velocity").SetValue(Vector2.zero);
        player.data.sinceGrounded = 0f;

        KLog.Line($"Teleport activated for player {player.playerID} -> ({world.x:F1}, {world.y:F1}).");
        return true;
    }
}
