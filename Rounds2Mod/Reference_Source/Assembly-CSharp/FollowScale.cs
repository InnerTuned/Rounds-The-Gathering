using System;
using UnityEngine;

// Token: 0x02000059 RID: 89
public class FollowScale : MonoBehaviour
{
	// Token: 0x0600019F RID: 415 RVA: 0x0000AC28 File Offset: 0x00008E28
	private void Update()
	{
		if (this.target)
		{
			base.transform.localScale = this.target.transform.localScale;
		}
	}

	// Token: 0x04000239 RID: 569
	public Transform target;
}
