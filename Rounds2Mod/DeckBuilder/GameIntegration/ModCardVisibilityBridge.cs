using System;
using System.Linq;
using System.Reflection;

namespace DeckBuilder.GameIntegration;

/// <summary>
/// Optional cross-mod hook to hide cards from DeckBuilder UI and runtime deck pools.
/// Currently wired to ShieldsMod's resistance card visibility API.
/// </summary>
internal static class ModCardVisibilityBridge
{
    private static bool _successfullyResolved;
    private static Func<string, bool> _isHiddenFromDecks;

    internal static bool IsHidden(CardInfo card)
    {
        if (card == null || string.IsNullOrEmpty(card.cardName))
            return false;

        TryResolve();
        return _isHiddenFromDecks != null && _isHiddenFromDecks(card.cardName);
    }

    private static void TryResolve()
    {
        if (_successfullyResolved)
            return;

        try
        {
            var asm = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "ShieldsMod");
            if (asm == null)
                return;

            Type type = asm.GetType("ShieldsMod.Cards.ResistanceDeckVisibility");
            if (type == null)
            {
                RTGLog.Line("ModCardVisibilityBridge: ShieldsMod loaded but ResistanceDeckVisibility type not found.");
                return;
            }

            MethodInfo method = type.GetMethod(
                "IsHiddenFromDecks",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(string) },
                null);

            if (method == null)
            {
                RTGLog.Line("ModCardVisibilityBridge: IsHiddenFromDecks method not found.");
                return;
            }

            _isHiddenFromDecks = name =>
            {
                if (string.IsNullOrEmpty(name))
                    return false;
                return (bool)method.Invoke(null, new object[] { name });
            };

            _successfullyResolved = true;
            RTGLog.Line("ModCardVisibilityBridge: Successfully resolved ShieldsMod visibility API.");
        }
        catch (Exception ex)
        {
            RTGLog.Warn($"ModCardVisibilityBridge resolve failed: {ex.Message}");
        }
    }
}
