using System;
using System.Linq;
using System.Reflection;

namespace ShieldsMod.Integration;

/// <summary>Optional DeckBuilder hook to register cyclical cards.</summary>
internal static class DeckBuilderCyclicalBridge
{
    private static bool _resolved;
    private static Action<string> _register;

    internal static void Register(string cardName)
    {
        if (string.IsNullOrEmpty(cardName))
            return;

        TryResolve();
        try
        {
            _register?.Invoke(cardName);
        }
        catch
        {
            // DeckBuilder not loaded or API unavailable.
        }
    }

    private static void TryResolve()
    {
        if (_resolved)
            return;

        _resolved = true;

        try
        {
            var asm = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "DeckBuilder");
            if (asm == null)
                return;

            Type type = asm.GetType("DeckBuilder.Data.CyclicalCardRegistry");
            if (type == null)
                return;

            MethodInfo method = type.GetMethod(
                "Register",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(string) },
                null);

            if (method == null)
                return;

            _register = name => method.Invoke(null, new object[] { name });
        }
        catch
        {
            _register = null;
        }
    }
}
