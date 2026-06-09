using System;

namespace Photon.Pun.Simple
{
	// Token: 0x02000262 RID: 610
	public interface IOnContactEvent
	{
		// Token: 0x06000D56 RID: 3414
		Consumption OnContactEvent(ContactEvent contactEvent);

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x06000D57 RID: 3415
		ContactType TriggerOn { get; }
	}
}
