using System;
using UnityEngine;

// Token: 0x020000AD RID: 173
public class ProjectileHitSurfaceShield : ProjectileHitSurface
{
	// Token: 0x060003CC RID: 972 RVA: 0x000176D7 File Offset: 0x000158D7
	public override ProjectileHitSurface.HasToStop HitSurface(HitInfo hit, GameObject projectile)
	{
		if (Vector3.Angle(base.transform.parent.forward, projectile.transform.forward) < 90f)
		{
			return ProjectileHitSurface.HasToStop.HasToStop;
		}
		return ProjectileHitSurface.HasToStop.CanKeepGoing;
	}
}
