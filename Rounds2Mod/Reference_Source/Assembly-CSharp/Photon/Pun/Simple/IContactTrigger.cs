using System;
using System.Collections.Generic;

namespace Photon.Pun.Simple
{
	// Token: 0x02000261 RID: 609
	public interface IContactTrigger
	{
		// Token: 0x17000097 RID: 151
		// (get) Token: 0x06000D4A RID: 3402
		NetObject NetObj { get; }

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x06000D4B RID: 3403
		// (set) Token: 0x06000D4C RID: 3404
		byte Index { get; set; }

		// Token: 0x06000D4D RID: 3405
		Consumption ContactCallbacks(ContactEvent contactEvent);

		// Token: 0x06000D4E RID: 3406
		void OnContact(IContactTrigger otherCT, ContactType contactType);

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x06000D4F RID: 3407
		// (set) Token: 0x06000D50 RID: 3408
		IContactTrigger Proxy { get; set; }

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x06000D51 RID: 3409
		// (set) Token: 0x06000D52 RID: 3410
		bool PreventRepeats { get; set; }

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x06000D53 RID: 3411
		List<IContactSystem> ContactSystems { get; }

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x06000D54 RID: 3412
		ISyncContact SyncContact { get; }

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x06000D55 RID: 3413
		IContactGroupsAssign ContactGroupsAssign { get; }
	}
}
