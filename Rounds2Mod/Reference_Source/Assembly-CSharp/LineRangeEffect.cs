using System;
using UnityEngine;

// Token: 0x0200015F RID: 351
public class LineRangeEffect : MonoBehaviour
{
	// Token: 0x06000709 RID: 1801 RVA: 0x000269B1 File Offset: 0x00024BB1
	private void Start()
	{
		this.spawned = base.GetComponent<SpawnedAttack>();
		this.owner = base.GetComponent<SpawnedAttack>().spawner;
		this.lineEffect = base.GetComponentInChildren<LineEffect>();
	}

	// Token: 0x0600070A RID: 1802 RVA: 0x000269DC File Offset: 0x00024BDC
	private void Update()
	{
		if (this.done)
		{
			return;
		}
		if (!this.spawned.IsMine())
		{
			return;
		}
		Player closestPlayerInTeam = PlayerManager.instance.GetClosestPlayerInTeam(base.transform.position, PlayerManager.instance.GetOtherTeam(this.owner.teamID), true);
		if (closestPlayerInTeam)
		{
			float num = 2f;
			float radius = this.lineEffect.GetRadius();
			float num2 = Vector2.Distance(base.transform.position, closestPlayerInTeam.transform.position);
			if (num2 < radius + num && num2 > radius - num)
			{
				this.done = true;
				closestPlayerInTeam.data.healthHandler.CallTakeDamage(this.dmg * base.transform.localScale.x * (closestPlayerInTeam.transform.position - base.transform.position).normalized, closestPlayerInTeam.transform.position, null, this.owner, true);
				closestPlayerInTeam.data.healthHandler.CallTakeForce(this.knockback * base.transform.localScale.x * (closestPlayerInTeam.transform.position - base.transform.position).normalized, 1, false, false, 0f);
			}
		}
	}

	// Token: 0x04000877 RID: 2167
	public float dmg;

	// Token: 0x04000878 RID: 2168
	public float knockback;

	// Token: 0x04000879 RID: 2169
	private LineEffect lineEffect;

	// Token: 0x0400087A RID: 2170
	private Player owner;

	// Token: 0x0400087B RID: 2171
	private SpawnedAttack spawned;

	// Token: 0x0400087C RID: 2172
	private bool done;
}
