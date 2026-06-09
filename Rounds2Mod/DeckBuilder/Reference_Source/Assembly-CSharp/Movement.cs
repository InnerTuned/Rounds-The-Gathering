using System;
using UnityEngine;

// Token: 0x02000083 RID: 131
public class Movement : MonoBehaviour
{
	// Token: 0x060002CB RID: 715 RVA: 0x000118D9 File Offset: 0x0000FAD9
	private void Start()
	{
		this.rig = base.GetComponent<Rigidbody2D>();
		this.input = base.GetComponent<GeneralInput>();
		this.data = base.GetComponent<CharacterData>();
		this.stats = base.GetComponent<CharacterStatModifiers>();
		this.health = base.GetComponent<HealthHandler>();
	}

	// Token: 0x060002CC RID: 716 RVA: 0x00011918 File Offset: 0x0000FB18
	private void FixedUpdate()
	{
		if (this.input.direction != Vector3.zero)
		{
			this.moveDir = this.input.direction;
			this.idleForce = Vector3.Lerp(this.idleForce, this.moveDir, Time.fixedDeltaTime * 6f);
		}
		else if (this.data.isGrounded || this.data.isWallGrab)
		{
			this.idleForce = Vector3.Lerp(this.idleForce, Vector3.zero, 15f * Time.fixedDeltaTime);
		}
		else
		{
			this.idleForce = Vector3.Lerp(this.idleForce, Vector3.zero, 3f * Time.fixedDeltaTime);
		}
		if (this.input.direction == Vector3.zero)
		{
			this.moveDir = this.idleForce;
		}
		if (this.data.isGrounded || this.data.isWallGrab)
		{
			this.knockBack = Vector3.Lerp(this.knockBack, Vector3.zero, 10f * Time.fixedDeltaTime);
		}
		else
		{
			this.knockBack = Vector3.Lerp(this.knockBack, Vector3.zero, 7f * Time.fixedDeltaTime);
		}
		this.rig.AddForce(this.knockBack * 0.1f * this.rig.mass, 0);
		if (this.moveDir.y > 0f)
		{
			this.moveDir.y = 0f;
		}
		this.rig.AddForce(-this.rig.velocity * 0.01f * this.drag * this.rig.mass, 0);
		this.rig.AddTorque(-this.rig.angularVelocity * 0.01f * this.Angulardrag * this.rig.mass, 0);
		if (this.input.jumpIsPressed)
		{
			this.rig.AddForce(Vector2.up * 20f * this.rig.mass, 0);
		}
		this.rig.AddForce(this.moveDir * this.force * (1f - this.stats.GetSlow()) * this.rig.mass, 0);
	}

	// Token: 0x060002CD RID: 717 RVA: 0x00011BA4 File Offset: 0x0000FDA4
	private void Update()
	{
		if (this.input.jumpWasPressed)
		{
			this.Jump();
		}
		if (this.data.isGrounded && this.data.sinceJump > 0.2f)
		{
			this.data.currentJumps = this.data.jumps;
		}
	}

	// Token: 0x060002CE RID: 718 RVA: 0x00011BFC File Offset: 0x0000FDFC
	private void Jump()
	{
		if (this.data.sinceJump < 0.15f)
		{
			return;
		}
		Vector3 a = Vector3.up;
		Vector3 vector = this.data.groundPos;
		if (this.data.sinceGrounded < 0.1f)
		{
			if (this.data.sinceGrounded > 0.05f)
			{
				vector = base.transform.position;
			}
			this.data.currentJumps = this.data.jumps;
		}
		else if (this.data.sinceWallGrab < 0.1f)
		{
			a = Vector2.up + this.data.wallNormal;
			vector = this.data.wallPos;
			this.data.currentJumps = this.data.jumps;
		}
		else
		{
			if (this.data.currentJumps <= 0)
			{
				return;
			}
			vector = base.transform.position;
		}
		this.data.currentJumps--;
		this.rig.velocity = new Vector3(this.rig.velocity.x, 0f);
		this.health.TakeForce(Vector2.up * this.jumpForce * 1f * (1f - this.stats.GetSlow()) * this.rig.mass, 1, false, false, 0f);
		this.data.sinceJump = 0f;
		this.data.sinceGrounded = 0f;
		for (int i = 0; i < this.jumpPart.Length; i++)
		{
			this.jumpPart[i].transform.position = new Vector3(vector.x, vector.y, 5f) - a * 0f;
			this.jumpPart[i].transform.rotation = Quaternion.LookRotation(this.rig.velocity);
			this.jumpPart[i].Play();
		}
	}

	// Token: 0x040003DE RID: 990
	public float jumpForce;

	// Token: 0x040003DF RID: 991
	public float force;

	// Token: 0x040003E0 RID: 992
	public float torque;

	// Token: 0x040003E1 RID: 993
	public float drag = 1f;

	// Token: 0x040003E2 RID: 994
	public float Angulardrag = 1f;

	// Token: 0x040003E3 RID: 995
	public ParticleSystem[] jumpPart;

	// Token: 0x040003E4 RID: 996
	private GeneralInput input;

	// Token: 0x040003E5 RID: 997
	private Rigidbody2D rig;

	// Token: 0x040003E6 RID: 998
	private CharacterData data;

	// Token: 0x040003E7 RID: 999
	private CharacterStatModifiers stats;

	// Token: 0x040003E8 RID: 1000
	public Vector2 knockBack;

	// Token: 0x040003E9 RID: 1001
	private HealthHandler health;

	// Token: 0x040003EA RID: 1002
	private Vector3 moveDir;

	// Token: 0x040003EB RID: 1003
	private Vector3 idleForce;
}
