using System;

namespace Photon.Pun.Simple
{
	// Token: 0x02000276 RID: 630
	public interface IVitalsContactReactor : IContactReactor
	{
		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x06000DA1 RID: 3489
		VitalNameType VitalNameType { get; }

		// Token: 0x06000DA2 RID: 3490
		double DischargeValue(ContactType contactType = ContactType.Undefined);

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x06000DA3 RID: 3491
		bool AllowOverload { get; }

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x06000DA4 RID: 3492
		bool Propagate { get; }
	}
}
