using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000AB RID: 171
public class ProjectileHit : RayHit
{
	// Token: 0x060003BC RID: 956 RVA: 0x00016D04 File Offset: 0x00014F04
	private void Start()
	{
		this.move = base.GetComponent<MoveTransform>();
		this.view = base.GetComponent<PhotonView>();
		this.effects.AddRange(base.GetComponentsInChildren<RayHitEffect>());
		this.effects.Sort((RayHitEffect p1, RayHitEffect p2) => p2.priority.CompareTo(p1.priority));
		this.spawnedAttack = base.GetComponent<SpawnedAttack>();
		if (this.spawnedAttack && !this.ownPlayer)
		{
			this.ownPlayer = this.spawnedAttack.spawner;
		}
		if (this.ownPlayer && !this.fullSelfDamage)
		{
			base.StartCoroutine(this.HoldPlayer(this.ownPlayer.GetComponent<HealthHandler>()));
		}
		this.damage *= base.transform.localScale.x;
		this.force *= Mathf.Pow(this.damage / 55f, 2f);
	}

	// Token: 0x060003BD RID: 957 RVA: 0x00016E08 File Offset: 0x00015008
	public void ResortHitEffects()
	{
		this.effects.Sort((RayHitEffect p1, RayHitEffect p2) => p2.priority.CompareTo(p1.priority));
	}

	// Token: 0x060003BE RID: 958 RVA: 0x00016E34 File Offset: 0x00015034
	private IEnumerator HoldPlayer(HealthHandler player)
	{
		if (player)
		{
			this.playersHit.Add(player);
		}
		yield return new WaitForSeconds(this.holdPlayerFor);
		if (this.playersHit.Contains(player))
		{
			this.playersHit.Remove(player);
		}
		yield break;
	}

	// Token: 0x060003BF RID: 959 RVA: 0x00016E4A File Offset: 0x0001504A
	private void Update()
	{
		this.sinceReflect += TimeHandler.deltaTime;
	}

	// Token: 0x060003C0 RID: 960 RVA: 0x00016E5E File Offset: 0x0001505E
	public void AddPlayerToHeld(HealthHandler health)
	{
		base.StartCoroutine(this.HoldPlayer(health));
	}

	// Token: 0x060003C1 RID: 961 RVA: 0x00016E6E File Offset: 0x0001506E
	public void RemoveOwnPlayerFromPlayersHit()
	{
		if (!this.ownPlayer)
		{
			return;
		}
		if (this.playersHit.Contains(this.ownPlayer.GetComponent<HealthHandler>()))
		{
			this.playersHit.Remove(this.ownPlayer.GetComponent<HealthHandler>());
		}
	}

	// Token: 0x060003C2 RID: 962 RVA: 0x00016EB0 File Offset: 0x000150B0
	public override void Hit(HitInfo hit, bool forceCall = false)
	{
		int num = -1;
		if (hit.transform)
		{
			PhotonView component = hit.transform.root.GetComponent<PhotonView>();
			if (component)
			{
				num = component.ViewID;
			}
		}
		int num2 = -1;
		if (num == -1)
		{
			Collider2D[] componentsInChildren = MapManager.instance.currentMap.Map.GetComponentsInChildren<Collider2D>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i] == hit.collider)
				{
					num2 = i;
				}
			}
		}
		HealthHandler healthHandler = null;
		if (hit.transform)
		{
			healthHandler = hit.transform.GetComponent<HealthHandler>();
		}
		bool flag = false;
		if (healthHandler)
		{
			if (this.playersHit.Contains(healthHandler))
			{
				return;
			}
			if (this.view.IsMine && healthHandler.GetComponent<Block>().IsBlocking())
			{
				flag = true;
			}
			base.StartCoroutine(this.HoldPlayer(healthHandler));
		}
		if (this.view.IsMine || forceCall)
		{
			if (this.sendCollisions)
			{
				this.view.RPC("RPCA_DoHit", 0, new object[]
				{
					hit.point,
					hit.normal,
					this.move.velocity,
					num,
					num2,
					flag
				});
				return;
			}
			this.RPCA_DoHit(hit.point, hit.normal, this.move.velocity, num, num2, flag);
		}
	}

	// Token: 0x060003C3 RID: 963 RVA: 0x00017038 File Offset: 0x00015238
	[PunRPC]
	public void RPCA_DoHit(Vector2 hitPoint, Vector2 hitNormal, Vector2 vel, int viewID = -1, int colliderID = -1, bool wasBlocked = false)
	{
		HitInfo hitInfo = new HitInfo();
		if (this.move)
		{
			this.move.velocity = vel;
		}
		hitInfo.point = hitPoint;
		hitInfo.normal = hitNormal;
		hitInfo.collider = null;
		if (viewID != -1)
		{
			PhotonView photonView = PhotonNetwork.GetPhotonView(viewID);
			hitInfo.collider = photonView.GetComponentInChildren<Collider2D>();
			hitInfo.transform = photonView.transform;
		}
		else if (colliderID != -1)
		{
			hitInfo.collider = MapManager.instance.currentMap.Map.GetComponentsInChildren<Collider2D>()[colliderID];
			hitInfo.transform = hitInfo.collider.transform;
		}
		HealthHandler healthHandler = null;
		if (hitInfo.transform)
		{
			healthHandler = hitInfo.transform.GetComponent<HealthHandler>();
		}
		if (this.isAllowedToSpawnObjects)
		{
			base.transform.position = hitInfo.point;
		}
		if (hitInfo.collider)
		{
			ProjectileHitSurface component = hitInfo.collider.GetComponent<ProjectileHitSurface>();
			if (component && component.HitSurface(hitInfo, base.gameObject) == ProjectileHitSurface.HasToStop.HasToStop)
			{
				return;
			}
		}
		if (healthHandler)
		{
			Block component2 = healthHandler.GetComponent<Block>();
			if (wasBlocked)
			{
				component2.DoBlock(base.gameObject, base.transform.forward, hitInfo.point);
				if (this.destroyOnBlock)
				{
					this.DestroyMe();
				}
				this.sinceReflect = 0f;
				return;
			}
			CharacterStatModifiers component3 = healthHandler.GetComponent<CharacterStatModifiers>();
			if (this.movementSlow != 0f && !wasBlocked)
			{
				component3.RPCA_AddSlow(this.movementSlow, false);
			}
		}
		float num = 1f;
		PlayerVelocity playerVelocity = null;
		if (hitInfo.transform)
		{
			playerVelocity = hitInfo.transform.GetComponentInParent<PlayerVelocity>();
		}
		if (hitInfo.collider)
		{
			Damagable componentInParent = hitInfo.collider.GetComponentInParent<Damagable>();
			if (componentInParent)
			{
				if (healthHandler && this.percentageDamage != 0f)
				{
					this.damage += healthHandler.GetComponent<CharacterData>().maxHealth * this.percentageDamage;
				}
				if (this.hasControl)
				{
					if (this.bulletImmunity != "" && healthHandler)
					{
						healthHandler.GetComponent<PlayerImmunity>().IsImune(0.1f, (this.bulletCanDealDeamage ? this.damage : 1f) * this.dealDamageMultiplierr, this.bulletImmunity);
					}
					if (componentInParent.GetComponent<DamagableEvent>())
					{
						componentInParent.CallTakeDamage(base.transform.forward * this.damage * this.dealDamageMultiplierr, hitInfo.point, this.ownWeapon, this.ownPlayer, true);
					}
					else
					{
						componentInParent.CallTakeDamage(base.transform.forward * (this.bulletCanDealDeamage ? this.damage : 1f) * this.dealDamageMultiplierr, hitInfo.point, this.ownWeapon, this.ownPlayer, true);
					}
				}
			}
		}
		if (playerVelocity)
		{
			float num2 = 1f;
			float d = Mathf.Clamp(playerVelocity.mass / 100f * num2, 0f, 1f) * num2;
			float d2 = 1f;
			playerVelocity.AddForce(-playerVelocity.velocity * 0.1f * playerVelocity.mass, 1);
			if (healthHandler)
			{
				num *= 3f;
				if (this.hasControl)
				{
					healthHandler.CallTakeForce(base.transform.forward * d2 * d * this.force, 1, false, false, 0f);
				}
			}
		}
		if (this.isAllowedToSpawnObjects && !wasBlocked)
		{
			GamefeelManager.GameFeel(base.transform.forward * num * this.shake);
			DynamicParticles.instance.PlayBulletHit(this.damage, base.transform, hitInfo, this.projectileColor);
			for (int i = 0; i < this.objectsToSpawn.Length; i++)
			{
				ObjectsToSpawn.SpawnObject(base.transform, hitInfo, this.objectsToSpawn[i], healthHandler, this.team, this.damage, this.spawnedAttack, wasBlocked);
			}
			base.transform.position = hitInfo.point + hitInfo.normal * 0.01f;
		}
		if (hitInfo.transform)
		{
			NetworkPhysicsObject component4 = hitInfo.transform.GetComponent<NetworkPhysicsObject>();
			if (component4 && this.canPushBox)
			{
				component4.BulletPush(base.transform.forward * (this.force * 0.5f + this.damage * 100f), hitInfo.transform.InverseTransformPoint(hitInfo.point), this.spawnedAttack.spawner.data);
			}
		}
		bool flag = false;
		if (this.effects != null && this.effects.Count != 0)
		{
			for (int j = 0; j < this.effects.Count; j++)
			{
				HasToReturn hasToReturn = this.effects[j].DoHitEffect(hitInfo);
				if (hasToReturn == HasToReturn.hasToReturn)
				{
					flag = true;
				}
				if (hasToReturn == HasToReturn.hasToReturnNow)
				{
					return;
				}
			}
		}
		if (flag)
		{
			return;
		}
		if (this.hitAction != null)
		{
			this.hitAction.Invoke();
		}
		if (this.hitActionWithData != null)
		{
			this.hitActionWithData.Invoke(hitInfo);
		}
		this.deathEvent.Invoke();
		this.DestroyMe();
	}

	// Token: 0x060003C4 RID: 964 RVA: 0x000175C0 File Offset: 0x000157C0
	private void DestroyMe()
	{
		if (this.view)
		{
			if (this.view.IsMine)
			{
				PhotonNetwork.Destroy(base.gameObject);
				return;
			}
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060003C5 RID: 965 RVA: 0x000175F3 File Offset: 0x000157F3
	[PunRPC]
	public void RPCA_CallCustomAction(string actionKey)
	{
		this.customActions[actionKey].Invoke();
	}

	// Token: 0x060003C6 RID: 966 RVA: 0x00017606 File Offset: 0x00015806
	[PunRPC]
	public void RPCA_CallCustomActionV2V2(string actionKey, Vector2 v1, Vector2 v2)
	{
		this.customActionsV2V2[actionKey].Invoke(v1, v2);
	}

	// Token: 0x060003C7 RID: 967 RVA: 0x0001761B File Offset: 0x0001581B
	public void AddHitAction(Action action)
	{
		this.hitAction = (Action)Delegate.Combine(this.hitAction, action);
	}

	// Token: 0x060003C8 RID: 968 RVA: 0x00017634 File Offset: 0x00015834
	public void AddHitActionWithData(Action<HitInfo> action)
	{
		this.hitActionWithData = (Action<HitInfo>)Delegate.Combine(this.hitActionWithData, action);
	}

	// Token: 0x0400050B RID: 1291
	[HideInInspector]
	public bool canPushBox = true;

	// Token: 0x0400050C RID: 1292
	public float force;

	// Token: 0x0400050D RID: 1293
	public float damage;

	// Token: 0x0400050E RID: 1294
	public float stun;

	// Token: 0x0400050F RID: 1295
	public float percentageDamage;

	// Token: 0x04000510 RID: 1296
	public float movementSlow;

	// Token: 0x04000511 RID: 1297
	public float shake;

	// Token: 0x04000512 RID: 1298
	public ObjectsToSpawn[] objectsToSpawn;

	// Token: 0x04000513 RID: 1299
	public PlayerSkin team;

	// Token: 0x04000514 RID: 1300
	[HideInInspector]
	public Player ownPlayer;

	// Token: 0x04000515 RID: 1301
	[HideInInspector]
	public GameObject ownWeapon;

	// Token: 0x04000516 RID: 1302
	public AnimationCurve effectOverTimeCurve;

	// Token: 0x04000517 RID: 1303
	[HideInInspector]
	public List<RayHitEffect> effects;

	// Token: 0x04000518 RID: 1304
	private List<HealthHandler> playersHit = new List<HealthHandler>();

	// Token: 0x04000519 RID: 1305
	public Color projectileColor = Color.black;

	// Token: 0x0400051A RID: 1306
	private Action hitAction;

	// Token: 0x0400051B RID: 1307
	private Action<HitInfo> hitActionWithData;

	// Token: 0x0400051C RID: 1308
	[HideInInspector]
	public bool unblockable;

	// Token: 0x0400051D RID: 1309
	[HideInInspector]
	public bool fullSelfDamage;

	// Token: 0x0400051E RID: 1310
	[FoldoutGroup("Special", 0)]
	public UnityEvent deathEvent;

	// Token: 0x0400051F RID: 1311
	[FoldoutGroup("Special", 0)]
	public bool destroyOnBlock;

	// Token: 0x04000520 RID: 1312
	[FoldoutGroup("Special", 0)]
	public float holdPlayerFor = 0.5f;

	// Token: 0x04000521 RID: 1313
	[FoldoutGroup("Special", 0)]
	public string bulletImmunity = "";

	// Token: 0x04000522 RID: 1314
	private SpawnedAttack spawnedAttack;

	// Token: 0x04000523 RID: 1315
	[HideInInspector]
	public float sinceReflect = 10f;

	// Token: 0x04000524 RID: 1316
	[HideInInspector]
	public float dealDamageMultiplierr = 1f;

	// Token: 0x04000525 RID: 1317
	internal bool hasControl;

	// Token: 0x04000526 RID: 1318
	[HideInInspector]
	public PhotonView view;

	// Token: 0x04000527 RID: 1319
	private MoveTransform move;

	// Token: 0x04000528 RID: 1320
	[HideInInspector]
	public bool bulletCanDealDeamage = true;

	// Token: 0x04000529 RID: 1321
	[HideInInspector]
	public bool isAllowedToSpawnObjects = true;

	// Token: 0x0400052A RID: 1322
	public bool sendCollisions = true;

	// Token: 0x0400052B RID: 1323
	public Dictionary<string, Action> customActions = new Dictionary<string, Action>();

	// Token: 0x0400052C RID: 1324
	public Dictionary<string, Action<Vector2, Vector2>> customActionsV2V2 = new Dictionary<string, Action<Vector2, Vector2>>();
}
