using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Sonigon;
using UnityEngine;

// Token: 0x02000013 RID: 19
public class Block : MonoBehaviour
{
	// Token: 0x0600005A RID: 90 RVA: 0x0000457C File Offset: 0x0000277C
	public float Cooldown()
	{
		return (this.cooldown + this.cdAdd) * this.cdMultiplier;
	}

	// Token: 0x0600005B RID: 91 RVA: 0x00004592 File Offset: 0x00002792
	private void Start()
	{
		this.input = base.GetComponent<GeneralInput>();
		this.data = base.GetComponent<CharacterData>();
		this.health = base.GetComponent<HealthHandler>();
		this.sinceBlock = 100f;
	}

	// Token: 0x0600005C RID: 92 RVA: 0x000045C4 File Offset: 0x000027C4
	private void Update()
	{
		if (!this.input)
		{
			return;
		}
		if (!this.data.playerVel.simulated)
		{
			return;
		}
		if (!this.blockedLastFrame)
		{
			this.blockedThisFrame = false;
		}
		this.blockedLastFrame = false;
		this.sinceBlock += TimeHandler.deltaTime;
		this.counter += TimeHandler.deltaTime;
		if (this.counter > this.Cooldown() && !this.active)
		{
			this.active = true;
			this.reloadParticle.Play();
			this.BlockRechargeAction.Invoke();
			SoundManager.Instance.Play(this.soundBlockRecharged, base.transform);
		}
		if (this.input.shieldWasPressed)
		{
			this.TryBlock();
		}
	}

	// Token: 0x0600005D RID: 93 RVA: 0x00004688 File Offset: 0x00002888
	public void DoBlockAtPosition(bool firstBlock, bool dontSetCD = false, BlockTrigger.BlockTriggerType triggerType = BlockTrigger.BlockTriggerType.Default, Vector3 blockPos = default(Vector3), bool onlyBlockEffects = false)
	{
		this.blockedAtPos = blockPos;
		this.RPCA_DoBlock(firstBlock, dontSetCD, triggerType, blockPos, onlyBlockEffects);
	}

	// Token: 0x0600005E RID: 94 RVA: 0x000046A0 File Offset: 0x000028A0
	internal void ResetStats()
	{
		this.objectsToSpawn = new List<GameObject>();
		this.sinceBlock = 10f;
		this.cooldown = 4f;
		this.counter = 1000f;
		this.cdMultiplier = 1f;
		this.cdAdd = 0f;
		this.forceToAdd = 0f;
		this.forceToAddUp = 0f;
		this.autoBlock = false;
		this.blockedThisFrame = false;
		this.additionalBlocks = 0;
		this.healing = 0f;
		this.delayOtherActions = false;
	}

	// Token: 0x0600005F RID: 95 RVA: 0x0000472C File Offset: 0x0000292C
	public void CallDoBlock(bool firstBlock, bool dontSetCD = false, BlockTrigger.BlockTriggerType triggerType = BlockTrigger.BlockTriggerType.Default, Vector3 useBlockPos = default(Vector3), bool onlyBlockEffects = false)
	{
		this.data.view.RPC("RPCA_DoBlock", 0, new object[]
		{
			firstBlock,
			dontSetCD,
			(int)triggerType,
			useBlockPos,
			onlyBlockEffects
		});
	}

	// Token: 0x06000060 RID: 96 RVA: 0x00004784 File Offset: 0x00002984
	[PunRPC]
	public void RPCA_DoBlock(bool firstBlock, bool dontSetCD = false, BlockTrigger.BlockTriggerType triggerType = BlockTrigger.BlockTriggerType.Default, Vector3 useBlockPos = default(Vector3), bool onlyBlockEffects = false)
	{
		if (triggerType == BlockTrigger.BlockTriggerType.Default && firstBlock)
		{
			for (int i = 0; i < this.additionalBlocks; i++)
			{
				base.StartCoroutine(this.DelayBlock(((float)i + 1f) * this.timeBetweenBlocks));
			}
		}
		base.StartCoroutine(this.IDoBlock(firstBlock, dontSetCD, triggerType, useBlockPos, onlyBlockEffects));
	}

	// Token: 0x06000061 RID: 97 RVA: 0x000047DA File Offset: 0x000029DA
	private IEnumerator IDoBlock(bool firstBlock, bool dontSetCD = false, BlockTrigger.BlockTriggerType triggerType = BlockTrigger.BlockTriggerType.Default, Vector3 useBlockPos = default(Vector3), bool onlyBlockEffects = false)
	{
		this.active = false;
		Vector3 position = base.transform.position;
		if (useBlockPos != Vector3.zero)
		{
			base.transform.position = useBlockPos;
		}
		if (this.SuperFirstBlockAction != null)
		{
			this.SuperFirstBlockAction.Invoke(triggerType);
		}
		if (this.FirstBlockActionThatDelaysOthers != null)
		{
			this.FirstBlockActionThatDelaysOthers.Invoke(triggerType);
		}
		if (useBlockPos != Vector3.zero)
		{
			base.transform.position = position;
		}
		if (!onlyBlockEffects)
		{
			this.sinceBlock = 0f;
		}
		if (this.delayOtherActions)
		{
			yield return new WaitForSeconds(0.2f);
		}
		position = base.transform.position;
		if (useBlockPos != Vector3.zero)
		{
			base.transform.position = useBlockPos;
		}
		if (this.BlockActionEarly != null)
		{
			this.BlockActionEarly.Invoke(triggerType);
		}
		if (this.BlockAction != null)
		{
			this.BlockAction.Invoke(triggerType);
		}
		if (firstBlock)
		{
			if (this.forceToAdd != 0f)
			{
				this.health.TakeForce(this.data.hand.transform.forward * this.forceToAdd * this.data.playerVel.mass * 0.01f, 1, false, false, 0f);
			}
			if (this.forceToAddUp != 0f)
			{
				this.health.TakeForce(Vector3.up * this.forceToAddUp * this.data.playerVel.mass * 0.01f, 1, false, false, 0f);
			}
		}
		this.blockedLastFrame = true;
		bool flag = false;
		for (int i = 0; i < this.data.currentCards.Count; i++)
		{
			if (this.data.currentCards[i].soundDisableBlockBasic)
			{
				flag = true;
				break;
			}
		}
		if (!flag && triggerType != BlockTrigger.BlockTriggerType.ShieldCharge)
		{
			SoundManager.Instance.Play(this.soundBlockStart, base.transform);
		}
		if (!onlyBlockEffects)
		{
			this.particle.Play();
		}
		if (!dontSetCD)
		{
			this.counter = 0f;
		}
		GamefeelManager.GameFeel(Random.insideUnitCircle.normalized * 1f);
		if (!onlyBlockEffects)
		{
			this.sinceBlock = 0f;
		}
		this.Spawn();
		this.health.Heal(this.healing);
		if (useBlockPos != Vector3.zero)
		{
			base.transform.position = position;
		}
		yield break;
	}

	// Token: 0x06000062 RID: 98 RVA: 0x00004810 File Offset: 0x00002A10
	public void ShowStatusEffectBlock()
	{
		if (Time.unscaledTime < this.lastStatusBlock + 0.25f)
		{
			return;
		}
		if (!this.data.view.IsMine)
		{
			return;
		}
		this.lastStatusBlock = Time.unscaledTime;
		this.data.view.RPC("RPCA_ShowStatusEffectBlock", 0, Array.Empty<object>());
	}

	// Token: 0x06000063 RID: 99 RVA: 0x0000486A File Offset: 0x00002A6A
	[PunRPC]
	public void RPCA_ShowStatusEffectBlock()
	{
		SoundManager.Instance.Play(this.soundBlockStatusEffect, base.transform);
		this.statusBlockPart.Play();
	}

	// Token: 0x06000064 RID: 100 RVA: 0x00004890 File Offset: 0x00002A90
	public void TryBlock()
	{
		if (this.counter < this.Cooldown())
		{
			return;
		}
		this.RPCA_DoBlock(true, false, BlockTrigger.BlockTriggerType.Default, default(Vector3), false);
		this.counter = 0f;
	}

	// Token: 0x06000065 RID: 101 RVA: 0x000048CA File Offset: 0x00002ACA
	private IEnumerator DelayBlock(float t)
	{
		yield return new WaitForSeconds(t);
		yield return new WaitForEndOfFrame();
		this.RPCA_DoBlock(false, true, BlockTrigger.BlockTriggerType.Echo, default(Vector3), false);
		yield break;
	}

	// Token: 0x06000066 RID: 102 RVA: 0x000048E0 File Offset: 0x00002AE0
	public void Spawn()
	{
		for (int i = 0; i < this.objectsToSpawn.Count; i++)
		{
			SpawnedAttack component = Object.Instantiate<GameObject>(this.objectsToSpawn[i], base.transform.position, Quaternion.identity).GetComponent<SpawnedAttack>();
			if (component)
			{
				component.spawner = base.GetComponent<Player>();
			}
		}
	}

	// Token: 0x06000067 RID: 103 RVA: 0x00004940 File Offset: 0x00002B40
	public void blocked(GameObject projectile, Vector3 forward, Vector3 hitPos)
	{
		SoundManager.Instance.Play(this.soundBlockBlocked, base.transform);
		projectile.GetComponent<ProjectileHit>().RemoveOwnPlayerFromPlayersHit();
		projectile.GetComponent<ProjectileHit>().AddPlayerToHeld(base.GetComponent<HealthHandler>());
		projectile.GetComponent<MoveTransform>().velocity *= -1f;
		projectile.GetComponent<RayCastTrail>().WasBlocked();
		this.blockedPart.transform.position = hitPos + base.transform.forward * 5f;
		this.blockedPart.transform.rotation = Quaternion.LookRotation(-forward * 1.5f);
		GamefeelManager.GameFeel(forward);
		this.blockedPart.Play();
		SpawnedAttack componentInParent = projectile.GetComponentInParent<SpawnedAttack>();
		if (componentInParent && componentInParent.spawner.gameObject == base.transform.root.gameObject)
		{
			return;
		}
		if (this.BlockProjectileAction != null)
		{
			this.BlockProjectileAction.Invoke(projectile, forward, hitPos);
		}
	}

	// Token: 0x06000068 RID: 104 RVA: 0x00004A53 File Offset: 0x00002C53
	public void ResetCD(bool soundPlay)
	{
		this.active = true;
		this.reloadParticle.Play();
		this.counter = this.Cooldown() + 1f;
		if (soundPlay)
		{
			SoundManager.Instance.Play(this.soundBlockRecharged, base.transform);
		}
	}

	// Token: 0x06000069 RID: 105 RVA: 0x00004A92 File Offset: 0x00002C92
	public bool TryBlockMe(GameObject toBlock, Vector3 forward, Vector3 hitPos)
	{
		if (this.sinceBlock < 0.3f)
		{
			this.blocked(toBlock, forward, hitPos);
			this.sinceBlock = 0f;
			this.particle.Play();
			return true;
		}
		return false;
	}

	// Token: 0x0600006A RID: 106 RVA: 0x00004AC3 File Offset: 0x00002CC3
	public void DoBlock(GameObject toBlock, Vector3 forward, Vector3 hitPos)
	{
		this.sinceBlock = 0f;
		this.blocked(toBlock, forward, hitPos);
		this.particle.Play();
	}

	// Token: 0x0600006B RID: 107 RVA: 0x00004AE4 File Offset: 0x00002CE4
	public bool IsBlocking()
	{
		if (this.sinceBlock < 0.3f)
		{
			this.ShowStatusEffectBlock();
		}
		return this.sinceBlock < 0.3f;
	}

	// Token: 0x0600006C RID: 108 RVA: 0x00004B06 File Offset: 0x00002D06
	public bool IsOnCD()
	{
		return this.counter < this.Cooldown();
	}

	// Token: 0x0400004F RID: 79
	[Header("Sounds")]
	public SoundEvent soundBlockStart;

	// Token: 0x04000050 RID: 80
	public SoundEvent soundBlockRecharged;

	// Token: 0x04000051 RID: 81
	public SoundEvent soundBlockBlocked;

	// Token: 0x04000052 RID: 82
	public SoundEvent soundBlockStatusEffect;

	// Token: 0x04000053 RID: 83
	[Header("Settings")]
	public List<GameObject> objectsToSpawn = new List<GameObject>();

	// Token: 0x04000054 RID: 84
	public float sinceBlock;

	// Token: 0x04000055 RID: 85
	private GeneralInput input;

	// Token: 0x04000056 RID: 86
	public ParticleSystem particle;

	// Token: 0x04000057 RID: 87
	public ParticleSystem reloadParticle;

	// Token: 0x04000058 RID: 88
	public ParticleSystem blockedPart;

	// Token: 0x04000059 RID: 89
	public float cooldown;

	// Token: 0x0400005A RID: 90
	public float counter = 1000f;

	// Token: 0x0400005B RID: 91
	public float cdMultiplier = 1f;

	// Token: 0x0400005C RID: 92
	public float cdAdd;

	// Token: 0x0400005D RID: 93
	public float forceToAdd;

	// Token: 0x0400005E RID: 94
	public float forceToAddUp;

	// Token: 0x0400005F RID: 95
	public bool autoBlock;

	// Token: 0x04000060 RID: 96
	public bool blockedThisFrame;

	// Token: 0x04000061 RID: 97
	public int additionalBlocks;

	// Token: 0x04000062 RID: 98
	public float healing;

	// Token: 0x04000063 RID: 99
	private float timeBetweenBlocks = 0.2f;

	// Token: 0x04000064 RID: 100
	private CharacterData data;

	// Token: 0x04000065 RID: 101
	private HealthHandler health;

	// Token: 0x04000066 RID: 102
	private bool active = true;

	// Token: 0x04000067 RID: 103
	public Action BlockRechargeAction;

	// Token: 0x04000068 RID: 104
	private bool blockedLastFrame;

	// Token: 0x04000069 RID: 105
	public Action<BlockTrigger.BlockTriggerType> BlockAction;

	// Token: 0x0400006A RID: 106
	public Action<BlockTrigger.BlockTriggerType> BlockActionEarly;

	// Token: 0x0400006B RID: 107
	public Action<BlockTrigger.BlockTriggerType> FirstBlockActionThatDelaysOthers;

	// Token: 0x0400006C RID: 108
	public Action<BlockTrigger.BlockTriggerType> SuperFirstBlockAction;

	// Token: 0x0400006D RID: 109
	public Vector3 blockedAtPos;

	// Token: 0x0400006E RID: 110
	public bool delayOtherActions;

	// Token: 0x0400006F RID: 111
	public ParticleSystem statusBlockPart;

	// Token: 0x04000070 RID: 112
	private float lastStatusBlock;

	// Token: 0x04000071 RID: 113
	public Action<GameObject, Vector3, Vector3> BlockProjectileAction;
}
