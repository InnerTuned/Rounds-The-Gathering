using System;
using UnityEngine;

// Token: 0x020000CB RID: 203
public class SetLocalScale : MonoBehaviour
{
	// Token: 0x06000437 RID: 1079 RVA: 0x00019A4C File Offset: 0x00017C4C
	public void Set()
	{
		base.transform.localScale = this.localScale;
	}

	// Token: 0x040005C2 RID: 1474
	public Vector3 localScale = Vector3.one;
}
