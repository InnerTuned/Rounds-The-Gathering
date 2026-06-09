using System;
using UnityEngine;

// Token: 0x02000183 RID: 387
public class PlayerAIMinion : MonoBehaviour
{
	// Token: 0x060007DA RID: 2010 RVA: 0x0002A830 File Offset: 0x00028A30
	private void Start()
	{
		this.api = base.GetComponentInParent<PlayerAPI>();
		this.moveDirection = this.api.data.master.data.aimDirection;
		if (this.moveDirection.x > 0.5f)
		{
			this.moveDirection.x = 1f;
		}
		else if (this.moveDirection.x < -0.5f)
		{
			this.moveDirection.x = -1f;
		}
		else
		{
			this.moveDirection.x = 0f;
		}
		this.moveDirection.y = 0f;
		base.transform.root.gameObject.AddComponent<RemoveAfterSeconds>().seconds = 4f;
	}

	// Token: 0x060007DB RID: 2011 RVA: 0x0002A8F8 File Offset: 0x00028AF8
	private void Update()
	{
		Player otherPlayer = PlayerManager.instance.GetOtherPlayer(this.api.data.master);
		this.api.Move(this.moveDirection);
		if (this.api.data.isWallGrab)
		{
			this.api.Jump();
		}
		if (!this.api.data.isGrounded)
		{
			this.api.Jump();
		}
		if (otherPlayer)
		{
			this.api.SetAimDirection(this.GetAimDirForHitting(otherPlayer.transform.position));
			if (PlayerManager.instance.CanSeePlayer(base.transform.position, otherPlayer).canSee && Vector2.Distance(base.transform.position, otherPlayer.transform.position) < this.range)
			{
				this.api.Attack();
			}
		}
	}

	// Token: 0x060007DC RID: 2012 RVA: 0x0002A9EC File Offset: 0x00028BEC
	private Vector2 GetAimDirForHitting(Vector3 point)
	{
		Vector3 v = point - base.transform.position;
		this.api.SetAimDirection(v);
		this.api.GetMyBullet();
		float time = Mathf.Abs(point.x - base.transform.position.x);
		return point + Vector3.up * this.m_AimCompensastionCurve.Evaluate(time) - base.transform.position;
	}

	// Token: 0x04000934 RID: 2356
	public AnimationCurve m_AimCompensastionCurve;

	// Token: 0x04000935 RID: 2357
	public float range = 5f;

	// Token: 0x04000936 RID: 2358
	private PlayerAPI api;

	// Token: 0x04000937 RID: 2359
	private Vector2 moveDirection;
}
