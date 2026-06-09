using System;

namespace Photon.Pun.Simple
{
	// Token: 0x020002CF RID: 719
	public interface IOnSnapshot
	{
		// Token: 0x06000F67 RID: 3943
		bool OnSnapshot(int pre1FrameId, int snapFrameId, int targFrameId, bool prevIsValid, bool snapIsValid, bool targIsValid);
	}
}
