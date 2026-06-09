using System;
using UnityEngine;

// Token: 0x020001AA RID: 426
public class ScaleEvent : MonoBehaviour
{
	// Token: 0x06000886 RID: 2182 RVA: 0x0002D688 File Offset: 0x0002B888
	private void Start()
	{
		ScaleEventInstace scaleEventInstace = null;
		float num = 0f;
		for (int i = 0; i < this.events.Length; i++)
		{
			if (base.transform.localScale.x > this.events[i].threshold && this.events[i].threshold > num)
			{
				num = this.events[i].threshold;
				scaleEventInstace = this.events[i];
			}
		}
		if (scaleEventInstace != null)
		{
			scaleEventInstace.scaleEvent.Invoke();
		}
	}

	// Token: 0x040009CB RID: 2507
	public ScaleEventInstace[] events;
}
