using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000DA RID: 218
public class SpawnObjectOnDealDamage : DealtDamageEffect
{
	// Token: 0x06000462 RID: 1122 RVA: 0x0001A342 File Offset: 0x00018542
	private void Start()
	{
		this.spawn = base.GetComponent<SpawnObjectEffect>();
		this.dmgEffect = base.GetComponent<DamageEffect>();
	}

	// Token: 0x06000463 RID: 1123 RVA: 0x0001A35C File Offset: 0x0001855C
	public override void DealtDamage(Vector2 damage, bool selfDamage, Player damagedPlayer = null)
	{
		if (selfDamage && !this.allowSelfDmg)
		{
			return;
		}
		this.damageDealt += damage.magnitude;
		if (this.damageDealt > this.damageNeeded && Time.time > this.time + this.cd)
		{
			this.time = Time.time;
			this.damageDealt = 0f;
			if (this.spawn)
			{
				this.spawn.DoEffect(damage);
			}
			if (this.dmgEffect)
			{
				this.dmgEffect.DoDamageEffect(damage, selfDamage, damagedPlayer);
			}
			this.triggerEvent.Invoke();
		}
	}

	// Token: 0x040005E3 RID: 1507
	public UnityEvent triggerEvent;

	// Token: 0x040005E4 RID: 1508
	public float damageNeeded = 25f;

	// Token: 0x040005E5 RID: 1509
	public float cd = 0.2f;

	// Token: 0x040005E6 RID: 1510
	private float time;

	// Token: 0x040005E7 RID: 1511
	private float damageDealt;

	// Token: 0x040005E8 RID: 1512
	private SpawnObjectEffect spawn;

	// Token: 0x040005E9 RID: 1513
	private DamageEffect dmgEffect;

	// Token: 0x040005EA RID: 1514
	public bool allowSelfDmg;
}
