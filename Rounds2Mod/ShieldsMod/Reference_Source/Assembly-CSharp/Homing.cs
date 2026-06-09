using System;
using Photon.Pun;
using Sonigon;
using UnityEngine;

// Token: 0x0200006F RID: 111
public class Homing : MonoBehaviour
{
	// Token: 0x0600023E RID: 574 RVA: 0x0000E3A6 File Offset: 0x0000C5A6
	private void Start()
	{
		this.move = base.GetComponentInParent<MoveTransform>();
		this.flicks = base.GetComponentsInChildren<FlickerEvent>();
		this.view = base.GetComponentInParent<PhotonView>();
		base.GetComponentInParent<SyncProjectile>().active = true;
	}

	// Token: 0x0600023F RID: 575 RVA: 0x0000E3D8 File Offset: 0x0000C5D8
	private void Update()
	{
		Player closestPlayer = PlayerManager.instance.GetClosestPlayer(base.transform.position, true);
		if (!closestPlayer)
		{
			if (this.isOn)
			{
				this.move.simulateGravity--;
				this.soundHomingCanPlay = true;
			}
			this.isOn = false;
			for (int i = 0; i < this.flicks.Length; i++)
			{
				this.flicks[i].isOn = false;
			}
			this.rot1.target = 50f;
			this.rot2.target = -50f;
			return;
		}
		Vector3 a = closestPlayer.transform.position + base.transform.right * this.move.selectedSpread * Vector3.Distance(base.transform.position, closestPlayer.transform.position) * this.spread;
		float num = Vector3.Angle(base.transform.root.forward, a - base.transform.position);
		if (num < 70f)
		{
			this.move.velocity -= this.move.velocity * num * TimeHandler.deltaTime * this.scalingDrag;
			this.move.velocity -= this.move.velocity * TimeHandler.deltaTime * this.drag;
			this.move.velocity += Vector3.ClampMagnitude(a - base.transform.position, 1f) * TimeHandler.deltaTime * this.move.localForce.magnitude * 0.025f * this.amount;
			this.move.velocity.z = 0f;
			this.move.velocity += Vector3.up * TimeHandler.deltaTime * this.move.gravity * this.move.multiplier;
			if (!this.isOn)
			{
				this.move.simulateGravity++;
				if (this.soundHomingCanPlay)
				{
					this.soundHomingCanPlay = false;
					SoundManager.Instance.PlayAtPosition(this.soundHomingFound, SoundManager.Instance.GetTransform(), base.transform);
				}
			}
			this.isOn = true;
			for (int j = 0; j < this.flicks.Length; j++)
			{
				this.flicks[j].isOn = true;
			}
			this.rot1.target = 10f;
			this.rot2.target = -10f;
			return;
		}
		if (this.isOn)
		{
			this.move.simulateGravity--;
			this.soundHomingCanPlay = true;
		}
		this.isOn = false;
		for (int k = 0; k < this.flicks.Length; k++)
		{
			this.flicks[k].isOn = false;
		}
		this.rot1.target = 50f;
		this.rot2.target = -50f;
	}

	// Token: 0x04000322 RID: 802
	[Header("Sound")]
	public SoundEvent soundHomingFound;

	// Token: 0x04000323 RID: 803
	private bool soundHomingCanPlay = true;

	// Token: 0x04000324 RID: 804
	[Header("Settings")]
	public float amount = 1f;

	// Token: 0x04000325 RID: 805
	public float scalingDrag = 1f;

	// Token: 0x04000326 RID: 806
	public float drag = 1f;

	// Token: 0x04000327 RID: 807
	public float spread = 1f;

	// Token: 0x04000328 RID: 808
	private MoveTransform move;

	// Token: 0x04000329 RID: 809
	private bool isOn;

	// Token: 0x0400032A RID: 810
	public RotSpring rot1;

	// Token: 0x0400032B RID: 811
	public RotSpring rot2;

	// Token: 0x0400032C RID: 812
	private FlickerEvent[] flicks;

	// Token: 0x0400032D RID: 813
	private PhotonView view;
}
