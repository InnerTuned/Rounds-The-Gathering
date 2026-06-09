using System;
using Photon.Pun.Simple.ContactGroups;

namespace Photon.Pun.Simple
{
	// Token: 0x02000260 RID: 608
	public interface IContactSystem
	{
		// Token: 0x17000090 RID: 144
		// (get) Token: 0x06000D40 RID: 3392
		NetObject NetObj { get; }

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x06000D41 RID: 3393
		int ViewID { get; }

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x06000D42 RID: 3394
		bool IsMine { get; }

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x06000D43 RID: 3395
		// (set) Token: 0x06000D44 RID: 3396
		byte SystemIndex { get; set; }

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x06000D45 RID: 3397
		Mount DefaultMount { get; }

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x06000D46 RID: 3398
		int ValidMountsMask { get; }

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x06000D47 RID: 3399
		IContactGroupMask ValidContactGroups { get; }

		// Token: 0x06000D48 RID: 3400
		Consumption TryTrigger(IContactReactor reactor, ContactEvent contactEvent, int compatibleMounts);

		// Token: 0x06000D49 RID: 3401
		Mount TryPickup(IContactReactor reactor, ContactEvent contactEvent);
	}
}
