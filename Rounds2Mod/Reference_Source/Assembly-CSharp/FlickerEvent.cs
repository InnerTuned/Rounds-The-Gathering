using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200014A RID: 330
public class FlickerEvent : MonoBehaviour
{
	// Token: 0x060006B1 RID: 1713 RVA: 0x000027C8 File Offset: 0x000009C8
	private void Start()
	{
	}

	// Token: 0x060006B2 RID: 1714 RVA: 0x00025548 File Offset: 0x00023748
	private void Update()
	{
		this.c += TimeHandler.deltaTime;
		if (this.isOn)
		{
			if (this.c > this.interval)
			{
				this.flickedOn = !this.flickedOn;
				if (this.flickedOn)
				{
					this.onEvent.Invoke();
				}
				else
				{
					this.offEvent.Invoke();
				}
				this.c = 0f;
				return;
			}
		}
		else if (this.flickedOn && this.c > this.interval)
		{
			this.c = 0f;
			this.offEvent.Invoke();
			this.flickedOn = false;
		}
	}

	// Token: 0x04000813 RID: 2067
	public UnityEvent onEvent;

	// Token: 0x04000814 RID: 2068
	public UnityEvent offEvent;

	// Token: 0x04000815 RID: 2069
	public float interval;

	// Token: 0x04000816 RID: 2070
	private float c;

	// Token: 0x04000817 RID: 2071
	public bool isOn;

	// Token: 0x04000818 RID: 2072
	private bool flickedOn;
}
