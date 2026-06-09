using System;
using Sonigon;
using Sonigon.Internal;
using UnityEngine;

// Token: 0x020000B3 RID: 179
public class RayHitBulletSound : RayHitEffect
{
	// Token: 0x060003E2 RID: 994 RVA: 0x00017D07 File Offset: 0x00015F07
	private void Start()
	{
		this.projectileHit = base.GetComponent<ProjectileHit>();
		this.rayHitReflect = base.GetComponent<RayHitReflect>();
		this.moveTransform = base.GetComponent<MoveTransform>();
	}

	// Token: 0x060003E3 RID: 995 RVA: 0x00017D30 File Offset: 0x00015F30
	public override HasToReturn DoHitEffect(HitInfo hit)
	{
		if (this.disableImpact)
		{
			return HasToReturn.canContinue;
		}
		if (this.localImpactVelocityToIntensity)
		{
			this.soundIntensity.intensity = this.moveTransform.velocity.magnitude;
		}
		if (this.playLocalImpact)
		{
			if (this.soundLocalImpact != null && hit.collider != null && hit.collider.tag != "Player")
			{
				if (this.localImpactVelocityToIntensity)
				{
					SoundManager.Instance.PlayAtPosition(this.soundLocalImpact, SoundManager.Instance.GetTransform(), hit.point, new SoundParameterBase[]
					{
						this.soundIntensity
					});
				}
				else
				{
					SoundManager.Instance.PlayAtPosition(this.soundLocalImpact, SoundManager.Instance.GetTransform(), hit.point);
				}
			}
		}
		else
		{
			this.projectileHit.ownPlayer.data.weaponHandler.gun.soundGun.PlayImpact(hit, this.rayHitReflect);
		}
		return HasToReturn.canContinue;
	}

	// Token: 0x04000547 RID: 1351
	[Header("Sound Settings")]
	public bool disableImpact;

	// Token: 0x04000548 RID: 1352
	public bool playLocalImpact;

	// Token: 0x04000549 RID: 1353
	public SoundEvent soundLocalImpact;

	// Token: 0x0400054A RID: 1354
	public bool localImpactVelocityToIntensity;

	// Token: 0x0400054B RID: 1355
	private ProjectileHit projectileHit;

	// Token: 0x0400054C RID: 1356
	private RayHitReflect rayHitReflect;

	// Token: 0x0400054D RID: 1357
	private MoveTransform moveTransform;

	// Token: 0x0400054E RID: 1358
	private SoundParameterIntensity soundIntensity = new SoundParameterIntensity(1f, 1);
}
