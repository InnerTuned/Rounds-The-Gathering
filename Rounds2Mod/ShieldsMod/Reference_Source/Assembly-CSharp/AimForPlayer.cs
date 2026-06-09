using System;
using UnityEngine;

// Token: 0x02000008 RID: 8
public class AimForPlayer : MonoBehaviour
{
	// Token: 0x06000029 RID: 41 RVA: 0x00002E58 File Offset: 0x00001058
	private void Start()
	{
		this.spawned = base.GetComponentInParent<SpawnedAttack>();
	}

	// Token: 0x0600002A RID: 42 RVA: 0x00002E68 File Offset: 0x00001068
	private void Update()
	{
		Player player;
		if (this.target == AimForPlayer.Target.OtherPlayer)
		{
			player = PlayerManager.instance.GetOtherPlayer(this.spawned.spawner);
		}
		else
		{
			player = PlayerManager.instance.GetClosestPlayer(base.transform.position, true);
		}
		if (player && PlayerManager.instance.CanSeePlayer(base.transform.position, player).canSee)
		{
			base.transform.rotation = Quaternion.LookRotation(player.transform.position + Vector3.up * Vector3.Distance(player.transform.position, base.transform.position) * 0.1f * this.upOffset - base.transform.position, Vector3.forward);
		}
	}

	// Token: 0x0400001E RID: 30
	public float upOffset;

	// Token: 0x0400001F RID: 31
	public AimForPlayer.Target target;

	// Token: 0x04000020 RID: 32
	private SpawnedAttack spawned;

	// Token: 0x02000327 RID: 807
	public enum Target
	{
		// Token: 0x04001002 RID: 4098
		OtherPlayer,
		// Token: 0x04001003 RID: 4099
		Closest
	}
}
