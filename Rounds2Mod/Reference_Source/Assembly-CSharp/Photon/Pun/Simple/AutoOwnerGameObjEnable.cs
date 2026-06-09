using System;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x02000257 RID: 599
	public class AutoOwnerGameObjEnable : MonoBehaviour, IOnAuthorityChanged
	{
		// Token: 0x06000CF9 RID: 3321 RVA: 0x00040F3C File Offset: 0x0003F13C
		public void Start()
		{
			PhotonView componentInParent = base.GetComponentInParent<PhotonView>();
			if (componentInParent)
			{
				this.SwitchAuth(componentInParent.IsMine);
			}
		}

		// Token: 0x06000CFA RID: 3322 RVA: 0x00040F64 File Offset: 0x0003F164
		public void OnAuthorityChanged(bool isMine, bool controllerChanged)
		{
			global::Debug.Log("AuthChanged");
			this.SwitchAuth(isMine);
		}

		// Token: 0x06000CFB RID: 3323 RVA: 0x00040F77 File Offset: 0x0003F177
		private void SwitchAuth(bool isMine)
		{
			base.gameObject.SetActive((this.enableIf == AutoOwnerGameObjEnable.EnableIf.Owner) ? isMine : (!isMine));
		}

		// Token: 0x04000CA3 RID: 3235
		public AutoOwnerGameObjEnable.EnableIf enableIf;

		// Token: 0x020003C6 RID: 966
		public enum EnableIf
		{
			// Token: 0x040012E2 RID: 4834
			Owner,
			// Token: 0x040012E3 RID: 4835
			Other
		}
	}
}
