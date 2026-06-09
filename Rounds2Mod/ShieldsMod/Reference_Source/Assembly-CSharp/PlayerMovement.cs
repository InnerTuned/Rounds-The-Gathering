using System;
using UnityEngine;

// Token: 0x0200009E RID: 158
public class PlayerMovement : MonoBehaviour
{
	// Token: 0x06000387 RID: 903 RVA: 0x00015B82 File Offset: 0x00013D82
	private void Start()
	{
		this.data = base.GetComponent<CharacterData>();
		this.stats = base.GetComponent<CharacterStatModifiers>();
	}

	// Token: 0x06000388 RID: 904 RVA: 0x00015B9C File Offset: 0x00013D9C
	private void FixedUpdate()
	{
		if (!this.data.isPlaying)
		{
			return;
		}
		this.Move(this.data.input.direction);
		if (this.data.isWallGrab && this.data.wallDistance < 0.7f)
		{
			Vector2 velocity = this.data.playerVel.velocity;
			if (this.data.input.direction.y >= 0f)
			{
				float x = this.data.input.direction.x;
			}
			this.data.playerVel.velocity = velocity;
		}
		this.data.playerVel.velocity -= this.data.playerVel.velocity * TimeHandler.timeScale * 0.01f * 0.1f * this.extraDrag * this.multiplier;
		this.data.playerVel.angularVelocity -= this.data.playerVel.angularVelocity * TimeHandler.timeScale * 0.01f * 0.1f * this.extraAngularDrag * this.multiplier;
	}

	// Token: 0x06000389 RID: 905 RVA: 0x000027C8 File Offset: 0x000009C8
	private void Update()
	{
	}

	// Token: 0x0600038A RID: 906 RVA: 0x00015CF4 File Offset: 0x00013EF4
	public void Move(Vector2 direction)
	{
		this.UpdateMultiplier();
		if (this.data.isStunned)
		{
			return;
		}
		direction.y = Mathf.Clamp(direction.y, -1f, 0f);
		direction.y *= 2f;
		this.data.playerVel.AddForce(direction * TimeHandler.timeScale * (1f - this.stats.GetSlow()) * this.stats.movementSpeed * this.force * this.data.playerVel.mass * 0.01f * this.multiplier, 0);
	}

	// Token: 0x0600038B RID: 907 RVA: 0x00015DB8 File Offset: 0x00013FB8
	private void UpdateMultiplier()
	{
		this.multiplier = 1f;
		if (!this.data.isGrounded)
		{
			this.multiplier = this.airControl;
		}
	}

	// Token: 0x040004A0 RID: 1184
	public float force;

	// Token: 0x040004A1 RID: 1185
	public float airControl = 0.3f;

	// Token: 0x040004A2 RID: 1186
	public float extraDrag;

	// Token: 0x040004A3 RID: 1187
	public float extraAngularDrag;

	// Token: 0x040004A4 RID: 1188
	public float wallGrabDrag;

	// Token: 0x040004A5 RID: 1189
	private CharacterData data;

	// Token: 0x040004A6 RID: 1190
	private CharacterStatModifiers stats;

	// Token: 0x040004A7 RID: 1191
	private float multiplier = 1f;
}
