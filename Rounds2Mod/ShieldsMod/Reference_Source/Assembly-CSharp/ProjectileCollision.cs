using System;
using UnityEngine;

// Token: 0x020000A8 RID: 168
public class ProjectileCollision : ProjectileHitSurface
{
	// Token: 0x060003B2 RID: 946 RVA: 0x000167B8 File Offset: 0x000149B8
	private void Start()
	{
		this.rpc = base.GetComponentInParent<ChildRPC>();
		this.rpc.childRPCs.Add("KillBullet", new Action(this.Die));
		this.reflect = base.GetComponentInParent<RayHitReflect>();
		ProjectileHit componentInParent = base.GetComponentInParent<ProjectileHit>();
		if (this.scaleWithDMG)
		{
			base.transform.localScale *= (componentInParent.damage / 55f + 1f) * 0.5f;
			this.health = componentInParent.damage;
		}
		this.startDMG = componentInParent.damage;
		this.deathThreshold = this.health * 0.1f;
	}

	// Token: 0x060003B3 RID: 947 RVA: 0x00016868 File Offset: 0x00014A68
	public override ProjectileHitSurface.HasToStop HitSurface(HitInfo hit, GameObject projectile)
	{
		if (Vector2.Angle(base.transform.root.forward, projectile.transform.forward) < 45f)
		{
			return ProjectileHitSurface.HasToStop.HasToStop;
		}
		if (this.reflect && this.reflect.timeOfBounce + 0.5f > Time.time)
		{
			return ProjectileHitSurface.HasToStop.HasToStop;
		}
		ProjectileCollision componentInChildren = projectile.GetComponentInChildren<ProjectileCollision>();
		if (componentInChildren)
		{
			this.reflect = componentInChildren.GetComponentInParent<RayHitReflect>();
			if (this.reflect && this.reflect.timeOfBounce + 0.5f > Time.time)
			{
				return ProjectileHitSurface.HasToStop.HasToStop;
			}
			float dmg = this.health;
			float dmg2 = componentInChildren.health;
			componentInChildren.TakeDamage(dmg);
			this.TakeDamage(dmg2);
		}
		return ProjectileHitSurface.HasToStop.HasToStop;
	}

	// Token: 0x060003B4 RID: 948 RVA: 0x00016930 File Offset: 0x00014B30
	public void TakeDamage(float dmg)
	{
		if (this.hasCollided)
		{
			return;
		}
		this.health -= dmg;
		if (!this.rpc)
		{
			return;
		}
		if (this.health < this.deathThreshold)
		{
			this.rpc.CallFunction("KillBullet");
		}
	}

	// Token: 0x060003B5 RID: 949 RVA: 0x00016980 File Offset: 0x00014B80
	public void Die()
	{
		if (this.hasCollided)
		{
			return;
		}
		this.hasCollided = true;
		RaycastHit2D raycastHit2D = default(RaycastHit2D);
		raycastHit2D.normal = -base.transform.root.forward;
		raycastHit2D.point = base.transform.position;
		Object.Instantiate<GameObject>(this.sparkObject, base.transform.position, base.transform.rotation).transform.localScale = Vector3.one * ((this.startDMG / 55f + 1f) * 0.5f);
		base.GetComponentInParent<ProjectileHit>().Hit(HitInfo.GetHitInfo(raycastHit2D), true);
	}

	// Token: 0x040004EF RID: 1263
	public bool scaleWithDMG;

	// Token: 0x040004F0 RID: 1264
	public float health;

	// Token: 0x040004F1 RID: 1265
	private float deathThreshold;

	// Token: 0x040004F2 RID: 1266
	private RayHitReflect reflect;

	// Token: 0x040004F3 RID: 1267
	private ChildRPC rpc;

	// Token: 0x040004F4 RID: 1268
	private float startDMG;

	// Token: 0x040004F5 RID: 1269
	private bool hasCollided;

	// Token: 0x040004F6 RID: 1270
	public GameObject sparkObject;
}
