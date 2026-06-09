using System;
using UnityEngine;

// Token: 0x0200017E RID: 382
public class PerlinWordScale : MonoBehaviour
{
	// Token: 0x060007C4 RID: 1988 RVA: 0x00029C08 File Offset: 0x00027E08
	private void Start()
	{
		float t = Mathf.PerlinNoise(base.transform.position.x * this.scale, base.transform.position.y * this.scale);
		base.transform.localScale *= Mathf.Lerp(this.min, this.max, t);
	}

	// Token: 0x04000913 RID: 2323
	public float scale = 1f;

	// Token: 0x04000914 RID: 2324
	public float min = 0.5f;

	// Token: 0x04000915 RID: 2325
	public float max = 2f;
}
