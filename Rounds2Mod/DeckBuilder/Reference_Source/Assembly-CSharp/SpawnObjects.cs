using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000DC RID: 220
public class SpawnObjects : MonoBehaviour
{
	// Token: 0x06000468 RID: 1128 RVA: 0x0001A4BC File Offset: 0x000186BC
	private void ConfigureObject(GameObject go)
	{
		SpawnedAttack spawnedAttack = go.GetComponent<SpawnedAttack>();
		if (!spawnedAttack)
		{
			spawnedAttack = go.AddComponent<SpawnedAttack>();
		}
		spawnedAttack.spawner = base.transform.root.GetComponent<Player>();
		if (!spawnedAttack.spawner)
		{
			SpawnedAttack componentInParent = base.transform.GetComponentInParent<SpawnedAttack>();
			if (componentInParent)
			{
				componentInParent.CopySpawnedAttackTo(go);
			}
		}
		AttackLevel componentInParent2 = base.GetComponentInParent<AttackLevel>();
		if (componentInParent2)
		{
			spawnedAttack.attackLevel = componentInParent2.attackLevel;
		}
		if (this.inheritScale)
		{
			go.transform.localScale *= base.transform.localScale.x;
		}
		if (this.SpawnedAction != null)
		{
			this.SpawnedAction.Invoke(go);
		}
	}

	// Token: 0x06000469 RID: 1129 RVA: 0x0001A57C File Offset: 0x0001877C
	public void Spawn()
	{
		for (int i = 0; i < this.objectToSpawn.Length; i++)
		{
			Quaternion rotation = Quaternion.identity;
			if (this.spawnRot == SpawnObjects.SpawnRot.TransformRotation)
			{
				rotation = base.transform.rotation;
			}
			GameObject go = Object.Instantiate<GameObject>(this.objectToSpawn[i], base.transform.position, rotation);
			this.ConfigureObject(go);
			this.mostRecentlySpawnedObject = go;
		}
		if (this.destroyObject)
		{
			Object.Destroy(base.gameObject);
		}
		if (this.destroyRoot)
		{
			Object.Destroy(base.transform.root.gameObject);
		}
	}

	// Token: 0x040005F1 RID: 1521
	public GameObject[] objectToSpawn;

	// Token: 0x040005F2 RID: 1522
	public SpawnObjects.SpawnRot spawnRot;

	// Token: 0x040005F3 RID: 1523
	public bool inheritScale;

	// Token: 0x040005F4 RID: 1524
	public bool destroyObject;

	// Token: 0x040005F5 RID: 1525
	public bool destroyRoot;

	// Token: 0x040005F6 RID: 1526
	private PhotonView view;

	// Token: 0x040005F7 RID: 1527
	public Action<GameObject> SpawnedAction;

	// Token: 0x040005F8 RID: 1528
	[HideInInspector]
	public GameObject mostRecentlySpawnedObject;

	// Token: 0x0200037B RID: 891
	public enum SpawnRot
	{
		// Token: 0x040011BA RID: 4538
		Identity,
		// Token: 0x040011BB RID: 4539
		TransformRotation
	}
}
