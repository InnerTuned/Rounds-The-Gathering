using System;
using TMPro;
using UnityEngine;

// Token: 0x02000127 RID: 295
public class CrownPos : MonoBehaviour
{
	// Token: 0x060005C2 RID: 1474 RVA: 0x00020B80 File Offset: 0x0001ED80
	public float GetOffset()
	{
		if (!(this.text.text == ""))
		{
			return 0.45f;
		}
		return 0f;
	}

	// Token: 0x04000768 RID: 1896
	public TextMeshProUGUI text;
}
