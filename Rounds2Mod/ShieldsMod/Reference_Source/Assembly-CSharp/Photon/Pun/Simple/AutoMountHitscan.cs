using System;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x0200029D RID: 669
	public class AutoMountHitscan : HitscanComponent
	{
		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x06000EC4 RID: 3780 RVA: 0x00047BA8 File Offset: 0x00045DA8
		public SyncState SyncState
		{
			get
			{
				return this.syncState;
			}
		}

		// Token: 0x06000EC5 RID: 3781 RVA: 0x00047BB0 File Offset: 0x00045DB0
		public override void OnAwake()
		{
			base.OnAwake();
			if (this.netObj)
			{
				this.syncState = this.netObj.GetComponent<SyncState>();
			}
		}

		// Token: 0x06000EC6 RID: 3782 RVA: 0x00047BD8 File Offset: 0x00045DD8
		public override void OnAuthorityChanged(bool isMine, bool controllerChanged)
		{
			base.OnAuthorityChanged(isMine, controllerChanged);
			List<IOnPreSimulate> onPreSimulateCallbacks = this.netObj.onPreSimulateCallbacks;
			bool flag = onPreSimulateCallbacks.Contains(this);
			base.OnAuthorityChanged(isMine, controllerChanged);
			if (isMine)
			{
				if (!flag)
				{
					onPreSimulateCallbacks.Add(this);
					return;
				}
			}
			else if (flag)
			{
				onPreSimulateCallbacks.Remove(this);
			}
		}

		// Token: 0x06000EC7 RID: 3783 RVA: 0x00047C24 File Offset: 0x00045E24
		public override void OnPreSimulate(int frameId, int subFrameId)
		{
			if (subFrameId == TickEngineSettings.sendEveryXTick - 1)
			{
				this.triggerQueued = true;
				base.OnPreSimulate(frameId, subFrameId);
				if (this.foundMounts.Count != 0)
				{
					do
					{
						Mount attachTo = this.foundMounts.Dequeue();
						this.syncState.SoftMount(attachTo);
					}
					while (this.foundMounts.Count != 0);
					return;
				}
				this.syncState.SoftMount(null);
			}
		}

		// Token: 0x06000EC8 RID: 3784 RVA: 0x00047C8C File Offset: 0x00045E8C
		public override bool ProcessHit(Collider hit)
		{
			Mount nestedComponentInParents = NestedComponentUtilities.GetNestedComponentInParents<Mount, NetObject>(hit.transform);
			if (nestedComponentInParents)
			{
				this.foundMounts.Enqueue(nestedComponentInParents);
			}
			return false;
		}

		// Token: 0x04000DC5 RID: 3525
		protected SyncState syncState;

		// Token: 0x04000DC6 RID: 3526
		private Queue<Mount> foundMounts = new Queue<Mount>();
	}
}
