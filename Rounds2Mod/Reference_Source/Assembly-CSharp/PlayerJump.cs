using System;
using UnityEngine;

// Token: 0x0200009B RID: 155
public class PlayerJump : MonoBehaviour
{
	// Token: 0x0600035E RID: 862 RVA: 0x00014DB4 File Offset: 0x00012FB4
	private void Start()
	{
		this.stats = base.GetComponent<CharacterStatModifiers>();
		this.data = base.GetComponent<CharacterData>();
	}

	// Token: 0x0600035F RID: 863 RVA: 0x00014DD0 File Offset: 0x00012FD0
	private void Update()
	{
		if (this.data.input.jumpWasPressed)
		{
			this.Jump(false, 1f);
		}
		if (this.data.input.jumpIsPressed && this.data.sinceJump < 0.2f)
		{
			this.data.playerVel.AddForce(Vector2.up * TimeHandler.deltaTime * 2f * this.data.stats.jump * this.data.playerVel.mass * (1f - this.stats.GetSlow()) * this.upForce, 0);
		}
	}

	// Token: 0x06000360 RID: 864 RVA: 0x00014E98 File Offset: 0x00013098
	public void Jump(bool forceJump = false, float multiplier = 1f)
	{
		if (!forceJump)
		{
			if (this.data.sinceJump < 0.1f)
			{
				return;
			}
			if (this.data.currentJumps <= 0 && this.data.sinceWallGrab > 0.1f)
			{
				return;
			}
		}
		Vector3 a = Vector3.up;
		Vector3 vector = this.data.groundPos;
		if (this.JumpAction != null)
		{
			this.JumpAction.Invoke();
		}
		bool flag = false;
		if (this.data.sinceWallGrab < 0.1f && !this.data.isGrounded)
		{
			a = Vector2.up * 0.8f + this.data.wallNormal * 0.4f;
			vector = this.data.wallPos;
			this.data.currentJumps = this.data.jumps;
			flag = true;
		}
		else
		{
			if (this.data.sinceGrounded > 0.05f)
			{
				vector = base.transform.position;
			}
			this.data.currentJumps = this.data.jumps;
		}
		if (this.data.playerVel.velocity.y < 0f)
		{
			this.data.playerVel.velocity = new Vector2(this.data.playerVel.velocity.x, 0f);
		}
		this.data.sinceGrounded = 0f;
		this.data.sinceJump = 0f;
		this.data.isGrounded = false;
		this.data.isWallGrab = false;
		this.data.currentJumps--;
		this.data.playerVel.AddForce(a * multiplier * 0.01f * this.data.stats.jump * this.data.playerVel.mass * (1f - this.stats.GetSlow()) * this.upForce, 1);
		if (!flag)
		{
			this.data.playerVel.AddForce(Vector2.right * multiplier * this.sideForce * 0.01f * this.data.stats.jump * this.data.playerVel.mass * (1f - this.stats.GetSlow()) * this.data.playerVel.velocity.x, 1);
		}
		for (int i = 0; i < this.jumpPart.Length; i++)
		{
			this.jumpPart[i].transform.position = new Vector3(vector.x, vector.y, 5f) - a * 0f;
			this.jumpPart[i].transform.rotation = Quaternion.LookRotation(this.data.playerVel.velocity);
			this.jumpPart[i].Play();
		}
	}

	// Token: 0x0400048E RID: 1166
	private CharacterData data;

	// Token: 0x0400048F RID: 1167
	public float upForce;

	// Token: 0x04000490 RID: 1168
	private CharacterStatModifiers stats;

	// Token: 0x04000491 RID: 1169
	public ParticleSystem[] jumpPart;

	// Token: 0x04000492 RID: 1170
	public float sideForce = 1f;

	// Token: 0x04000493 RID: 1171
	public Action JumpAction;
}
