using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000016 RID: 22
public class BlockTrigger : MonoBehaviour
{
	// Token: 0x06000073 RID: 115 RVA: 0x00004B98 File Offset: 0x00002D98
	private void Start()
	{
		this.effects = base.GetComponents<BlockEffect>();
		Block componentInParent = base.GetComponentInParent<Block>();
		componentInParent.SuperFirstBlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Combine(componentInParent.SuperFirstBlockAction, new Action<BlockTrigger.BlockTriggerType>(this.DoSuperFirstBlock));
		componentInParent.FirstBlockActionThatDelaysOthers = (Action<BlockTrigger.BlockTriggerType>)Delegate.Combine(componentInParent.FirstBlockActionThatDelaysOthers, new Action<BlockTrigger.BlockTriggerType>(this.DoFirstBlockThatDelaysOthers));
		componentInParent.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Combine(componentInParent.BlockAction, new Action<BlockTrigger.BlockTriggerType>(this.DoBlock));
		componentInParent.BlockActionEarly = (Action<BlockTrigger.BlockTriggerType>)Delegate.Combine(componentInParent.BlockActionEarly, new Action<BlockTrigger.BlockTriggerType>(this.DoBlockEarly));
		componentInParent.BlockProjectileAction = (Action<GameObject, Vector3, Vector3>)Delegate.Combine(componentInParent.BlockProjectileAction, new Action<GameObject, Vector3, Vector3>(this.DoBlockedProjectile));
		componentInParent.BlockRechargeAction = (Action)Delegate.Combine(componentInParent.BlockRechargeAction, new Action(this.DoBlockRecharge));
		if (this.delayOtherActions)
		{
			base.GetComponentInParent<Block>().delayOtherActions = true;
		}
	}

	// Token: 0x06000074 RID: 116 RVA: 0x00004C98 File Offset: 0x00002E98
	private void OnDestroy()
	{
		Block componentInParent = base.GetComponentInParent<Block>();
		if (componentInParent && componentInParent.SuperFirstBlockAction != null)
		{
			Block block = componentInParent;
			block.SuperFirstBlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(block.SuperFirstBlockAction, new Action<BlockTrigger.BlockTriggerType>(this.DoSuperFirstBlock));
		}
		if (componentInParent && componentInParent.FirstBlockActionThatDelaysOthers != null)
		{
			Block block2 = componentInParent;
			block2.FirstBlockActionThatDelaysOthers = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(block2.FirstBlockActionThatDelaysOthers, new Action<BlockTrigger.BlockTriggerType>(this.DoFirstBlockThatDelaysOthers));
		}
		if (componentInParent && componentInParent.BlockAction != null)
		{
			Block block3 = componentInParent;
			block3.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(block3.BlockAction, new Action<BlockTrigger.BlockTriggerType>(this.DoBlock));
		}
		if (componentInParent && componentInParent.BlockActionEarly != null)
		{
			Block block4 = componentInParent;
			block4.BlockActionEarly = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(block4.BlockActionEarly, new Action<BlockTrigger.BlockTriggerType>(this.DoBlockEarly));
		}
		if (componentInParent && componentInParent.BlockProjectileAction != null)
		{
			Block block5 = componentInParent;
			block5.BlockProjectileAction = (Action<GameObject, Vector3, Vector3>)Delegate.Remove(block5.BlockProjectileAction, new Action<GameObject, Vector3, Vector3>(this.DoBlockedProjectile));
		}
		if (componentInParent && componentInParent.BlockRechargeAction != null)
		{
			Block block6 = componentInParent;
			block6.BlockRechargeAction = (Action)Delegate.Remove(block6.BlockRechargeAction, new Action(this.DoBlockRecharge));
		}
	}

	// Token: 0x06000075 RID: 117 RVA: 0x00004DD8 File Offset: 0x00002FD8
	public void DoSuperFirstBlock(BlockTrigger.BlockTriggerType triggerType)
	{
		if (triggerType == this.blackListedType)
		{
			return;
		}
		if (this.lastTriggerTime + this.cooldown > Time.time)
		{
			return;
		}
		this.lastTriggerTime = Time.time;
		this.triggerSuperFirstBlock.Invoke();
	}

	// Token: 0x06000076 RID: 118 RVA: 0x00004E0F File Offset: 0x0000300F
	public void DoFirstBlockThatDelaysOthers(BlockTrigger.BlockTriggerType triggerType)
	{
		if (triggerType == this.blackListedType)
		{
			return;
		}
		if (this.lastTriggerTime + this.cooldown > Time.time)
		{
			return;
		}
		this.lastTriggerTime = Time.time;
		this.triggerFirstBlockThatDelaysOthers.Invoke();
	}

	// Token: 0x06000077 RID: 119 RVA: 0x00004E46 File Offset: 0x00003046
	public void DoBlockEarly(BlockTrigger.BlockTriggerType triggerType)
	{
		if (triggerType == this.blackListedType)
		{
			return;
		}
		if (this.lastTriggerTime + this.cooldown > Time.time)
		{
			return;
		}
		this.lastTriggerTime = Time.time;
		this.triggerEventEarly.Invoke();
	}

	// Token: 0x06000078 RID: 120 RVA: 0x00004E7D File Offset: 0x0000307D
	public void DoBlock(BlockTrigger.BlockTriggerType triggerType)
	{
		if (triggerType == this.blackListedType)
		{
			return;
		}
		if (this.lastTriggerTime + this.cooldown > Time.time)
		{
			return;
		}
		this.lastTriggerTime = Time.time;
		this.triggerEvent.Invoke();
	}

	// Token: 0x06000079 RID: 121 RVA: 0x00004EB4 File Offset: 0x000030B4
	public void DoBlockedProjectile(GameObject projectile, Vector3 forward, Vector3 hitPos)
	{
		if (this.lastTriggerTimeSuccessful + this.cooldownSuccess > Time.time)
		{
			return;
		}
		this.lastTriggerTimeSuccessful = Time.time;
		this.successfulBlockEvent.Invoke();
		for (int i = 0; i < this.effects.Length; i++)
		{
			this.effects[i].DoBlockedProjectile(projectile, forward, hitPos);
		}
	}

	// Token: 0x0600007A RID: 122 RVA: 0x00004F0F File Offset: 0x0000310F
	public void DoBlockRecharge()
	{
		this.blockRechargeEvent.Invoke();
	}

	// Token: 0x04000074 RID: 116
	public UnityEvent triggerEvent;

	// Token: 0x04000075 RID: 117
	public UnityEvent triggerEventEarly;

	// Token: 0x04000076 RID: 118
	public bool delayOtherActions;

	// Token: 0x04000077 RID: 119
	public UnityEvent triggerFirstBlockThatDelaysOthers;

	// Token: 0x04000078 RID: 120
	public UnityEvent triggerSuperFirstBlock;

	// Token: 0x04000079 RID: 121
	public UnityEvent successfulBlockEvent;

	// Token: 0x0400007A RID: 122
	public UnityEvent blockRechargeEvent;

	// Token: 0x0400007B RID: 123
	private BlockEffect[] effects;

	// Token: 0x0400007C RID: 124
	public float cooldown;

	// Token: 0x0400007D RID: 125
	private float lastTriggerTime = -5f;

	// Token: 0x0400007E RID: 126
	public BlockTrigger.BlockTriggerType blackListedType = BlockTrigger.BlockTriggerType.None;

	// Token: 0x0400007F RID: 127
	public float cooldownSuccess;

	// Token: 0x04000080 RID: 128
	private float lastTriggerTimeSuccessful = -5f;

	// Token: 0x0200032C RID: 812
	public enum BlockTriggerType
	{
		// Token: 0x04001019 RID: 4121
		Default,
		// Token: 0x0400101A RID: 4122
		None,
		// Token: 0x0400101B RID: 4123
		ShieldCharge,
		// Token: 0x0400101C RID: 4124
		Echo,
		// Token: 0x0400101D RID: 4125
		Empower
	}
}
