using System;

namespace Photon.Compression
{
	// Token: 0x020001F0 RID: 496
	public enum SerializationFlags
	{
		// Token: 0x04000B26 RID: 2854
		None,
		// Token: 0x04000B27 RID: 2855
		HasContent,
		// Token: 0x04000B28 RID: 2856
		Force,
		// Token: 0x04000B29 RID: 2857
		ForceReliable = 4,
		// Token: 0x04000B2A RID: 2858
		SendToSelf = 8,
		// Token: 0x04000B2B RID: 2859
		NewConnection = 16,
		// Token: 0x04000B2C RID: 2860
		IsComplete = 32
	}
}
