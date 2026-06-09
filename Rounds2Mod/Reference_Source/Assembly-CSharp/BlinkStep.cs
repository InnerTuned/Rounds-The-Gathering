using System;
using UnityEngine;

// Token: 0x020000FD RID: 253
public class BlinkStep : MonoBehaviour
{
	// Token: 0x0600050B RID: 1291 RVA: 0x0001CE1A File Offset: 0x0001B01A
	private void Start()
	{
		this.data = base.GetComponentInParent<CharacterData>();
	}

	// Token: 0x0600050C RID: 1292 RVA: 0x0001CE28 File Offset: 0x0001B028
	private void Update()
	{
		if (!this.data.view.IsMine)
		{
			return;
		}
		this.counter += TimeHandler.deltaTime;
		if (this.counter > this.interval)
		{
			this.counter = 0f;
			this.data.block.CallDoBlock(true, true, BlockTrigger.BlockTriggerType.Default, default(Vector3), false);
		}
	}

	// Token: 0x040006A7 RID: 1703
	public float interval = 0.29f;

	// Token: 0x040006A8 RID: 1704
	private CharacterData data;

	// Token: 0x040006A9 RID: 1705
	private float counter;
}
