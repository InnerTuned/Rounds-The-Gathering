using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000027 RID: 39
public class CardInfoDisplayer : MonoBehaviour
{
	// Token: 0x060000B8 RID: 184 RVA: 0x00005E44 File Offset: 0x00004044
	public void DrawCard(CardInfoStat[] stats, string cardName, string description = "", Sprite image = null, bool charge = false)
	{
		if (charge)
		{
			this.chargeObj.SetActive(true);
			this.chargeObj.transform.SetParent(this.grid.transform, true);
		}
		if (description != "")
		{
			this.effectText.text = description;
			this.effectText.gameObject.SetActive(true);
			this.effectText.transform.SetParent(this.grid.transform, true);
		}
		this.nameText.text = cardName.ToUpper();
		for (int i = 0; i < stats.Length; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.statObject, this.grid.transform.position, this.grid.transform.rotation, this.grid.transform);
			gameObject.SetActive(true);
			gameObject.transform.localScale = Vector3.one;
			TextMeshProUGUI component = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
			TextMeshProUGUI component2 = gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
			component.text = stats[i].stat;
			if (stats[i].simepleAmount != CardInfoStat.SimpleAmount.notAssigned && !Optionshandler.showCardStatNumbers)
			{
				component2.text = stats[i].GetSimpleAmount();
			}
			else
			{
				component2.text = stats[i].amount;
			}
			component2.color = (stats[i].positive ? this.positiveColor : this.negativeColor);
		}
		if (image)
		{
			this.icon.sprite = image;
		}
		this.effectText.transform.position += Vector3.up * 0.3f;
	}

	// Token: 0x040000B2 RID: 178
	public Color negativeColor;

	// Token: 0x040000B3 RID: 179
	public Color positiveColor;

	// Token: 0x040000B4 RID: 180
	public GameObject statObject;

	// Token: 0x040000B5 RID: 181
	public GameObject grid;

	// Token: 0x040000B6 RID: 182
	public GameObject chargeObj;

	// Token: 0x040000B7 RID: 183
	public TextMeshProUGUI effectText;

	// Token: 0x040000B8 RID: 184
	public TextMeshProUGUI nameText;

	// Token: 0x040000B9 RID: 185
	public Image icon;
}
