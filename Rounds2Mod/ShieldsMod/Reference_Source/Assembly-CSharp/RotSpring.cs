using System;
using UnityEngine;

// Token: 0x020001A9 RID: 425
public class RotSpring : MonoBehaviour
{
	// Token: 0x06000883 RID: 2179 RVA: 0x0002D57C File Offset: 0x0002B77C
	private void Start()
	{
		if (this.x)
		{
			this.currentValue = base.transform.localEulerAngles.x;
			return;
		}
		if (this.y)
		{
			this.currentValue = base.transform.localEulerAngles.y;
			return;
		}
		if (this.z)
		{
			this.currentValue = base.transform.localEulerAngles.z;
		}
	}

	// Token: 0x06000884 RID: 2180 RVA: 0x0002D5E8 File Offset: 0x0002B7E8
	private void Update()
	{
		this.vel = FRILerp.Lerp(this.vel, (this.target - this.currentValue) * this.spring, this.damper);
		this.currentValue += this.vel * TimeHandler.deltaTime;
		base.transform.localEulerAngles = new Vector3(this.x ? this.currentValue : 0f, this.y ? this.currentValue : 0f, this.z ? this.currentValue : 0f);
	}

	// Token: 0x040009C3 RID: 2499
	public bool x;

	// Token: 0x040009C4 RID: 2500
	public bool y;

	// Token: 0x040009C5 RID: 2501
	public bool z;

	// Token: 0x040009C6 RID: 2502
	public float target;

	// Token: 0x040009C7 RID: 2503
	public float spring;

	// Token: 0x040009C8 RID: 2504
	public float damper;

	// Token: 0x040009C9 RID: 2505
	private float currentValue;

	// Token: 0x040009CA RID: 2506
	private float vel;
}
