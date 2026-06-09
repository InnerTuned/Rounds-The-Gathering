using System;

namespace Photon.Pun.Simple
{
	// Token: 0x0200025D RID: 605
	public interface IConsumable
	{
		// Token: 0x1700008D RID: 141
		// (get) Token: 0x06000D3C RID: 3388
		// (set) Token: 0x06000D3D RID: 3389
		double Charges { get; set; }

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x06000D3E RID: 3390
		Consumption Consumption { get; }

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x06000D3F RID: 3391
		ConsumedDespawn ConsumedDespawn { get; }
	}
}
