using System;
using UnityEngine;

// Token: 0x0200019E RID: 414
public class Rage : MonoBehaviour
{
	// Token: 0x06000859 RID: 2137 RVA: 0x0002CB50 File Offset: 0x0002AD50
	private void Start()
	{
		this.player = base.GetComponentInParent<Player>();
		this.level = base.GetComponentInParent<AttackLevel>();
		this.part = base.GetComponentInChildren<ParticleSystem>().emission;
		this.player.data.SetWobbleObjectChild(base.GetComponentInChildren<ParticleSystem>().transform);
	}

	// Token: 0x0600085A RID: 2138 RVA: 0x0002CBA4 File Offset: 0x0002ADA4
	private void Update()
	{
		float healthPercentage = this.player.data.HealthPercentage;
		this.player.data.stats.rageSpeed = Mathf.Pow(this.curve.Evaluate(healthPercentage), this.level.LevelScale());
		this.part.rateOverTime = this.partCurve.Evaluate(this.player.data.stats.rageSpeed);
	}

	// Token: 0x0400099A RID: 2458
	private Player player;

	// Token: 0x0400099B RID: 2459
	private AttackLevel level;

	// Token: 0x0400099C RID: 2460
	public AnimationCurve curve;

	// Token: 0x0400099D RID: 2461
	public AnimationCurve partCurve;

	// Token: 0x0400099E RID: 2462
	private ParticleSystem.EmissionModule part;
}
