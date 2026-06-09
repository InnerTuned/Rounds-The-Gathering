using System;
using System.Linq;
using System.Reflection;

namespace ShieldsMod.Cards;

/// <summary>
/// Bridges to DeckBuilder's public CardPrerequisiteRegistry via reflection.
/// Using reflection (rather than a hard assembly reference) means ShieldsMod still
/// loads cleanly if DeckBuilder isn't installed — the cards just won't be
/// unlock-gated in that case.
/// </summary>
internal static class RtgPrerequisiteBridge
{
    private static MethodInfo _registerMethod;
    private static bool _resolved;

    public static bool IsAvailable
    {
        get
        {
            Resolve();
            return _registerMethod != null;
        }
    }

    private static void Resolve()
    {
        if (_resolved) return;
        _resolved = true;

        try
        {
            Assembly asm = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "DeckBuilder");
            if (asm == null)
            {
                SLog.Warn("DeckBuilder assembly not found — shield unlock chain will not be registered.");
                return;
            }

            Type type = asm.GetType("DeckBuilder.CardPrerequisiteRegistry");
            if (type == null)
            {
                SLog.Warn("DeckBuilder CardPrerequisiteRegistry type not found.");
                return;
            }

            _registerMethod = type.GetMethod("RegisterPrerequisite",
                BindingFlags.Public | BindingFlags.Static,
                null, new[] { typeof(string), typeof(string) }, null);

            if (_registerMethod == null)
                SLog.Warn("RegisterPrerequisite(string, string) not found on CardPrerequisiteRegistry.");
        }
        catch (Exception ex)
        {
            SLog.Error($"Failed to resolve DeckBuilder registry: {ex.Message}");
        }
    }

    /// <summary>Registers that <paramref name="cardName"/> requires <paramref name="requiredCardName"/>.</summary>
    internal static bool Register(string cardName, string requiredCardName)
    {
        Resolve();
        if (_registerMethod == null) return false;

        try
        {
            _registerMethod.Invoke(null, new object[] { cardName, requiredCardName });
            return true;
        }
        catch (Exception ex)
        {
            SLog.Error($"RegisterPrerequisite invoke failed: {ex.Message}");
            return false;
        }
    }
}
