using System;
using UnityEngine;

// Token: 0x0200001A RID: 26
public class BounceTrigger : MonoBehaviour
{
	// Token: 0x06000089 RID: 137 RVA: 0x000051E2 File Offset: 0x000033E2
	private void Start()
	{
		this.bounceEffects = base.GetComponents<BounceEffect>();
		RayHitReflect componentInParent = base.GetComponentInParent<RayHitReflect>();
		componentInParent.reflectAction = (Action<HitInfo>)Delegate.Combine(componentInParent.reflectAction, new Action<HitInfo>(this.Reflect));
	}

	// Token: 0x0600008A RID: 138 RVA: 0x00005218 File Offset: 0x00003418
	public void Reflect(HitInfo hit)
	{
		for (int i = 0; i < this.bounceEffects.Length; i++)
		{
			this.bounceEffects[i].DoBounce(hit);
		}
	}

	// Token: 0x04000085 RID: 133
	private BounceEffect[] bounceEffects;
}
