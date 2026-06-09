using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001A5 RID: 421
public class RespawnEvent : MonoBehaviour
{
	// Token: 0x06000874 RID: 2164 RVA: 0x0002D242 File Offset: 0x0002B442
	private void Start()
	{
		HealthHandler healthHandler = base.GetComponentInParent<Player>().data.healthHandler;
		healthHandler.reviveAction = (Action)Delegate.Combine(healthHandler.reviveAction, new Action(this.DoEvent));
	}

	// Token: 0x06000875 RID: 2165 RVA: 0x0002D275 File Offset: 0x0002B475
	public void DoEvent()
	{
		this.reviveEvent.Invoke();
	}

	// Token: 0x040009B2 RID: 2482
	public UnityEvent reviveEvent;
}
