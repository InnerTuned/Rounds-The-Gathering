using System;
using UnityEngine;

// Token: 0x020000E8 RID: 232
public class Thruster : MonoBehaviour
{
	// Token: 0x06000491 RID: 1169 RVA: 0x0001B1FC File Offset: 0x000193FC
	private void Start()
	{
		this.force *= Mathf.Pow(base.transform.localScale.x, this.pow);
		this.drag *= Mathf.Pow(base.transform.localScale.x, this.pow);
		this.follow = base.GetComponent<FollowLocalPos>();
		if (this.follow.target)
		{
			this.pushed = this.follow.target.gameObject.GetComponent<NetworkPhysicsObject>();
		}
		float num = 1f;
		if (!this.follow.targetPlayer)
		{
			num = 0.2f;
		}
		else if (this.pushed)
		{
			num = 0.5f;
		}
		ParticleSystem.MainModule mainModule;
		base.GetComponentInChildren<ParticleSystem>().main.duration = mainModule.duration * num;
		base.GetComponent<DelayEvent>().time *= num;
		base.GetComponent<RemoveAfterSeconds>().seconds *= num;
		this.startForward = base.transform.forward;
	}

	// Token: 0x06000492 RID: 1170 RVA: 0x0001B31C File Offset: 0x0001951C
	private void FixedUpdate()
	{
		if (this.follow)
		{
			if (this.follow.target)
			{
				if (this.player)
				{
					if (this.player)
					{
						this.player.data.healthHandler.TakeForce(this.startForward * this.force, 0, false, false, 0f);
					}
					else
					{
						this.rig.AddForce(this.startForward * this.force, 0);
					}
				}
				else if (!this.checkedForPlayer)
				{
					this.player = this.follow.target.transform.root.GetComponent<Player>();
					this.checkedForPlayer = true;
					if (!this.player && !this.pushed)
					{
						base.enabled = false;
					}
				}
			}
			if (this.pushed && this.pushed.photonView.IsMine)
			{
				this.pushed.RPCA_SendForce(base.transform.forward * this.force * this.physicsObjectM, this.pushed.transform.InverseTransformPoint(base.transform.position));
			}
		}
	}

	// Token: 0x0400062A RID: 1578
	public float force;

	// Token: 0x0400062B RID: 1579
	public float drag;

	// Token: 0x0400062C RID: 1580
	public float pow = 1f;

	// Token: 0x0400062D RID: 1581
	public float physicsObjectM = 1f;

	// Token: 0x0400062E RID: 1582
	private FollowLocalPos follow;

	// Token: 0x0400062F RID: 1583
	private Rigidbody2D rig;

	// Token: 0x04000630 RID: 1584
	private Vector2 startForward;

	// Token: 0x04000631 RID: 1585
	private NetworkPhysicsObject pushed;

	// Token: 0x04000632 RID: 1586
	private Player player;

	// Token: 0x04000633 RID: 1587
	private bool checkedForPlayer;
}
