using System;
using Sonigon;
using Sonigon.Internal;
using UnityEngine;

namespace SoundImplementation
{
	// Token: 0x020001D7 RID: 471
	public class SoundHierarchyPlay : MonoBehaviour
	{
		// Token: 0x06000956 RID: 2390 RVA: 0x0003050D File Offset: 0x0002E70D
		private Transform GetCurrentInstanceIDTransform()
		{
			if (this.soundHierarchySpawn.soundPolyGrouping == SoundPolyGrouping.global)
			{
				return SoundManager.Instance.GetTransform();
			}
			return this.instanceIDTransform;
		}

		// Token: 0x06000957 RID: 2391 RVA: 0x00030530 File Offset: 0x0002E730
		private void FixedUpdate()
		{
			if (this.soundHierarchyDepth == SoundHierarchyDepth.bulletParent && this.soundHierarchySpawn.soundParentVelocityToIntensity && this.bulletHasTriggered && this.soundHierarchySpawn != null)
			{
				this.soundParameterIntensityParentContinious.intensity = Vector3.Distance(this.lastPosition, this.transformPosition.position);
				this.lastPosition = this.transformPosition.position;
			}
		}

		// Token: 0x06000958 RID: 2392 RVA: 0x00030595 File Offset: 0x0002E795
		private void OnDisable()
		{
			if (this.bulletHasTriggered && this.stopOnDisablePlaying)
			{
				this.stopOnDisablePlaying = false;
				SoundManager.Instance.StopAtPosition(this.soundHierarchySpawn.soundChildChildStopOnDisable, this.transformPosition, true);
			}
		}

		// Token: 0x06000959 RID: 2393 RVA: 0x000305CC File Offset: 0x0002E7CC
		public void PlayBullet()
		{
			this.transformPosition = base.transform;
			this.bulletHasTriggered = true;
			this.lastPosition = this.transformPosition.position;
			if (this.soundHierarchyDepth == SoundHierarchyDepth.bulletParent)
			{
				if (this.soundHierarchySpawn.soundParent != null)
				{
					if (this.soundHierarchySpawn.soundParentVelocityToIntensity)
					{
						SoundManager.Instance.PlayAtPosition(this.soundHierarchySpawn.soundParent, this.GetCurrentInstanceIDTransform(), this.transformPosition, new SoundParameterBase[]
						{
							this.soundParameterIntensityParentContinious
						});
					}
					else
					{
						SoundManager.Instance.PlayAtPosition(this.soundHierarchySpawn.soundParent, this.GetCurrentInstanceIDTransform(), this.transformPosition);
					}
				}
				if (this.soundHierarchySpawn.soundParentStopOnDisable != null)
				{
					this.stopOnDisablePlaying = true;
					SoundManager.Instance.PlayAtPosition(this.soundHierarchySpawn.soundParentStopOnDisable, this.GetCurrentInstanceIDTransform(), this.transformPosition);
					return;
				}
			}
			else if (this.soundHierarchyDepth == SoundHierarchyDepth.bulletChild && this.soundHierarchySpawn.soundChild != null)
			{
				if (this.soundHierarchySpawn.soundChildVelocityToIntensity)
				{
					SoundManager.Instance.PlayAtPosition(this.soundHierarchySpawn.soundChild, this.GetCurrentInstanceIDTransform(), this.transformPosition, new SoundParameterBase[]
					{
						this.soundParameterIntensityChildOnce
					});
					return;
				}
				SoundManager.Instance.PlayAtPosition(this.soundHierarchySpawn.soundChild, this.GetCurrentInstanceIDTransform(), this.transformPosition);
				return;
			}
			else if (this.soundHierarchyDepth == SoundHierarchyDepth.bulletChildChild)
			{
				if (this.soundHierarchySpawn.soundChildChild != null)
				{
					SoundManager.Instance.PlayAtPosition(this.soundHierarchySpawn.soundChildChild, this.GetCurrentInstanceIDTransform(), this.transformPosition);
				}
				if (this.soundHierarchySpawn.soundChildChildStopOnDisable != null)
				{
					this.stopOnDisablePlaying = true;
					SoundManager.Instance.PlayAtPosition(this.soundHierarchySpawn.soundChildChildStopOnDisable, this.GetCurrentInstanceIDTransform(), this.transformPosition);
				}
			}
		}

		// Token: 0x04000A91 RID: 2705
		[NonSerialized]
		public Transform instanceIDTransform;

		// Token: 0x04000A92 RID: 2706
		[NonSerialized]
		public SoundHierarchySpawn soundHierarchySpawn;

		// Token: 0x04000A93 RID: 2707
		[NonSerialized]
		public SoundHierarchyDepth soundHierarchyDepth;

		// Token: 0x04000A94 RID: 2708
		private SoundParameterIntensity soundParameterIntensityParentContinious = new SoundParameterIntensity(1f, 0);

		// Token: 0x04000A95 RID: 2709
		private SoundParameterIntensity soundParameterIntensityChildOnce = new SoundParameterIntensity(0f, 1);

		// Token: 0x04000A96 RID: 2710
		private Vector3 lastPosition;

		// Token: 0x04000A97 RID: 2711
		private Transform transformPosition;

		// Token: 0x04000A98 RID: 2712
		private bool bulletHasTriggered;

		// Token: 0x04000A99 RID: 2713
		private bool stopOnDisablePlaying;
	}
}
