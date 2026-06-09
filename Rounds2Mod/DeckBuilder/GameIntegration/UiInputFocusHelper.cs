using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DeckBuilder.GameIntegration;

internal static class UiInputFocusHelper
{
    internal static bool IsTextInputFocused()
    {
        EventSystem es = EventSystem.current;
        if (es == null || es.currentSelectedGameObject == null)
            return false;

        GameObject selected = es.currentSelectedGameObject;
        return selected.GetComponent<TMP_InputField>() != null
            || selected.GetComponentInParent<TMP_InputField>() != null
            || selected.GetComponent<InputField>() != null
            || selected.GetComponentInParent<InputField>() != null;
    }
}
