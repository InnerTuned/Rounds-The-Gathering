using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000E6 RID: 230
public class Teleport : MonoBehaviour
{
	// Token: 0x06000489 RID: 1161 RVA: 0x0001B084 File Offset: 0x00019284
	private void Start()
	{
		this.parts = base.GetComponentsInChildren<ParticleSystem>();
		this.data = base.GetComponentInParent<CharacterData>();
		this.level = base.GetComponentInParent<AttackLevel>();
		Block componentInParent = base.GetComponentInParent<Block>();
		componentInParent.SuperFirstBlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Combine(componentInParent.SuperFirstBlockAction, new Action<BlockTrigger.BlockTriggerType>(this.Go));
	}

	// Token: 0x0600048A RID: 1162 RVA: 0x0001B0DC File Offset: 0x000192DC
	private void OnDestroy()
	{
		Block componentInParent = base.GetComponentInParent<Block>();
		componentInParent.SuperFirstBlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(componentInParent.SuperFirstBlockAction, new Action<BlockTrigger.BlockTriggerType>(this.Go));
	}

	// Token: 0x0600048B RID: 1163 RVA: 0x0001B105 File Offset: 0x00019305
	public void Go(BlockTrigger.BlockTriggerType triggerType)
	{
		base.StartCoroutine(this.DelayMove(triggerType, base.transform.position));
	}

	// Token: 0x0600048C RID: 1164 RVA: 0x0001B120 File Offset: 0x00019320
	private IEnumerator DelayMove(BlockTrigger.BlockTriggerType triggerType, Vector3 beforePos)
	{
		if (triggerType == BlockTrigger.BlockTriggerType.Empower)
		{
			yield return new WaitForSeconds(0f);
		}
		Vector3 vector = base.transform.position;
		Vector3 position = base.transform.position;
		int num = 10;
		float d = this.distance * (float)this.level.attackLevel / (float)num;
		for (int i = 0; i < num; i++)
		{
			vector += d * this.data.aimDirection;
			if (!Physics2D.OverlapCircle(vector, 0.5f))
			{
				position = vector;
			}
		}
		for (int j = 0; j < this.remainParts.Length; j++)
		{
			this.remainParts[j].transform.position = base.transform.root.position;
			this.remainParts[j].Play();
		}
		base.GetComponentInParent<PlayerCollision>().IgnoreWallForFrames(2);
		if (triggerType == BlockTrigger.BlockTriggerType.Empower)
		{
			position = beforePos;
		}
		base.transform.root.position = position;
		for (int k = 0; k < this.parts.Length; k++)
		{
			this.parts[k].transform.position = position;
			this.parts[k].Play();
		}
		this.data.playerVel.velocity *= 0f;
		this.data.sinceGrounded = 0f;
		yield break;
	}

	// Token: 0x04000620 RID: 1568
	public ParticleSystem[] parts;

	// Token: 0x04000621 RID: 1569
	public ParticleSystem[] remainParts;

	// Token: 0x04000622 RID: 1570
	public float distance = 10f;

	// Token: 0x04000623 RID: 1571
	public LayerMask mask;

	// Token: 0x04000624 RID: 1572
	private CharacterData data;

	// Token: 0x04000625 RID: 1573
	private AttackLevel level;
}
