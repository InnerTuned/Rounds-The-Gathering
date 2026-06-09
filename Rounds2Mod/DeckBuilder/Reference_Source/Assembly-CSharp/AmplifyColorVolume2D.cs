using System;
using UnityEngine;

// Token: 0x02000144 RID: 324
[RequireComponent(typeof(BoxCollider2D))]
[AddComponentMenu("Image Effects/Amplify Color Volume 2D")]
public class AmplifyColorVolume2D : AmplifyColorVolumeBase
{
	// Token: 0x06000634 RID: 1588 RVA: 0x00023248 File Offset: 0x00021448
	private void OnTriggerEnter2D(Collider2D other)
	{
		AmplifyColorTriggerProxy2D component = other.GetComponent<AmplifyColorTriggerProxy2D>();
		if (component != null && component.OwnerEffect.UseVolumes && (component.OwnerEffect.VolumeCollisionMask & 1 << base.gameObject.layer) != 0)
		{
			component.OwnerEffect.EnterVolume(this);
		}
	}

	// Token: 0x06000635 RID: 1589 RVA: 0x000232A4 File Offset: 0x000214A4
	private void OnTriggerExit2D(Collider2D other)
	{
		AmplifyColorTriggerProxy2D component = other.GetComponent<AmplifyColorTriggerProxy2D>();
		if (component != null && component.OwnerEffect.UseVolumes && (component.OwnerEffect.VolumeCollisionMask & 1 << base.gameObject.layer) != 0)
		{
			component.OwnerEffect.ExitVolume(this);
		}
	}
}
