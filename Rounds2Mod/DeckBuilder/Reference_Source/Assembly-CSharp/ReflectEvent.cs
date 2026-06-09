using System;
using UnityEngine.Events;

// Token: 0x020001A0 RID: 416
public class ReflectEvent : RayHitEffect
{
	// Token: 0x06000862 RID: 2146 RVA: 0x0002CFC6 File Offset: 0x0002B1C6
	public override HasToReturn DoHitEffect(HitInfo hit)
	{
		this.bounceEvent.Invoke();
		return HasToReturn.canContinue;
	}

	// Token: 0x040009A8 RID: 2472
	public UnityEvent bounceEvent;
}
