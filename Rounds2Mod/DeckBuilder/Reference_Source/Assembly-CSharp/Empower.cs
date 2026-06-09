using System;
using Sonigon;
using UnityEngine;

// Token: 0x0200004F RID: 79
public class Empower : MonoBehaviour
{
	// Token: 0x0600017A RID: 378 RVA: 0x0000983C File Offset: 0x00007A3C
	private void Start()
	{
		this.particleTransform = base.transform.GetChild(0);
		this.data = base.GetComponentInParent<CharacterData>();
		this.parts = base.GetComponentsInChildren<ParticleSystem>();
		HealthHandler healthHandler = base.GetComponentInParent<Player>().data.healthHandler;
		healthHandler.reviveAction = (Action)Delegate.Combine(healthHandler.reviveAction, new Action(this.ResetEmpower));
		Gun gun = this.data.weaponHandler.gun;
		gun.ShootPojectileAction = (Action<GameObject>)Delegate.Combine(gun.ShootPojectileAction, new Action<GameObject>(this.Attack));
		Block componentInParent = base.GetComponentInParent<Block>();
		componentInParent.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Combine(componentInParent.BlockAction, new Action<BlockTrigger.BlockTriggerType>(this.Block));
	}

	// Token: 0x0600017B RID: 379 RVA: 0x000098FC File Offset: 0x00007AFC
	private void OnDestroy()
	{
		HealthHandler healthHandler = base.GetComponentInParent<Player>().data.healthHandler;
		healthHandler.reviveAction = (Action)Delegate.Remove(healthHandler.reviveAction, new Action(this.ResetEmpower));
		Gun gun = this.data.weaponHandler.gun;
		gun.ShootPojectileAction = (Action<GameObject>)Delegate.Remove(gun.ShootPojectileAction, new Action<GameObject>(this.Attack));
		Block componentInParent = base.GetComponentInParent<Block>();
		componentInParent.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(componentInParent.BlockAction, new Action<BlockTrigger.BlockTriggerType>(this.Block));
	}

	// Token: 0x0600017C RID: 380 RVA: 0x00009992 File Offset: 0x00007B92
	private void ResetEmpower()
	{
		this.empowered = false;
	}

	// Token: 0x0600017D RID: 381 RVA: 0x0000999B File Offset: 0x00007B9B
	public void Block(BlockTrigger.BlockTriggerType trigger)
	{
		if (trigger == BlockTrigger.BlockTriggerType.Echo || trigger == BlockTrigger.BlockTriggerType.Empower || trigger == BlockTrigger.BlockTriggerType.ShieldCharge)
		{
			return;
		}
		this.empowered = true;
	}

	// Token: 0x0600017E RID: 382 RVA: 0x000099B4 File Offset: 0x00007BB4
	public void Attack(GameObject projectile)
	{
		SpawnedAttack component = projectile.GetComponent<SpawnedAttack>();
		if (!component)
		{
			return;
		}
		if (this.empowered)
		{
			ProjectileHit component2 = projectile.GetComponent<ProjectileHit>();
			MoveTransform component3 = projectile.GetComponent<MoveTransform>();
			component.SetColor(this.empowerColor);
			component2.damage *= this.dmgMultiplier;
			component3.localForce *= this.speedMultiplier;
			if (this.addObjectToBullet)
			{
				Object.Instantiate<GameObject>(this.addObjectToBullet, projectile.transform.position, projectile.transform.rotation, projectile.transform);
			}
		}
		this.empowered = false;
	}

	// Token: 0x0600017F RID: 383 RVA: 0x00009A58 File Offset: 0x00007C58
	private void Update()
	{
		if (this.empowered)
		{
			this.particleTransform.transform.position = this.data.weaponHandler.gun.transform.position;
			this.particleTransform.transform.rotation = this.data.weaponHandler.gun.transform.rotation;
			if (!this.isOn)
			{
				SoundManager.Instance.PlayAtPosition(this.soundEmpowerSpawn, SoundManager.Instance.GetTransform(), base.transform);
				for (int i = 0; i < this.parts.Length; i++)
				{
					this.parts[i].Play();
				}
				this.isOn = true;
				return;
			}
		}
		else if (this.isOn)
		{
			for (int j = 0; j < this.parts.Length; j++)
			{
				this.parts[j].Stop();
			}
			this.isOn = false;
		}
	}

	// Token: 0x040001E7 RID: 487
	public SoundEvent soundEmpowerSpawn;

	// Token: 0x040001E8 RID: 488
	public GameObject addObjectToBullet;

	// Token: 0x040001E9 RID: 489
	public float dmgMultiplier = 2f;

	// Token: 0x040001EA RID: 490
	public float speedMultiplier = 2f;

	// Token: 0x040001EB RID: 491
	public Color empowerColor;

	// Token: 0x040001EC RID: 492
	private CharacterData data;

	// Token: 0x040001ED RID: 493
	private ParticleSystem[] parts;

	// Token: 0x040001EE RID: 494
	private Transform particleTransform;

	// Token: 0x040001EF RID: 495
	private bool empowered;

	// Token: 0x040001F0 RID: 496
	private bool isOn;
}
