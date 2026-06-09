using System;
using UnityEngine;

// Token: 0x020000BC RID: 188
public class ResetBlock : MonoBehaviour
{
	// Token: 0x060003FB RID: 1019 RVA: 0x0001869A File Offset: 0x0001689A
	private void Start()
	{
		this.block = base.transform.root.GetComponent<Block>();
	}

	// Token: 0x060003FC RID: 1020 RVA: 0x000186B2 File Offset: 0x000168B2
	public void Go()
	{
		this.block.ResetCD(false);
	}

	// Token: 0x04000578 RID: 1400
	private Block block;
}
