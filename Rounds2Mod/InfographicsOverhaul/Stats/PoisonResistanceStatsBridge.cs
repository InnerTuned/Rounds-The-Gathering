using System;
using System.Linq;
using System.Reflection;

namespace InfoOverhaul.Stats;

/// <summary>Reads poison resistance from ShieldsMod via reflection (soft dependency).</summary>
internal static class PoisonResistanceStatsBridge
{
    private static bool _resolved;
    private static FieldInfo _instanceField;
    private static MethodInfo _getPercent;

    public static bool IsAvailable
    {
        get
        {
            Resolve();
            return _instanceField != null && _getPercent != null;
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
                return;

            var type = asm.GetType("ShieldsMod.Poison.PoisonResistanceManager");
            if (type == null)
                return;

            _instanceField = type.GetField("instance", BindingFlags.Public | BindingFlags.Static);
            _getPercent = type.GetMethod("GetReductionPercent", BindingFlags.Public | BindingFlags.Instance);
        }
        catch (Exception ex)
        {
            IOLog.Warn($"PoisonResistanceStatsBridge resolve failed: {ex.Message}");
        }
    }

    public static bool TryGetPercent(int playerID, out float percent)
    {
        percent = 0f;
        Resolve();

        if (_instanceField == null || _getPercent == null)
            return false;

        try
        {
            var manager = _instanceField.GetValue(null);
            if (manager == null)
                return false;

            percent = (float)_getPercent.Invoke(manager, new object[] { playerID });
            return true;
        }
        catch (Exception ex)
        {
            IOLog.Warn($"TryGetPercent failed: {ex.Message}");
            return false;
        }
    }
}
