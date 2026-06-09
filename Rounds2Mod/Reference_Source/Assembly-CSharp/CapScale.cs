using System;
using UnityEngine;

// Token: 0x02000103 RID: 259
public class CapScale : MonoBehaviour
{
	// Token: 0x0600051B RID: 1307 RVA: 0x0001D0FC File Offset: 0x0001B2FC
	private void Start()
	{
		if (base.transform.localScale.x < this.min)
		{
			base.transform.localScale = Vector3.one * this.min;
		}
		if (base.transform.localScale.x > this.max)
		{
			base.transform.localScale = Vector3.one * this.max;
		}
	}

	// Token: 0x040006B4 RID: 1716
	public float min = 2f;

	// Token: 0x040006B5 RID: 1717
	public float max = 10f;
}
