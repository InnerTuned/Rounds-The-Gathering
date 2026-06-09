using System;
using UnityEngine;

// Token: 0x02000159 RID: 345
public class Implosion : MonoBehaviour
{
	// Token: 0x060006F6 RID: 1782 RVA: 0x00026690 File Offset: 0x00024890
	private void Start()
	{
		Explosion component = base.GetComponent<Explosion>();
		component.HitTargetAction = (Action<Damagable, float>)Delegate.Combine(component.HitTargetAction, new Action<Damagable, float>(this.HitTarget));
		this.clampDist *= base.transform.localScale.x;
		this.force *= base.transform.localScale.x;
	}

	// Token: 0x060006F7 RID: 1783 RVA: 0x000266FE File Offset: 0x000248FE
	public void HitTarget(Damagable damageble, float distance)
	{
		this.DoPull(damageble, distance);
	}

	// Token: 0x060006F8 RID: 1784 RVA: 0x00026708 File Offset: 0x00024908
	private void DoPull(Damagable damageble, float distance)
	{
		bool flag = base.GetComponent<SpawnedAttack>().IsMine();
		HealthHandler component = damageble.GetComponent<HealthHandler>();
		CharacterData component2 = damageble.GetComponent<CharacterData>();
		(base.transform.position - component.transform.position) * 0.25f;
		if (flag)
		{
			component2.view.RPC("RPCA_SendForceTowardsPointOverTime", 0, new object[]
			{
				this.force,
				this.drag,
				this.clampDist,
				base.transform.position,
				this.time,
				0,
				false,
				false
			});
		}
	}

	// Token: 0x04000864 RID: 2148
	public float force;

	// Token: 0x04000865 RID: 2149
	public float drag;

	// Token: 0x04000866 RID: 2150
	public float time;

	// Token: 0x04000867 RID: 2151
	public float clampDist;
}
