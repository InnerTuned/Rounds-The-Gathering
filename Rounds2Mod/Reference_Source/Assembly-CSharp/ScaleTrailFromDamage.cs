using System;
using UnityEngine;

// Token: 0x020000C4 RID: 196
public class ScaleTrailFromDamage : MonoBehaviour
{
	// Token: 0x06000420 RID: 1056 RVA: 0x0001924C File Offset: 0x0001744C
	private void Start()
	{
		float num = 55f;
		ProjectileHit componentInParent = base.GetComponentInParent<ProjectileHit>();
		base.GetComponentInParent<RayCastTrail>();
		if (componentInParent)
		{
			num = componentInParent.damage;
		}
		TrailRenderer componentInChildren = base.GetComponentInChildren<TrailRenderer>();
		this.startWidth = componentInChildren.widthMultiplier;
		this.startTime = componentInChildren.time;
		if (componentInChildren)
		{
			componentInChildren.widthMultiplier *= (1f + num / 55f) / 2f;
			componentInChildren.time *= Mathf.Clamp((1f + num / 55f) / 2f, 0f, 25f);
			if (num > 100f)
			{
				componentInChildren.numCapVertices = 5;
			}
			if (num > 500f)
			{
				componentInChildren.numCapVertices = 10;
			}
		}
	}

	// Token: 0x06000421 RID: 1057 RVA: 0x00019314 File Offset: 0x00017514
	public void Rescale()
	{
		float num = 55f;
		ProjectileHit componentInParent = base.GetComponentInParent<ProjectileHit>();
		RayCastTrail componentInParent2 = base.GetComponentInParent<RayCastTrail>();
		if (componentInParent)
		{
			num = componentInParent.damage;
		}
		TrailRenderer componentInChildren = base.GetComponentInChildren<TrailRenderer>();
		if (componentInChildren)
		{
			float extraSize = componentInParent2.extraSize;
			componentInChildren.widthMultiplier = this.startWidth * ((1f + num / 55f) / 2f);
			componentInChildren.time = this.startTime * Mathf.Clamp((1f + num / 55f) / 2f, 0f, 25f);
			if (num > 100f)
			{
				componentInChildren.numCapVertices = 5;
			}
			if (num > 100f)
			{
				componentInChildren.numCapVertices = 10;
			}
		}
	}

	// Token: 0x040005A7 RID: 1447
	private float startWidth;

	// Token: 0x040005A8 RID: 1448
	private float startTime;
}
