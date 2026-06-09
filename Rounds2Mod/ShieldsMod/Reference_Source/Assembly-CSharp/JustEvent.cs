using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200015B RID: 347
public class JustEvent : MonoBehaviour
{
	// Token: 0x060006FD RID: 1789 RVA: 0x000267E0 File Offset: 0x000249E0
	public void Go()
	{
		this.justEvent.Invoke();
	}

	// Token: 0x04000868 RID: 2152
	public UnityEvent justEvent;
}
