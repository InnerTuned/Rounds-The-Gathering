using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001B9 RID: 441
public class StartEvent : MonoBehaviour
{
	// Token: 0x060008C3 RID: 2243 RVA: 0x0002E183 File Offset: 0x0002C383
	private void Start()
	{
		this.startEvent.Invoke();
	}

	// Token: 0x040009FF RID: 2559
	public UnityEvent startEvent;
}
