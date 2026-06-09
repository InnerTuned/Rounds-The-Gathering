using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200009A RID: 154
public class PlayerInRangeTrigger : MonoBehaviour
{
	// Token: 0x0600035B RID: 859 RVA: 0x00014C0C File Offset: 0x00012E0C
	private void Start()
	{
		this.ownPlayer = base.transform.root.GetComponent<Player>();
		if (!this.ownPlayer)
		{
			this.ownPlayer = base.transform.root.GetComponentInParent<SpawnedAttack>().spawner;
		}
		if (this.scaleWithRange)
		{
			this.range *= base.transform.localScale.x;
		}
	}

	// Token: 0x0600035C RID: 860 RVA: 0x00014C7C File Offset: 0x00012E7C
	private void Update()
	{
		this.counter += TimeHandler.deltaTime;
		this.inRange = false;
		this.target = null;
		if (this.done)
		{
			return;
		}
		Player player = null;
		if (this.targetType == PlayerInRangeTrigger.TargetType.OtherPlayer)
		{
			player = PlayerManager.instance.GetOtherPlayer(this.ownPlayer);
		}
		if (this.targetType == PlayerInRangeTrigger.TargetType.Any)
		{
			player = PlayerManager.instance.GetClosestPlayer(base.transform.position, false);
		}
		if (PlayerManager.instance.CanSeePlayer(base.transform.position, player).canSee && Vector3.Distance(base.transform.position, player.transform.position) < this.range * base.transform.root.localScale.x && !player.data.dead)
		{
			if (this.counter < this.cooldown)
			{
				return;
			}
			this.counter = 0f;
			this.triggerEvent.Invoke();
			this.inRange = true;
			this.target = player;
			if (!this.repeating)
			{
				this.done = true;
			}
		}
	}

	// Token: 0x04000483 RID: 1155
	public PlayerInRangeTrigger.TargetType targetType;

	// Token: 0x04000484 RID: 1156
	public float range = 5f;

	// Token: 0x04000485 RID: 1157
	public float cooldown;

	// Token: 0x04000486 RID: 1158
	public bool repeating;

	// Token: 0x04000487 RID: 1159
	private float counter;

	// Token: 0x04000488 RID: 1160
	private bool done;

	// Token: 0x04000489 RID: 1161
	[HideInInspector]
	public bool inRange;

	// Token: 0x0400048A RID: 1162
	public UnityEvent triggerEvent;

	// Token: 0x0400048B RID: 1163
	[HideInInspector]
	public Player target;

	// Token: 0x0400048C RID: 1164
	private Player ownPlayer;

	// Token: 0x0400048D RID: 1165
	public bool scaleWithRange;

	// Token: 0x0200036E RID: 878
	public enum TargetType
	{
		// Token: 0x04001182 RID: 4482
		Any,
		// Token: 0x04001183 RID: 4483
		OtherPlayer
	}
}
