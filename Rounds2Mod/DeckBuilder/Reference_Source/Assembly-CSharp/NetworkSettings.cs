using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000175 RID: 373
public class NetworkSettings : MonoBehaviour
{
	// Token: 0x06000797 RID: 1943 RVA: 0x0002926B File Offset: 0x0002746B
	private void Start()
	{
		PhotonNetwork.SendRate = 30;
		PhotonNetwork.SerializationRate = 30;
	}
}
