using System;
using UnityEngine;

// Token: 0x020000F9 RID: 249
public class Zip : MonoBehaviour
{
	// Token: 0x060004FA RID: 1274 RVA: 0x000027C8 File Offset: 0x000009C8
	private void Start()
	{
	}

	// Token: 0x060004FB RID: 1275 RVA: 0x0001C68C File Offset: 0x0001A88C
	private void Update()
	{
		this.count += TimeHandler.deltaTime;
		if (this.count > this.turn)
		{
			this.count = 0f;
			this.up *= -1;
		}
		base.transform.root.position += base.transform.up * this.multiplier * (float)this.up * Time.smoothDeltaTime;
	}

	// Token: 0x0400068A RID: 1674
	public float multiplier = 1f;

	// Token: 0x0400068B RID: 1675
	public float turn = 0.2f;

	// Token: 0x0400068C RID: 1676
	private float count;

	// Token: 0x0400068D RID: 1677
	private int up = 1;
}
