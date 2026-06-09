using System;
using UnityEngine;

// Token: 0x020001A3 RID: 419
public class RescaleFromDamage : MonoBehaviour
{
	// Token: 0x0600086E RID: 2158 RVA: 0x0002D14D File Offset: 0x0002B34D
	private void Start()
	{
		this.startScale = base.transform.localScale.x;
	}

	// Token: 0x0600086F RID: 2159 RVA: 0x0002D168 File Offset: 0x0002B368
	public void Rescale()
	{
		if (!this.hit)
		{
			this.hit = base.GetComponentInParent<ProjectileHit>();
		}
		if (this.hit)
		{
			base.transform.localScale = Vector3.one * this.startScale * (this.hit.damage / 55f);
		}
	}

	// Token: 0x040009AD RID: 2477
	private ProjectileHit hit;

	// Token: 0x040009AE RID: 2478
	private float startScale;
}
