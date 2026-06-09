using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001AD RID: 429
public class SetOfflineMode : MonoBehaviour
{
	// Token: 0x06000892 RID: 2194 RVA: 0x0002D85E File Offset: 0x0002BA5E
	private void Awake()
	{
		if (this.doIt)
		{
			this.SetOffline();
		}
	}

	// Token: 0x06000893 RID: 2195 RVA: 0x0002D86E File Offset: 0x0002BA6E
	public void SetOffline()
	{
		PhotonNetwork.OfflineMode = true;
		PhotonNetwork.JoinRandomRoom();
	}

	// Token: 0x06000894 RID: 2196 RVA: 0x0002D87C File Offset: 0x0002BA7C
	public void SetOnline()
	{
		if (PhotonNetwork.InRoom)
		{
			PhotonNetwork.LeaveRoom(true);
		}
		PhotonNetwork.OfflineMode = false;
	}

	// Token: 0x040009D2 RID: 2514
	public bool doIt = true;
}
