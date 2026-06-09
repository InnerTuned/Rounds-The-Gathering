using System;
using Sonigon;
using UnityEngine;

namespace SoundImplementation
{
	// Token: 0x020001DD RID: 477
	public class SoundPlayerStatic : MonoBehaviour
	{
		// Token: 0x06000969 RID: 2409 RVA: 0x00030A7D File Offset: 0x0002EC7D
		public void PlayButtonHover()
		{
			SoundManager.Instance.Play(this.soundButtonHover, base.transform);
		}

		// Token: 0x0600096A RID: 2410 RVA: 0x00030A95 File Offset: 0x0002EC95
		public void PlayButtonClick()
		{
			SoundManager.Instance.Play(this.soundButtonClick, base.transform);
		}

		// Token: 0x0600096B RID: 2411 RVA: 0x00030AAD File Offset: 0x0002ECAD
		public void PlayMatchFound()
		{
			SoundManager.Instance.Play(this.soundMatchFound, base.transform);
		}

		// Token: 0x0600096C RID: 2412 RVA: 0x00030AC5 File Offset: 0x0002ECC5
		public void PlayPlayerAdded()
		{
			SoundManager.Instance.Play(this.soundPlayerAdded, base.transform);
		}

		// Token: 0x0600096D RID: 2413 RVA: 0x00030ADD File Offset: 0x0002ECDD
		public void PlayPlayerBallAppear()
		{
			SoundManager.Instance.Play(this.soundPlayerBallAppear, base.transform);
		}

		// Token: 0x0600096E RID: 2414 RVA: 0x00030AF5 File Offset: 0x0002ECF5
		public void PlayPlayerBallDisappear()
		{
			SoundManager.Instance.Play(this.soundPlayerBallDisappear, base.transform);
		}

		// Token: 0x0600096F RID: 2415 RVA: 0x00030B0D File Offset: 0x0002ED0D
		public void PlayLevelTransitionIn()
		{
			SoundManager.Instance.Play(this.soundLevelTransitionIn, base.transform);
		}

		// Token: 0x06000970 RID: 2416 RVA: 0x00030B25 File Offset: 0x0002ED25
		public void PlayLevelTransitionOut()
		{
			SoundManager.Instance.Play(this.soundLevelTransitionOut, base.transform);
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000971 RID: 2417 RVA: 0x00030B3D File Offset: 0x0002ED3D
		// (set) Token: 0x06000972 RID: 2418 RVA: 0x00030B44 File Offset: 0x0002ED44
		public static SoundPlayerStatic Instance { get; private set; }

		// Token: 0x06000973 RID: 2419 RVA: 0x00030B4C File Offset: 0x0002ED4C
		private void Awake()
		{
			this.InstanceCheck();
			if (this.useDontDestroyOnLoad && !this.isGoingToDelete)
			{
				Object.DontDestroyOnLoad(base.gameObject);
			}
		}

		// Token: 0x06000974 RID: 2420 RVA: 0x00030B6F File Offset: 0x0002ED6F
		private void InstanceCheck()
		{
			if (SoundPlayerStatic.Instance == null)
			{
				SoundPlayerStatic.Instance = this;
				return;
			}
			if (SoundPlayerStatic.Instance != this)
			{
				this.isGoingToDelete = true;
				if (Application.isPlaying)
				{
					Object.Destroy(base.gameObject);
				}
			}
		}

		// Token: 0x04000AB9 RID: 2745
		[SerializeField]
		private SoundEvent soundButtonHover;

		// Token: 0x04000ABA RID: 2746
		[SerializeField]
		private SoundEvent soundButtonClick;

		// Token: 0x04000ABB RID: 2747
		[SerializeField]
		private SoundEvent soundMatchFound;

		// Token: 0x04000ABC RID: 2748
		[SerializeField]
		private SoundEvent soundPlayerAdded;

		// Token: 0x04000ABD RID: 2749
		[SerializeField]
		private SoundEvent soundPlayerBallAppear;

		// Token: 0x04000ABE RID: 2750
		[SerializeField]
		private SoundEvent soundPlayerBallDisappear;

		// Token: 0x04000ABF RID: 2751
		[SerializeField]
		private SoundEvent soundLevelTransitionIn;

		// Token: 0x04000AC0 RID: 2752
		[SerializeField]
		private SoundEvent soundLevelTransitionOut;

		// Token: 0x04000AC1 RID: 2753
		private bool isGoingToDelete;

		// Token: 0x04000AC2 RID: 2754
		private bool useDontDestroyOnLoad = true;

		// Token: 0x04000AC3 RID: 2755
		private bool debugInstanceDestroyed = true;
	}
}
