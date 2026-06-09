using System;
using UnityEngine;

// Token: 0x020000B8 RID: 184
public class RegenerateAfterStandStill : MonoBehaviour
{
	// Token: 0x060003ED RID: 1005 RVA: 0x000180F8 File Offset: 0x000162F8
	private void Start()
	{
		this.trigger = base.GetComponent<PlayerInRangeTrigger>();
		this.data = base.GetComponentInParent<CharacterData>();
	}

	// Token: 0x060003EE RID: 1006 RVA: 0x00018112 File Offset: 0x00016312
	private void Update()
	{
		if (this.trigger.inRange)
		{
			this.data.healthHandler.Heal(this.heal);
		}
	}

	// Token: 0x0400055F RID: 1375
	private CharacterData data;

	// Token: 0x04000560 RID: 1376
	public float heal = 2f;

	// Token: 0x04000561 RID: 1377
	private PlayerInRangeTrigger trigger;
}
