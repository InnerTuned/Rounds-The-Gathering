using System;
using UnityEngine;

// Token: 0x020001B6 RID: 438
public class SoundAudioListenerPosition : MonoBehaviour
{
	// Token: 0x060008BB RID: 2235 RVA: 0x0002DFA4 File Offset: 0x0002C1A4
	private void Awake()
	{
		this.audioListener = Object.FindObjectOfType<AudioListener>();
		if (this.audioListener == null)
		{
			global::Debug.LogError("No AudioListener Found");
		}
		else
		{
			this.audioListenerTransform = this.audioListener.transform;
		}
		this.cachedTransform = base.transform;
	}

	// Token: 0x060008BC RID: 2236 RVA: 0x0002DFF4 File Offset: 0x0002C1F4
	private void Update()
	{
		if (this.audioListenerTransform != null)
		{
			this.postion = this.cachedTransform.position;
			this.postion.z = this.positionZaxis;
			this.audioListenerTransform.position = this.postion;
		}
	}

	// Token: 0x040009F5 RID: 2549
	private float positionZaxis = -50f;

	// Token: 0x040009F6 RID: 2550
	private AudioListener audioListener;

	// Token: 0x040009F7 RID: 2551
	private Transform audioListenerTransform;

	// Token: 0x040009F8 RID: 2552
	private Transform cachedTransform;

	// Token: 0x040009F9 RID: 2553
	private Vector3 postion;
}
