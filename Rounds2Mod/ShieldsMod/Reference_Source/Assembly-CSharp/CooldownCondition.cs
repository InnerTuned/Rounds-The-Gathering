using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000123 RID: 291
public class CooldownCondition : MonoBehaviour
{
	// Token: 0x060005B5 RID: 1461 RVA: 0x00020896 File Offset: 0x0001EA96
	public void TryEvent()
	{
		if (Time.time < this.lastTime + this.cooldown)
		{
			return;
		}
		this.lastTime = Time.time;
		this.triggerEvent.Invoke();
	}

	// Token: 0x04000754 RID: 1876
	public UnityEvent triggerEvent;

	// Token: 0x04000755 RID: 1877
	public float cooldown = 0.25f;

	// Token: 0x04000756 RID: 1878
	private float lastTime = -100f;
}
