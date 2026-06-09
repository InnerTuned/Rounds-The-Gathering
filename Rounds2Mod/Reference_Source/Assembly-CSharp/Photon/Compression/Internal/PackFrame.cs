using System;
using Photon.Utilities;

namespace Photon.Compression.Internal
{
	// Token: 0x0200023C RID: 572
	public abstract class PackFrame
	{
		// Token: 0x04000C4D RID: 3149
		public FastBitMask128 mask;

		// Token: 0x04000C4E RID: 3150
		public FastBitMask128 isCompleteMask;
	}
}
