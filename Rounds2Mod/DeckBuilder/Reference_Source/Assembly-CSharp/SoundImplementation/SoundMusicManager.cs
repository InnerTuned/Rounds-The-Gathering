using System;
using Sonigon;
using Sonigon.Internal;
using UnityEngine;

namespace SoundImplementation
{
	// Token: 0x020001DC RID: 476
	public class SoundMusicManager : MonoBehaviour
	{
		// Token: 0x0600095E RID: 2398 RVA: 0x00030844 File Offset: 0x0002EA44
		public void PlayMainMenu()
		{
			this.StopAmbience();
			if (!this.musicMainMenuPlaying)
			{
				this.musicMainMenuPlaying = true;
				if (SoundVolumeManager.Instance.mainMenuFirstTime)
				{
					SoundVolumeManager.Instance.mainMenuFirstTime = false;
					SoundManager.Instance.PlayMusic(this.musicMainMenu, true, true, new SoundParameterBase[]
					{
						new SoundParameterFadeInLength(0f, 1)
					});
				}
				else
				{
					SoundManager.Instance.PlayMusic(this.musicMainMenu, true, true);
				}
				this.musicIngamePlaying = false;
				if (this.debugMusicPlay)
				{
					global::Debug.LogWarning("MUSIC: PlayMainMenu");
				}
			}
		}

		// Token: 0x0600095F RID: 2399 RVA: 0x000308D0 File Offset: 0x0002EAD0
		public void PlayIngame(bool isCard)
		{
			this.PlayAmbience();
			if (isCard)
			{
				this.musicIntensityIngame.intensity = 0f;
				if (this.debugMusicPlay)
				{
					global::Debug.LogWarning("MUSIC: Ingame Instensity 0");
				}
			}
			else
			{
				this.musicIntensityIngame.intensity = 1f;
				if (this.debugMusicPlay)
				{
					global::Debug.LogWarning("MUSIC: Ingame Instensity 1");
				}
			}
			if (!this.musicIngamePlaying)
			{
				this.musicIngamePlaying = true;
				SoundManager.Instance.PlayMusic(this.musicIngame, true, true, new SoundParameterBase[]
				{
					this.musicIntensityIngame
				});
				this.musicMainMenuPlaying = false;
				if (this.debugMusicPlay)
				{
					global::Debug.LogWarning("MUSIC: PlayIngame");
				}
			}
		}

		// Token: 0x06000960 RID: 2400 RVA: 0x00030975 File Offset: 0x0002EB75
		public void StopAllMusic()
		{
			SoundManager.Instance.StopAllMusic(true);
			this.musicMainMenuPlaying = false;
			this.musicIngamePlaying = false;
		}

		// Token: 0x06000961 RID: 2401 RVA: 0x00030990 File Offset: 0x0002EB90
		private void PlayAmbience()
		{
			if (!this.ambiencePlaying)
			{
				this.ambiencePlaying = true;
				SoundManager.Instance.Play(this.ambience, base.transform);
			}
		}

		// Token: 0x06000962 RID: 2402 RVA: 0x000309B7 File Offset: 0x0002EBB7
		private void StopAmbience()
		{
			if (this.ambiencePlaying)
			{
				this.ambiencePlaying = false;
				SoundManager.Instance.Stop(this.ambience, base.transform, true);
			}
		}

		// Token: 0x06000963 RID: 2403 RVA: 0x000309DF File Offset: 0x0002EBDF
		private void Start()
		{
			if (!this.musicFirstPlay)
			{
				this.musicFirstPlay = true;
				this.PlayMainMenu();
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000964 RID: 2404 RVA: 0x000309F6 File Offset: 0x0002EBF6
		// (set) Token: 0x06000965 RID: 2405 RVA: 0x000309FD File Offset: 0x0002EBFD
		public static SoundMusicManager Instance { get; private set; }

		// Token: 0x06000966 RID: 2406 RVA: 0x00030A05 File Offset: 0x0002EC05
		private void Awake()
		{
			this.InstanceCheck();
			if (this.useDontDestroyOnLoad && !this.isGoingToDelete)
			{
				Object.DontDestroyOnLoad(base.gameObject);
			}
		}

		// Token: 0x06000967 RID: 2407 RVA: 0x00030A28 File Offset: 0x0002EC28
		private void InstanceCheck()
		{
			if (SoundMusicManager.Instance == null)
			{
				SoundMusicManager.Instance = this;
				return;
			}
			if (SoundMusicManager.Instance != this)
			{
				this.isGoingToDelete = true;
				if (Application.isPlaying)
				{
					Object.Destroy(base.gameObject);
				}
			}
		}

		// Token: 0x04000AAD RID: 2733
		private bool isGoingToDelete;

		// Token: 0x04000AAE RID: 2734
		private bool useDontDestroyOnLoad;

		// Token: 0x04000AAF RID: 2735
		private bool debugMusicPlay;

		// Token: 0x04000AB0 RID: 2736
		[Header("Music, press Pause to toggle the volume of the music")]
		public SoundEvent musicMainMenu;

		// Token: 0x04000AB1 RID: 2737
		public SoundEvent musicIngame;

		// Token: 0x04000AB2 RID: 2738
		private bool musicFirstPlay;

		// Token: 0x04000AB3 RID: 2739
		private bool musicMainMenuPlaying;

		// Token: 0x04000AB4 RID: 2740
		private bool musicIngamePlaying;

		// Token: 0x04000AB5 RID: 2741
		private SoundParameterIntensity musicIntensityIngame = new SoundParameterIntensity(1f, 0);

		// Token: 0x04000AB6 RID: 2742
		public SoundEvent ambience;

		// Token: 0x04000AB7 RID: 2743
		private bool ambiencePlaying;
	}
}
