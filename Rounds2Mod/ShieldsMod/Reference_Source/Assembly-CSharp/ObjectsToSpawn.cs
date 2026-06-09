using System;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x020000AA RID: 170
[Serializable]
public class ObjectsToSpawn
{
	// Token: 0x060003B9 RID: 953 RVA: 0x00016A98 File Offset: 0x00014C98
	public static GameObject[] SpawnObject(Transform spawnerTransform, HitInfo hit, ObjectsToSpawn objectToSpawn, HealthHandler playerHealth, PlayerSkin playerSkins, float damage = 55f, SpawnedAttack spawnedAttack = null, bool wasBlocked = false)
	{
		GameObject[] array = new GameObject[objectToSpawn.numberOfSpawns];
		for (int i = 0; i < objectToSpawn.numberOfSpawns; i++)
		{
			if (!wasBlocked || !objectToSpawn.stickToAllTargets)
			{
				Vector3 position = hit.point + hit.normal * objectToSpawn.normalOffset + (objectToSpawn.zeroZ ? Vector3.zero : (Vector3.forward * 5f));
				Quaternion rotation = Quaternion.LookRotation(spawnerTransform.forward);
				if (objectToSpawn.direction == ObjectsToSpawn.Direction.normal)
				{
					rotation = Quaternion.LookRotation(hit.normal + Vector2.right * 0.005f);
				}
				if (objectToSpawn.direction == ObjectsToSpawn.Direction.identity)
				{
					rotation = Quaternion.identity;
				}
				if ((objectToSpawn.spawnOn != ObjectsToSpawn.SpawnOn.notPlayer || !playerHealth) && (objectToSpawn.spawnOn != ObjectsToSpawn.SpawnOn.player || playerHealth) && objectToSpawn.effect)
				{
					GameObject gameObject = Object.Instantiate<GameObject>(objectToSpawn.effect, position, rotation);
					if (objectToSpawn.spawnAsChild && hit.transform)
					{
						gameObject.transform.SetParent(hit.transform, true);
					}
					if (spawnedAttack)
					{
						spawnedAttack.CopySpawnedAttackTo(gameObject);
					}
					array[i] = gameObject;
					SetTeamColor.TeamColorThis(gameObject, playerSkins);
					if ((objectToSpawn.stickToBigTargets && !playerHealth && (!hit.rigidbody || hit.rigidbody.mass > 500f)) || objectToSpawn.stickToAllTargets)
					{
						gameObject.AddComponent<FollowLocalPos>().Follow(hit.transform);
					}
					if (objectToSpawn.scaleFromDamage != 0f)
					{
						gameObject.transform.localScale *= 1f * (1f - objectToSpawn.scaleFromDamage) + damage / 55f * objectToSpawn.scaleFromDamage;
					}
					if (objectToSpawn.scaleStacks)
					{
						gameObject.transform.localScale *= 1f + (float)objectToSpawn.stacks * objectToSpawn.scaleStackM;
					}
				}
			}
		}
		return array;
	}

	// Token: 0x060003BA RID: 954 RVA: 0x00016CBC File Offset: 0x00014EBC
	public static void SpawnObject(ObjectsToSpawn objectToSpawn, Vector3 position, Quaternion rotation)
	{
		for (int i = 0; i < objectToSpawn.numberOfSpawns; i++)
		{
			Object.Instantiate<GameObject>(objectToSpawn.effect, position, rotation);
		}
	}

	// Token: 0x040004FC RID: 1276
	[FoldoutGroup("OnHit", 0)]
	public GameObject effect;

	// Token: 0x040004FD RID: 1277
	[FoldoutGroup("OnHit", 0)]
	public ObjectsToSpawn.Direction direction;

	// Token: 0x040004FE RID: 1278
	[FoldoutGroup("OnHit", 0)]
	public ObjectsToSpawn.SpawnOn spawnOn;

	// Token: 0x040004FF RID: 1279
	[FoldoutGroup("OnHit", 0)]
	public bool spawnAsChild;

	// Token: 0x04000500 RID: 1280
	[FoldoutGroup("OnHit", 0)]
	public int numberOfSpawns = 1;

	// Token: 0x04000501 RID: 1281
	[FoldoutGroup("OnHit", 0)]
	public float normalOffset;

	// Token: 0x04000502 RID: 1282
	[FoldoutGroup("OnHit", 0)]
	public bool stickToBigTargets;

	// Token: 0x04000503 RID: 1283
	[FoldoutGroup("OnHit", 0)]
	public bool stickToAllTargets;

	// Token: 0x04000504 RID: 1284
	[FoldoutGroup("OnHit", 0)]
	public bool zeroZ;

	// Token: 0x04000505 RID: 1285
	[FoldoutGroup("OnProjectile", 0)]
	public GameObject AddToProjectile;

	// Token: 0x04000506 RID: 1286
	[FoldoutGroup("OnProjectile", 0)]
	public bool removeScriptsFromProjectileObject;

	// Token: 0x04000507 RID: 1287
	[FoldoutGroup("Stacking", 0)]
	public bool scaleStacks;

	// Token: 0x04000508 RID: 1288
	[FoldoutGroup("Stacking", 0)]
	public float scaleStackM = 0.5f;

	// Token: 0x04000509 RID: 1289
	[FoldoutGroup("Scaling", 0)]
	public float scaleFromDamage;

	// Token: 0x0400050A RID: 1290
	[HideInInspector]
	public int stacks;

	// Token: 0x02000371 RID: 881
	public enum Direction
	{
		// Token: 0x04001191 RID: 4497
		forward,
		// Token: 0x04001192 RID: 4498
		normal,
		// Token: 0x04001193 RID: 4499
		identity
	}

	// Token: 0x02000372 RID: 882
	public enum SpawnOn
	{
		// Token: 0x04001195 RID: 4501
		all,
		// Token: 0x04001196 RID: 4502
		player,
		// Token: 0x04001197 RID: 4503
		notPlayer
	}
}
