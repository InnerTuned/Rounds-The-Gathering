using System;
using UnityEngine;

// Token: 0x02000099 RID: 153
public class PlayerInRangeSlow : MonoBehaviour
{
	// Token: 0x06000358 RID: 856 RVA: 0x00014B6A File Offset: 0x00012D6A
	private void Start()
	{
		this.trigger = base.GetComponent<PlayerInRangeTrigger>();
		this.level = base.GetComponent<AttackLevel>();
	}

	// Token: 0x06000359 RID: 857 RVA: 0x00014B84 File Offset: 0x00012D84
	private void Update()
	{
		if (this.trigger.inRange)
		{
			this.trigger.target.data.stats.AddSlowAddative(this.slowAmount * (float)this.level.attackLevel, this.maxSlow + ((float)this.level.attackLevel - 1f) * 0.25f, false);
		}
	}

	// Token: 0x0400047F RID: 1151
	public float slowAmount = 0.1f;

	// Token: 0x04000480 RID: 1152
	public float maxSlow = 0.5f;

	// Token: 0x04000481 RID: 1153
	private PlayerInRangeTrigger trigger;

	// Token: 0x04000482 RID: 1154
	private AttackLevel level;
}
