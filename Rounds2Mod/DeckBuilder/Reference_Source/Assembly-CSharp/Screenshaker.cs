using System;
using UnityEngine;

// Token: 0x020000C7 RID: 199
public class Screenshaker : GameFeeler
{
	// Token: 0x0600042A RID: 1066 RVA: 0x000197E8 File Offset: 0x000179E8
	private void Update()
	{
		float d = Mathf.Clamp(this.ignoreTimeScale ? Time.unscaledDeltaTime : TimeHandler.deltaTime, 0f, 0.015f);
		this.velocity -= this.velocity.normalized * Mathf.Pow(this.velocity.magnitude, 0.8f) * this.damper * d;
		this.velocity -= base.transform.localPosition * d * this.spring;
		base.transform.position += this.velocity * d;
	}

	// Token: 0x0600042B RID: 1067 RVA: 0x000198B9 File Offset: 0x00017AB9
	private void ShakeInternal(Vector2 direction)
	{
		if (direction.magnitude < this.threshold)
		{
			return;
		}
		direction = Vector2.ClampMagnitude(direction, this.clamp);
		this.velocity += direction * this.shakeforce;
	}

	// Token: 0x0600042C RID: 1068 RVA: 0x000198F6 File Offset: 0x00017AF6
	public override void OnGameFeel(Vector2 feelDirection)
	{
		if (this.ignoreTimeScale)
		{
			return;
		}
		feelDirection = Vector2.ClampMagnitude(feelDirection, this.clamp);
		this.ShakeInternal(feelDirection);
	}

	// Token: 0x0600042D RID: 1069 RVA: 0x00019916 File Offset: 0x00017B16
	public override void OnUIGameFeel(Vector2 feelDirection)
	{
		if (!this.ignoreTimeScale)
		{
			return;
		}
		feelDirection = Vector2.ClampMagnitude(feelDirection, this.clamp);
		this.ShakeInternal(feelDirection);
	}

	// Token: 0x040005B3 RID: 1459
	public bool ignoreTimeScale;

	// Token: 0x040005B4 RID: 1460
	public float spring = 100f;

	// Token: 0x040005B5 RID: 1461
	public float damper = 100f;

	// Token: 0x040005B6 RID: 1462
	public float shakeforce = 10f;

	// Token: 0x040005B7 RID: 1463
	public float threshold;

	// Token: 0x040005B8 RID: 1464
	private Vector2 velocity;

	// Token: 0x040005B9 RID: 1465
	private float clamp = 100f;
}
