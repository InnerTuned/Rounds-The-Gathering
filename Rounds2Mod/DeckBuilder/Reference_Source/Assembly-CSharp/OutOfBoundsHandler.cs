using System;
using Sonigon;
using UnityEngine;

// Token: 0x02000089 RID: 137
public class OutOfBoundsHandler : MonoBehaviour
{
	// Token: 0x060002E5 RID: 741 RVA: 0x000126D0 File Offset: 0x000108D0
	private void Start()
	{
		base.transform.position = Vector3.up * 200f;
		this.data = base.transform.root.GetComponent<CharacterData>();
		this.rpc = this.data.GetComponent<ChildRPC>();
		this.rpc.childRPCs.Add("OutOfBounds", new Action(this.RPCA_DisplayOutOfBounds));
		this.rpc.childRPCs.Add("ShieldOutOfBounds", new Action(this.RPCA_DisplayOutOfBoundsShield));
		this.mainCam = MainCam.instance.transform.GetComponent<Camera>();
		base.transform.SetParent(null);
	}

	// Token: 0x060002E6 RID: 742 RVA: 0x00012784 File Offset: 0x00010984
	private void LateUpdate()
	{
		if (!this.data)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (!this.data.playerVel.simulated)
		{
			return;
		}
		if (!this.data.isPlaying)
		{
			return;
		}
		float x = Mathf.InverseLerp(-35.56f, 35.56f, this.data.transform.position.x);
		float y = Mathf.InverseLerp(-20f, 20f, this.data.transform.position.y);
		Vector3 vector = new Vector3(x, y, 0f);
		vector = new Vector3(Mathf.Clamp(vector.x, 0f, 1f), Mathf.Clamp(vector.y, 0f, 1f), vector.z);
		this.almostOutOfBounds = false;
		this.outOfBounds = false;
		if (vector.x <= 0f || vector.x >= 1f || vector.y >= 1f || vector.y <= 0f)
		{
			this.outOfBounds = true;
		}
		else if (vector.x < this.warningPercentage || vector.x > 1f - this.warningPercentage || vector.y > 1f - this.warningPercentage || vector.y < this.warningPercentage)
		{
			this.almostOutOfBounds = true;
			if (vector.x < this.warningPercentage)
			{
				vector.x = 0f;
			}
			if (vector.x > 1f - this.warningPercentage)
			{
				vector.x = 1f;
			}
			if (vector.y < this.warningPercentage)
			{
				vector.y = 0f;
			}
			if (vector.y > 1f - this.warningPercentage)
			{
				vector.y = 1f;
			}
		}
		this.counter += TimeHandler.deltaTime;
		if (this.almostOutOfBounds && !this.data.dead)
		{
			base.transform.position = this.GetPoint(vector);
			base.transform.rotation = Quaternion.LookRotation(Vector3.forward, -(this.data.transform.position - base.transform.position));
			if (this.counter > 0.1f)
			{
				this.counter = 0f;
				this.warning.Play();
			}
		}
		if (this.outOfBounds && !this.data.dead)
		{
			this.data.sinceGrounded = 0f;
			base.transform.position = this.GetPoint(vector);
			base.transform.rotation = Quaternion.LookRotation(Vector3.forward, -(this.data.transform.position - base.transform.position));
			if (this.counter > 0.1f && this.data.view.IsMine)
			{
				this.counter = 0f;
				if (this.data.block.IsBlocking())
				{
					this.rpc.CallFunction("ShieldOutOfBounds");
					this.data.playerVel.velocity *= 0f;
					this.data.healthHandler.CallTakeForce(base.transform.up * 400f * this.data.playerVel.mass, 1, false, true, 0f);
					this.data.transform.position = base.transform.position;
					return;
				}
				this.rpc.CallFunction("OutOfBounds");
				this.data.healthHandler.CallTakeForce(base.transform.up * 200f * this.data.playerVel.mass, 1, false, true, 0f);
				this.data.healthHandler.CallTakeDamage(51f * base.transform.up, this.data.transform.position, null, null, true);
			}
		}
	}

	// Token: 0x060002E7 RID: 743 RVA: 0x00012BE8 File Offset: 0x00010DE8
	private Vector3 GetPoint(Vector3 p)
	{
		float x = Mathf.Lerp(-35.56f, 35.56f, p.x);
		float y = Mathf.Lerp(-20f, 20f, p.y);
		return new Vector3(x, y, 0f);
	}

	// Token: 0x060002E8 RID: 744 RVA: 0x00012C2C File Offset: 0x00010E2C
	private void RPCA_DisplayOutOfBounds()
	{
		SoundManager.Instance.Play(this.data.playerSounds.soundCharacterDamageScreenEdge, this.data.transform);
		this.burst.Play();
		this.wall.Play();
		this.burstBig.Play();
		this.data.sinceGrounded = 0f;
	}

	// Token: 0x060002E9 RID: 745 RVA: 0x00012C90 File Offset: 0x00010E90
	private void RPCA_DisplayOutOfBoundsShield()
	{
		SoundManager.Instance.Play(this.data.playerSounds.soundCharacterDamageScreenEdge, this.data.transform);
		this.shieldBurst.Play();
		this.shieldWall.Play();
		this.shieldBurstBig.Play();
		this.data.sinceGrounded = 0f;
	}

	// Token: 0x04000415 RID: 1045
	private bool outOfBounds;

	// Token: 0x04000416 RID: 1046
	private bool almostOutOfBounds;

	// Token: 0x04000417 RID: 1047
	private Camera mainCam;

	// Token: 0x04000418 RID: 1048
	private CharacterData data;

	// Token: 0x04000419 RID: 1049
	public ParticleSystem wall;

	// Token: 0x0400041A RID: 1050
	public ParticleSystem burst;

	// Token: 0x0400041B RID: 1051
	public ParticleSystem burstBig;

	// Token: 0x0400041C RID: 1052
	public ParticleSystem warning;

	// Token: 0x0400041D RID: 1053
	public ParticleSystem shieldWall;

	// Token: 0x0400041E RID: 1054
	public ParticleSystem shieldBurst;

	// Token: 0x0400041F RID: 1055
	public ParticleSystem shieldBurstBig;

	// Token: 0x04000420 RID: 1056
	private ChildRPC rpc;

	// Token: 0x04000421 RID: 1057
	private float counter;

	// Token: 0x04000422 RID: 1058
	private float warningPercentage = 0.1f;
}
