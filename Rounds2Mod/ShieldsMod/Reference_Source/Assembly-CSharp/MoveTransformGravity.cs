using System;
using UnityEngine;

// Token: 0x0200016C RID: 364
public class MoveTransformGravity : MonoBehaviour
{
	// Token: 0x06000750 RID: 1872 RVA: 0x00027B23 File Offset: 0x00025D23
	private void Start()
	{
		this.move = base.GetComponentInParent<MoveTransform>();
	}

	// Token: 0x06000751 RID: 1873 RVA: 0x00027B34 File Offset: 0x00025D34
	private void Update()
	{
		this.counter += TimeHandler.deltaTime;
		this.move.velocity += Vector3.down * Mathf.Pow(this.amount * this.counter, this.pow);
	}

	// Token: 0x040008BD RID: 2237
	public float amount = 1f;

	// Token: 0x040008BE RID: 2238
	public float pow = 1.5f;

	// Token: 0x040008BF RID: 2239
	private MoveTransform move;

	// Token: 0x040008C0 RID: 2240
	private float counter;
}
