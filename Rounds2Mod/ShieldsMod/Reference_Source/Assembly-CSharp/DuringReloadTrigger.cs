using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000131 RID: 305
public class DuringReloadTrigger : MonoBehaviour
{
	// Token: 0x060005DA RID: 1498 RVA: 0x00020F1B File Offset: 0x0001F11B
	private void Start()
	{
		this.gunAmmo = base.GetComponentInParent<WeaponHandler>().gun;
	}

	// Token: 0x060005DB RID: 1499 RVA: 0x00020F30 File Offset: 0x0001F130
	private void Update()
	{
		if (this.gunAmmo.isReloading)
		{
			if (!this.triggering)
			{
				this.triggerStartEvent.Invoke();
				this.triggering = true;
			}
			this.triggerEvent.Invoke();
			return;
		}
		if (this.triggering)
		{
			this.triggerEndEvent.Invoke();
			this.triggering = false;
		}
	}

	// Token: 0x04000772 RID: 1906
	public UnityEvent triggerEvent;

	// Token: 0x04000773 RID: 1907
	public UnityEvent triggerStartEvent;

	// Token: 0x04000774 RID: 1908
	public UnityEvent triggerEndEvent;

	// Token: 0x04000775 RID: 1909
	private Gun gunAmmo;

	// Token: 0x04000776 RID: 1910
	private bool triggering;
}
