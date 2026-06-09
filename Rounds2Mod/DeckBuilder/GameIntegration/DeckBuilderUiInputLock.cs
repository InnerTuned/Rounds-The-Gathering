using UnboundLib;
using UnityEngine;
using DeckBuilder.UI;

namespace DeckBuilder.GameIntegration;

/// <summary>Tells Unbound to set GameManager.lockInput while any DeckBuilder screen is open.</summary>
internal sealed class DeckBuilderUiInputLock : MonoBehaviour
{
    private const string LockKey = "DeckBuilder";

    private void Update()
    {
        Unbound.lockInputBools[LockKey] = IsDeckBuilderUiOpen();
    }

    private void OnDestroy()
    {
        Unbound.lockInputBools.Remove(LockKey);
    }

    private static bool IsDeckBuilderUiOpen() =>
        DeckSelectorScreen.instance?.IsOpen == true
        || CreateDeckScreen.instance?.IsOpen == true
        || DeckEditorScreen.instance?.IsOpen == true;
}
