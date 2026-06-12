using System;
using System.Linq;
using System.Reflection;

namespace DeckBuilder.GameIntegration;

/// <summary>
/// Optional cross-mod hook to detect Keybound cards for deck-builder limits.
/// </summary>
internal static class KeyboundCardBridge
{
    private static bool _successfullyResolved;
    private static Func<string, bool> _isKeybound;

    private static bool _searchEntryResolved;
    private static Action<int, CardInfo, Action<int>, Action> _startKeybindFromSearch;

    internal static bool IsKeybound(CardInfo card) =>
        card != null && IsKeybound(card.cardName);

    /// <summary>
    /// Launches Keybound's keybind step for a keybound card chosen from the search modal.
    /// Consumption is deferred until <paramref name="onBound"/>; <paramref name="onCancel"/>
    /// restores the draft hand. Returns false if Keybound isn't installed.
    /// </summary>
    internal static bool TryStartKeybindFromSearch(
        int pickerID, CardInfo card, Action<int> onBound, Action onCancel)
    {
        ResolveSearchEntry();
        if (_startKeybindFromSearch == null)
            return false;

        _startKeybindFromSearch(pickerID, card, onBound, onCancel);
        return true;
    }

    private static void ResolveSearchEntry()
    {
        if (_searchEntryResolved)
            return;
        _searchEntryResolved = true;

        try
        {
            var asm = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "Keybound");
            Type type = asm?.GetType("Keybound.Patches.KeyboundPickPatches");
            MethodInfo method = type?.GetMethod("StartKeybindFromSearch",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(int), typeof(CardInfo), typeof(Action<int>), typeof(Action) },
                null);

            if (method != null)
                _startKeybindFromSearch = (pickerID, card, onBound, onCancel) =>
                    method.Invoke(null, new object[] { pickerID, card, onBound, onCancel });
        }
        catch
        {
            _startKeybindFromSearch = null;
        }
    }

    internal static bool IsKeybound(string cardName)
    {
        if (string.IsNullOrEmpty(cardName))
            return false;

        TryResolve();
        return _isKeybound != null && _isKeybound(cardName);
    }

    private static void TryResolve()
    {
        if (_successfullyResolved)
            return;

        try
        {
            var asm = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "Keybound");
            if (asm == null)
                return;

            Type type = asm.GetType("Keybound.Cards.KeyboundDeckRules");
            if (type == null)
                return;

            MethodInfo method = type.GetMethod(
                "IsKeybound",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(string) },
                null);

            if (method == null)
                return;

            _isKeybound = name =>
            {
                if (string.IsNullOrEmpty(name))
                    return false;
                return (bool)method.Invoke(null, new object[] { name });
            };

            _successfullyResolved = true;
        }
        catch
        {
            _isKeybound = null;
        }
    }
}
