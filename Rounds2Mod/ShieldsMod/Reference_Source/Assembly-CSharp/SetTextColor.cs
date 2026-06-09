using System;
using TMPro;
using UnityEngine;

// Token: 0x020001B1 RID: 433
public class SetTextColor : MonoBehaviour
{
	// Token: 0x0600089E RID: 2206 RVA: 0x0002D9A3 File Offset: 0x0002BBA3
	public void SetColor(int id)
	{
		base.GetComponent<TextMeshProUGUI>().color = this.colors[id];
	}

	// Token: 0x040009D9 RID: 2521
	public Color[] colors;
}
