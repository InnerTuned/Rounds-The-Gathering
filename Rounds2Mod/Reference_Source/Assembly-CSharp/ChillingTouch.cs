using System;
using UnityEngine;

// Token: 0x0200011E RID: 286
public class ChillingTouch : DamageEffect
{
	// Token: 0x060005A5 RID: 1445 RVA: 0x00020619 File Offset: 0x0001E819
	private void Start()
	{
		this.level = base.GetComponent<AttackLevel>();
	}

	// Token: 0x060005A6 RID: 1446 RVA: 0x00020628 File Offset: 0x0001E828
	public override void DoDamageEffect(Vector2 dmg, bool selfDmg, Player damagedPlayer = null)
	{
		damagedPlayer.data.stats.RPCA_AddSlow((this.baseSlow + dmg.magnitude * this.scalingSlow) * (1f + ((float)this.level.attackLevel - 1f) * 0.3f), false);
	}

	// Token: 0x04000744 RID: 1860
	public float baseSlow = 0.2f;

	// Token: 0x04000745 RID: 1861
	public float scalingSlow = 0.01f;

	// Token: 0x04000746 RID: 1862
	private AttackLevel level;
}
