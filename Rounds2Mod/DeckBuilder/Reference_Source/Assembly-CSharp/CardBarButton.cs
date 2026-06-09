using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000106 RID: 262
public class CardBarButton : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x06000525 RID: 1317 RVA: 0x0001D3E4 File Offset: 0x0001B5E4
	public void OnPointerEnter(PointerEventData eventData)
	{
		base.GetComponentInParent<CardBar>().OnHover(this.card, base.transform.position);
		base.transform.localScale = Vector3.one * 1.1f;
	}

	// Token: 0x06000526 RID: 1318 RVA: 0x0001D41C File Offset: 0x0001B61C
	public void OnPointerExit(PointerEventData eventData)
	{
		base.GetComponentInParent<CardBar>().StopHover();
		base.transform.localScale = Vector3.one * 1f;
	}

	// Token: 0x040006BC RID: 1724
	internal CardInfo card;
}
