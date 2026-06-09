using System;
using UnityEngine;

// Token: 0x020001A4 RID: 420
public class RescaleFromDamagePart : MonoBehaviour
{
	// Token: 0x06000871 RID: 2161 RVA: 0x0002D1CC File Offset: 0x0002B3CC
	private void Start()
	{
		this.part = base.GetComponent<ParticleSystem>();
		this.startSize = this.part.startSize;
	}

	// Token: 0x06000872 RID: 2162 RVA: 0x0002D1EC File Offset: 0x0002B3EC
	public void Rescale()
	{
		if (!this.hit)
		{
			this.hit = base.GetComponentInParent<ProjectileHit>();
		}
		if (this.hit)
		{
			this.part.startSize = this.startSize * (this.hit.damage / 55f);
		}
	}

	// Token: 0x040009AF RID: 2479
	private ProjectileHit hit;

	// Token: 0x040009B0 RID: 2480
	private float startSize;

	// Token: 0x040009B1 RID: 2481
	private ParticleSystem part;
}
