using System;
using UnityEngine;

// Token: 0x02000014 RID: 20
public abstract class BlockEffect : MonoBehaviour
{
	// Token: 0x0600006E RID: 110
	public abstract void DoBlockedProjectile(GameObject projectile, Vector3 forward, Vector3 hitPos);
}
