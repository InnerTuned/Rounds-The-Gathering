using System;
using UnityEngine;

// Token: 0x02000058 RID: 88
public class FollowRandomPlayer : MonoBehaviour
{
	// Token: 0x0600019B RID: 411 RVA: 0x000027C8 File Offset: 0x000009C8
	private void Start()
	{
	}

	// Token: 0x0600019C RID: 412 RVA: 0x0000AAAC File Offset: 0x00008CAC
	private void Update()
	{
		if (!this.targetPlayer)
		{
			this.GetNewPlayer();
			return;
		}
		if (this.snap)
		{
			base.transform.position = new Vector3(this.X ? this.targetPlayer.position.x : base.transform.position.x, this.Y ? this.targetPlayer.position.y : base.transform.position.y, 0f);
			return;
		}
		base.transform.position = Vector3.Lerp(base.transform.position, new Vector3(this.X ? this.targetPlayer.position.x : base.transform.position.x, this.Y ? this.targetPlayer.position.y : base.transform.position.y, 0f), TimeHandler.deltaTime * 5f);
	}

	// Token: 0x0600019D RID: 413 RVA: 0x0000ABC4 File Offset: 0x00008DC4
	private void GetNewPlayer()
	{
		if (PlayerManager.instance.players.Count > 0)
		{
			this.targetPlayer = PlayerManager.instance.players[Random.Range(0, PlayerManager.instance.players.Count)].transform;
		}
	}

	// Token: 0x04000235 RID: 565
	public bool X = true;

	// Token: 0x04000236 RID: 566
	public bool Y = true;

	// Token: 0x04000237 RID: 567
	private Transform targetPlayer;

	// Token: 0x04000238 RID: 568
	public bool snap;
}
