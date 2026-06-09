using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x0200018D RID: 397
public class PlayerName : MonoBehaviour
{
	// Token: 0x06000808 RID: 2056 RVA: 0x0002BEB2 File Offset: 0x0002A0B2
	private void Start()
	{
		base.GetComponentInParent<TextMeshProUGUI>().text = base.GetComponentInParent<PhotonView>().Owner.NickName;
	}
}
