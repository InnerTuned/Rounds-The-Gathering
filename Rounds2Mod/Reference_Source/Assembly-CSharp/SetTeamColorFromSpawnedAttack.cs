using System;
using UnityEngine;

// Token: 0x020000D3 RID: 211
public class SetTeamColorFromSpawnedAttack : MonoBehaviour
{
	// Token: 0x0600044E RID: 1102 RVA: 0x00019EDA File Offset: 0x000180DA
	private void Start()
	{
		base.GetComponent<SetTeamColor>().Set(PlayerSkinBank.GetPlayerSkinColors(base.GetComponentInParent<SpawnedAttack>().spawner.playerID));
	}
}
