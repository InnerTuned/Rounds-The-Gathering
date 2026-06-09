using System;
using UnityEngine;

// Token: 0x020000A7 RID: 167
public class PowerUps : MonoBehaviour
{
	// Token: 0x060003AF RID: 943 RVA: 0x00016725 File Offset: 0x00014925
	private void Start()
	{
		this.EffectWeapon(this.weapon);
	}

	// Token: 0x060003B0 RID: 944 RVA: 0x00016733 File Offset: 0x00014933
	private void EffectWeapon(GameObject weapon)
	{
		weapon.GetComponent<Gun>();
	}

	// Token: 0x040004D5 RID: 1237
	public GameObject weapon;

	// Token: 0x040004D6 RID: 1238
	public float damage = 1f;

	// Token: 0x040004D7 RID: 1239
	public float knockback = 1f;

	// Token: 0x040004D8 RID: 1240
	public float attackSpeed = 1f;

	// Token: 0x040004D9 RID: 1241
	public float projectileSpeed = 1f;

	// Token: 0x040004DA RID: 1242
	public float spread;

	// Token: 0x040004DB RID: 1243
	public float evenSpread;

	// Token: 0x040004DC RID: 1244
	public float gravity = 1f;

	// Token: 0x040004DD RID: 1245
	public int projectiles = 1;

	// Token: 0x040004DE RID: 1246
	public int reflects;

	// Token: 0x040004DF RID: 1247
	public int smartBounce;

	// Token: 0x040004E0 RID: 1248
	public int bulletPortal;

	// Token: 0x040004E1 RID: 1249
	public int randomBounces;

	// Token: 0x040004E2 RID: 1250
	public int bursts = 1;

	// Token: 0x040004E3 RID: 1251
	public float lifeSteal;

	// Token: 0x040004E4 RID: 1252
	public float projectileSize;

	// Token: 0x040004E5 RID: 1253
	public float damageAfterDistanceMultiplier = 1f;

	// Token: 0x040004E6 RID: 1254
	public float distancceForDamageAfterDistanceMultiplier = 5f;

	// Token: 0x040004E7 RID: 1255
	public float timeToReachFullMovementMultiplier;

	// Token: 0x040004E8 RID: 1256
	public ObjectsToSpawn[] objectsToSpawn;

	// Token: 0x040004E9 RID: 1257
	public bool waveMovement;

	// Token: 0x040004EA RID: 1258
	public bool teleport;

	// Token: 0x040004EB RID: 1259
	public bool spawnSkelletonSquare;

	// Token: 0x040004EC RID: 1260
	public float explodeNearEnemyRange;

	// Token: 0x040004ED RID: 1261
	public float explodeNearEnemyDamage;

	// Token: 0x040004EE RID: 1262
	public float hitMovementMultiplier = 1f;
}
