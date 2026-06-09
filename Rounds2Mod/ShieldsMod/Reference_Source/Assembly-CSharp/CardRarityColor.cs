using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000109 RID: 265
public class CardRarityColor : MonoBehaviour
{
	// Token: 0x06000535 RID: 1333 RVA: 0x0001DC0B File Offset: 0x0001BE0B
	private void Awake()
	{
		CardVisuals componentInParent = base.GetComponentInParent<CardVisuals>();
		componentInParent.toggleSelectionAction = (Action<bool>)Delegate.Combine(componentInParent.toggleSelectionAction, new Action<bool>(this.Toggle));
	}

	// Token: 0x06000536 RID: 1334 RVA: 0x0001DC34 File Offset: 0x0001BE34
	public void Toggle(bool isOn)
	{
		CardInfo componentInParent = base.GetComponentInParent<CardInfo>();
		if (componentInParent.rarity == CardInfo.Rarity.Uncommon)
		{
			base.GetComponent<Image>().color = (isOn ? this.uncommonColor : this.uncommonColorOff);
		}
		if (componentInParent.rarity == CardInfo.Rarity.Rare)
		{
			base.GetComponent<Image>().color = (isOn ? this.rareColor : this.rareColorOff);
		}
	}

	// Token: 0x040006D1 RID: 1745
	public Color uncommonColor;

	// Token: 0x040006D2 RID: 1746
	public Color rareColor;

	// Token: 0x040006D3 RID: 1747
	public Color uncommonColorOff;

	// Token: 0x040006D4 RID: 1748
	public Color rareColorOff;
}
