using System;
using System.Collections.Generic;
using Sonigon;
using Sonigon.Internal;
using UnityEngine;

namespace SoundImplementation
{
	// Token: 0x020001D6 RID: 470
	[Serializable]
	public class SoundGun
	{
		// Token: 0x06000945 RID: 2373 RVA: 0x0002F9C0 File Offset: 0x0002DBC0
		public void SetGun(Gun gun)
		{
			this.parentGun = gun;
		}

		// Token: 0x06000946 RID: 2374 RVA: 0x0002F9C9 File Offset: 0x0002DBC9
		public void SetGunTransform(Transform transform)
		{
			this.gunTransform = transform;
		}

		// Token: 0x06000947 RID: 2375 RVA: 0x0002F9D4 File Offset: 0x0002DBD4
		public void PlayImpact(HitInfo hit, RayHitReflect rayHitReflect)
		{
			bool flag = false;
			if (hit.transform != null)
			{
				if (hit.transform.tag == "Player")
				{
					flag = true;
				}
			}
			else
			{
				SoundManager.Instance.PlayAtPosition(this.soundImpactBullet, SoundManager.Instance.GetTransform(), hit.point, new SoundParameterBase[]
				{
					this.soundParameterVolumeDecibelImpact
				});
			}
			if (rayHitReflect != null && rayHitReflect.reflects > 0 && !flag)
			{
				SoundManager.Instance.PlayAtPosition(this.soundImpactBounce, SoundManager.Instance.GetTransform(), hit.point, new SoundParameterBase[]
				{
					this.soundParameterVolumeDecibelImpact
				});
				return;
			}
			this.PlayImpactModifiers(flag, hit.point);
		}

		// Token: 0x06000948 RID: 2376 RVA: 0x0002FA98 File Offset: 0x0002DC98
		private void PlayImpactModifiers(bool isPlayer, Vector2 position)
		{
			for (int i = 0; i < this.soundImpactModifierCurrentList.Count; i++)
			{
				if (this.soundImpactModifierCurrentList[i] != null)
				{
					if (isPlayer)
					{
						SoundManager.Instance.PlayAtPosition(this.soundImpactModifierCurrentList[i].impactCharacter, SoundManager.Instance.GetTransform(), position, new SoundParameterBase[]
						{
							this.soundParameterVolumeDecibelImpact,
							this.soundDamageToExplosionParameterIntensity
						});
					}
					else
					{
						SoundManager.Instance.PlayAtPosition(this.soundImpactModifierCurrentList[i].impactEnvironment, SoundManager.Instance.GetTransform(), position, new SoundParameterBase[]
						{
							this.soundParameterVolumeDecibelImpact,
							this.soundDamageToExplosionParameterIntensity
						});
					}
				}
			}
		}

		// Token: 0x06000949 RID: 2377 RVA: 0x0002FB63 File Offset: 0x0002DD63
		public void ClearSoundModifiers()
		{
			this.soundShotModifierAllList.Clear();
			this.soundImpactModifierAllList.Clear();
			this.RefreshSoundModifiers();
		}

		// Token: 0x0600094A RID: 2378 RVA: 0x0002FB81 File Offset: 0x0002DD81
		public void AddSoundShotModifier(SoundShotModifier soundShotModifier)
		{
			if (soundShotModifier != null)
			{
				this.soundShotModifierAllList.Insert(0, soundShotModifier);
			}
		}

		// Token: 0x0600094B RID: 2379 RVA: 0x0002FB99 File Offset: 0x0002DD99
		public void AddSoundImpactModifier(SoundImpactModifier soundImpactModifier)
		{
			if (soundImpactModifier != null)
			{
				this.soundImpactModifierAllList.Insert(0, soundImpactModifier);
			}
		}

		// Token: 0x0600094C RID: 2380 RVA: 0x0002FBB4 File Offset: 0x0002DDB4
		public void RefreshSoundModifiers()
		{
			this.StopAutoPlayTail();
			if (this.howManyShotModifiers < 1)
			{
				this.howManyShotModifiers = 1;
			}
			if (this.howManyImpactModifiers < 1)
			{
				this.howManyImpactModifiers = 1;
			}
			int num = 0;
			for (int i = 0; i < this.soundShotModifierAllList.Count; i++)
			{
				if (num < this.soundShotModifierAllList[i].priority)
				{
					num = this.soundShotModifierAllList[i].priority;
				}
			}
			this.soundShotModifierCurrentList.Clear();
			int num2 = 0;
			for (int j = num; j >= 0; j--)
			{
				for (int k = 0; k < this.soundShotModifierAllList.Count; k++)
				{
					if (this.soundShotModifierAllList[k].priority == j && !this.soundShotModifierCurrentList.Contains(this.soundShotModifierAllList[k]))
					{
						this.soundShotModifierCurrentList.Add(this.soundShotModifierAllList[k]);
						num2++;
					}
					if (num2 >= this.howManyShotModifiers)
					{
						break;
					}
				}
				if (num2 >= this.howManyShotModifiers)
				{
					break;
				}
			}
			if (this.soundShotModifierCurrentList.Count == 0)
			{
				this.soundShotModifierCurrentList.Add(this.soundShotModifierBasic);
			}
			if (this.soundShotModifierCurrentList.Count == 1)
			{
				this.soundParameterVolumeDecibelShot.volumeDecibel = 0f;
			}
			else if (this.soundShotModifierCurrentList.Count == 2)
			{
				this.soundParameterVolumeDecibelShot.volumeDecibel = this.shotModifierLowerVolumeDbIf2;
			}
			else if (this.soundShotModifierCurrentList.Count == 3)
			{
				this.soundParameterVolumeDecibelShot.volumeDecibel = this.shotModifierLowerVolumeDbIf3;
			}
			this.soundDamageToExplosionParameterIntensity.intensity = this.parentGun.damage;
			if (this.soundImpactModifierDamageToExplosionMedium != null && this.soundImpactModifierDamageToExplosionHuge != null)
			{
				if (this.parentGun.damage > 2.6f)
				{
					if (this.parentGun.damage > 8f)
					{
						if (!this.soundImpactModifierAllList.Contains(this.soundImpactModifierDamageToExplosionHuge))
						{
							this.soundImpactModifierAllList.Insert(0, this.soundImpactModifierDamageToExplosionHuge);
						}
						this.soundImpactModifierAllList.Remove(this.soundImpactModifierDamageToExplosionMedium);
					}
					else
					{
						if (!this.soundImpactModifierAllList.Contains(this.soundImpactModifierDamageToExplosionMedium))
						{
							this.soundImpactModifierAllList.Insert(0, this.soundImpactModifierDamageToExplosionMedium);
						}
						this.soundImpactModifierAllList.Remove(this.soundImpactModifierDamageToExplosionHuge);
					}
				}
				else
				{
					this.soundImpactModifierAllList.Remove(this.soundImpactModifierDamageToExplosionMedium);
					this.soundImpactModifierAllList.Remove(this.soundImpactModifierDamageToExplosionHuge);
				}
			}
			int num3 = 0;
			for (int l = 0; l < this.soundImpactModifierAllList.Count; l++)
			{
				if (num3 < this.soundImpactModifierAllList[l].priority)
				{
					num3 = this.soundImpactModifierAllList[l].priority;
				}
			}
			this.soundImpactModifierCurrentList.Clear();
			int num4 = 0;
			for (int m = num3; m >= 0; m--)
			{
				for (int n = 0; n < this.soundImpactModifierAllList.Count; n++)
				{
					if (this.soundImpactModifierAllList[n].priority == m && !this.soundImpactModifierCurrentList.Contains(this.soundImpactModifierAllList[n]))
					{
						this.soundImpactModifierCurrentList.Add(this.soundImpactModifierAllList[n]);
						num4++;
					}
					if (num4 >= this.howManyImpactModifiers)
					{
						break;
					}
				}
				if (num4 >= this.howManyImpactModifiers)
				{
					break;
				}
			}
			if (this.soundImpactModifierCurrentList.Count == 0)
			{
				this.soundImpactModifierCurrentList.Add(this.soundImpactModifierBasic);
			}
			if (this.soundImpactModifierCurrentList.Count == 1)
			{
				this.soundParameterVolumeDecibelImpact.volumeDecibel = 0f;
				return;
			}
			if (this.soundImpactModifierCurrentList.Count == 2)
			{
				this.soundParameterVolumeDecibelImpact.volumeDecibel = this.impactModifierLowerVolumeDbIf2;
				return;
			}
			if (this.soundImpactModifierCurrentList.Count == 3)
			{
				this.soundParameterVolumeDecibelImpact.volumeDecibel = this.impactModifierLowerVolumeDbIf3;
			}
		}

		// Token: 0x0600094D RID: 2381 RVA: 0x0002FF8E File Offset: 0x0002E18E
		public void StopAutoPlayTail()
		{
			this.autoFirstShot = true;
			this.StopInternalSingleAutoPlayTail();
			this.StopInternalShotgunAutoPlayTail();
		}

		// Token: 0x0600094E RID: 2382 RVA: 0x0002FFA4 File Offset: 0x0002E1A4
		private void StopInternalSingleAutoPlayTail()
		{
			if (this.singleAutoIsPlaying)
			{
				this.singleAutoIsPlaying = false;
				for (int i = 0; i < this.soundShotModifierCurrentList.Count; i++)
				{
					if (this.soundShotModifierCurrentList[i] != null)
					{
						SoundManager.Instance.Stop(this.soundShotModifierCurrentList[i].singleAutoLoop, this.gunTransform, true);
						SoundManager.Instance.Play(this.soundShotModifierCurrentList[i].singleAutoTail, this.gunTransform, new SoundParameterBase[]
						{
							this.soundParameterVolumeDecibelShot,
							this.autoPitchRatio
						});
					}
				}
			}
		}

		// Token: 0x0600094F RID: 2383 RVA: 0x0003004C File Offset: 0x0002E24C
		private void StopInternalShotgunAutoPlayTail()
		{
			if (this.shotgunAutoIsPlaying)
			{
				this.shotgunAutoIsPlaying = false;
				for (int i = 0; i < this.soundShotModifierCurrentList.Count; i++)
				{
					if (this.soundShotModifierCurrentList[i] != null)
					{
						SoundManager.Instance.Stop(this.soundShotModifierCurrentList[i].shotgunAutoLoop, this.gunTransform, true);
						SoundManager.Instance.Play(this.soundShotModifierCurrentList[i].shotgunAutoTail, this.gunTransform, new SoundParameterBase[]
						{
							this.soundParameterVolumeDecibelShot
						});
					}
				}
			}
		}

		// Token: 0x06000950 RID: 2384 RVA: 0x000300E8 File Offset: 0x0002E2E8
		private void PlaySingle()
		{
			this.StopInternalSingleAutoPlayTail();
			this.StopInternalShotgunAutoPlayTail();
			for (int i = 0; i < this.soundShotModifierCurrentList.Count; i++)
			{
				if (this.soundShotModifierCurrentList[i] != null)
				{
					SoundManager.Instance.Play(this.soundShotModifierCurrentList[i].single, this.gunTransform, new SoundParameterBase[]
					{
						this.soundParameterVolumeDecibelShot
					});
				}
			}
		}

		// Token: 0x06000951 RID: 2385 RVA: 0x0003015C File Offset: 0x0002E35C
		private void PlayShotgun()
		{
			this.StopInternalSingleAutoPlayTail();
			this.StopInternalShotgunAutoPlayTail();
			for (int i = 0; i < this.soundShotModifierCurrentList.Count; i++)
			{
				if (this.soundShotModifierCurrentList[i] != null)
				{
					SoundManager.Instance.Play(this.soundShotModifierCurrentList[i].shotgun, this.gunTransform, new SoundParameterBase[]
					{
						this.soundParameterVolumeDecibelShot
					});
				}
			}
		}

		// Token: 0x06000952 RID: 2386 RVA: 0x000301D0 File Offset: 0x0002E3D0
		private void PlaySingleAuto()
		{
			this.StopInternalShotgunAutoPlayTail();
			if (!this.singleAutoIsPlaying)
			{
				this.singleAutoIsPlaying = true;
				for (int i = 0; i < this.soundShotModifierCurrentList.Count; i++)
				{
					if (this.soundShotModifierCurrentList[i] != null)
					{
						SoundManager.Instance.Play(this.soundShotModifierCurrentList[i].singleAutoLoop, this.gunTransform, new SoundParameterBase[]
						{
							this.soundParameterVolumeDecibelShot,
							this.autoPitchRatio
						});
					}
				}
			}
		}

		// Token: 0x06000953 RID: 2387 RVA: 0x00030258 File Offset: 0x0002E458
		private void PlayShotgunAuto()
		{
			this.StopInternalSingleAutoPlayTail();
			if (!this.shotgunAutoIsPlaying)
			{
				this.shotgunAutoIsPlaying = true;
				for (int i = 0; i < this.soundShotModifierCurrentList.Count; i++)
				{
					if (this.soundShotModifierCurrentList[i] != null)
					{
						SoundManager.Instance.Play(this.soundShotModifierCurrentList[i].shotgunAutoLoop, this.gunTransform, new SoundParameterBase[]
						{
							this.soundParameterVolumeDecibelShot
						});
					}
				}
			}
		}

		// Token: 0x06000954 RID: 2388 RVA: 0x000302D4 File Offset: 0x0002E4D4
		public void PlayShot(int currentNumberOfProjectiles)
		{
			if ((this.parentGun.bursts > 0 && !this.parentGun.useCharge && !this.parentGun.dontAllowAutoFire) || (this.parentGun.attackSpeed / this.parentGun.attackSpeedMultiplier < 0.15f && !this.parentGun.useCharge && !this.parentGun.dontAllowAutoFire))
			{
				if (currentNumberOfProjectiles > 1)
				{
					this.PlayShotgunAuto();
					return;
				}
				if (this.autoFirstShot)
				{
					this.autoFirstShot = false;
					this.autoShotsCalculate = 0;
					this.autoStartTime = Time.realtimeSinceStartup;
				}
				else
				{
					this.autoShotsCalculate++;
					this.autoTimeSinceStart = Time.realtimeSinceStartup - this.autoStartTime;
					this.autoDelayBetweenShotsNew = this.autoTimeSinceStart / (float)this.autoShotsCalculate;
					this.autoPitchRatio.pitchRatio = this.autoDelayBetweenShotsDefault / this.autoDelayBetweenShotsNew;
				}
				if (this.autoPitchRatio.pitchRatio < this.autoPitchThresholdLow)
				{
					if (!this.singleAutoIsPlaying && !this.shotgunAutoIsPlaying)
					{
						this.PlaySingle();
						return;
					}
				}
				else
				{
					if (this.autoPitchRatio.pitchRatio > this.autoPitchThresholdHigh)
					{
						this.PlayShotgunAuto();
						return;
					}
					if (!this.shotgunAutoIsPlaying)
					{
						this.PlaySingleAuto();
						return;
					}
				}
			}
			else
			{
				if (currentNumberOfProjectiles > 1)
				{
					this.PlayShotgun();
					return;
				}
				this.PlaySingle();
			}
		}

		// Token: 0x04000A71 RID: 2673
		private Gun parentGun;

		// Token: 0x04000A72 RID: 2674
		private Transform gunTransform;

		// Token: 0x04000A73 RID: 2675
		[Header("Sound Shot")]
		public int howManyShotModifiers = 3;

		// Token: 0x04000A74 RID: 2676
		[Range(-12f, 0f)]
		public float shotModifierLowerVolumeDbIf2 = -3f;

		// Token: 0x04000A75 RID: 2677
		[Range(-12f, 0f)]
		public float shotModifierLowerVolumeDbIf3 = -6f;

		// Token: 0x04000A76 RID: 2678
		public SoundShotModifier soundShotModifierBasic;

		// Token: 0x04000A77 RID: 2679
		private List<SoundShotModifier> soundShotModifierAllList = new List<SoundShotModifier>();

		// Token: 0x04000A78 RID: 2680
		private List<SoundShotModifier> soundShotModifierCurrentList = new List<SoundShotModifier>();

		// Token: 0x04000A79 RID: 2681
		private bool singleAutoIsPlaying;

		// Token: 0x04000A7A RID: 2682
		private bool shotgunAutoIsPlaying;

		// Token: 0x04000A7B RID: 2683
		[Header("Sound Impact")]
		public int howManyImpactModifiers = 2;

		// Token: 0x04000A7C RID: 2684
		[Range(-12f, 0f)]
		public float impactModifierLowerVolumeDbIf2 = -3f;

		// Token: 0x04000A7D RID: 2685
		[Range(-12f, 0f)]
		public float impactModifierLowerVolumeDbIf3 = -6f;

		// Token: 0x04000A7E RID: 2686
		public SoundImpactModifier soundImpactModifierBasic;

		// Token: 0x04000A7F RID: 2687
		public SoundImpactModifier soundImpactModifierDamageToExplosionMedium;

		// Token: 0x04000A80 RID: 2688
		public SoundImpactModifier soundImpactModifierDamageToExplosionHuge;

		// Token: 0x04000A81 RID: 2689
		private SoundParameterIntensity soundDamageToExplosionParameterIntensity = new SoundParameterIntensity(0f, 1);

		// Token: 0x04000A82 RID: 2690
		public SoundEvent soundImpactBounce;

		// Token: 0x04000A83 RID: 2691
		public SoundEvent soundImpactBullet;

		// Token: 0x04000A84 RID: 2692
		private List<SoundImpactModifier> soundImpactModifierAllList = new List<SoundImpactModifier>();

		// Token: 0x04000A85 RID: 2693
		private List<SoundImpactModifier> soundImpactModifierCurrentList = new List<SoundImpactModifier>();

		// Token: 0x04000A86 RID: 2694
		private SoundParameterVolumeDecibel soundParameterVolumeDecibelShot = new SoundParameterVolumeDecibel(0f, 1);

		// Token: 0x04000A87 RID: 2695
		private SoundParameterVolumeDecibel soundParameterVolumeDecibelImpact = new SoundParameterVolumeDecibel(0f, 1);

		// Token: 0x04000A88 RID: 2696
		private bool autoFirstShot = true;

		// Token: 0x04000A89 RID: 2697
		private float autoStartTime;

		// Token: 0x04000A8A RID: 2698
		private float autoTimeSinceStart;

		// Token: 0x04000A8B RID: 2699
		private int autoShotsCalculate;

		// Token: 0x04000A8C RID: 2700
		private float autoDelayBetweenShotsDefault = 0.05f;

		// Token: 0x04000A8D RID: 2701
		private float autoDelayBetweenShotsNew;

		// Token: 0x04000A8E RID: 2702
		private SoundParameterPitchRatio autoPitchRatio = new SoundParameterPitchRatio(1f, 0);

		// Token: 0x04000A8F RID: 2703
		private float autoPitchThresholdLow = 0.5f;

		// Token: 0x04000A90 RID: 2704
		private float autoPitchThresholdHigh = 3f;
	}
}
