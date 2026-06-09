using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000D8 RID: 216
public class SpawnedAttack : MonoBehaviour
{
	// Token: 0x06000459 RID: 1113 RVA: 0x00019F8E File Offset: 0x0001818E
	private void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x0600045A RID: 1114 RVA: 0x00019F9C File Offset: 0x0001819C
	[PunRPC]
	public void RPCA_SetSpawner(int spawnerID)
	{
		this.spawner = PhotonNetwork.GetPhotonView(spawnerID).GetComponent<Player>();
	}

	// Token: 0x0600045B RID: 1115 RVA: 0x00019FB0 File Offset: 0x000181B0
	public void CopySpawnedAttackTo(GameObject to)
	{
		SpawnedAttack spawnedAttack = to.GetComponent<SpawnedAttack>();
		if (!spawnedAttack)
		{
			spawnedAttack = to.AddComponent<SpawnedAttack>();
		}
		spawnedAttack.spawner = this.spawner;
		spawnedAttack.attackLevel = this.attackLevel;
	}

	// Token: 0x0600045C RID: 1116 RVA: 0x00019FEC File Offset: 0x000181EC
	public void SetColor(Color color)
	{
		TrailRenderer[] componentsInChildren = base.GetComponentsInChildren<TrailRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].startColor = color;
			componentsInChildren[i].endColor = color;
		}
		ProjectileHit component = base.GetComponent<ProjectileHit>();
		if (component)
		{
			component.projectileColor = color;
		}
	}

	// Token: 0x0600045D RID: 1117 RVA: 0x0001A036 File Offset: 0x00018236
	public bool IsMine()
	{
		if (this.view)
		{
			return this.view.IsMine;
		}
		return this.spawner && this.spawner.data.view.IsMine;
	}

	// Token: 0x040005D2 RID: 1490
	public Player spawner;

	// Token: 0x040005D3 RID: 1491
	public int attackLevel;

	// Token: 0x040005D4 RID: 1492
	public int attackID;

	// Token: 0x040005D5 RID: 1493
	public PhotonView view;
}
