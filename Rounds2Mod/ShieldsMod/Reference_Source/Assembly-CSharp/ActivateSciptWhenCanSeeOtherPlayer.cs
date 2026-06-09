using System;
using UnityEngine;

// Token: 0x02000005 RID: 5
public class ActivateSciptWhenCanSeeOtherPlayer : MonoBehaviour
{
	// Token: 0x06000020 RID: 32 RVA: 0x00002C3B File Offset: 0x00000E3B
	private void Start()
	{
		this.spawned = base.GetComponentInParent<SpawnedAttack>();
	}

	// Token: 0x06000021 RID: 33 RVA: 0x00002C4C File Offset: 0x00000E4C
	private void Update()
	{
		Player player;
		if (this.target == ActivateSciptWhenCanSeeOtherPlayer.Target.OtherPlayer)
		{
			player = PlayerManager.instance.GetOtherPlayer(this.spawned.spawner);
		}
		else
		{
			player = PlayerManager.instance.GetClosestPlayer(base.transform.position, true);
		}
		if (!player)
		{
			this.script.enabled = false;
			return;
		}
		if (PlayerManager.instance.CanSeePlayer(base.transform.position, player).canSee)
		{
			this.script.enabled = true;
			return;
		}
		this.script.enabled = false;
	}

	// Token: 0x04000013 RID: 19
	public ActivateSciptWhenCanSeeOtherPlayer.Target target;

	// Token: 0x04000014 RID: 20
	private SpawnedAttack spawned;

	// Token: 0x04000015 RID: 21
	public MonoBehaviour script;

	// Token: 0x02000326 RID: 806
	public enum Target
	{
		// Token: 0x04000FFF RID: 4095
		OtherPlayer,
		// Token: 0x04001000 RID: 4096
		Closest
	}
}
