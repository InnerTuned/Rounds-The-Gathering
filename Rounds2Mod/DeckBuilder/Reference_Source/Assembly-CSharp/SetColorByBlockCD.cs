using System;
using UnityEngine;

// Token: 0x020000C9 RID: 201
public class SetColorByBlockCD : MonoBehaviour
{
	// Token: 0x06000431 RID: 1073 RVA: 0x00019978 File Offset: 0x00017B78
	private void Start()
	{
		this.block = base.GetComponentInParent<Block>();
		this.spriteRend = base.GetComponent<SpriteRenderer>();
	}

	// Token: 0x06000432 RID: 1074 RVA: 0x00019994 File Offset: 0x00017B94
	private void Update()
	{
		if (this.isOnCD != this.block.IsOnCD())
		{
			this.isOnCD = this.block.IsOnCD();
			if (this.isOnCD)
			{
				this.spriteRend.color = this.onCDColor;
				return;
			}
			this.spriteRend.color = this.offCDColor;
		}
	}

	// Token: 0x040005BA RID: 1466
	private Block block;

	// Token: 0x040005BB RID: 1467
	private SpriteRenderer spriteRend;

	// Token: 0x040005BC RID: 1468
	public Color onCDColor;

	// Token: 0x040005BD RID: 1469
	public Color offCDColor;

	// Token: 0x040005BE RID: 1470
	private bool isOnCD;
}
