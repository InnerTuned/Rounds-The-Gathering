using System;
using UnityEngine;

// Token: 0x020000FC RID: 252
public class ApplyRootScale : MonoBehaviour
{
	// Token: 0x06000509 RID: 1289 RVA: 0x0001CDD4 File Offset: 0x0001AFD4
	private void Start()
	{
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, base.transform.root.localScale, this.amount);
	}

	// Token: 0x040006A6 RID: 1702
	public float amount = 0.5f;
}
