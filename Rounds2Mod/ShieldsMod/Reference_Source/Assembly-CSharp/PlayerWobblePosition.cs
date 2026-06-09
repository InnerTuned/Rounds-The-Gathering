using System;
using UnityEngine;

// Token: 0x020000A4 RID: 164
public class PlayerWobblePosition : MonoBehaviour
{
	// Token: 0x060003A5 RID: 933 RVA: 0x000163A3 File Offset: 0x000145A3
	private void Start()
	{
		this.physicsPos = base.transform.position;
		this.player = base.GetComponentInParent<Player>();
	}

	// Token: 0x060003A6 RID: 934 RVA: 0x000163C4 File Offset: 0x000145C4
	private void Update()
	{
		float num = Mathf.Clamp(TimeHandler.deltaTime, 0f, 0.03f);
		Vector3 a = this.player.transform.position;
		if (this.prediction > 0f)
		{
			a += this.player.data.playerVel.velocity * this.prediction;
		}
		this.velocity += (a - this.physicsPos) * num * this.spring;
		this.velocity -= this.velocity * this.drag * num;
		this.physicsPos += num * this.multiplier * this.velocity;
		base.transform.position = this.physicsPos;
	}

	// Token: 0x040004C5 RID: 1221
	private Vector3 physicsPos;

	// Token: 0x040004C6 RID: 1222
	public float drag = 15f;

	// Token: 0x040004C7 RID: 1223
	public float spring = 1000f;

	// Token: 0x040004C8 RID: 1224
	public float multiplier = 1f;

	// Token: 0x040004C9 RID: 1225
	public float prediction;

	// Token: 0x040004CA RID: 1226
	private Vector3 velocity;

	// Token: 0x040004CB RID: 1227
	private Player player;
}
