using System;
using UnityEngine;

// Token: 0x020000E3 RID: 227
public class StunPlayer : MonoBehaviour
{
	// Token: 0x06000480 RID: 1152 RVA: 0x0001AC04 File Offset: 0x00018E04
	public void Go()
	{
		if (!this.target)
		{
			this.target = base.GetComponentInParent<Player>();
			if (this.targetPlayer == StunPlayer.TargetPlayer.OtherPlayer)
			{
				this.target = PlayerManager.instance.GetOtherPlayer(this.target);
			}
		}
		this.target.data.stunHandler.AddStun(this.time);
	}

	// Token: 0x04000609 RID: 1545
	public StunPlayer.TargetPlayer targetPlayer;

	// Token: 0x0400060A RID: 1546
	public float time = 0.5f;

	// Token: 0x0400060B RID: 1547
	private Player target;

	// Token: 0x0200037C RID: 892
	public enum TargetPlayer
	{
		// Token: 0x040011BD RID: 4541
		OtherPlayer,
		// Token: 0x040011BE RID: 4542
		Self
	}
}
