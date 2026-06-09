using System;
using UnityEngine.Events;

// Token: 0x02000051 RID: 81
[Serializable]
public class DelayedEvent
{
	// Token: 0x04000202 RID: 514
	public UnityEvent eventTrigger;

	// Token: 0x04000203 RID: 515
	public float delay;

	// Token: 0x04000204 RID: 516
	public int cycles = 1;

	// Token: 0x04000205 RID: 517
	public int cyclesPerLvl;
}
