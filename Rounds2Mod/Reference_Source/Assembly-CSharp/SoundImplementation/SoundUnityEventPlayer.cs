using System;
using Sonigon;
using UnityEngine;

namespace SoundImplementation
{
	// Token: 0x020001E1 RID: 481
	public class SoundUnityEventPlayer : MonoBehaviour
	{
		// Token: 0x0600097C RID: 2428 RVA: 0x00030CBA File Offset: 0x0002EEBA
		private void Initialize()
		{
			if (!this.initialized)
			{
				this.initialized = true;
				this.transformPosition = base.transform;
			}
		}

		// Token: 0x0600097D RID: 2429 RVA: 0x00030CD7 File Offset: 0x0002EED7
		private Transform GetTransformInstanceID()
		{
			if (this.useLocalTransformForInstanceID)
			{
				return this.transformPosition;
			}
			return SoundManager.Instance.GetTransform();
		}

		// Token: 0x0600097E RID: 2430 RVA: 0x00030CF2 File Offset: 0x0002EEF2
		private void Start()
		{
			if (this.triggerStartOnStart)
			{
				this.PlayStart();
			}
		}

		// Token: 0x0600097F RID: 2431 RVA: 0x00030D02 File Offset: 0x0002EF02
		private void OnEnable()
		{
			if (this.triggerStartOnEnable)
			{
				this.PlayStart();
			}
		}

		// Token: 0x06000980 RID: 2432 RVA: 0x00030D14 File Offset: 0x0002EF14
		public void PlayStart()
		{
			this.Initialize();
			if (this.soundStart != null)
			{
				SoundManager.Instance.PlayAtPosition(this.soundStart, this.GetTransformInstanceID(), this.transformPosition);
			}
			if (this.soundStartLoop != null)
			{
				this.soundStartLoopPlaying = true;
				SoundManager.Instance.PlayAtPosition(this.soundStartLoop, this.GetTransformInstanceID(), this.transformPosition);
			}
		}

		// Token: 0x06000981 RID: 2433 RVA: 0x00030D82 File Offset: 0x0002EF82
		private void OnDestroy()
		{
			if (this.triggerEndOnDestroy)
			{
				this.PlayEnd();
			}
		}

		// Token: 0x06000982 RID: 2434 RVA: 0x00030D92 File Offset: 0x0002EF92
		private void OnDisable()
		{
			if (this.triggerEndOnDisable)
			{
				this.PlayEnd();
			}
		}

		// Token: 0x06000983 RID: 2435 RVA: 0x00030DA4 File Offset: 0x0002EFA4
		public void PlayEnd()
		{
			this.Initialize();
			if (this.soundEnd != null)
			{
				SoundManager.Instance.PlayAtPosition(this.soundEnd, this.GetTransformInstanceID(), this.transformPosition);
			}
			if (this.soundStartLoopPlaying && this.soundStartLoop != null)
			{
				this.soundStartLoopPlaying = false;
				SoundManager.Instance.StopAtPosition(this.soundStartLoop, this.transformPosition, true);
			}
		}

		// Token: 0x04000ACF RID: 2767
		public bool triggerStartOnStart;

		// Token: 0x04000AD0 RID: 2768
		public bool triggerStartOnEnable;

		// Token: 0x04000AD1 RID: 2769
		public bool triggerEndOnDestroy;

		// Token: 0x04000AD2 RID: 2770
		public bool triggerEndOnDisable;

		// Token: 0x04000AD3 RID: 2771
		public bool useLocalTransformForInstanceID;

		// Token: 0x04000AD4 RID: 2772
		public SoundEvent soundStart;

		// Token: 0x04000AD5 RID: 2773
		public SoundEvent soundStartLoop;

		// Token: 0x04000AD6 RID: 2774
		public SoundEvent soundEnd;

		// Token: 0x04000AD7 RID: 2775
		private bool soundStartLoopPlaying;

		// Token: 0x04000AD8 RID: 2776
		private bool initialized;

		// Token: 0x04000AD9 RID: 2777
		private Transform transformPosition;
	}
}
