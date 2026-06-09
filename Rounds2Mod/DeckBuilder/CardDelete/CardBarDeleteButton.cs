using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DeckBuilder.CardDelete;

public class CardBarDeleteButton : MonoBehaviour,
    IPointerClickHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    private static readonly FieldInfo s_cardField =
        AccessTools.Field(typeof(CardBarButton), "card");

    private Image icon;
    private Color originalColor;

    private void Awake()
    {
        icon = GetComponentInChildren<Image>();
        if (icon != null)
            originalColor = icon.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!CardDeleteManager.isPickPhase || !CardDeleteManager.IsLocalPlayerTurn())
            return;

        if (icon != null)
            icon.color = new Color(1f, 0.28f, 0.28f, originalColor.a);

        var card = GetCardInfo();
        if (card != null)
            CardDeleteLog.Line($"Hover (delete mode): '{card.cardName}' on '{gameObject.name}'");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (icon != null)
            icon.color = originalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CardDeleteLog.Section("Card bar click");

        if (!CardDeleteManager.isPickPhase)
        {
            CardDeleteLog.Line("Ignored: not in pick phase.");
            return;
        }

        if (!CardDeleteManager.IsLocalPlayerTurn())
        {
            CardDeleteLog.Line("Ignored: not local player's turn.");
            return;
        }

        if (GetComponent<CardBarButton>() == null)
        {
            CardDeleteLog.Warn("Ignored: no CardBarButton on this object.");
            return;
        }

        CardInfo card = GetCardInfo();
        if (card == null)
        {
            CardDeleteLog.Warn("Ignored: CardBarButton.card is null.");
            return;
        }

        CardDeleteLog.Line($"Valid delete click on '{card.cardName}' (objectName='{card.name}').");
        CardDeleteManager.instance?.RequestDelete(card);
    }

    private CardInfo GetCardInfo()
    {
        var btn = GetComponent<CardBarButton>();
        return btn == null ? null : s_cardField?.GetValue(btn) as CardInfo;
    }

    private void OnDestroy()
    {
        if (icon != null)
            icon.color = originalColor;
    }
}
