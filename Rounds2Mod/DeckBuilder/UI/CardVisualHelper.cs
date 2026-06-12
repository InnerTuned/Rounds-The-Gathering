using UnboundLib;
using UnityEngine;
using UnityEngine.UI;

namespace DeckBuilder.UI;

/// <summary>Instantiates card prefab visuals for menu/search UIs.</summary>
internal static class CardVisualHelper
{
    private static readonly Color UncommonAccent = new Color(0f, 0.5f, 1f, 1f);
    private static readonly Color RareAccent = new Color(1f, 0.2f, 1f, 1f);

    public static void SetupCardVisual(CardInfo cardInfo, GameObject parent,
        float scale = 15f, Vector2? anchoredPosition = null)
    {
        if (cardInfo == null || parent == null)
            return;

        GameObject cardObject = Object.Instantiate(cardInfo.gameObject, parent.transform);
        cardObject.name = "CardVisual";
        cardObject.SetActive(true);

        var back = FindChildByName(cardObject, "Back");
        if (back != null) Object.Destroy(back);

        var damagable = FindChildByName(cardObject, "Damagable");
        if (damagable != null) Object.Destroy(damagable);

        var particles = FindChildByName(cardObject, "UI_ParticleSystem");
        if (particles != null) Object.Destroy(particles);

        var blockFront = FindChildByName(cardObject, "BlockFront");
        if (blockFront != null) blockFront.SetActive(false);

        // Prevent CardVisuals.Start from applying draft-table desaturation (alpha 0.15).
        foreach (var cv in cardObject.GetComponentsInChildren<CardVisuals>(true))
            cv.enabled = false;

        foreach (var anim in cardObject.GetComponentsInChildren<Animator>(true))
            anim.enabled = false;

        foreach (var curveAnim in cardObject.GetComponentsInChildren<CurveAnimation>(true))
            curveAnim.enabled = false;

        var cardRt = cardObject.GetOrAddComponent<RectTransform>();
        cardRt.localScale = Vector3.one * scale;
        cardRt.anchorMin = new Vector2(0.5f, 0.5f);
        cardRt.anchorMax = new Vector2(0.5f, 0.5f);
        cardRt.pivot = new Vector2(0.5f, 0.5f);
        cardRt.anchoredPosition = anchoredPosition ?? new Vector2(0f, 10f);

        foreach (var graphic in cardObject.GetComponentsInChildren<Graphic>(true))
            graphic.raycastTarget = false;

        PlaceCardArt(cardInfo, cardObject);
        FinalizeStaticCardVisual(cardObject, cardInfo);
    }

    /// <summary>
    /// Forces full-brightness menu display — mirrors UnboundLib ToggleCardsMenuHandler behavior.
    /// </summary>
    public static void FinalizeStaticCardVisual(GameObject cardObject, CardInfo cardInfo)
    {
        if (cardObject == null)
            return;

        DisableDarkenOverlay(cardObject);

        foreach (var cg in cardObject.GetComponentsInChildren<CanvasGroup>(true))
            cg.alpha = 1f;

        var grid = FindChildByName(cardObject, "Grid");
        if (grid != null)
        {
            var gridCg = grid.GetComponent<CanvasGroup>();
            if (gridCg != null)
                gridCg.alpha = 1f;
        }

        foreach (var curve in cardObject.GetComponentsInChildren<CurveAnimation>(true))
        {
            curve.enabled = true;
            if (curve.gameObject.activeInHierarchy)
                curve.PlayIn();
        }

        ApplyFrameColors(cardObject, cardInfo);
    }

    private static void PlaceCardArt(CardInfo cardInfo, GameObject cardObject)
    {
        if (cardInfo?.cardArt == null)
            return;

        var artTransform = FindChildByName(cardObject, "Art");
        if (artTransform == null)
            return;

        var clone = Object.Instantiate(cardInfo.cardArt, artTransform.transform);
        clone.transform.SetAsFirstSibling();

        var cloneRt = clone.GetComponent<RectTransform>();
        if (cloneRt != null)
        {
            cloneRt.anchorMin = Vector2.zero;
            cloneRt.anchorMax = Vector2.one;
            cloneRt.offsetMin = Vector2.zero;
            cloneRt.offsetMax = Vector2.zero;
            cloneRt.localPosition = Vector3.zero;
            cloneRt.localScale = Vector3.one;
        }
        else
        {
            clone.transform.localPosition = Vector3.zero;
            clone.transform.localScale = Vector3.one;
        }
    }

    private static void DisableDarkenOverlay(GameObject cardObject)
    {
        var darkenRoot = cardObject.transform.Find("Darken");
        if (darkenRoot != null)
        {
            var inner = darkenRoot.Find("Darken");
            if (inner != null)
                inner.gameObject.SetActive(false);

            darkenRoot.gameObject.SetActive(false);
        }
    }

    private static void ApplyFrameColors(GameObject cardObject, CardInfo cardInfo)
    {
        if (cardInfo == null || CardChoice.instance == null)
            return;

        var front = FindChildByName(cardObject, "Front");
        if (front == null)
            return;

        Color cardColor = CardChoice.instance.GetCardColor(cardInfo.colorTheme);

        foreach (var img in front.GetComponentsInChildren<Image>(true))
        {
            if (img.gameObject.name.Contains("FRAME"))
                img.color = cardColor;
        }

        if (front.transform.childCount > 1)
        {
            var nameText = front.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
            if (nameText != null)
            {
                nameText.text = cardInfo.cardName.ToUpper();
                nameText.color = cardColor;
            }
        }

        if (cardInfo.rarity == CardInfo.Rarity.Common)
            return;

        Color accent = cardInfo.rarity == CardInfo.Rarity.Uncommon ? UncommonAccent : RareAccent;
        foreach (Transform child in front.GetComponentsInChildren<Transform>(true))
        {
            if (child.name != "Triangle")
                continue;

            var img = child.GetComponent<Image>();
            if (img != null)
                img.color = accent;
        }
    }

    internal static GameObject FindChildByName(GameObject parent, string name)
    {
        if (parent == null)
            return null;

        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == name)
                return child.gameObject;
        }

        return null;
    }
}
