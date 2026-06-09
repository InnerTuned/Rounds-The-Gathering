using System;

namespace Photon.Pun.Simple
{
	// Token: 0x0200026F RID: 623
	public interface IProjectileCannon
	{
		// Token: 0x170000AE RID: 174
		// (get) Token: 0x06000D86 RID: 3462
		PhotonView PhotonView { get; }

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06000D87 RID: 3463
		NetObject NetObj { get; }

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06000D88 RID: 3464
		IContactTrigger ContactTrigger { get; }

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x06000D89 RID: 3465
		int ViewID { get; }
	}
}
