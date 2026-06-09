using System;
using UnityEngine;

// Token: 0x020000B2 RID: 178
public class RayHitBash : RayHitEffect
{
	// Token: 0x060003DE RID: 990 RVA: 0x00017B19 File Offset: 0x00015D19
	private void Start()
	{
		this.move = base.GetComponentInParent<MoveTransform>();
		this.multiplier = base.transform.localScale.x;
	}

	// Token: 0x060003DF RID: 991 RVA: 0x00017B40 File Offset: 0x00015D40
	public override HasToReturn DoHitEffect(HitInfo hit)
	{
		if (!hit.transform)
		{
			return HasToReturn.canContinue;
		}
		StunHandler component = hit.transform.GetComponent<StunHandler>();
		if (component)
		{
			ProjectileHit componentInParent = base.GetComponentInParent<ProjectileHit>();
			float num = 25f;
			if (componentInParent)
			{
				num = componentInParent.damage;
			}
			float num2 = this.triggerChancePerTenDamage * num * 0.1f;
			num2 += this.baseTriggerChance;
			if (Random.value < num2)
			{
				float num3 = this.baseStunTime + this.stunTimePerTenDamage * num * 0.1f;
				this.SetMultiplier();
				num3 *= this.stunMultiplier;
				num3 = Mathf.Pow(num3, this.stunTimeExponent);
				num3 *= this.multiplier;
				if (this.cannotPermaStun)
				{
					num3 = Mathf.Clamp(num3, 0f, base.GetComponentInParent<SpawnedAttack>().spawner.data.weaponHandler.gun.attackSpeed * base.GetComponentInParent<SpawnedAttack>().spawner.data.stats.attackSpeedMultiplier + 0.3f);
				}
				if (num3 > this.stunTimeThreshold)
				{
					component.AddStun(num3);
				}
			}
		}
		return HasToReturn.canContinue;
	}

	// Token: 0x060003E0 RID: 992 RVA: 0x00017C60 File Offset: 0x00015E60
	private void SetMultiplier()
	{
		float distanceTravelled = this.move.distanceTravelled;
		if (this.multiplierPerTenMeterTravelled != 0f)
		{
			this.stunMultiplier = distanceTravelled * this.multiplierPerTenMeterTravelled * 0.1f;
		}
	}

	// Token: 0x0400053C RID: 1340
	public float triggerChancePerTenDamage = 0.1f;

	// Token: 0x0400053D RID: 1341
	public float baseTriggerChance = 0.2f;

	// Token: 0x0400053E RID: 1342
	[Space(15f)]
	public float stunMultiplier = 1f;

	// Token: 0x0400053F RID: 1343
	public float stunTimePerTenDamage = 0.1f;

	// Token: 0x04000540 RID: 1344
	public float baseStunTime = 1f;

	// Token: 0x04000541 RID: 1345
	public bool cannotPermaStun;

	// Token: 0x04000542 RID: 1346
	[Space(15f)]
	public float stunTimeThreshold = 0.2f;

	// Token: 0x04000543 RID: 1347
	public float stunTimeExponent = 1f;

	// Token: 0x04000544 RID: 1348
	public float multiplierPerTenMeterTravelled;

	// Token: 0x04000545 RID: 1349
	private MoveTransform move;

	// Token: 0x04000546 RID: 1350
	private float multiplier = 1f;
}
