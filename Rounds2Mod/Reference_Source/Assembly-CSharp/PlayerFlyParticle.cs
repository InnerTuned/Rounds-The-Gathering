using System;
using UnityEngine;

// Token: 0x02000189 RID: 393
public class PlayerFlyParticle : MonoBehaviour
{
	// Token: 0x060007FD RID: 2045 RVA: 0x0002BC28 File Offset: 0x00029E28
	private void Start()
	{
		this.part = base.GetComponent<ParticleSystem>();
		this.health = base.GetComponentInParent<HealthHandler>();
	}

	// Token: 0x060007FE RID: 2046 RVA: 0x0002BC44 File Offset: 0x00029E44
	private void Update()
	{
		if (this.part.isPlaying)
		{
			if (this.health.flyingFor < 0f)
			{
				this.part.Stop();
				return;
			}
		}
		else if (this.health.flyingFor > 0f)
		{
			this.part.Play();
		}
	}

	// Token: 0x04000959 RID: 2393
	private ParticleSystem part;

	// Token: 0x0400095A RID: 2394
	private HealthHandler health;
}
