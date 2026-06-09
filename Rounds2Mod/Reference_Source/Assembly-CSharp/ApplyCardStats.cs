using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000009 RID: 9
public class ApplyCardStats : MonoBehaviour
{
	// Token: 0x0600002C RID: 44 RVA: 0x00002F4F File Offset: 0x0000114F
	private void Start()
	{
		this.myGunStats = base.GetComponent<Gun>();
		this.myPlayerStats = base.GetComponent<CharacterStatModifiers>();
		this.myBlock = base.GetComponentInChildren<Block>();
		this.cardAudio = base.GetComponent<CardAudioModifier>();
		this.damagable = base.GetComponentInChildren<DamagableEvent>();
	}

	// Token: 0x0600002D RID: 45 RVA: 0x00002F90 File Offset: 0x00001190
	private void Update()
	{
		if (this.shootToPick && this.damagable.dead && this.damagable.lastPlayer)
		{
			this.Pick(this.damagable.lastPlayer.teamID, false, PickerType.Team);
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x0600002E RID: 46 RVA: 0x00002FE8 File Offset: 0x000011E8
	[PunRPC]
	public void RPCA_Pick(int[] actorIDs)
	{
		for (int i = 0; i < actorIDs.Length; i++)
		{
			this.playerToUpgrade = PlayerManager.instance.GetPlayerWithActorID(actorIDs[i]);
			this.ApplyStats();
			CardBarHandler.instance.AddCard(this.playerToUpgrade.playerID, base.GetComponent<CardInfo>().sourceCard);
		}
	}

	// Token: 0x0600002F RID: 47 RVA: 0x0000303C File Offset: 0x0000123C
	[PunRPC]
	public void OFFLINE_Pick(Player[] players)
	{
		for (int i = 0; i < players.Length; i++)
		{
			this.playerToUpgrade = players[i];
			this.ApplyStats();
			CardBarHandler.instance.AddCard(this.playerToUpgrade.playerID, base.GetComponent<CardInfo>().sourceCard);
		}
	}

	// Token: 0x06000030 RID: 48 RVA: 0x00003088 File Offset: 0x00001288
	public void Pick(int pickerID, bool forcePick = false, PickerType pickerType = PickerType.Team)
	{
		this.Start();
		if (this.done && !forcePick)
		{
			return;
		}
		this.done = true;
		Player[] array = PlayerManager.instance.GetPlayersInTeam(pickerID);
		if (pickerType == PickerType.Player)
		{
			array = new Player[]
			{
				PlayerManager.instance.players[pickerID]
			};
		}
		if (PhotonNetwork.OfflineMode)
		{
			this.OFFLINE_Pick(array);
			return;
		}
		int[] array2 = new int[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = array[i].data.view.ControllerActorNr;
		}
		base.GetComponent<PhotonView>().RPC("RPCA_Pick", 0, new object[]
		{
			array2
		});
	}

	// Token: 0x06000031 RID: 49 RVA: 0x00003130 File Offset: 0x00001330
	private void ApplyStats()
	{
		this.done = true;
		PlayerAudioModifyers component = this.playerToUpgrade.GetComponent<PlayerAudioModifyers>();
		Gun component2 = this.playerToUpgrade.GetComponent<Holding>().holdable.GetComponent<Gun>();
		Player component3 = this.playerToUpgrade.GetComponent<Player>();
		CharacterData component4 = this.playerToUpgrade.GetComponent<CharacterData>();
		HealthHandler component5 = this.playerToUpgrade.GetComponent<HealthHandler>();
		this.playerToUpgrade.GetComponent<Movement>();
		Gravity component6 = this.playerToUpgrade.GetComponent<Gravity>();
		Block component7 = this.playerToUpgrade.GetComponent<Block>();
		CharacterStatModifiers component8 = component3.GetComponent<CharacterStatModifiers>();
		GunAmmo componentInChildren = component2.GetComponentInChildren<GunAmmo>();
		if (componentInChildren && this.myGunStats)
		{
			componentInChildren.ammoReg += this.myGunStats.ammoReg;
			componentInChildren.maxAmmo += this.myGunStats.ammo;
			componentInChildren.maxAmmo = Mathf.Clamp(componentInChildren.maxAmmo, 1, 90);
			componentInChildren.reloadTimeMultiplier *= this.myGunStats.reloadTime;
			componentInChildren.reloadTimeAdd += this.myGunStats.reloadTimeAdd;
		}
		component3.data.currentCards.Add(base.GetComponent<CardInfo>().sourceCard);
		if (this.myGunStats)
		{
			if (this.myGunStats.lockGunToDefault)
			{
				component2.defaultCooldown = this.myGunStats.forceSpecificAttackSpeed;
				component2.lockGunToDefault = this.myGunStats.lockGunToDefault;
			}
			if (this.myGunStats && this.myGunStats.projectiles.Length != 0)
			{
				component2.projectiles[0].objectToSpawn = this.myGunStats.projectiles[0].objectToSpawn;
			}
			if (this.myGunStats)
			{
				ApplyCardStats.CopyGunStats(this.myGunStats, component2);
			}
		}
		if (this.myPlayerStats)
		{
			component8.sizeMultiplier *= this.myPlayerStats.sizeMultiplier;
			component4.maxHealth *= this.myPlayerStats.health;
			component8.ConfigureMassAndSize();
			component8.movementSpeed *= this.myPlayerStats.movementSpeed;
			component8.jump *= this.myPlayerStats.jump;
			component4.jumps += this.myPlayerStats.numberOfJumps;
			component6.gravityForce *= this.myPlayerStats.gravity;
			component5.regeneration += this.myPlayerStats.regen;
			if (this.myPlayerStats.AddObjectToPlayer)
			{
				component8.objectsAddedToPlayer.Add(Object.Instantiate<GameObject>(this.myPlayerStats.AddObjectToPlayer, component3.transform.position, component3.transform.rotation, component3.transform));
			}
			component8.lifeSteal += this.myPlayerStats.lifeSteal;
			component8.respawns += this.myPlayerStats.respawns;
			component8.secondsToTakeDamageOver += this.myPlayerStats.secondsToTakeDamageOver;
			if (this.myPlayerStats.refreshOnDamage)
			{
				component8.refreshOnDamage = true;
			}
			if (!this.myPlayerStats.automaticReload)
			{
				component8.automaticReload = false;
			}
		}
		if (this.myBlock)
		{
			if (this.myBlock.objectsToSpawn != null)
			{
				for (int i = 0; i < this.myBlock.objectsToSpawn.Count; i++)
				{
					component7.objectsToSpawn.Add(this.myBlock.objectsToSpawn[i]);
				}
			}
			component7.cdMultiplier *= this.myBlock.cdMultiplier;
			component7.cdAdd += this.myBlock.cdAdd;
			component7.forceToAdd += this.myBlock.forceToAdd;
			component7.forceToAddUp += this.myBlock.forceToAddUp;
			component7.additionalBlocks += this.myBlock.additionalBlocks;
			component7.healing += this.myBlock.healing;
			if (this.myBlock.autoBlock)
			{
				component7.autoBlock = this.myBlock.autoBlock;
			}
		}
		if (component && this.cardAudio)
		{
			component.AddToStack(this.cardAudio);
		}
		component8.WasUpdated();
		component5.Revive(true);
	}

	// Token: 0x06000032 RID: 50 RVA: 0x000035C4 File Offset: 0x000017C4
	public static void CopyGunStats(Gun copyFromGun, Gun copyToGun)
	{
		if (copyFromGun.unblockable)
		{
			copyToGun.unblockable = copyFromGun.unblockable;
		}
		if (copyFromGun.ignoreWalls)
		{
			copyToGun.ignoreWalls = copyFromGun.ignoreWalls;
		}
		float num = 1f;
		if (copyFromGun.numberOfProjectiles != 0 && copyToGun.numberOfProjectiles != 1)
		{
			num = (float)copyFromGun.numberOfProjectiles / ((float)copyFromGun.numberOfProjectiles + (float)copyToGun.numberOfProjectiles);
		}
		copyToGun.damage *= 1f - num * (1f - copyFromGun.damage);
		if (copyToGun.damage < 0.25f)
		{
			copyToGun.damage = 0.25f;
		}
		copyToGun.size += copyFromGun.size;
		float num2 = 1f;
		if (copyFromGun.chargeNumberOfProjectilesTo != 0f)
		{
			num2 = copyFromGun.chargeNumberOfProjectilesTo / (copyFromGun.chargeNumberOfProjectilesTo + copyToGun.chargeNumberOfProjectilesTo);
		}
		copyToGun.chargeDamageMultiplier *= 1f - num2 * (1f - copyFromGun.chargeDamageMultiplier);
		copyToGun.knockback *= 1f - num * (1f - copyFromGun.knockback);
		copyToGun.projectileSpeed *= copyFromGun.projectileSpeed;
		copyToGun.projectielSimulatonSpeed *= copyFromGun.projectielSimulatonSpeed;
		copyToGun.gravity *= copyFromGun.gravity;
		copyToGun.multiplySpread *= copyFromGun.multiplySpread;
		copyToGun.attackSpeed *= copyFromGun.attackSpeed;
		copyToGun.bodyRecoil *= copyFromGun.recoilMuiltiplier;
		copyToGun.speedMOnBounce *= copyFromGun.speedMOnBounce;
		copyToGun.dmgMOnBounce *= copyFromGun.dmgMOnBounce;
		copyToGun.bulletDamageMultiplier *= copyFromGun.bulletDamageMultiplier;
		copyToGun.spread += copyFromGun.spread;
		copyToGun.drag += copyFromGun.drag;
		copyToGun.timeBetweenBullets += copyFromGun.timeBetweenBullets;
		copyToGun.dragMinSpeed += copyFromGun.dragMinSpeed;
		copyToGun.evenSpread += copyFromGun.evenSpread;
		copyToGun.numberOfProjectiles += copyFromGun.numberOfProjectiles;
		copyToGun.reflects += copyFromGun.reflects;
		copyToGun.smartBounce += copyFromGun.smartBounce;
		copyToGun.bulletPortal += copyFromGun.bulletPortal;
		copyToGun.randomBounces += copyFromGun.randomBounces;
		copyToGun.bursts += copyFromGun.bursts;
		copyToGun.slow += copyFromGun.slow;
		copyToGun.overheatMultiplier += copyFromGun.overheatMultiplier;
		copyToGun.projectileSize += copyFromGun.projectileSize;
		copyToGun.percentageDamage += copyFromGun.percentageDamage;
		copyToGun.damageAfterDistanceMultiplier *= copyFromGun.damageAfterDistanceMultiplier;
		copyToGun.timeToReachFullMovementMultiplier *= copyFromGun.timeToReachFullMovementMultiplier;
		copyToGun.cos += copyFromGun.cos;
		if (copyFromGun.dontAllowAutoFire)
		{
			copyToGun.dontAllowAutoFire = true;
		}
		if (copyFromGun.destroyBulletAfter != 0f)
		{
			copyToGun.destroyBulletAfter = copyFromGun.destroyBulletAfter;
		}
		copyToGun.chargeSpreadTo += copyFromGun.chargeSpreadTo;
		copyToGun.chargeSpeedTo += copyFromGun.chargeSpeedTo;
		copyToGun.chargeEvenSpreadTo += copyFromGun.chargeEvenSpreadTo;
		copyToGun.chargeNumberOfProjectilesTo += copyFromGun.chargeNumberOfProjectilesTo;
		copyToGun.chargeRecoilTo += copyFromGun.chargeRecoilTo;
		if (copyFromGun.projectileColor != Color.black)
		{
			if (copyToGun.projectileColor == Color.black)
			{
				copyToGun.projectileColor = copyFromGun.projectileColor;
			}
			float r = Mathf.Pow((copyToGun.projectileColor.r * copyToGun.projectileColor.r + copyFromGun.projectileColor.r * copyFromGun.projectileColor.r) / 2f, 0.5f);
			float g = Mathf.Pow((copyToGun.projectileColor.g * copyToGun.projectileColor.g + copyFromGun.projectileColor.g * copyFromGun.projectileColor.g) / 2f, 0.5f);
			float b = Mathf.Pow((copyToGun.projectileColor.b * copyToGun.projectileColor.b + copyFromGun.projectileColor.b * copyFromGun.projectileColor.b) / 2f, 0.5f);
			Color rgbColor = new Color(r, g, b, 1f);
			float h = 0f;
			float s = 0f;
			float v = 0f;
			Color.RGBToHSV(rgbColor, out h, out s, out v);
			s = 1f;
			v = 1f;
			copyToGun.projectileColor = Color.HSVToRGB(h, s, v);
		}
		List<ObjectsToSpawn> list = new List<ObjectsToSpawn>();
		for (int i = 0; i < copyToGun.objectsToSpawn.Length; i++)
		{
			list.Add(copyToGun.objectsToSpawn[i]);
		}
		for (int j = 0; j < copyFromGun.objectsToSpawn.Length; j++)
		{
			bool flag = false;
			for (int k = 0; k < list.Count; k++)
			{
				if (list[k].effect && copyFromGun.objectsToSpawn[j].effect)
				{
					if (list[k].effect.name == copyFromGun.objectsToSpawn[j].effect.name && list[k].scaleStacks)
					{
						list[k].stacks++;
						flag = true;
					}
				}
				else if (list[k].AddToProjectile && copyFromGun.objectsToSpawn[j].AddToProjectile && list[k].AddToProjectile.name == copyFromGun.objectsToSpawn[j].AddToProjectile.name && list[k].scaleStacks)
				{
					list[k].stacks++;
					flag = true;
				}
			}
			if (!flag)
			{
				list.Add(copyFromGun.objectsToSpawn[j]);
			}
		}
		copyToGun.objectsToSpawn = list.ToArray();
		if (copyFromGun.useCharge)
		{
			copyToGun.useCharge = copyFromGun.useCharge;
		}
		copyToGun.soundGun.AddSoundShotModifier(copyFromGun.soundShotModifier);
		copyToGun.soundGun.AddSoundImpactModifier(copyFromGun.soundImpactModifier);
		copyToGun.soundGun.RefreshSoundModifiers();
	}

	// Token: 0x04000021 RID: 33
	private Gun myGunStats;

	// Token: 0x04000022 RID: 34
	private CharacterStatModifiers myPlayerStats;

	// Token: 0x04000023 RID: 35
	private Block myBlock;

	// Token: 0x04000024 RID: 36
	private Player playerToUpgrade;

	// Token: 0x04000025 RID: 37
	private CardAudioModifier cardAudio;

	// Token: 0x04000026 RID: 38
	private bool done;

	// Token: 0x04000027 RID: 39
	private DamagableEvent damagable;

	// Token: 0x04000028 RID: 40
	public bool shootToPick;
}
