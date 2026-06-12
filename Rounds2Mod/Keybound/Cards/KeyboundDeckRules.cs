using Keybound.Core;

namespace Keybound.Cards;

/// <summary>Public API for DeckBuilder to query keybound card rules.</summary>
public static class KeyboundDeckRules
{
    public static bool IsKeybound(string cardName) => KeyboundCardRegistry.IsKeybound(cardName);
}
