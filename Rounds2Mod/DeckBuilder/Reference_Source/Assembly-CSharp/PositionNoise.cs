using System;
using UnityEngine;

// Token: 0x02000193 RID: 403
public class PositionNoise : CardAnimation
{
	// Token: 0x06000831 RID: 2097 RVA: 0x0002C59D File Offset: 0x0002A79D
	private void Start()
	{
		this.startPos = base.transform.localPosition;
		this.startSeed = Random.Range(0f, 100000f);
	}

	// Token: 0x06000832 RID: 2098 RVA: 0x0002C5C8 File Offset: 0x0002A7C8
	private void Update()
	{
		Vector2 v = new Vector2(Mathf.PerlinNoise(this.startSeed + Time.unscaledTime * this.speed, this.startSeed + Time.unscaledTime * this.speed - 0.5f), Mathf.PerlinNoise(this.startSeed + Time.unscaledTime * this.speed, this.startSeed + Time.unscaledTime * this.speed) - 0.5f);
		base.transform.localPosition = this.startPos + v * this.amount;
	}

	// Token: 0x0400098A RID: 2442
	public float amount;

	// Token: 0x0400098B RID: 2443
	public float speed = 1f;

	// Token: 0x0400098C RID: 2444
	private float startSeed;

	// Token: 0x0400098D RID: 2445
	private Vector3 startPos;
}
