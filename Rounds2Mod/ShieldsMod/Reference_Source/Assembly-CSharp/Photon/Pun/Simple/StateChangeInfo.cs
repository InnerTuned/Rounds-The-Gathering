using System;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x020002AA RID: 682
	public struct StateChangeInfo
	{
		// Token: 0x06000EE9 RID: 3817 RVA: 0x00048320 File Offset: 0x00046520
		public StateChangeInfo(StateChangeInfo src)
		{
			this.objState = src.objState;
			this.mount = src.mount;
			this.offsetPos = src.offsetPos;
			this.offsetRot = src.offsetRot;
			this.velocity = src.velocity;
			this.force = src.force;
		}

		// Token: 0x06000EEA RID: 3818 RVA: 0x00048375 File Offset: 0x00046575
		public StateChangeInfo(ObjState itemState, Mount mount, Vector3? offsetPos, Quaternion? offsetRot, Vector3? velocity, bool force)
		{
			this.objState = itemState;
			this.mount = mount;
			this.offsetPos = offsetPos;
			this.offsetRot = offsetRot;
			this.velocity = velocity;
			this.force = force;
		}

		// Token: 0x06000EEB RID: 3819 RVA: 0x000483A4 File Offset: 0x000465A4
		public StateChangeInfo(ObjState itemState, Mount mount, Vector3? offsetPos, Vector3? velocity, bool force)
		{
			this.objState = itemState;
			this.mount = mount;
			this.offsetPos = offsetPos;
			this.offsetRot = default(Quaternion?);
			this.velocity = velocity;
			this.force = force;
		}

		// Token: 0x06000EEC RID: 3820 RVA: 0x000483D7 File Offset: 0x000465D7
		public StateChangeInfo(ObjState itemState, Mount mount, bool force)
		{
			this.objState = itemState;
			this.mount = mount;
			this.offsetPos = default(Vector3?);
			this.offsetRot = default(Quaternion?);
			this.velocity = default(Vector3?);
			this.force = force;
		}

		// Token: 0x04000DFB RID: 3579
		public ObjState objState;

		// Token: 0x04000DFC RID: 3580
		public Mount mount;

		// Token: 0x04000DFD RID: 3581
		public Vector3? offsetPos;

		// Token: 0x04000DFE RID: 3582
		public Quaternion? offsetRot;

		// Token: 0x04000DFF RID: 3583
		public Vector3? velocity;

		// Token: 0x04000E00 RID: 3584
		public bool force;
	}
}
