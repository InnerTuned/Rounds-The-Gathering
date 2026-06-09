using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200012C RID: 300
public class DisableEvent : MonoBehaviour
{
	// Token: 0x060005CE RID: 1486 RVA: 0x00020D52 File Offset: 0x0001EF52
	public void OnDisable()
	{
		this.disableEvent.Invoke();
	}

	// Token: 0x0400076C RID: 1900
	public UnityEvent disableEvent;
}
