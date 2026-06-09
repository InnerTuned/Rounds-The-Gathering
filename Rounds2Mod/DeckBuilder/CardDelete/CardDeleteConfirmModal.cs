using System;
using DeckBuilder.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DeckBuilder.CardDelete;

/// <summary>Opaque confirmation dialog shown before deleting a card from the hand.</summary>
internal class CardDeleteConfirmModal : MonoBehaviour
{
    private Canvas _canvas;
    private TextMeshProUGUI _messageText;
    private TextMeshProUGUI _deltaText;
    private Action _onConfirm;

    private void Awake()
    {
        BuildUI();
        Hide();
    }

    private void BuildUI()
    {
        _canvas = UIHelper.CreateFullscreenCanvas("DB_CardDeleteConfirm", sortOrder: 260);

        var backdrop = UIHelper.CreatePanel(_canvas.transform, "Backdrop",
            Vector2.zero, Vector2.one, bg: new Color(0.04f, 0.05f, 0.08f, 1f));
        var backdropImg = backdrop.GetComponent<Image>();
        if (backdropImg != null)
            backdropImg.raycastTarget = true;

        var dialog = UIHelper.CreatePanel(_canvas.transform, "Dialog",
            new Vector2(0.22f, 0.28f), new Vector2(0.78f, 0.72f),
            bg: new Color(0.08f, 0.09f, 0.14f, 1f));

        _messageText = UIHelper.CreateText(dialog, "Message", "",
            fontSize: 24, alignment: TextAlignmentOptions.Center,
            color: new Color(0.92f, 0.94f, 1f));
        var messageRt = _messageText.GetComponent<RectTransform>();
        messageRt.anchorMin = new Vector2(0.06f, 0.62f);
        messageRt.anchorMax = new Vector2(0.94f, 0.94f);
        messageRt.offsetMin = messageRt.offsetMax = Vector2.zero;
        _messageText.enableWordWrapping = true;

        _deltaText = UIHelper.CreateText(dialog, "DeltaPreview", "",
            fontSize: 20, alignment: TextAlignmentOptions.Center,
            color: new Color(0.82f, 0.86f, 0.96f));
        var deltaRt = _deltaText.GetComponent<RectTransform>();
        deltaRt.anchorMin = new Vector2(0.08f, 0.28f);
        deltaRt.anchorMax = new Vector2(0.92f, 0.60f);
        deltaRt.offsetMin = deltaRt.offsetMax = Vector2.zero;
        _deltaText.enableWordWrapping = true;
        _deltaText.richText = true;

        var noBtn = UIHelper.CreateButton(dialog, "NoBtn", "No",
            Vector2.zero, Vector2.zero, fontSize: 22,
            bgColor: new Color(0.25f, 0.25f, 0.30f));
        var noRt = noBtn.GetComponent<RectTransform>();
        noRt.anchorMin = new Vector2(0.10f, 0.08f);
        noRt.anchorMax = new Vector2(0.46f, 0.22f);
        noRt.offsetMin = noRt.offsetMax = Vector2.zero;
        noBtn.onClick.AddListener(Hide);

        var yesBtn = UIHelper.CreateButton(dialog, "YesBtn", "Yes",
            Vector2.zero, Vector2.zero, fontSize: 22,
            bgColor: new Color(0.55f, 0.12f, 0.12f));
        var yesRt = yesBtn.GetComponent<RectTransform>();
        yesRt.anchorMin = new Vector2(0.54f, 0.08f);
        yesRt.anchorMax = new Vector2(0.90f, 0.22f);
        yesRt.offsetMin = yesRt.offsetMax = Vector2.zero;
        yesBtn.onClick.AddListener(Confirm);
    }

    public void Show(Player player, CardInfo card, Action onConfirm)
    {
        if (card == null)
            return;

        _onConfirm = onConfirm;
        _messageText.text = $"Are you sure you want to delete {card.cardName}?";
        _deltaText.text = CardDeleteDeltaBridge.FormatRemovalPreview(player, card);
        _canvas.gameObject.SetActive(true);
        CardDeleteLog.Line($"Delete confirm modal shown for '{card.cardName}'.");
    }

    public void Hide()
    {
        _onConfirm = null;
        if (_canvas != null)
            _canvas.gameObject.SetActive(false);
    }

    public bool IsVisible => _canvas != null && _canvas.gameObject.activeSelf;

    private void Confirm()
    {
        CardDeleteLog.Line("Delete confirm modal — Yes clicked.");
        Action confirm = _onConfirm;
        Hide();
        confirm?.Invoke();
    }

    private void OnDestroy()
    {
        if (_canvas != null)
            Destroy(_canvas.gameObject);
    }
}
