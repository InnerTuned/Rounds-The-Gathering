using System;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x02000256 RID: 598
	public class AutoOwnerComponentEnable : NetComponent, IOnAuthorityChanged
	{
		// Token: 0x06000CF4 RID: 3316 RVA: 0x00040E70 File Offset: 0x0003F070
		public override void OnStart()
		{
			base.OnStart();
			this.SwitchAuth(base.IsMine);
		}

		// Token: 0x06000CF5 RID: 3317 RVA: 0x00040E84 File Offset: 0x0003F084
		public override void OnJoinedRoom()
		{
			base.OnJoinedRoom();
			this.SwitchAuth(base.IsMine);
		}

		// Token: 0x06000CF6 RID: 3318 RVA: 0x00040E98 File Offset: 0x0003F098
		public override void OnAuthorityChanged(bool isMine, bool controllerChanged)
		{
			base.OnAuthorityChanged(isMine, controllerChanged);
			this.SwitchAuth(isMine);
		}

		// Token: 0x06000CF7 RID: 3319 RVA: 0x00040EAC File Offset: 0x0003F0AC
		private void SwitchAuth(bool isMine)
		{
			for (int i = 0; i < this.componentToggles.Count; i++)
			{
				AutoOwnerComponentEnable.ComponentToggle componentToggle = this.componentToggles[i];
				if (componentToggle != null && componentToggle.enableIfOwned != AutoOwnerComponentEnable.EnableIf.Ignore && componentToggle.component != null)
				{
					componentToggle.component.enabled = ((componentToggle.enableIfOwned == AutoOwnerComponentEnable.EnableIf.Owner) ? isMine : (!isMine));
				}
			}
		}

		// Token: 0x04000C9D RID: 3229
		public bool includeChildren = true;

		// Token: 0x04000C9E RID: 3230
		public bool includeUnity = true;

		// Token: 0x04000C9F RID: 3231
		public bool includePhoton;

		// Token: 0x04000CA0 RID: 3232
		public bool includeSimple;

		// Token: 0x04000CA1 RID: 3233
		[HideInInspector]
		[SerializeField]
		private List<AutoOwnerComponentEnable.ComponentToggle> componentToggles = new List<AutoOwnerComponentEnable.ComponentToggle>();

		// Token: 0x04000CA2 RID: 3234
		[HideInInspector]
		[SerializeField]
		private List<Behaviour> componentLookup = new List<Behaviour>();

		// Token: 0x020003C4 RID: 964
		public enum EnableIf
		{
			// Token: 0x040012DC RID: 4828
			Ignore,
			// Token: 0x040012DD RID: 4829
			Owner,
			// Token: 0x040012DE RID: 4830
			Other
		}

		// Token: 0x020003C5 RID: 965
		[Serializable]
		public class ComponentToggle
		{
			// Token: 0x040012DF RID: 4831
			public Behaviour component;

			// Token: 0x040012E0 RID: 4832
			public AutoOwnerComponentEnable.EnableIf enableIfOwned = AutoOwnerComponentEnable.EnableIf.Owner;
		}
	}
}
