using System;
using UnityEngine;

namespace Photon.Pun.Example
{
	// Token: 0x02000254 RID: 596
	public class TakeOwnership : MonoBehaviour
	{
		// Token: 0x06000CED RID: 3309 RVA: 0x000027C8 File Offset: 0x000009C8
		private void Start()
		{
		}

		// Token: 0x06000CEE RID: 3310 RVA: 0x00040DD2 File Offset: 0x0003EFD2
		private void Update()
		{
			if (Input.GetKeyDown(this.keycode))
			{
				this.TransferOwner();
			}
		}

		// Token: 0x06000CEF RID: 3311 RVA: 0x00040DE7 File Offset: 0x0003EFE7
		public void TransferOwner()
		{
			base.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
		}

		// Token: 0x04000C9B RID: 3227
		public KeyCode keycode = KeyCode.C;
	}
}
