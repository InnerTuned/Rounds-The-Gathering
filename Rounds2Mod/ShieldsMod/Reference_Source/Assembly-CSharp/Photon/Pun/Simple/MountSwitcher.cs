using System;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x020002A4 RID: 676
	public class MountSwitcher : NetComponent, IOnPreUpdate, IOnPreSimulate
	{
		// Token: 0x06000ED8 RID: 3800 RVA: 0x00047D14 File Offset: 0x00045F14
		public override void OnAwake()
		{
			base.OnAwake();
			if (this.netObj)
			{
				this.syncState = this.netObj.GetComponent<SyncState>();
			}
			if (!base.GetComponent<SyncState>())
			{
				global::Debug.LogWarning(string.Concat(new string[]
				{
					base.GetType().Name,
					" on '",
					base.transform.parent.name,
					"/",
					base.name,
					"' needs to be on the root of NetObject with component ",
					typeof(SyncState).Name,
					". Disabling."
				}));
				this.netObj.RemoveInterfaces(this);
			}
		}

		// Token: 0x06000ED9 RID: 3801 RVA: 0x00047DCA File Offset: 0x00045FCA
		public void OnPreUpdate()
		{
			if (Input.GetKeyDown(this.keycode))
			{
				this.triggered = true;
			}
		}

		// Token: 0x06000EDA RID: 3802 RVA: 0x00047DE0 File Offset: 0x00045FE0
		public void OnPreSimulate(int frameId, int subFrameId)
		{
			if (this.triggered)
			{
				this.triggered = false;
				Mount currentMount = this.syncState.CurrentMount;
				if (currentMount == null)
				{
					return;
				}
				if (!currentMount.IsMine)
				{
					return;
				}
				global::Debug.Log(string.Concat(new object[]
				{
					"Try change to mount : ",
					currentMount,
					" : ",
					currentMount.IsMine.ToString(),
					" : ",
					this.mount.id
				}));
				this.syncState.ChangeMount(this.mount.id);
			}
		}

		// Token: 0x04000DDA RID: 3546
		public KeyCode keycode = KeyCode.M;

		// Token: 0x04000DDB RID: 3547
		public MountSelector mount = new MountSelector(1);

		// Token: 0x04000DDC RID: 3548
		protected bool triggered;

		// Token: 0x04000DDD RID: 3549
		protected SyncState syncState;
	}
}
