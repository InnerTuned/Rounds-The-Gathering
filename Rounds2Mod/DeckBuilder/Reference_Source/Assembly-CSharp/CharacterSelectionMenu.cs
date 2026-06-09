using System;
using UnityEngine;

// Token: 0x02000119 RID: 281
public class CharacterSelectionMenu : MonoBehaviour
{
	// Token: 0x0600058B RID: 1419 RVA: 0x0002006E File Offset: 0x0001E26E
	private void Start()
	{
		PlayerManager instance = PlayerManager.instance;
		instance.PlayerJoinedAction = (Action<Player>)Delegate.Combine(instance.PlayerJoinedAction, new Action<Player>(this.PlayerJoined));
	}

	// Token: 0x0600058C RID: 1420 RVA: 0x00020096 File Offset: 0x0001E296
	private void PlayerJoined(Player joinedPlayer)
	{
		base.transform.GetChild(0).GetChild(PlayerManager.instance.players.Count - 1).GetComponent<CharacterSelectionInstance>().StartPicking(joinedPlayer);
	}

	// Token: 0x0600058D RID: 1421 RVA: 0x000027C8 File Offset: 0x000009C8
	private void Update()
	{
	}
}
