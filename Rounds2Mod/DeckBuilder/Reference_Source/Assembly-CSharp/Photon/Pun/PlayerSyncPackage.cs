using System;
using UnityEngine;

namespace Photon.Pun
{
	// Token: 0x02000251 RID: 593
	public class PlayerSyncPackage
	{
		// Token: 0x04000C8B RID: 3211
		public Vector3 pos;

		// Token: 0x04000C8C RID: 3212
		public Vector3 dir;

		// Token: 0x04000C8D RID: 3213
		public Vector3 aim;

		// Token: 0x04000C8E RID: 3214
		public float sinceGrounded;

		// Token: 0x04000C8F RID: 3215
		public bool holdJump;

		// Token: 0x04000C90 RID: 3216
		public bool jump;

		// Token: 0x04000C91 RID: 3217
		public Vector2 vel;

		// Token: 0x04000C92 RID: 3218
		public float timeDelta;
	}
}
