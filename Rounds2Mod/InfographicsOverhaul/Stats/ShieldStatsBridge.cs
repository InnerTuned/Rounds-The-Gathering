using System;
using System.Linq;
using System.Reflection;

namespace InfoOverhaul.Stats;

/// <summary>Reads shield health from ShieldsMod via reflection (soft dependency).</summary>
internal static class ShieldStatsBridge
{
    private static bool _resolved;
    private static PropertyInfo _instanceProp;
    private static MethodInfo _getShield;
    private static FieldInfo _currentField;
    private static FieldInfo _maxField;

    public static bool IsAvailable
    {
        get
        {
            Resolve();
            return _instanceProp != null && _getShield != null;
        }
    }

    private static void Resolve()
    {
        if (_resolved) return;
        _resolved = true;

        try
        {
            var asm = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "ShieldsMod");
            if (asm == null)
            {
                IOLog.Line("ShieldsMod not loaded — Shield Health will show as N/A.");
                return;
            }

            var managerType = asm.GetType("ShieldsMod.Shield.ShieldManager");
            var stateType = asm.GetType("ShieldsMod.Shield.ShieldState");
            if (managerType == null || stateType == null)
            {
                IOLog.Warn("ShieldsMod shield types not found.");
                return;
            }

            _instanceProp = managerType.GetProperty("instance", BindingFlags.Public | BindingFlags.Static);
            _getShield = managerType.GetMethod("GetShield", BindingFlags.Public | BindingFlags.Instance);
            _currentField = stateType.GetField("Current", BindingFlags.Public | BindingFlags.Instance);
            _maxField = stateType.GetField("Max", BindingFlags.Public | BindingFlags.Instance);

            if (_instanceProp == null || _getShield == null || _currentField == null || _maxField == null)
                IOLog.Warn("ShieldsMod shield API incomplete.");
        }
        catch (Exception ex)
        {
            IOLog.Error($"ShieldStatsBridge resolve failed: {ex.Message}");
        }
    }

    public static bool TryGetShieldHealth(int playerID, out float current, out float max)
    {
        current = 0f;
        max = 0f;
        Resolve();

        if (_instanceProp == null || _getShield == null)
            return false;

        try
        {
            var manager = _instanceProp.GetValue(null);
            if (manager == null)
                return false;

            var state = _getShield.Invoke(manager, new object[] { playerID });
            if (state == null)
                return false;

            current = (float)_currentField.GetValue(state);
            max = (float)_maxField.GetValue(state);
            return true;
        }
        catch (Exception ex)
        {
            IOLog.Warn($"TryGetShieldHealth failed: {ex.Message}");
            return false;
        }
    }
}
