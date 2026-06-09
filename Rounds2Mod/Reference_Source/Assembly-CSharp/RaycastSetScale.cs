using System;
using UnityEngine;

// Token: 0x020000AF RID: 175
public class RaycastSetScale : MonoBehaviour
{
	// Token: 0x060003D0 RID: 976 RVA: 0x00017754 File Offset: 0x00015954
	private void Start()
	{
		this.raycast = base.GetComponent<RaycastForward>();
	}

	// Token: 0x060003D1 RID: 977 RVA: 0x00017764 File Offset: 0x00015964
	private void LateUpdate()
	{
		if (this.raycast.hit.transform)
		{
			base.transform.localScale = new Vector3(base.transform.localScale.x, base.transform.localScale.y, this.raycast.hit.distance);
			return;
		}
		base.transform.localScale = new Vector3(base.transform.localScale.x, base.transform.localScale.y, this.raycast.distance);
	}

	// Token: 0x04000530 RID: 1328
	private RaycastForward raycast;
}
