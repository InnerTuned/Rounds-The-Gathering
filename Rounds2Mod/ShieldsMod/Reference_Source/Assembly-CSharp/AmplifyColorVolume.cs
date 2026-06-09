using System;
using UnityEngine;

// Token: 0x02000143 RID: 323
[RequireComponent(typeof(BoxCollider))]
[AddComponentMenu("Image Effects/Amplify Color Volume")]
public class AmplifyColorVolume : AmplifyColorVolumeBase
{
	// Token: 0x06000631 RID: 1585 RVA: 0x00023188 File Offset: 0x00021388
	private void OnTriggerEnter(Collider other)
	{
		AmplifyColorTriggerProxy component = other.GetComponent<AmplifyColorTriggerProxy>();
		if (component != null && component.OwnerEffect.UseVolumes && (component.OwnerEffect.VolumeCollisionMask & 1 << base.gameObject.layer) != 0)
		{
			component.OwnerEffect.EnterVolume(this);
		}
	}

	// Token: 0x06000632 RID: 1586 RVA: 0x000231E4 File Offset: 0x000213E4
	private void OnTriggerExit(Collider other)
	{
		AmplifyColorTriggerProxy component = other.GetComponent<AmplifyColorTriggerProxy>();
		if (component != null && component.OwnerEffect.UseVolumes && (component.OwnerEffect.VolumeCollisionMask & 1 << base.gameObject.layer) != 0)
		{
			component.OwnerEffect.ExitVolume(this);
		}
	}
}
