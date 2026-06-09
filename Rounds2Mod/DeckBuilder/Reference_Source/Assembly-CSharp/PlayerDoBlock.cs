using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000187 RID: 391
public class PlayerDoBlock : MonoBehaviour
{
	// Token: 0x060007F7 RID: 2039 RVA: 0x0002BBB7 File Offset: 0x00029DB7
	private void Start()
	{
		this.data = base.GetComponentInParent<CharacterData>();
		this.sync = base.GetComponentInParent<SyncPlayerMovement>();
	}

	// Token: 0x060007F8 RID: 2040 RVA: 0x0002BBD1 File Offset: 0x00029DD1
	public void DoBlock()
	{
		if (this.data.view.IsMine)
		{
			this.sync.SendBlock(BlockTrigger.BlockTriggerType.Default, true, true);
		}
	}

	// Token: 0x04000955 RID: 2389
	private CharacterData data;

	// Token: 0x04000956 RID: 2390
	private SyncPlayerMovement sync;
}
