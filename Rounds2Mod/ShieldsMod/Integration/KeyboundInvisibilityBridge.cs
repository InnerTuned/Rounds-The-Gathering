using System;
using System.Reflection;
using UnityEngine;

namespace ShieldsMod.Integration;

/// <summary>
/// Reflection bridge to query Keybound's InvisibilityManager without a hard dependency.
/// </summary>
internal static class KeyboundInvisibilityBridge
{
    private static bool _resolved;
    private static MethodInfo _isActiveMethod;

    /// <summary>The transparent white color Keybound uses when invisible.</summary>
    internal static readonly Color InvisibleColor = new Color(1f, 1f, 1f, 0.2f);

    private static void TryResolve()
    {
        if (_resolved)
            return;

        _resolved = true;

        try
        {
            var keyboundAsm = Array.Find(AppDomain.CurrentDomain.GetAssemblies(),
                a => a.GetName().Name == "Keybound");
            if (keyboundAsm == null)
                return;

            var managerType = keyboundAsm.GetType("Keybound.Effects.InvisibilityManager");
            if (managerType == null)
                return;

            _isActiveMethod = managerType.GetMethod("IsActive",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                null, new[] { typeof(Player) }, null);
        }
        catch
        {
            _isActiveMethod = null;
        }
    }

    /// <summary>Returns true if the player is currently invisible (Keybound effect active).</summary>
    internal static bool IsPlayerInvisible(Player player)
    {
        if (player == null)
            return false;

        TryResolve();

        if (_isActiveMethod == null)
            return false;

        try
        {
            return (bool)_isActiveMethod.Invoke(null, new object[] { player });
        }
        catch
        {
            return false;
        }
    }
}
