using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Sirenix.OdinInspector;
using SoundImplementation;
using UnityEngine;

// Token: 0x02000069 RID: 105
public class Gun : Weapon
{
	// Token: 0x06000205 RID: 517 RVA: 0x0000C5E8 File Offset: 0x0000A7E8
	internal float GetRangeCompensation(float distance)
	{
		return Mathf.Pow(distance, 2f) * 0.015f / this.projectileSpeed;
	}

	// Token: 0x06000206 RID: 518 RVA: 0x0000C604 File Offset: 0x0000A804
	internal void ResetStats()
	{
		this.isReloading = false;
		this.damage = 1f;
		this.reloadTime = 1f;
		this.reloadTimeAdd = 0f;
		this.recoilMuiltiplier = 1f;
		this.gunAmmo.reloadTimeMultiplier = 1f;
		this.gunAmmo.reloadTimeAdd = 0f;
		this.knockback = 1f;
		this.attackSpeed = 0.3f;
		this.projectileSpeed = 1f;
		this.projectielSimulatonSpeed = 1f;
		this.gravity = 1f;
		this.damageAfterDistanceMultiplier = 1f;
		this.bulletDamageMultiplier = 1f;
		this.multiplySpread = 1f;
		this.shakeM = 1f;
		this.ammo = 0;
		this.ammoReg = 0f;
		this.size = 0f;
		this.overheatMultiplier = 0f;
		this.timeToReachFullMovementMultiplier = 0f;
		this.numberOfProjectiles = 1;
		this.bursts = 0;
		this.reflects = 0;
		this.smartBounce = 0;
		this.bulletPortal = 0;
		this.randomBounces = 0;
		this.timeBetweenBullets = 0f;
		this.projectileSize = 0f;
		this.speedMOnBounce = 1f;
		this.dmgMOnBounce = 1f;
		this.drag = 0f;
		this.dragMinSpeed = 1f;
		this.spread = 0f;
		this.evenSpread = 0f;
		this.percentageDamage = 0f;
		this.cos = 0f;
		this.slow = 0f;
		this.chargeNumberOfProjectilesTo = 0f;
		this.destroyBulletAfter = 0f;
		this.forceSpecificAttackSpeed = 0f;
		this.lockGunToDefault = false;
		this.unblockable = false;
		this.ignoreWalls = false;
		this.currentCharge = 0f;
		this.useCharge = false;
		this.waveMovement = false;
		this.teleport = false;
		this.spawnSkelletonSquare = false;
		this.explodeNearEnemyRange = 0f;
		this.explodeNearEnemyDamage = 0f;
		this.hitMovementMultiplier = 1f;
		this.isProjectileGun = false;
		this.defaultCooldown = 1f;
		this.attackSpeedMultiplier = 1f;
		this.objectsToSpawn = new ObjectsToSpawn[0];
		base.GetComponentInChildren<GunAmmo>().maxAmmo = 3;
		base.GetComponentInChildren<GunAmmo>().ReDrawTotalBullets();
		this.projectileColor = Color.black;
	}

	// Token: 0x17000007 RID: 7
	// (get) Token: 0x06000207 RID: 519 RVA: 0x0000C866 File Offset: 0x0000AA66
	private float usedCooldown
	{
		get
		{
			if (!this.lockGunToDefault)
			{
				return this.attackSpeed;
			}
			return this.defaultCooldown;
		}
	}

	// Token: 0x06000208 RID: 520 RVA: 0x0000C880 File Offset: 0x0000AA80
	private void Start()
	{
		this.gunAmmo = base.GetComponentInChildren<GunAmmo>();
		Gun[] componentsInChildren = base.transform.root.GetComponentsInChildren<Gun>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i] == this)
			{
				this.gunID = i;
			}
		}
		if (!this.player)
		{
			this.player = base.GetComponentInParent<Player>();
		}
		if (!this.player)
		{
			ProjectileHit componentInParent = base.GetComponentInParent<ProjectileHit>();
			if (componentInParent)
			{
				this.player = componentInParent.ownPlayer;
			}
		}
		if (!this.player)
		{
			SpawnedAttack component = base.transform.root.GetComponent<SpawnedAttack>();
			if (component)
			{
				this.player = component.spawner;
			}
		}
		this.holdable = base.GetComponent<Holdable>();
		this.defaultCooldown = this.usedCooldown;
		ShootPos componentInChildren = base.GetComponentInChildren<ShootPos>();
		if (componentInChildren)
		{
			this.shootPosition = componentInChildren.transform;
		}
		else
		{
			this.shootPosition = base.transform;
		}
		this.rig = base.GetComponent<Rigidbody2D>();
		this.soundGun.SetGun(this);
		this.soundGun.SetGunTransform(base.transform);
		this.soundGun.RefreshSoundModifiers();
	}

	// Token: 0x06000209 RID: 521 RVA: 0x0000C9B8 File Offset: 0x0000ABB8
	private void Update()
	{
		if (this.holdable && this.holdable.holder && this.holdable.holder.player)
		{
			this.player = this.holdable.holder.player;
		}
		this.sinceAttack += TimeHandler.deltaTime * this.attackSpeedMultiplier;
		if (!GameManager.instance.battleOngoing || (this.player != null && (!this.player.data.isPlaying || this.player.data.dead)))
		{
			this.soundGun.StopAutoPlayTail();
		}
	}

	// Token: 0x0600020A RID: 522 RVA: 0x0000CA73 File Offset: 0x0000AC73
	private void OnDestroy()
	{
		this.soundGun.StopAutoPlayTail();
	}

	// Token: 0x0600020B RID: 523 RVA: 0x0000CA80 File Offset: 0x0000AC80
	public bool IsReady(float readuIn = 0f)
	{
		return this.sinceAttack + readuIn * this.attackSpeedMultiplier > this.usedCooldown;
	}

	// Token: 0x0600020C RID: 524 RVA: 0x0000CA99 File Offset: 0x0000AC99
	public float ReadyAmount()
	{
		return this.sinceAttack / this.usedCooldown;
	}

	// Token: 0x0600020D RID: 525 RVA: 0x0000CAA8 File Offset: 0x0000ACA8
	public override bool Attack(float charge, bool forceAttack = false, float damageM = 1f, float recoilM = 1f, bool useAmmo = true)
	{
		if (this.sinceAttack < this.usedCooldown && !forceAttack)
		{
			return false;
		}
		if (this.isReloading && !forceAttack)
		{
			return false;
		}
		this.sinceAttack = 0f;
		int attacks = Mathf.Clamp(Mathf.RoundToInt(0.5f * charge / this.attackSpeed), 1, 10);
		if (this.lockGunToDefault)
		{
			attacks = 1;
		}
		base.StartCoroutine(this.DoAttacks(charge, forceAttack, damageM, attacks, recoilM, useAmmo));
		return true;
	}

	// Token: 0x0600020E RID: 526 RVA: 0x0000CB1D File Offset: 0x0000AD1D
	private IEnumerator DoAttacks(float charge, bool forceAttack = false, float damageM = 1f, int attacks = 1, float recoilM = 1f, bool useAmmo = true)
	{
		int num;
		for (int i = 0; i < attacks; i = num + 1)
		{
			this.DoAttack(charge, forceAttack, damageM, recoilM, useAmmo);
			yield return new WaitForSeconds(0.3f / (float)attacks);
			num = i;
		}
		yield break;
	}

	// Token: 0x0600020F RID: 527 RVA: 0x0000CB5C File Offset: 0x0000AD5C
	private void DoAttack(float charge, bool forceAttack = false, float damageM = 1f, float recoilM = 1f, bool useAmmo = true)
	{
		float num = 1f * (1f + charge * this.chargeRecoilTo) * recoilM;
		if (this.rig)
		{
			this.rig.AddForce(this.rig.mass * this.recoil * Mathf.Clamp(this.usedCooldown, 0f, 1f) * -base.transform.up, 1);
		}
		this.holdable;
		if (this.attackAction != null)
		{
			this.attackAction.Invoke();
		}
		base.StartCoroutine(this.FireBurst(charge, forceAttack, damageM, recoilM, useAmmo));
	}

	// Token: 0x06000210 RID: 528 RVA: 0x0000CC10 File Offset: 0x0000AE10
	private bool CheckIsMine()
	{
		bool result = false;
		if (this.holdable && this.holdable.holder)
		{
			result = this.holdable.holder.player.data.view.IsMine;
		}
		else
		{
			Player componentInParent = base.GetComponentInParent<Player>();
			if (componentInParent)
			{
				result = componentInParent.data.view.IsMine;
			}
		}
		return result;
	}

	// Token: 0x06000211 RID: 529 RVA: 0x0000CC81 File Offset: 0x0000AE81
	private IEnumerator FireBurst(float charge, bool forceAttack = false, float damageM = 1f, float recoilM = 1f, bool useAmmo = true)
	{
		int currentNumberOfProjectiles = this.lockGunToDefault ? 1 : (this.numberOfProjectiles + Mathf.RoundToInt(this.chargeNumberOfProjectilesTo * charge));
		if (!this.lockGunToDefault)
		{
		}
		if (this.timeBetweenBullets == 0f)
		{
			GamefeelManager.GameFeel(base.transform.up * this.shake);
			this.soundGun.PlayShot(currentNumberOfProjectiles);
		}
		int num;
		for (int ii = 0; ii < Mathf.Clamp(this.bursts, 1, 100); ii = num + 1)
		{
			for (int i = 0; i < this.projectiles.Length; i++)
			{
				for (int j = 0; j < currentNumberOfProjectiles; j++)
				{
					if (this.CheckIsMine())
					{
						this.spawnPos = base.transform.position;
						if (this.player)
						{
							this.player.GetComponent<PlayerAudioModifyers>().SetStacks();
							if (this.holdable)
							{
								this.spawnPos = this.player.transform.position;
							}
						}
						GameObject gameObject = PhotonNetwork.Instantiate(this.projectiles[i].objectToSpawn.gameObject.name, this.spawnPos, this.getShootRotation(j, currentNumberOfProjectiles, charge), 0, null);
						if (this.holdable)
						{
							if (useAmmo)
							{
								if (PhotonNetwork.OfflineMode)
								{
									gameObject.GetComponent<ProjectileInit>().OFFLINE_Init(this.holdable.holder.player.playerID, currentNumberOfProjectiles, damageM, Random.Range(0f, 1f));
								}
								else
								{
									gameObject.GetComponent<PhotonView>().RPC("RPCA_Init", 0, new object[]
									{
										this.holdable.holder.view.OwnerActorNr,
										currentNumberOfProjectiles,
										damageM,
										Random.Range(0f, 1f)
									});
								}
							}
							else if (PhotonNetwork.OfflineMode)
							{
								gameObject.GetComponent<ProjectileInit>().OFFLINE_Init_noAmmoUse(this.holdable.holder.player.playerID, currentNumberOfProjectiles, damageM, Random.Range(0f, 1f));
							}
							else
							{
								gameObject.GetComponent<PhotonView>().RPC("RPCA_Init_noAmmoUse", 0, new object[]
								{
									this.holdable.holder.view.OwnerActorNr,
									currentNumberOfProjectiles,
									damageM,
									Random.Range(0f, 1f)
								});
							}
						}
						else if (PhotonNetwork.OfflineMode)
						{
							gameObject.GetComponent<ProjectileInit>().OFFLINE_Init_SeparateGun(base.GetComponentInParent<Player>().playerID, this.gunID, currentNumberOfProjectiles, damageM, Random.Range(0f, 1f));
						}
						else
						{
							gameObject.GetComponent<PhotonView>().RPC("RPCA_Init_SeparateGun", 0, new object[]
							{
								base.GetComponentInParent<CharacterData>().view.OwnerActorNr,
								this.gunID,
								currentNumberOfProjectiles,
								damageM,
								Random.Range(0f, 1f)
							});
						}
					}
					if (this.timeBetweenBullets != 0f)
					{
						GamefeelManager.GameFeel(base.transform.up * this.shake);
						this.soundGun.PlayShot(currentNumberOfProjectiles);
					}
				}
			}
			if (this.bursts > 1 && ii + 1 == Mathf.Clamp(this.bursts, 1, 100))
			{
				this.soundGun.StopAutoPlayTail();
			}
			if (this.timeBetweenBullets > 0f)
			{
				yield return new WaitForSeconds(this.timeBetweenBullets);
			}
			num = ii;
		}
		yield break;
	}

	// Token: 0x06000212 RID: 530 RVA: 0x0000CCA8 File Offset: 0x0000AEA8
	public void BulletInit(GameObject bullet, int usedNumberOfProjectiles, float damageM, float randomSeed, bool useAmmo = true)
	{
		this.spawnedAttack = bullet.GetComponent<SpawnedAttack>();
		if (!this.spawnedAttack)
		{
			this.spawnedAttack = bullet.AddComponent<SpawnedAttack>();
		}
		if (!bullet.GetComponentInChildren<DontChangeMe>())
		{
			this.ApplyProjectileStats(bullet, usedNumberOfProjectiles, damageM, randomSeed);
		}
		if (this.soundDisableRayHitBulletSound)
		{
			RayHitBulletSound component = bullet.GetComponent<RayHitBulletSound>();
			if (component != null)
			{
				component.disableImpact = true;
			}
		}
		this.ApplyPlayerStuff(bullet);
		if (this.ShootPojectileAction != null)
		{
			this.ShootPojectileAction.Invoke(bullet);
		}
		if (useAmmo && this.gunAmmo)
		{
			this.gunAmmo.Shoot(bullet);
		}
	}

	// Token: 0x06000213 RID: 531 RVA: 0x0000CD4C File Offset: 0x0000AF4C
	private Quaternion getShootRotation(int bulletID, int numOfProj, float charge)
	{
		Vector3 vector = this.shootPosition.forward;
		if (this.forceShootDir != Vector3.zero)
		{
			vector = this.forceShootDir;
		}
		float d = this.multiplySpread * Mathf.Clamp(1f + charge * this.chargeSpreadTo, 0f, float.PositiveInfinity);
		float num = Random.Range(-this.spread, this.spread);
		num /= (1f + this.projectileSpeed * 0.5f) * 0.5f;
		vector += Vector3.Cross(vector, Vector3.forward) * num * d;
		return Quaternion.LookRotation(this.lockGunToDefault ? this.shootPosition.forward : vector);
	}

	// Token: 0x06000214 RID: 532 RVA: 0x0000CE0C File Offset: 0x0000B00C
	private void ApplyPlayerStuff(GameObject obj)
	{
		ProjectileHit component = obj.GetComponent<ProjectileHit>();
		component.ownWeapon = base.gameObject;
		if (this.player)
		{
			component.ownPlayer = this.player;
		}
		this.spawnedAttack.spawner = this.player;
		this.spawnedAttack.attackID = this.attackID;
	}

	// Token: 0x06000215 RID: 533 RVA: 0x0000CE68 File Offset: 0x0000B068
	private void ApplyProjectileStats(GameObject obj, int numOfProj = 1, float damageM = 1f, float randomSeed = 0f)
	{
		ProjectileHit component = obj.GetComponent<ProjectileHit>();
		component.dealDamageMultiplierr *= this.bulletDamageMultiplier;
		component.damage *= this.damage * damageM;
		component.percentageDamage = this.percentageDamage;
		component.stun = component.damage / 150f;
		component.force *= this.knockback;
		component.movementSlow = this.slow;
		component.hasControl = this.CheckIsMine();
		component.projectileColor = this.projectileColor;
		component.unblockable = this.unblockable;
		RayCastTrail component2 = obj.GetComponent<RayCastTrail>();
		if (this.ignoreWalls)
		{
			component2.mask = component2.ignoreWallsMask;
		}
		if (component2)
		{
			component2.extraSize += this.size;
		}
		if (this.player)
		{
			PlayerSkin playerSkinColors = PlayerSkinBank.GetPlayerSkinColors(this.player.playerID);
			component.team = playerSkinColors;
			obj.GetComponent<RayCastTrail>().teamID = this.player.playerID;
			SetTeamColor.TeamColorThis(obj, playerSkinColors);
		}
		List<ObjectsToSpawn> list = new List<ObjectsToSpawn>();
		for (int i = 0; i < this.objectsToSpawn.Length; i++)
		{
			list.Add(this.objectsToSpawn[i]);
			if (this.objectsToSpawn[i].AddToProjectile && (!this.objectsToSpawn[i].AddToProjectile.gameObject.GetComponent<StopRecursion>() || !this.isProjectileGun))
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.objectsToSpawn[i].AddToProjectile, component.transform.position, component.transform.rotation, component.transform);
				gameObject.transform.localScale *= 1f * (1f - this.objectsToSpawn[i].scaleFromDamage) + component.damage / 55f * this.objectsToSpawn[i].scaleFromDamage;
				if (this.objectsToSpawn[i].scaleStacks)
				{
					gameObject.transform.localScale *= 1f + (float)this.objectsToSpawn[i].stacks * this.objectsToSpawn[i].scaleStackM;
				}
				if (this.objectsToSpawn[i].removeScriptsFromProjectileObject)
				{
					MonoBehaviour[] componentsInChildren = gameObject.GetComponentsInChildren<MonoBehaviour>();
					for (int j = 0; j < componentsInChildren.Length; j++)
					{
						if (componentsInChildren[j].GetType().ToString() != "SoundImplementation.SoundUnityEventPlayer")
						{
							Object.Destroy(componentsInChildren[j]);
						}
						global::Debug.Log(componentsInChildren[j].GetType().ToString());
					}
				}
			}
		}
		component.objectsToSpawn = list.ToArray();
		if (this.reflects > 0)
		{
			RayHitReflect rayHitReflect = obj.gameObject.AddComponent<RayHitReflect>();
			rayHitReflect.reflects = this.reflects;
			rayHitReflect.speedM = this.speedMOnBounce;
			rayHitReflect.dmgM = this.dmgMOnBounce;
		}
		if (!this.forceSpecificShake)
		{
			float num = component.damage / 100f * ((1f + this.usedCooldown) / 2f) / ((1f + (float)numOfProj) / 2f) * 2f;
			float num2 = Mathf.Clamp((0.2f + component.damage * (((float)this.numberOfProjectiles + 2f) / 2f) / 100f * ((1f + this.usedCooldown) / 2f)) * 1f, 0f, 3f);
			component.shake = num * this.shakeM;
			this.shake = num2;
		}
		MoveTransform component3 = obj.GetComponent<MoveTransform>();
		component3.localForce *= this.projectileSpeed;
		component3.simulationSpeed *= this.projectielSimulatonSpeed;
		component3.gravity *= this.gravity;
		component3.worldForce *= this.gravity;
		component3.drag = this.drag;
		component3.drag = Mathf.Clamp(component3.drag, 0f, 45f);
		component3.velocitySpread = Mathf.Clamp(this.spread * 50f, 0f, 50f);
		component3.dragMinSpeed = this.dragMinSpeed;
		component3.localForce *= Mathf.Lerp(1f - component3.velocitySpread * 0.01f, 1f + component3.velocitySpread * 0.01f, randomSeed);
		component3.selectedSpread = 0f;
		if (this.damageAfterDistanceMultiplier != 1f)
		{
			obj.AddComponent<ChangeDamageMultiplierAfterDistanceTravelled>().muiltiplier = this.damageAfterDistanceMultiplier;
		}
		if (this.cos > 0f)
		{
			obj.gameObject.AddComponent<Cos>().multiplier = this.cos;
		}
		if (this.destroyBulletAfter != 0f)
		{
			obj.GetComponent<RemoveAfterSeconds>().seconds = this.destroyBulletAfter;
		}
		if (this.spawnedAttack && this.projectileColor != Color.black)
		{
			this.spawnedAttack.SetColor(this.projectileColor);
		}
	}

	// Token: 0x06000216 RID: 534 RVA: 0x0000D392 File Offset: 0x0000B592
	public void AddAttackAction(Action action)
	{
		this.attackAction = (Action)Delegate.Combine(this.attackAction, action);
	}

	// Token: 0x06000217 RID: 535 RVA: 0x0000D3AB File Offset: 0x0000B5AB
	internal void RemoveAttackAction(Action action)
	{
		this.attackAction = (Action)Delegate.Remove(this.attackAction, action);
	}

	// Token: 0x040002AD RID: 685
	[Header("Sound Player Settings")]
	public SoundGun soundGun = new SoundGun();

	// Token: 0x040002AE RID: 686
	[Header("Sound Card Settings")]
	public SoundShotModifier soundShotModifier;

	// Token: 0x040002AF RID: 687
	public SoundImpactModifier soundImpactModifier;

	// Token: 0x040002B0 RID: 688
	public bool soundDisableRayHitBulletSound;

	// Token: 0x040002B1 RID: 689
	[Header("Settings")]
	public float recoil;

	// Token: 0x040002B2 RID: 690
	public float bodyRecoil;

	// Token: 0x040002B3 RID: 691
	public float shake;

	// Token: 0x040002B4 RID: 692
	public bool forceSpecificShake;

	// Token: 0x040002B5 RID: 693
	public ProjectilesToSpawn[] projectiles;

	// Token: 0x040002B6 RID: 694
	private Rigidbody2D rig;

	// Token: 0x040002B7 RID: 695
	public Transform shootPosition;

	// Token: 0x040002B8 RID: 696
	[HideInInspector]
	public Player player;

	// Token: 0x040002B9 RID: 697
	[HideInInspector]
	public bool isReloading;

	// Token: 0x040002BA RID: 698
	[Header("Multiply")]
	[FoldoutGroup("Stats", 0)]
	public float damage = 1f;

	// Token: 0x040002BB RID: 699
	[FoldoutGroup("Stats", 0)]
	public float reloadTime = 1f;

	// Token: 0x040002BC RID: 700
	[FoldoutGroup("Stats", 0)]
	public float reloadTimeAdd;

	// Token: 0x040002BD RID: 701
	[FoldoutGroup("Stats", 0)]
	public float recoilMuiltiplier = 1f;

	// Token: 0x040002BE RID: 702
	[FoldoutGroup("Stats", 0)]
	public float knockback = 1f;

	// Token: 0x040002BF RID: 703
	[FoldoutGroup("Stats", 0)]
	public float attackSpeed = 1f;

	// Token: 0x040002C0 RID: 704
	[FoldoutGroup("Stats", 0)]
	public float projectileSpeed = 1f;

	// Token: 0x040002C1 RID: 705
	[FoldoutGroup("Stats", 0)]
	public float projectielSimulatonSpeed = 1f;

	// Token: 0x040002C2 RID: 706
	[FoldoutGroup("Stats", 0)]
	public float gravity = 1f;

	// Token: 0x040002C3 RID: 707
	[FoldoutGroup("Stats", 0)]
	public float damageAfterDistanceMultiplier = 1f;

	// Token: 0x040002C4 RID: 708
	[FoldoutGroup("Stats", 0)]
	public float bulletDamageMultiplier = 1f;

	// Token: 0x040002C5 RID: 709
	[FoldoutGroup("Stats", 0)]
	public float multiplySpread = 1f;

	// Token: 0x040002C6 RID: 710
	[FoldoutGroup("Stats", 0)]
	public float shakeM = 1f;

	// Token: 0x040002C7 RID: 711
	[Header("Add")]
	[FoldoutGroup("Stats", 0)]
	public int ammo;

	// Token: 0x040002C8 RID: 712
	[FoldoutGroup("Stats", 0)]
	public float ammoReg;

	// Token: 0x040002C9 RID: 713
	[FoldoutGroup("Stats", 0)]
	public float size;

	// Token: 0x040002CA RID: 714
	[FoldoutGroup("Stats", 0)]
	public float overheatMultiplier;

	// Token: 0x040002CB RID: 715
	[FoldoutGroup("Stats", 0)]
	public float timeToReachFullMovementMultiplier;

	// Token: 0x040002CC RID: 716
	[FoldoutGroup("Stats", 0)]
	public int numberOfProjectiles;

	// Token: 0x040002CD RID: 717
	[FoldoutGroup("Stats", 0)]
	public int bursts;

	// Token: 0x040002CE RID: 718
	[FoldoutGroup("Stats", 0)]
	public int reflects;

	// Token: 0x040002CF RID: 719
	[FoldoutGroup("Stats", 0)]
	public int smartBounce;

	// Token: 0x040002D0 RID: 720
	[FoldoutGroup("Stats", 0)]
	public int bulletPortal;

	// Token: 0x040002D1 RID: 721
	[FoldoutGroup("Stats", 0)]
	public int randomBounces;

	// Token: 0x040002D2 RID: 722
	[FoldoutGroup("Stats", 0)]
	public float timeBetweenBullets;

	// Token: 0x040002D3 RID: 723
	[FoldoutGroup("Stats", 0)]
	public float projectileSize;

	// Token: 0x040002D4 RID: 724
	[FoldoutGroup("Stats", 0)]
	public float speedMOnBounce = 1f;

	// Token: 0x040002D5 RID: 725
	[FoldoutGroup("Stats", 0)]
	public float dmgMOnBounce = 1f;

	// Token: 0x040002D6 RID: 726
	[FoldoutGroup("Stats", 0)]
	public float drag;

	// Token: 0x040002D7 RID: 727
	[FoldoutGroup("Stats", 0)]
	public float dragMinSpeed = 1f;

	// Token: 0x040002D8 RID: 728
	[FoldoutGroup("Stats", 0)]
	public float spread;

	// Token: 0x040002D9 RID: 729
	[FoldoutGroup("Stats", 0)]
	public float evenSpread;

	// Token: 0x040002DA RID: 730
	[FoldoutGroup("Stats", 0)]
	public float percentageDamage;

	// Token: 0x040002DB RID: 731
	[FoldoutGroup("Stats", 0)]
	public float cos;

	// Token: 0x040002DC RID: 732
	[FoldoutGroup("Stats", 0)]
	public float slow;

	// Token: 0x040002DD RID: 733
	[FoldoutGroup("Stats", 0)]
	[Header("Charge Multiply")]
	public float chargeDamageMultiplier = 1f;

	// Token: 0x040002DE RID: 734
	[FoldoutGroup("Stats", 0)]
	[Header("(1 + Charge * x) Multiply")]
	public float chargeSpreadTo;

	// Token: 0x040002DF RID: 735
	[FoldoutGroup("Stats", 0)]
	public float chargeEvenSpreadTo;

	// Token: 0x040002E0 RID: 736
	[FoldoutGroup("Stats", 0)]
	public float chargeSpeedTo;

	// Token: 0x040002E1 RID: 737
	[FoldoutGroup("Stats", 0)]
	public float chargeRecoilTo;

	// Token: 0x040002E2 RID: 738
	[FoldoutGroup("Stats", 0)]
	[Header("(1 + Charge * x) Add")]
	public float chargeNumberOfProjectilesTo;

	// Token: 0x040002E3 RID: 739
	[FoldoutGroup("Stats", 0)]
	[Header("Special")]
	public float destroyBulletAfter;

	// Token: 0x040002E4 RID: 740
	[FoldoutGroup("Stats", 0)]
	public float forceSpecificAttackSpeed;

	// Token: 0x040002E5 RID: 741
	[FoldoutGroup("Stats", 0)]
	public bool lockGunToDefault;

	// Token: 0x040002E6 RID: 742
	[FoldoutGroup("Stats", 0)]
	public bool unblockable;

	// Token: 0x040002E7 RID: 743
	[FoldoutGroup("Stats", 0)]
	public bool ignoreWalls;

	// Token: 0x040002E8 RID: 744
	[HideInInspector]
	public float currentCharge;

	// Token: 0x040002E9 RID: 745
	public bool useCharge;

	// Token: 0x040002EA RID: 746
	public bool dontAllowAutoFire;

	// Token: 0x040002EB RID: 747
	public ObjectsToSpawn[] objectsToSpawn;

	// Token: 0x040002EC RID: 748
	public Color projectileColor = Color.black;

	// Token: 0x040002ED RID: 749
	public bool waveMovement;

	// Token: 0x040002EE RID: 750
	public bool teleport;

	// Token: 0x040002EF RID: 751
	public bool spawnSkelletonSquare;

	// Token: 0x040002F0 RID: 752
	public float explodeNearEnemyRange;

	// Token: 0x040002F1 RID: 753
	public float explodeNearEnemyDamage;

	// Token: 0x040002F2 RID: 754
	public float hitMovementMultiplier = 1f;

	// Token: 0x040002F3 RID: 755
	private Action attackAction;

	// Token: 0x040002F4 RID: 756
	[HideInInspector]
	public bool isProjectileGun;

	// Token: 0x040002F5 RID: 757
	[HideInInspector]
	public float defaultCooldown = 1f;

	// Token: 0x040002F6 RID: 758
	[HideInInspector]
	public int attackID = -1;

	// Token: 0x040002F7 RID: 759
	public float attackSpeedMultiplier = 1f;

	// Token: 0x040002F8 RID: 760
	private int gunID = -1;

	// Token: 0x040002F9 RID: 761
	private GunAmmo gunAmmo;

	// Token: 0x040002FA RID: 762
	private Vector3 spawnPos;

	// Token: 0x040002FB RID: 763
	public Action<GameObject> ShootPojectileAction;

	// Token: 0x040002FC RID: 764
	private float spreadOfLastBullet;

	// Token: 0x040002FD RID: 765
	private SpawnedAttack spawnedAttack;

	// Token: 0x040002FE RID: 766
	[HideInInspector]
	internal Vector3 forceShootDir;
}
