using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001BA RID: 442
public class StatsAfterDealingDamage : MonoBehaviour
{
	// Token: 0x060008C5 RID: 2245 RVA: 0x0002E190 File Offset: 0x0002C390
	private void Start()
	{
		this.data = base.GetComponentInParent<CharacterData>();
	}

	// Token: 0x060008C6 RID: 2246 RVA: 0x0002E1A0 File Offset: 0x0002C3A0
	private void Update()
	{
		bool flag = this.data.stats.sinceDealtDamage < this.duration;
		if (this.isOn != flag)
		{
			this.isOn = flag;
			Vector3 localScale = base.transform.localScale;
			if (this.isOn)
			{
				this.data.health *= this.hpMultiplier;
				this.data.maxHealth *= this.hpMultiplier;
				this.data.stats.movementSpeed *= this.movementSpeedMultiplier;
				this.data.stats.jump *= this.jumpMultiplier;
				this.data.stats.ConfigureMassAndSize();
				this.startEvent.Invoke();
				return;
			}
			this.data.health /= this.hpMultiplier;
			this.data.maxHealth /= this.hpMultiplier;
			this.data.stats.movementSpeed /= this.movementSpeedMultiplier;
			this.data.stats.jump /= this.jumpMultiplier;
			this.data.stats.ConfigureMassAndSize();
			this.endEvent.Invoke();
		}
	}

	// Token: 0x060008C7 RID: 2247 RVA: 0x0002E2FC File Offset: 0x0002C4FC
	public void Interupt()
	{
		if (this.isOn)
		{
			this.data.health /= this.hpMultiplier;
			this.data.maxHealth /= this.hpMultiplier;
			this.data.stats.movementSpeed /= this.movementSpeedMultiplier;
			this.data.stats.jump /= this.jumpMultiplier;
			this.data.stats.ConfigureMassAndSize();
			this.endEvent.Invoke();
			this.isOn = false;
		}
	}

	// Token: 0x04000A00 RID: 2560
	public float duration = 3f;

	// Token: 0x04000A01 RID: 2561
	public float movementSpeedMultiplier = 1f;

	// Token: 0x04000A02 RID: 2562
	public float jumpMultiplier = 1f;

	// Token: 0x04000A03 RID: 2563
	public float hpMultiplier = 1f;

	// Token: 0x04000A04 RID: 2564
	public UnityEvent startEvent;

	// Token: 0x04000A05 RID: 2565
	public UnityEvent endEvent;

	// Token: 0x04000A06 RID: 2566
	private bool isOn;

	// Token: 0x04000A07 RID: 2567
	private CharacterData data;
}
