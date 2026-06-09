using System;
using UnityEngine;

// Token: 0x020000CA RID: 202
public class SetEmissionOverTimeByVelocity : MonoBehaviour
{
	// Token: 0x06000434 RID: 1076 RVA: 0x000199F0 File Offset: 0x00017BF0
	private void Start()
	{
		this.rig = base.GetComponentInParent<PlayerVelocity>();
		this.part = base.GetComponent<ParticleSystem>();
	}

	// Token: 0x06000435 RID: 1077 RVA: 0x00019A0C File Offset: 0x00017C0C
	private void Update()
	{
		this.part.emission.rateOverDistance = this.curve.Evaluate(this.rig.velocity.magnitude);
	}

	// Token: 0x040005BF RID: 1471
	private ParticleSystem part;

	// Token: 0x040005C0 RID: 1472
	public AnimationCurve curve;

	// Token: 0x040005C1 RID: 1473
	private PlayerVelocity rig;
}
