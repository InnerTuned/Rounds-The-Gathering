using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000172 RID: 370
public class NetworkData : MonoBehaviour
{
	// Token: 0x06000782 RID: 1922 RVA: 0x000287CB File Offset: 0x000269CB
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000783 RID: 1923 RVA: 0x000287D9 File Offset: 0x000269D9
	private void Init()
	{
		if (this.inited)
		{
			return;
		}
		this.inited = true;
		if (PhotonNetwork.IsMasterClient)
		{
			global::Debug.Log("Why am i the master?");
		}
	}

	// Token: 0x06000784 RID: 1924 RVA: 0x000287FC File Offset: 0x000269FC
	private void Update()
	{
		if (PhotonNetwork.InRoom)
		{
			this.Init();
		}
	}

	// Token: 0x06000785 RID: 1925 RVA: 0x0002880B File Offset: 0x00026A0B
	private void RequestJoin()
	{
		this.photonView.RPC("RequestJoinMaster", 2, Array.Empty<object>());
		global::Debug.Log("Request join");
	}

	// Token: 0x06000786 RID: 1926 RVA: 0x00028830 File Offset: 0x00026A30
	[PunRPC]
	public void RequestJoinMaster()
	{
		string text = JsonUtility.ToJson(new InitPackage
		{
			currentMapID = MapManager.instance.currentLevelID
		});
		this.photonView.RPC("RequestJoinResponse", 1, new object[]
		{
			text
		});
	}

	// Token: 0x06000787 RID: 1927 RVA: 0x00028874 File Offset: 0x00026A74
	[PunRPC]
	public void RequestJoinResponse(string jsonResponse)
	{
		InitPackage initPackage = (InitPackage)JsonUtility.FromJson(jsonResponse, typeof(InitPackage));
		MapManager.instance.LoadLevelFromID(initPackage.currentMapID, false, true);
		global::Debug.Log("Got response");
	}

	// Token: 0x040008D3 RID: 2259
	private PhotonView photonView;

	// Token: 0x040008D4 RID: 2260
	private bool inited;
}
