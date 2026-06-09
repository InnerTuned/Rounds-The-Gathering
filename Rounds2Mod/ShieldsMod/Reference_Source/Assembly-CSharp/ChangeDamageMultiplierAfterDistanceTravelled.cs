using System;
using UnityEngine;

// Token: 0x0200002A RID: 42
public class ChangeDamageMultiplierAfterDistanceTravelled : MonoBehaviour
{
	// Token: 0x060000C2 RID: 194 RVA: 0x000063CA File Offset: 0x000045CA
	private void Awake()
	{
		this.trail = base.GetComponentInChildren<TrailRenderer>();
	}

	// Token: 0x060000C3 RID: 195 RVA: 0x000063D8 File Offset: 0x000045D8
	private void Start()
	{
		this.hit = base.GetComponent<ProjectileHit>();
		this.move = base.GetComponent<MoveTransform>();
	}

	// Token: 0x060000C4 RID: 196 RVA: 0x000063F4 File Offset: 0x000045F4
	private void Update()
	{
		if (this.move.distanceTravelled > this.distance)
		{
			this.hit.damage *= this.muiltiplier;
			this.hit.shake *= this.muiltiplier;
			if (this.trail)
			{
				this.trail.widthMultiplier *= 2f;
			}
			Object.Destroy(this);
		}
	}

	// Token: 0x040000C9 RID: 201
	public float muiltiplier = 2f;

	// Token: 0x040000CA RID: 202
	public float distance = 10f;

	// Token: 0x040000CB RID: 203
	private ProjectileHit hit;

	// Token: 0x040000CC RID: 204
	private MoveTransform move;

	// Token: 0x040000CD RID: 205
	private TrailRenderer trail;
}
