using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200019A RID: 410
public class ProjectileInit : MonoBehaviour
{
	// Token: 0x06000849 RID: 2121 RVA: 0x0002C903 File Offset: 0x0002AB03
	[PunRPC]
	internal void RPCA_Init(int senderID, int nrOfProj, float dmgM, float randomSeed)
	{
		PlayerManager.instance.GetPlayerWithActorID(senderID).data.weaponHandler.gun.BulletInit(base.gameObject, nrOfProj, dmgM, randomSeed, true);
	}

	// Token: 0x0600084A RID: 2122 RVA: 0x0002C92F File Offset: 0x0002AB2F
	internal void OFFLINE_Init(int senderID, int nrOfProj, float dmgM, float randomSeed)
	{
		PlayerManager.instance.players[senderID].data.weaponHandler.gun.BulletInit(base.gameObject, nrOfProj, dmgM, randomSeed, true);
	}

	// Token: 0x0600084B RID: 2123 RVA: 0x0002C960 File Offset: 0x0002AB60
	[PunRPC]
	internal void RPCA_Init_SeparateGun(int senderID, int gunID, int nrOfProj, float dmgM, float randomSeed)
	{
		this.GetChildGunWithID(gunID, PlayerManager.instance.GetPlayerWithActorID(senderID).gameObject).BulletInit(base.gameObject, nrOfProj, dmgM, randomSeed, true);
	}

	// Token: 0x0600084C RID: 2124 RVA: 0x0002C98A File Offset: 0x0002AB8A
	internal void OFFLINE_Init_SeparateGun(int senderID, int gunID, int nrOfProj, float dmgM, float randomSeed)
	{
		this.GetChildGunWithID(gunID, PlayerManager.instance.players[senderID].gameObject).BulletInit(base.gameObject, nrOfProj, dmgM, randomSeed, true);
	}

	// Token: 0x0600084D RID: 2125 RVA: 0x0002C9B9 File Offset: 0x0002ABB9
	private Gun GetChildGunWithID(int id, GameObject player)
	{
		if (this.guns == null)
		{
			this.guns = player.GetComponentsInChildren<Gun>();
		}
		return this.guns[id];
	}

	// Token: 0x0600084E RID: 2126 RVA: 0x0002C9D7 File Offset: 0x0002ABD7
	[PunRPC]
	internal void RPCA_Init_noAmmoUse(int senderID, int nrOfProj, float dmgM, float randomSeed)
	{
		PlayerManager.instance.GetPlayerWithActorID(senderID).data.weaponHandler.gun.BulletInit(base.gameObject, nrOfProj, dmgM, randomSeed, false);
	}

	// Token: 0x0600084F RID: 2127 RVA: 0x0002CA03 File Offset: 0x0002AC03
	internal void OFFLINE_Init_noAmmoUse(int senderID, int nrOfProj, float dmgM, float randomSeed)
	{
		PlayerManager.instance.players[senderID].data.weaponHandler.gun.BulletInit(base.gameObject, nrOfProj, dmgM, randomSeed, false);
	}

	// Token: 0x04000994 RID: 2452
	private Gun[] guns;
}
