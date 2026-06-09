using System;
using Sonigon;
using UnityEngine;

// Token: 0x020001BB RID: 443
public class StatsWhenFullHP : MonoBehaviour
{
	// Token: 0x060008C9 RID: 2249 RVA: 0x0002E3D4 File Offset: 0x0002C5D4
	private void Start()
	{
		this.data = base.GetComponentInParent<CharacterData>();
	}

	// Token: 0x060008CA RID: 2250 RVA: 0x0002E3E4 File Offset: 0x0002C5E4
	private void Update()
	{
		bool flag = this.data.health / this.data.maxHealth >= this.healthThreshold;
		if (flag != this.isOn)
		{
			this.isOn = flag;
			if (this.isOn)
			{
				if (this.playSound)
				{
					SoundManager.Instance.PlayAtPosition(this.soundPristineGrow, SoundManager.Instance.GetTransform(), base.transform);
				}
				this.data.health *= this.healthMultiplier;
				this.data.maxHealth *= this.healthMultiplier;
				this.data.stats.sizeMultiplier *= this.sizeMultiplier;
				this.data.stats.ConfigureMassAndSize();
				return;
			}
			if (this.playSound)
			{
				SoundManager.Instance.PlayAtPosition(this.soundPristineShrink, SoundManager.Instance.GetTransform(), base.transform);
			}
			this.data.health /= this.healthMultiplier;
			this.data.maxHealth /= this.healthMultiplier;
			this.data.stats.sizeMultiplier /= this.sizeMultiplier;
			this.data.stats.ConfigureMassAndSize();
		}
	}

	// Token: 0x04000A08 RID: 2568
	public bool playSound;

	// Token: 0x04000A09 RID: 2569
	public SoundEvent soundPristineGrow;

	// Token: 0x04000A0A RID: 2570
	public SoundEvent soundPristineShrink;

	// Token: 0x04000A0B RID: 2571
	public float healthMultiplier = 1f;

	// Token: 0x04000A0C RID: 2572
	public float sizeMultiplier = 1f;

	// Token: 0x04000A0D RID: 2573
	public float healthThreshold = 0.95f;

	// Token: 0x04000A0E RID: 2574
	private CharacterData data;

	// Token: 0x04000A0F RID: 2575
	private bool isOn;
}
