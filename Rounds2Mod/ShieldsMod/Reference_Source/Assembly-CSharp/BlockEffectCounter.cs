using System;
using UnityEngine;

// Token: 0x02000015 RID: 21
public class BlockEffectCounter : BlockEffect
{
	// Token: 0x06000070 RID: 112 RVA: 0x00004B51 File Offset: 0x00002D51
	private void Start()
	{
		this.spawn = base.GetComponentInParent<SpawnObjectEffect>();
	}

	// Token: 0x06000071 RID: 113 RVA: 0x00004B60 File Offset: 0x00002D60
	public override void DoBlockedProjectile(GameObject projectile, Vector3 forward, Vector3 hitPos)
	{
		for (int i = 0; i < this.ownAttacksToSendBack; i++)
		{
			this.spawn.DoEffect(forward);
		}
	}

	// Token: 0x04000072 RID: 114
	public int ownAttacksToSendBack;

	// Token: 0x04000073 RID: 115
	private SpawnObjectEffect spawn;
}
