using System;
using UnityEngine;

// Token: 0x020001CD RID: 461
public class UnparentObject : MonoBehaviour
{
	// Token: 0x06000921 RID: 2337 RVA: 0x0002F7B6 File Offset: 0x0002D9B6
	public void Unparent()
	{
		base.transform.SetParent(base.transform.root);
	}
}
