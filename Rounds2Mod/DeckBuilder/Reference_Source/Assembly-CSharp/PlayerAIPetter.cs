using System;
using UnityEngine;

// Token: 0x02000184 RID: 388
public class PlayerAIPetter : MonoBehaviour
{
	// Token: 0x060007DE RID: 2014 RVA: 0x0002AA89 File Offset: 0x00028C89
	private void Start()
	{
		this.api = base.GetComponentInParent<PlayerAPI>();
		this.m_collider = this.api.GetComponentInChildren<BoxCollider2D>();
	}

	// Token: 0x060007DF RID: 2015 RVA: 0x0002AAA8 File Offset: 0x00028CA8
	private void Update()
	{
		if ((double)Random.Range(0f, 1f) > 0.9)
		{
			this.api.Move(this.api.TowardsOtherPlayer() * -1f);
		}
		else
		{
			this.api.Move(this.api.TowardsOtherPlayer());
		}
		this.PredictionHit();
		this.api.Attack();
		if ((double)Random.Range(0f, 1f) > 0.9)
		{
			this.api.Jump();
		}
		this.AutoBlock();
	}

	// Token: 0x060007E0 RID: 2016 RVA: 0x0002AB48 File Offset: 0x00028D48
	private void AutoBlock()
	{
		foreach (BulletWrapper bulletWrapper in this.api.GetAllBullets())
		{
			RaycastHit2D raycastHit2D = Physics2D.Raycast(bulletWrapper.projectileMovement.transform.position, bulletWrapper.projectileMovement.velocity.normalized, bulletWrapper.velocity.magnitude * 5f * TimeHandler.deltaTime, this.m_layer);
			if (raycastHit2D.transform && (!bulletWrapper.projectileHit.ownPlayer || bulletWrapper.projectileHit.ownPlayer != this.api.player) && raycastHit2D.transform.root == base.transform.root)
			{
				global::Debug.Log("BLICOK");
				this.api.Block();
			}
		}
	}

	// Token: 0x060007E1 RID: 2017 RVA: 0x0002AC64 File Offset: 0x00028E64
	private void PredictionHit()
	{
		if (!this.api.GetOtherPlayer())
		{
			return;
		}
		float magnitude = (this.api.OtherPlayerPosition() - base.transform.position).magnitude;
		Vector2 b = this.api.GetOtherPlayer().data.playerVel.velocity * this.m_predDist * 0.1f * magnitude * 0.05f;
		this.api.SetAimDirection(this.api.TowardsOtherPlayer() + Vector2.up * this.aimCurve.Evaluate(magnitude) + b);
	}

	// Token: 0x04000938 RID: 2360
	private PlayerAPI api;

	// Token: 0x04000939 RID: 2361
	public LayerMask m_layer;

	// Token: 0x0400093A RID: 2362
	public AnimationCurve aimCurve;

	// Token: 0x0400093B RID: 2363
	public float m_shootRandom = 0.9f;

	// Token: 0x0400093C RID: 2364
	public float m_predDist = 1f;

	// Token: 0x0400093D RID: 2365
	public float m_timeSinceGround = 0.1f;

	// Token: 0x0400093E RID: 2366
	private BoxCollider2D m_collider;
}
