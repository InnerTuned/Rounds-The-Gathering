using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000F5 RID: 245
public class WasDealtDamageTrigger : WasDealtDamageEffect
{
	// Token: 0x060004E7 RID: 1255 RVA: 0x000027C8 File Offset: 0x000009C8
	private void Start()
	{
	}

	// Token: 0x060004E8 RID: 1256 RVA: 0x0001C098 File Offset: 0x0001A298
	public override void WasDealtDamage(Vector2 damage, bool selfDamage)
	{
		if (selfDamage && !this.allowSelfDamage)
		{
			return;
		}
		this.damageDealt += damage.magnitude;
		if (this.damageDealt > this.damageNeeded && Time.time > this.time + this.cd)
		{
			this.time = Time.time;
			this.damageDealt = 0f;
			this.triggerEvent.Invoke();
		}
	}

	// Token: 0x0400066D RID: 1645
	public float damageNeeded = 25f;

	// Token: 0x0400066E RID: 1646
	public float cd = 0.2f;

	// Token: 0x0400066F RID: 1647
	public bool allowSelfDamage;

	// Token: 0x04000670 RID: 1648
	private float time;

	// Token: 0x04000671 RID: 1649
	private float damageDealt;

	// Token: 0x04000672 RID: 1650
	public UnityEvent triggerEvent;
}
