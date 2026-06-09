using System;
using UnityEngine;

// Token: 0x0200006E RID: 110
public class HoldingObject : MonoBehaviour
{
	// Token: 0x0600023B RID: 571 RVA: 0x000027C8 File Offset: 0x000009C8
	private void Start()
	{
	}

	// Token: 0x0600023C RID: 572 RVA: 0x0000E330 File Offset: 0x0000C530
	private void Update()
	{
		if (CardChoice.instance.IsPicking)
		{
			base.transform.position = Vector3.up * 10000f;
			return;
		}
		if (Vector3.Distance(this.holder.transform.position, base.transform.position) > 100f)
		{
			base.transform.position = this.holder.transform.position;
		}
	}

	// Token: 0x04000321 RID: 801
	public Holding holder;
}
