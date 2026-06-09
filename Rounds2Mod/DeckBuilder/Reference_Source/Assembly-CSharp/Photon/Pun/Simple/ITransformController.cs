using System;

namespace Photon.Pun.Simple
{
	// Token: 0x020002B1 RID: 689
	public interface ITransformController
	{
		// Token: 0x170000FA RID: 250
		// (get) Token: 0x06000F2D RID: 3885
		bool HandlesInterpolation { get; }

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x06000F2E RID: 3886
		bool HandlesExtrapolation { get; }
	}
}
