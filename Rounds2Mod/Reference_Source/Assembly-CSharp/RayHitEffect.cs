using System;
using UnityEngine;

// Token: 0x020000B5 RID: 181
public abstract class RayHitEffect : MonoBehaviour
{
	// Token: 0x060003E5 RID: 997
	public abstract HasToReturn DoHitEffect(HitInfo hit);

	// Token: 0x04000553 RID: 1363
	public int priority;
}
