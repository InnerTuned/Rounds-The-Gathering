using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

// Token: 0x0200012F RID: 303
public class DisplayMatchPlayerNames : MonoBehaviour
{
	// Token: 0x060005D5 RID: 1493 RVA: 0x00020E20 File Offset: 0x0001F020
	public void ShowNames()
	{
		List<Player> list = Enumerable.ToList<Player>(Enumerable.Select<KeyValuePair<int, Player>, Player>(PhotonNetwork.CurrentRoom.Players, (KeyValuePair<int, Player> p) => p.Value));
		bool flag = PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(NetworkConnectionHandler.TWITCH_ROOM_AUDIENCE_RATING_KEY);
		for (int i = 0; i < list.Count; i++)
		{
			string text = flag ? (" (" + list[i].CustomProperties[NetworkConnectionHandler.TWITCH_PLAYER_SCORE_KEY].ToString() + ")") : string.Empty;
			if (i == 0)
			{
				this.local.text = list[i].NickName + text;
			}
			else
			{
				this.other.text = list[i].NickName + text;
			}
		}
	}

	// Token: 0x0400076F RID: 1903
	public TextMeshProUGUI local;

	// Token: 0x04000770 RID: 1904
	public TextMeshProUGUI other;
}
