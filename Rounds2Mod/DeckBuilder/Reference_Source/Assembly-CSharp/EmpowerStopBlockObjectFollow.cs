using System;
using UnityEngine;

// Token: 0x02000135 RID: 309
public class EmpowerStopBlockObjectFollow : MonoBehaviour
{
	// Token: 0x060005E4 RID: 1508 RVA: 0x000210E0 File Offset: 0x0001F2E0
	private void Start()
	{
		Block componentInParent = base.GetComponentInParent<Block>();
		componentInParent.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Combine(componentInParent.BlockAction, new Action<BlockTrigger.BlockTriggerType>(this.Block));
	}

	// Token: 0x060005E5 RID: 1509 RVA: 0x00021109 File Offset: 0x0001F309
	private void OnDestroy()
	{
		Block componentInParent = base.GetComponentInParent<Block>();
		componentInParent.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(componentInParent.BlockAction, new Action<BlockTrigger.BlockTriggerType>(this.Block));
	}

	// Token: 0x060005E6 RID: 1510 RVA: 0x00021132 File Offset: 0x0001F332
	private void Block(BlockTrigger.BlockTriggerType triggerTyp)
	{
		if (triggerTyp == BlockTrigger.BlockTriggerType.Empower && base.GetComponent<SpawnObjects>())
		{
			base.GetComponent<SpawnObjects>().mostRecentlySpawnedObject.GetComponent<FollowPlayer>().enabled = false;
		}
	}
}
