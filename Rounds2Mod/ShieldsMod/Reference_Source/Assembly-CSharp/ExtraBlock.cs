using System;
using UnityEngine;

// Token: 0x02000054 RID: 84
public class ExtraBlock : MonoBehaviour
{
	// Token: 0x0600018D RID: 397 RVA: 0x0000A72B File Offset: 0x0000892B
	private void Start()
	{
		this.data = base.GetComponentInParent<CharacterData>();
	}

	// Token: 0x0600018E RID: 398 RVA: 0x0000A73C File Offset: 0x0000893C
	public void Go()
	{
		this.data.block.RPCA_DoBlock(false, true, BlockTrigger.BlockTriggerType.Default, default(Vector3), false);
	}

	// Token: 0x04000228 RID: 552
	private CharacterData data;
}
