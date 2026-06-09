using System;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x02000295 RID: 661
	[RequireComponent(typeof(Mount))]
	public class MountThrow : NetComponent, IOnPreUpdate, IOnPreSimulate
	{
		// Token: 0x06000E5E RID: 3678 RVA: 0x00044E00 File Offset: 0x00043000
		public override void OnAwake()
		{
			base.OnAwake();
			if (this.mount == null)
			{
				this.mount = base.GetComponent<Mount>();
			}
		}

		// Token: 0x06000E5F RID: 3679 RVA: 0x00044E22 File Offset: 0x00043022
		public void OnPreUpdate()
		{
			if (!base.IsMine)
			{
				return;
			}
			if (Input.GetKeyDown(this.throwKey))
			{
				this.throwQueued = true;
			}
		}

		// Token: 0x06000E60 RID: 3680 RVA: 0x00044E44 File Offset: 0x00043044
		public void OnPreSimulate(int frameId, int subFrameId)
		{
			if (!this.throwQueued)
			{
				return;
			}
			this.throwQueued = false;
			List<IMountable> mountedObjs = this.mount.mountedObjs;
			for (int i = 0; i < mountedObjs.Count; i++)
			{
				IMountable mountable = mountedObjs[i];
				Rigidbody rb = mountable.Rb;
				if (rb && mountable.IsThrowable)
				{
					SyncState syncState = mountable as SyncState;
					if (syncState)
					{
						Transform transform = this.fromRoot ? this.mount.mountsLookup.transform : base.transform;
						Vector3 position = transform.TransformPoint(this.offset);
						Quaternion rotation = transform.rotation;
						Vector3 vector = (this.inheritRBVelocity && rb) ? (rb.velocity + transform.TransformVector(this.velocity)) : transform.TransformVector(this.velocity);
						syncState.Throw(position, rotation, vector);
					}
				}
			}
		}

		// Token: 0x04000D73 RID: 3443
		public KeyCode throwKey;

		// Token: 0x04000D74 RID: 3444
		public Mount mount;

		// Token: 0x04000D75 RID: 3445
		public bool fromRoot = true;

		// Token: 0x04000D76 RID: 3446
		public bool inheritRBVelocity = true;

		// Token: 0x04000D77 RID: 3447
		public Vector3 offset = new Vector3(0f, 3f, 0f);

		// Token: 0x04000D78 RID: 3448
		public Vector3 velocity = new Vector3(0f, 1f, 5f);

		// Token: 0x04000D79 RID: 3449
		private bool throwQueued;
	}
}
