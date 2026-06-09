using System;
using UnityEngine;

// Token: 0x020000D2 RID: 210
public class SetTeamColorFromParentPlayer : MonoBehaviour
{
	// Token: 0x0600044C RID: 1100 RVA: 0x00019E9C File Offset: 0x0001809C
	private void Start()
	{
		Player player = base.GetComponentInParent<Player>();
		if (!player)
		{
			player = base.GetComponentInParent<SpawnedAttack>().spawner;
		}
		base.GetComponent<SetTeamColor>().Set(PlayerSkinBank.GetPlayerSkinColors(player.playerID));
	}
}
