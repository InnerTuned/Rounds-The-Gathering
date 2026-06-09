using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000136 RID: 310
public class EnableEvent : MonoBehaviour
{
	// Token: 0x060005E8 RID: 1512 RVA: 0x0002115B File Offset: 0x0001F35B
	public void OnEnable()
	{
		this.enableEvent.Invoke();
	}

	// Token: 0x04000779 RID: 1913
	public UnityEvent enableEvent;
}
