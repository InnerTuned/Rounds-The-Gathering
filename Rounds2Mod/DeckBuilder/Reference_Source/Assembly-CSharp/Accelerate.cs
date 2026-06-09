using System;
using UnityEngine;

// Token: 0x02000004 RID: 4
public class Accelerate : MonoBehaviour
{
	// Token: 0x0600001D RID: 29 RVA: 0x00002B64 File Offset: 0x00000D64
	private void Start()
	{
		this.move = base.GetComponentInParent<MoveTransform>();
		this.move.multiplier *= this.startMultiplier;
		this.move.multiplier = Mathf.Clamp(this.move.multiplier, 0.01f, float.PositiveInfinity);
	}

	// Token: 0x0600001E RID: 30 RVA: 0x00002BBC File Offset: 0x00000DBC
	private void Update()
	{
		this.move.multiplier = Mathf.Clamp(this.move.multiplier + TimeHandler.deltaTime * this.acceleratonPerSecond * Mathf.Pow(this.move.multiplier, this.pow), 0f, 25f);
	}

	// Token: 0x0400000F RID: 15
	private MoveTransform move;

	// Token: 0x04000010 RID: 16
	public float startMultiplier = 0.5f;

	// Token: 0x04000011 RID: 17
	public float acceleratonPerSecond = 2f;

	// Token: 0x04000012 RID: 18
	public float pow = 1f;
}
