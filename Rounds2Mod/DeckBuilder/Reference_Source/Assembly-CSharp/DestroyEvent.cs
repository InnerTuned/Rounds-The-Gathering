using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000046 RID: 70
public class DestroyEvent : MonoBehaviour
{
	// Token: 0x06000158 RID: 344 RVA: 0x00008D5B File Offset: 0x00006F5B
	private void OnDestroy()
	{
		if (this.m_isQuitting)
		{
			return;
		}
		this.deathEvent.Invoke();
	}

	// Token: 0x06000159 RID: 345 RVA: 0x00008D71 File Offset: 0x00006F71
	private void OnApplicationQuit()
	{
		this.m_isQuitting = true;
	}

	// Token: 0x040001C9 RID: 457
	public UnityEvent deathEvent;

	// Token: 0x040001CA RID: 458
	private bool m_isQuitting;
}
