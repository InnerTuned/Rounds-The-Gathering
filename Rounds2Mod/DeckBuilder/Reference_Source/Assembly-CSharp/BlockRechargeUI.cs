using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000FE RID: 254
public class BlockRechargeUI : MonoBehaviour
{
	// Token: 0x0600050E RID: 1294 RVA: 0x0001CEA3 File Offset: 0x0001B0A3
	private void Start()
	{
		this.img = base.GetComponentInChildren<Image>();
		this.block = base.GetComponentInParent<Block>();
	}

	// Token: 0x0600050F RID: 1295 RVA: 0x0001CEBD File Offset: 0x0001B0BD
	private void Update()
	{
		this.img.fillAmount = this.block.counter / this.block.Cooldown();
	}

	// Token: 0x040006AA RID: 1706
	private Block block;

	// Token: 0x040006AB RID: 1707
	private Image img;
}
