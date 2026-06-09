using System;
using UnityEngine;

// Token: 0x020000AE RID: 174
public class RaycastForward : MonoBehaviour
{
	// Token: 0x060003CE RID: 974 RVA: 0x00017703 File Offset: 0x00015903
	private void LateUpdate()
	{
		this.hit = Physics2D.Raycast(base.transform.position, base.transform.forward, this.distance, this.mask);
	}

	// Token: 0x0400052D RID: 1325
	public LayerMask mask;

	// Token: 0x0400052E RID: 1326
	public float distance = 100f;

	// Token: 0x0400052F RID: 1327
	public RaycastHit2D hit;
}
