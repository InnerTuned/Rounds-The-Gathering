using System;
using Sonigon;
using Sonigon.Internal;
using UnityEngine;

// Token: 0x020001C2 RID: 450
public class TrickShot : MonoBehaviour
{
	// Token: 0x060008E2 RID: 2274 RVA: 0x0002EBC7 File Offset: 0x0002CDC7
	private void Awake()
	{
		this.trail = base.transform.root.GetComponentInChildren<ScaleTrailFromDamage>();
	}

	// Token: 0x060008E3 RID: 2275 RVA: 0x0002EBE0 File Offset: 0x0002CDE0
	private void Start()
	{
		this.projectileHit = base.GetComponentInParent<ProjectileHit>();
		this.move = base.GetComponentInParent<MoveTransform>();
		if (this.projectileHit != null)
		{
			if (this.soundGrowExplosion != null)
			{
				this.projectileHit.AddHitActionWithData(new Action<HitInfo>(this.SoundPlayGrowExplosion));
			}
			if (this.soundGrowWail != null)
			{
				this.soundGrowWailPlayed = true;
				SoundManager.Instance.Play(this.soundGrowWail, this.projectileHit.ownPlayer.transform);
			}
		}
	}

	// Token: 0x060008E4 RID: 2276 RVA: 0x0002EC70 File Offset: 0x0002CE70
	public void SoundPlayGrowExplosion(HitInfo hit)
	{
		if (!this.soundGrowExplosionPlayed)
		{
			this.soundGrowExplosionPlayed = true;
			if (this.soundGrowExplosion != null)
			{
				SoundManager.Instance.PlayAtPosition(this.soundGrowExplosion, this.projectileHit.ownPlayer.transform, hit.point, new SoundParameterBase[]
				{
					this.soundIntensity
				});
			}
			if (this.soundGrowWailPlayed)
			{
				SoundManager.Instance.Stop(this.soundGrowWail, this.projectileHit.ownPlayer.transform, true);
			}
		}
	}

	// Token: 0x060008E5 RID: 2277 RVA: 0x0002ED00 File Offset: 0x0002CF00
	private void Update()
	{
		if (this.move.distanceTravelled > this.removeAt)
		{
			Object.Destroy(this);
			return;
		}
		this.soundIntensity.intensity = this.move.distanceTravelled / this.removeAt;
		float num = this.move.distanceTravelled - this.lastDistanceTravelled;
		this.lastDistanceTravelled = this.move.distanceTravelled;
		float num2 = 1f + num * TimeHandler.deltaTime * base.transform.localScale.x * this.muiltiplier;
		this.projectileHit.damage *= num2;
		this.projectileHit.shake *= num2;
		if (this.trail)
		{
			this.trail.Rescale();
		}
	}

	// Token: 0x04000A2C RID: 2604
	[Header("Sound")]
	public SoundEvent soundGrowExplosion;

	// Token: 0x04000A2D RID: 2605
	public SoundEvent soundGrowWail;

	// Token: 0x04000A2E RID: 2606
	private bool soundGrowExplosionPlayed;

	// Token: 0x04000A2F RID: 2607
	private bool soundGrowWailPlayed;

	// Token: 0x04000A30 RID: 2608
	private SoundParameterIntensity soundIntensity = new SoundParameterIntensity(0f, 1);

	// Token: 0x04000A31 RID: 2609
	[Header("Settings")]
	public float muiltiplier = 1f;

	// Token: 0x04000A32 RID: 2610
	public float removeAt = 30f;

	// Token: 0x04000A33 RID: 2611
	private ProjectileHit projectileHit;

	// Token: 0x04000A34 RID: 2612
	private MoveTransform move;

	// Token: 0x04000A35 RID: 2613
	private ScaleTrailFromDamage trail;

	// Token: 0x04000A36 RID: 2614
	private float lastDistanceTravelled;
}
