using System;
using UnityEngine;

// Token: 0x0200005A RID: 90
public class FollowTransform : MonoBehaviour
{
	// Token: 0x060001A1 RID: 417 RVA: 0x0000AC52 File Offset: 0x00008E52
	private void Start()
	{
		this.spawnOffset = base.transform.position - this.target.position;
	}

	// Token: 0x060001A2 RID: 418 RVA: 0x0000AC75 File Offset: 0x00008E75
	private void LateUpdate()
	{
		if (this.target)
		{
			base.transform.position = this.target.position + this.spawnOffset + this.offset;
		}
	}

	// Token: 0x0400023A RID: 570
	public Transform target;

	// Token: 0x0400023B RID: 571
	private Vector3 spawnOffset;

	// Token: 0x0400023C RID: 572
	public Vector3 offset;
}
