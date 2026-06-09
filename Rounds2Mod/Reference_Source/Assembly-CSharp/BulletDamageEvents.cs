using System;
using UnityEngine;

// Token: 0x02000100 RID: 256
public class BulletDamageEvents : MonoBehaviour
{
	// Token: 0x06000513 RID: 1299 RVA: 0x0001CEF8 File Offset: 0x0001B0F8
	public void Start()
	{
		ProjectileHit componentInParent = base.GetComponentInParent<ProjectileHit>();
		for (int i = 0; i < this.damageEvents.Length; i++)
		{
			if (componentInParent.damage > this.damageEvents[i].dmg)
			{
				this.damageEvents[i].eventToCall.Invoke();
			}
		}
	}

	// Token: 0x040006AC RID: 1708
	public DmgEvent[] damageEvents;
}
