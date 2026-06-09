using System;
using Photon.Pun.Simple;
using UnityEngine.UI;

namespace Photon.Pun.Demo
{
	// Token: 0x02000255 RID: 597
	public class OwnerGUI : NetComponent
	{
		// Token: 0x06000CF1 RID: 3313 RVA: 0x00040E0E File Offset: 0x0003F00E
		public override void OnAwake()
		{
			base.OnAwake();
			this.text = base.GetComponentInChildren<Text>();
		}

		// Token: 0x06000CF2 RID: 3314 RVA: 0x00040E24 File Offset: 0x0003F024
		private void Update()
		{
			if (this.text)
			{
				PhotonView photonView = this.photonView;
				this.text.text = photonView.OwnerActorNr + " : " + photonView.ControllerActorNr;
			}
		}

		// Token: 0x04000C9C RID: 3228
		private Text text;
	}
}
