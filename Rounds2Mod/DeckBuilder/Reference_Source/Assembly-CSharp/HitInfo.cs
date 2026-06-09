using System;
using UnityEngine;

// Token: 0x020000A9 RID: 169
public class HitInfo
{
	// Token: 0x060003B7 RID: 951 RVA: 0x00016A44 File Offset: 0x00014C44
	internal static HitInfo GetHitInfo(RaycastHit2D raycastHit2D)
	{
		return new HitInfo
		{
			point = raycastHit2D.point,
			normal = raycastHit2D.normal,
			transform = raycastHit2D.transform,
			collider = raycastHit2D.collider,
			rigidbody = raycastHit2D.rigidbody
		};
	}

	// Token: 0x040004F7 RID: 1271
	public Vector2 point;

	// Token: 0x040004F8 RID: 1272
	public Vector2 normal;

	// Token: 0x040004F9 RID: 1273
	public Collider2D collider;

	// Token: 0x040004FA RID: 1274
	public Transform transform;

	// Token: 0x040004FB RID: 1275
	public Rigidbody2D rigidbody;
}
