using System;
using UnityEngine;

// Token: 0x020000E7 RID: 231
public class TeleportToOpponent : MonoBehaviour
{
	// Token: 0x0600048E RID: 1166 RVA: 0x000027C8 File Offset: 0x000009C8
	private void Start()
	{
	}

	// Token: 0x0600048F RID: 1167 RVA: 0x0001B150 File Offset: 0x00019350
	public void Go()
	{
		if (!this.target)
		{
			this.target = PlayerManager.instance.GetOtherPlayer(base.GetComponentInParent<Player>());
		}
		this.from.Play();
		base.transform.root.transform.position = this.target.transform.position + (this.target.transform.position - base.transform.position).normalized;
		this.to.Play();
	}

	// Token: 0x04000626 RID: 1574
	public float time = 0.5f;

	// Token: 0x04000627 RID: 1575
	public ParticleSystem from;

	// Token: 0x04000628 RID: 1576
	public ParticleSystem to;

	// Token: 0x04000629 RID: 1577
	private Player target;
}
