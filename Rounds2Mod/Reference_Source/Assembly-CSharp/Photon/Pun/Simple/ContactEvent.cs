using System;

namespace Photon.Pun.Simple
{
	// Token: 0x02000264 RID: 612
	public struct ContactEvent
	{
		// Token: 0x06000D59 RID: 3417 RVA: 0x00041DAE File Offset: 0x0003FFAE
		public ContactEvent(IContactSystem contactSystem, IContactTrigger contacter, ContactType contactType)
		{
			this.contactSystem = contactSystem;
			this.contactTrigger = contacter;
			this.contactType = contactType;
		}

		// Token: 0x06000D5A RID: 3418 RVA: 0x00041DC5 File Offset: 0x0003FFC5
		public ContactEvent(ContactEvent contactEvent)
		{
			this.contactSystem = contactEvent.contactSystem;
			this.contactTrigger = contactEvent.contactTrigger;
			this.contactType = contactEvent.contactType;
		}

		// Token: 0x04000CCE RID: 3278
		public readonly IContactSystem contactSystem;

		// Token: 0x04000CCF RID: 3279
		public readonly IContactTrigger contactTrigger;

		// Token: 0x04000CD0 RID: 3280
		public readonly ContactType contactType;
	}
}
