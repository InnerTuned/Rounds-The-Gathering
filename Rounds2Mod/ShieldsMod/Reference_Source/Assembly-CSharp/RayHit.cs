using System;
using UnityEngine;

// Token: 0x020000B1 RID: 177
public abstract class RayHit : MonoBehaviour
{
	// Token: 0x060003DC RID: 988
	public abstract void Hit(HitInfo hit, bool forceCall = false);
}
