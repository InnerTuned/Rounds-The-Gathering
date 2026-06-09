using System;
using UnityEngine;

// Token: 0x020000B7 RID: 183
public class RayHitReflect : RayHitEffect
{
	// Token: 0x060003EA RID: 1002 RVA: 0x00017F47 File Offset: 0x00016147
	private void Start()
	{
		this.move = base.GetComponent<MoveTransform>();
		this.projHit = base.GetComponent<ProjectileHit>();
	}

	// Token: 0x060003EB RID: 1003 RVA: 0x00017F64 File Offset: 0x00016164
	public override HasToReturn DoHitEffect(HitInfo hit)
	{
		if (hit.transform && hit.transform.GetComponent<Player>())
		{
			this.reflects -= 10;
		}
		if (this.reflects <= 0)
		{
			return HasToReturn.canContinue;
		}
		if (this.reflectAction != null)
		{
			this.reflectAction.Invoke(hit);
		}
		this.move.velocity = Vector2.Reflect(this.move.velocity, hit.normal);
		this.move.velocity *= this.speedM;
		this.projHit.damage *= this.dmgM;
		this.projHit.shake *= this.dmgM;
		if (this.dmgM > 1f)
		{
			float num = this.dmgM - 1f;
			num *= 0.6f;
			this.dmgM = num + 1f;
			ScaleTrailFromDamage componentInChildren = base.GetComponentInChildren<ScaleTrailFromDamage>();
			if (componentInChildren)
			{
				componentInChildren.Rescale();
			}
		}
		this.timeOfBounce = Time.time;
		base.transform.position = hit.point + this.move.velocity.normalized * 0.1f;
		this.reflects--;
		return HasToReturn.hasToReturn;
	}

	// Token: 0x04000558 RID: 1368
	private MoveTransform move;

	// Token: 0x04000559 RID: 1369
	private ProjectileHit projHit;

	// Token: 0x0400055A RID: 1370
	public int reflects = 1;

	// Token: 0x0400055B RID: 1371
	public float speedM = 1f;

	// Token: 0x0400055C RID: 1372
	public float dmgM = 1f;

	// Token: 0x0400055D RID: 1373
	[HideInInspector]
	public float timeOfBounce = -10000f;

	// Token: 0x0400055E RID: 1374
	public Action<HitInfo> reflectAction;
}
