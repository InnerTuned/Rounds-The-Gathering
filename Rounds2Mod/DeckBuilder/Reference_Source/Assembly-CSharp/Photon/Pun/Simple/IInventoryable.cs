using System;

namespace Photon.Pun.Simple
{
	// Token: 0x02000267 RID: 615
	public interface IInventoryable<T> : IContactable
	{
		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x06000D6E RID: 3438
		T Size { get; }
	}
}
