using System;

namespace Photon.Pun.Simple
{
	// Token: 0x020002CB RID: 715
	public interface IDeltaFrameChangeDetect : IUseKeyframes
	{
		// Token: 0x17000101 RID: 257
		// (get) Token: 0x06000F60 RID: 3936
		// (set) Token: 0x06000F61 RID: 3937
		bool UseDeltas { get; set; }
	}
}
