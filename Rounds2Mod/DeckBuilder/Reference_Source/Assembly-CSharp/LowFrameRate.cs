using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200017F RID: 383
public class LowFrameRate : MonoBehaviourPunCallbacks
{
	// Token: 0x060007C6 RID: 1990 RVA: 0x00029C9C File Offset: 0x00027E9C
	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();
		if (this.slowWhat == LowFrameRate.SlowWhat.Both || (PhotonNetwork.IsMasterClient && this.slowWhat == LowFrameRate.SlowWhat.Server) || (!PhotonNetwork.IsMasterClient && this.slowWhat == LowFrameRate.SlowWhat.Client))
		{
			Application.targetFrameRate = this.targetFrameRate;
			QualitySettings.vSyncCount = 0;
			return;
		}
		Application.targetFrameRate = 100;
	}

	// Token: 0x04000916 RID: 2326
	public LowFrameRate.SlowWhat slowWhat = LowFrameRate.SlowWhat.Server;

	// Token: 0x04000917 RID: 2327
	public int targetFrameRate = 10;

	// Token: 0x020003A2 RID: 930
	public enum SlowWhat
	{
		// Token: 0x0400125C RID: 4700
		Both,
		// Token: 0x0400125D RID: 4701
		Server,
		// Token: 0x0400125E RID: 4702
		Client
	}
}
