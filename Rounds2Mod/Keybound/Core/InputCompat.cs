using System;
using System.Reflection;
using UnityEngine;

namespace Keybound.Core;

/// <summary>
/// Calls Unity's legacy Input API via reflection against UnityEngine.CoreModule
/// so Keybound.dll does not reference UnityEngine.InputLegacyModule (not shipped by ROUNDS).
/// </summary>
internal static class InputCompat
{
    internal const int KeyEscape = 27;

    private static Type _keyCodeType;
    private static MethodInfo _getKeyDown;
    private static MethodInfo _getMouseButtonDown;
    private static MethodInfo _getMouseButtonUp;
    private static PropertyInfo _mousePosition;
    private static bool _resolved;
    private static bool _available;

    internal static bool GetKeyDown(int keyCode)
    {
        Resolve();
        if (!_available) return false;

        try
        {
            object key = Enum.ToObject(_keyCodeType, keyCode);
            return (bool)_getKeyDown.Invoke(null, new[] { key });
        }
        catch (Exception ex)
        {
            KLog.Warn($"InputCompat.GetKeyDown failed: {ex.Message}");
            return false;
        }
    }

    internal static bool GetMouseButtonDown(int button)
    {
        Resolve();
        if (_getMouseButtonDown == null) return false;
        try
        {
            return (bool)_getMouseButtonDown.Invoke(null, new object[] { button });
        }
        catch { return false; }
    }

    internal static bool GetMouseButtonUp(int button)
    {
        Resolve();
        if (_getMouseButtonUp == null) return false;
        try
        {
            return (bool)_getMouseButtonUp.Invoke(null, new object[] { button });
        }
        catch { return false; }
    }

    internal static Vector3 MousePosition
    {
        get
        {
            Resolve();
            if (!_available) return Vector3.zero;
            try
            {
                return (Vector3)_mousePosition.GetValue(null, null);
            }
            catch (Exception ex)
            {
                KLog.Warn($"InputCompat.MousePosition failed: {ex.Message}");
                return Vector3.zero;
            }
        }
    }

    private static void Resolve()
    {
        if (_resolved) return;
        _resolved = true;

        try
        {
            _keyCodeType = Type.GetType("UnityEngine.KeyCode, UnityEngine.CoreModule")
                        ?? Type.GetType("UnityEngine.KeyCode, UnityEngine");
            Type inputType = Type.GetType("UnityEngine.Input, UnityEngine.CoreModule")
                          ?? Type.GetType("UnityEngine.Input, UnityEngine");

            if (_keyCodeType == null || inputType == null)
            {
                KLog.Warn("InputCompat: UnityEngine.Input/KeyCode not found in CoreModule.");
                return;
            }

            _getKeyDown = inputType.GetMethod("GetKeyDown",
                BindingFlags.Public | BindingFlags.Static,
                null, new[] { _keyCodeType }, null);
            _getMouseButtonDown = inputType.GetMethod("GetMouseButtonDown",
                BindingFlags.Public | BindingFlags.Static,
                null, new[] { typeof(int) }, null);
            _getMouseButtonUp = inputType.GetMethod("GetMouseButtonUp",
                BindingFlags.Public | BindingFlags.Static,
                null, new[] { typeof(int) }, null);
            _mousePosition = inputType.GetProperty("mousePosition",
                BindingFlags.Public | BindingFlags.Static);

            _available = _getKeyDown != null && _mousePosition != null;
            if (_available)
                KLog.Line("InputCompat resolved against UnityEngine.CoreModule.");
            else
                KLog.Warn("InputCompat: GetKeyDown or mousePosition not found.");
        }
        catch (Exception ex)
        {
            KLog.Warn($"InputCompat resolve failed: {ex.Message}");
        }
    }
}
