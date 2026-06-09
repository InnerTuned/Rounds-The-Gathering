using System;
using UnityEngine;

// Token: 0x020000AC RID: 172
public abstract class ProjectileHitSurface : MonoBehaviour
{
	// Token: 0x060003CA RID: 970
	public abstract ProjectileHitSurface.HasToStop HitSurface(HitInfo hit, GameObject projectile);

	// Token: 0x02000375 RID: 885
	public enum HasToStop
	{
		// Token: 0x040011A0 RID: 4512
		HasToStop,
		// Token: 0x040011A1 RID: 4513
		CanKeepGoing
	}
}
