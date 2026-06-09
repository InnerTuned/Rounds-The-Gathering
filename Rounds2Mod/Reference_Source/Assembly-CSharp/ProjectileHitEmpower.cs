using System;

// Token: 0x02000199 RID: 409
public class ProjectileHitEmpower : RayHitEffect
{
	// Token: 0x06000847 RID: 2119 RVA: 0x0002C894 File Offset: 0x0002AA94
	public override HasToReturn DoHitEffect(HitInfo hit)
	{
		if (this.done)
		{
			return HasToReturn.canContinue;
		}
		base.GetComponentInParent<SpawnedAttack>().spawner.data.block.DoBlockAtPosition(true, true, BlockTrigger.BlockTriggerType.Empower, hit.point - base.transform.forward * 0.05f, true);
		this.done = true;
		return HasToReturn.canContinue;
	}

	// Token: 0x04000993 RID: 2451
	private bool done;
}
