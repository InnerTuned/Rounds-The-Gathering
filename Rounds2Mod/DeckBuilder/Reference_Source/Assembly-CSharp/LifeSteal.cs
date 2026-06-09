using System;
using UnityEngine;

// Token: 0x02000078 RID: 120
public class LifeSteal : DealtDamageEffect
{
	// Token: 0x06000278 RID: 632 RVA: 0x000101CF File Offset: 0x0000E3CF
	public override void DealtDamage(Vector2 damage, bool selfDamage, Player damagedPlayer = null)
	{
		if (selfDamage)
		{
			return;
		}
		if (!this.health)
		{
			this.health = base.GetComponentInParent<HealthHandler>();
		}
		this.health.Heal(damage.magnitude * this.multiplier);
	}

	// Token: 0x0400038C RID: 908
	private HealthHandler health;

	// Token: 0x0400038D RID: 909
	public float multiplier;
}
