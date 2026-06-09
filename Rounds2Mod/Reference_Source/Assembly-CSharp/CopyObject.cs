using System;
using UnityEngine;

// Token: 0x02000033 RID: 51
public class CopyObject : MonoBehaviour
{
	// Token: 0x06000104 RID: 260 RVA: 0x00007A93 File Offset: 0x00005C93
	public void CopySelf()
	{
		Object.Instantiate<GameObject>(base.gameObject, base.transform.position, base.transform.rotation, base.transform.parent);
	}
}
