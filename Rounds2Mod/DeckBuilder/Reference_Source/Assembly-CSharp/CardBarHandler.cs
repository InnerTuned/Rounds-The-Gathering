using System;
using UnityEngine;

// Token: 0x02000107 RID: 263
public class CardBarHandler : MonoBehaviour
{
	// Token: 0x06000528 RID: 1320 RVA: 0x0001D443 File Offset: 0x0001B643
	private void Start()
	{
		CardBarHandler.instance = this;
		this.cardBars = base.GetComponentsInChildren<CardBar>();
	}

	// Token: 0x06000529 RID: 1321 RVA: 0x0001D457 File Offset: 0x0001B657
	public void AddCard(int teamId, CardInfo card)
	{
		this.cardBars[teamId].AddCard(card);
	}

	// Token: 0x0600052A RID: 1322 RVA: 0x0001D468 File Offset: 0x0001B668
	public void ResetCardBards()
	{
		for (int i = 0; i < this.cardBars.Length; i++)
		{
			this.cardBars[i].ClearBar();
		}
	}

	// Token: 0x040006BD RID: 1725
	private CardBar[] cardBars;

	// Token: 0x040006BE RID: 1726
	public static CardBarHandler instance;
}
