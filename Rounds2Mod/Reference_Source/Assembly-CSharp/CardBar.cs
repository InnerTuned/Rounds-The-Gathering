using System;
using Sonigon;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000105 RID: 261
public class CardBar : MonoBehaviour
{
	// Token: 0x0600051E RID: 1310 RVA: 0x0001D18D File Offset: 0x0001B38D
	private void Start()
	{
		this.source = base.transform.GetChild(0).gameObject;
	}

	// Token: 0x0600051F RID: 1311 RVA: 0x0001D1A8 File Offset: 0x0001B3A8
	public void ClearBar()
	{
		if (this.currentCard)
		{
			Object.Destroy(this.currentCard);
		}
		for (int i = base.transform.childCount - 1; i >= 0; i--)
		{
			if (base.transform.GetChild(i).gameObject.activeSelf)
			{
				Object.Destroy(base.transform.GetChild(i).gameObject);
			}
		}
	}

	// Token: 0x06000520 RID: 1312 RVA: 0x0001D214 File Offset: 0x0001B414
	public void AddCard(CardInfo card)
	{
		SoundManager.Instance.Play(this.soundCardPick, base.transform);
		this.ci = card;
		GameObject gameObject = Object.Instantiate<GameObject>(this.source, this.source.transform.position, this.source.transform.rotation, this.source.transform.parent);
		gameObject.transform.localScale = Vector3.one;
		string text = card.cardName;
		text = text.Substring(0, 2);
		string text2 = text.get_Chars(0).ToString().ToUpper();
		if (text.Length > 1)
		{
			string text3 = text.get_Chars(1).ToString().ToLower();
			text = text2 + text3;
		}
		else
		{
			text = text2;
		}
		gameObject.GetComponentInChildren<TextMeshProUGUI>().text = text;
		gameObject.GetComponent<CardBarButton>().card = card;
		gameObject.gameObject.SetActive(true);
	}

	// Token: 0x06000521 RID: 1313 RVA: 0x0001D2FC File Offset: 0x0001B4FC
	public void OnHover(CardInfo card, Vector3 hoverPos)
	{
		if (this.currentCard)
		{
			Object.Destroy(this.currentCard);
		}
		this.currentCard = CardChoice.instance.AddCardVisual(card, this.cardPos.transform.position);
		Collider2D[] componentsInChildren = this.currentCard.transform.root.GetComponentsInChildren<Collider2D>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = false;
		}
		this.currentCard.GetComponentInChildren<Canvas>().sortingLayerName = "MostFront";
		this.currentCard.GetComponentInChildren<GraphicRaycaster>().enabled = false;
		this.currentCard.GetComponentInChildren<SetScaleToZero>().enabled = false;
		this.currentCard.GetComponentInChildren<SetScaleToZero>().transform.localScale = Vector3.one * 1.15f;
	}

	// Token: 0x06000522 RID: 1314 RVA: 0x0001D3CA File Offset: 0x0001B5CA
	public void StopHover()
	{
		if (this.currentCard)
		{
			Object.Destroy(this.currentCard);
		}
	}

	// Token: 0x06000523 RID: 1315 RVA: 0x000027C8 File Offset: 0x000009C8
	private void Update()
	{
	}

	// Token: 0x040006B6 RID: 1718
	[Header("Sounds")]
	public SoundEvent soundCardPick;

	// Token: 0x040006B7 RID: 1719
	[Header("Settings")]
	public GameObject pointer;

	// Token: 0x040006B8 RID: 1720
	[Header("Settings")]
	public GameObject cardPos;

	// Token: 0x040006B9 RID: 1721
	private GameObject source;

	// Token: 0x040006BA RID: 1722
	private CardInfo ci;

	// Token: 0x040006BB RID: 1723
	private GameObject currentCard;
}
