using System;
using UnityEngine;

// Token: 0x02000057 RID: 87
public class FollowPlayer : MonoBehaviour
{
	// Token: 0x06000198 RID: 408 RVA: 0x0000A993 File Offset: 0x00008B93
	private void Start()
	{
		this.startScale = base.transform.localScale;
		this.ownPlayer = base.GetComponentInParent<Player>();
		if (!this.ownPlayer)
		{
			this.ownPlayer = base.GetComponentInParent<SpawnedAttack>().spawner;
		}
	}

	// Token: 0x06000199 RID: 409 RVA: 0x0000A9D0 File Offset: 0x00008BD0
	private void LateUpdate()
	{
		Player otherPlayer;
		if (this.target == FollowPlayer.Target.Other)
		{
			otherPlayer = PlayerManager.instance.GetOtherPlayer(this.ownPlayer);
		}
		else
		{
			otherPlayer = this.ownPlayer;
		}
		if (this.inheritScale)
		{
			base.transform.localScale = new Vector3(otherPlayer.transform.localScale.x * this.startScale.x, otherPlayer.transform.localScale.y * this.startScale.y, otherPlayer.transform.localScale.z * this.startScale.z);
		}
		base.transform.position = otherPlayer.transform.position;
		base.transform.rotation = otherPlayer.transform.rotation;
	}

	// Token: 0x04000231 RID: 561
	public FollowPlayer.Target target;

	// Token: 0x04000232 RID: 562
	public bool inheritScale = true;

	// Token: 0x04000233 RID: 563
	private Vector3 startScale;

	// Token: 0x04000234 RID: 564
	private Player ownPlayer;

	// Token: 0x02000344 RID: 836
	public enum Target
	{
		// Token: 0x040010A5 RID: 4261
		Self,
		// Token: 0x040010A6 RID: 4262
		Other
	}
}
