using System;
using UnityEngine;

// Token: 0x020000DB RID: 219
public class SpawnObjectOnDealtDamage : WasDealtDamageEffect
{
	// Token: 0x06000465 RID: 1125 RVA: 0x0001A41E File Offset: 0x0001861E
	private void Start()
	{
		this.spawn = base.GetComponent<SpawnObjectEffect>();
	}

	// Token: 0x06000466 RID: 1126 RVA: 0x0001A42C File Offset: 0x0001862C
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
			this.spawn.DoEffect(damage);
		}
	}

	// Token: 0x040005EB RID: 1515
	public float damageNeeded = 25f;

	// Token: 0x040005EC RID: 1516
	public float cd = 0.2f;

	// Token: 0x040005ED RID: 1517
	public bool allowSelfDamage;

	// Token: 0x040005EE RID: 1518
	private float time;

	// Token: 0x040005EF RID: 1519
	private float damageDealt;

	// Token: 0x040005F0 RID: 1520
	private SpawnObjectEffect spawn;
}
