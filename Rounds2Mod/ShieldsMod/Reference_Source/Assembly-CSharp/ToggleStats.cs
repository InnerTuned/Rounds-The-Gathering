using System;
using UnityEngine;

// Token: 0x020001BE RID: 446
public class ToggleStats : MonoBehaviour
{
	// Token: 0x060008D2 RID: 2258 RVA: 0x0002E65A File Offset: 0x0002C85A
	private void Start()
	{
		this.data = base.GetComponentInParent<CharacterData>();
	}

	// Token: 0x060008D3 RID: 2259 RVA: 0x0002E668 File Offset: 0x0002C868
	public void TurnOn()
	{
		this.data.health *= this.hpMultiplier;
		this.data.maxHealth *= this.hpMultiplier;
		this.data.stats.movementSpeed *= this.movementSpeedMultiplier;
		this.data.stats.ConfigureMassAndSize();
	}

	// Token: 0x060008D4 RID: 2260 RVA: 0x0002E6D4 File Offset: 0x0002C8D4
	public void TurnOff()
	{
		this.data.health /= this.hpMultiplier;
		this.data.maxHealth /= this.hpMultiplier;
		this.data.stats.movementSpeed /= this.movementSpeedMultiplier;
		this.data.stats.ConfigureMassAndSize();
	}

	// Token: 0x04000A16 RID: 2582
	public float movementSpeedMultiplier = 1f;

	// Token: 0x04000A17 RID: 2583
	public float hpMultiplier = 1f;

	// Token: 0x04000A18 RID: 2584
	private CharacterData data;
}
