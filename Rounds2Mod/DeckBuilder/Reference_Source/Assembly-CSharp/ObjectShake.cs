using System;
using UnityEngine;

// Token: 0x02000088 RID: 136
public class ObjectShake : MonoBehaviour
{
	// Token: 0x060002E2 RID: 738 RVA: 0x000027C8 File Offset: 0x000009C8
	private void Start()
	{
	}

	// Token: 0x060002E3 RID: 739 RVA: 0x000125E4 File Offset: 0x000107E4
	private void Update()
	{
		this.counter += TimeHandler.deltaTime;
		if (this.counter > this.interval)
		{
			base.transform.localPosition = Random.insideUnitCircle * this.movementMultiplier * this.globalMultiplier * 0.1f;
			base.transform.localEulerAngles = new Vector3(base.transform.localEulerAngles.x, base.transform.localEulerAngles.y, (float)Random.Range(-10, 10) * this.globalMultiplier * this.rotationMultiplier);
			this.counter = 0f;
		}
	}

	// Token: 0x04000410 RID: 1040
	public float globalMultiplier = 1f;

	// Token: 0x04000411 RID: 1041
	public float interval = 0.1f;

	// Token: 0x04000412 RID: 1042
	public float movementMultiplier = 1f;

	// Token: 0x04000413 RID: 1043
	public float rotationMultiplier = 1f;

	// Token: 0x04000414 RID: 1044
	private float counter;
}
