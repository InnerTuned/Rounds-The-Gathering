using System;
using Photon.Utilities;

namespace Photon.Compression
{
	// Token: 0x0200021E RID: 542
	public interface IPackObjOnReadyChange
	{
		// Token: 0x06000C12 RID: 3090
		void OnPackObjReadyChange(FastBitMask128 readyMask, bool AllAreReady);
	}
}
