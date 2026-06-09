using System;
using UnityEngine;

// Token: 0x0200000F RID: 15
public class Background : MonoBehaviour
{
	// Token: 0x0600004B RID: 75 RVA: 0x0000400C File Offset: 0x0000220C
	private void Init()
	{
		this.parts = base.GetComponentsInChildren<ParticleSystem>();
		this.hasBeenInitiated = true;
	}

	// Token: 0x0600004C RID: 76 RVA: 0x00004024 File Offset: 0x00002224
	public void ToggleBackground(bool on)
	{
		if (!this.hasBeenInitiated)
		{
			this.Init();
		}
		for (int i = 0; i < this.parts.Length; i++)
		{
			if (on)
			{
				this.parts[i].Play();
			}
			else
			{
				this.parts[i].Stop();
			}
		}
	}

	// Token: 0x04000037 RID: 55
	private ParticleSystem[] parts;

	// Token: 0x04000038 RID: 56
	private bool hasBeenInitiated;
}
