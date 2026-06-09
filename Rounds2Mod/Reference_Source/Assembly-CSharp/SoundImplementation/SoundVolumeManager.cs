using System;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundImplementation
{
	// Token: 0x020001E2 RID: 482
	public class SoundVolumeManager : MonoBehaviour
	{
		// Token: 0x06000985 RID: 2437 RVA: 0x00030E18 File Offset: 0x0002F018
		public void SetAudioMixerVolumes(float masterVol, float musicVol, float sfxVol)
		{
			masterVol = (1f - masterVol) * this.mixerLowestVolDb;
			musicVol = (1f - musicVol) * this.mixerLowestVolDb;
			sfxVol = (1f - sfxVol) * this.mixerLowestVolDb;
			if (masterVol <= this.mixerLowestVolDb)
			{
				masterVol = -80f;
			}
			else
			{
				masterVol += this.masterVolOffsetDb;
			}
			if (musicVol <= this.mixerLowestVolDb)
			{
				musicVol = -80f;
			}
			else
			{
				musicVol += this.musVolOffsetDb;
			}
			if (sfxVol <= this.mixerLowestVolDb)
			{
				sfxVol = -80f;
			}
			else
			{
				sfxVol += this.sfxVolOffsetDb;
			}
			this.audioMixer.SetFloat(this.masterName, masterVol);
			this.audioMixer.SetFloat(this.musName, musicVol);
			this.audioMixer.SetFloat(this.sfxName, sfxVol);
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000986 RID: 2438 RVA: 0x00030EE2 File Offset: 0x0002F0E2
		// (set) Token: 0x06000987 RID: 2439 RVA: 0x00030EE9 File Offset: 0x0002F0E9
		public static SoundVolumeManager Instance { get; private set; }

		// Token: 0x06000988 RID: 2440 RVA: 0x00030EF1 File Offset: 0x0002F0F1
		private void Awake()
		{
			this.InstanceCheck();
			if (this.useDontDestroyOnLoad && !this.isGoingToDelete)
			{
				Object.DontDestroyOnLoad(base.gameObject);
			}
		}

		// Token: 0x06000989 RID: 2441 RVA: 0x00030F14 File Offset: 0x0002F114
		private void InstanceCheck()
		{
			if (SoundVolumeManager.Instance == null)
			{
				SoundVolumeManager.Instance = this;
				return;
			}
			if (SoundVolumeManager.Instance != this)
			{
				this.isGoingToDelete = true;
				if (Application.isPlaying)
				{
					Object.Destroy(base.gameObject);
				}
			}
		}

		// Token: 0x04000ADA RID: 2778
		public bool mainMenuFirstTime = true;

		// Token: 0x04000ADB RID: 2779
		private bool isGoingToDelete;

		// Token: 0x04000ADC RID: 2780
		private bool useDontDestroyOnLoad = true;

		// Token: 0x04000ADD RID: 2781
		private bool debugInstanceDestroyed = true;

		// Token: 0x04000ADE RID: 2782
		public AudioMixer audioMixer;

		// Token: 0x04000ADF RID: 2783
		private float mixerLowestVolDb = -60f;

		// Token: 0x04000AE0 RID: 2784
		private string masterName = "MasterPrivateVol";

		// Token: 0x04000AE1 RID: 2785
		private float masterVolOffsetDb = 16f;

		// Token: 0x04000AE2 RID: 2786
		private string musName = "MUSVol";

		// Token: 0x04000AE3 RID: 2787
		private float musVolOffsetDb = 10f;

		// Token: 0x04000AE4 RID: 2788
		private string sfxName = "SFXVol";

		// Token: 0x04000AE5 RID: 2789
		private float sfxVolOffsetDb = 10f;
	}
}
