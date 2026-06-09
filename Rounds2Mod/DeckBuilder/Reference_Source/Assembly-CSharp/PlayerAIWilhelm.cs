using System;
using UnityEngine;

// Token: 0x02000091 RID: 145
public class PlayerAIWilhelm : MonoBehaviour
{
	// Token: 0x06000323 RID: 803 RVA: 0x0001412B File Offset: 0x0001232B
	private void Start()
	{
		this.api = base.GetComponentInParent<PlayerAPI>();
	}

	// Token: 0x06000324 RID: 804 RVA: 0x0001413C File Offset: 0x0001233C
	private void Update()
	{
		this.api.Move(this.api.TowardsOtherPlayer());
		this.api.SetAimDirection(this.api.TowardsOtherPlayer() + this.api.GetOtherPlayer().data.playerVel.velocity * 0.1f);
		this.api.Attack();
		this.api.Jump();
		this.api.Block();
	}

	// Token: 0x04000457 RID: 1111
	private PlayerAPI api;
}
