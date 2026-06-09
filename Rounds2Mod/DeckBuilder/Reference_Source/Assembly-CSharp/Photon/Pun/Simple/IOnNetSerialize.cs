using System;
using Photon.Compression;

namespace Photon.Pun.Simple
{
	// Token: 0x020002CD RID: 717
	public interface IOnNetSerialize
	{
		// Token: 0x06000F63 RID: 3939
		SerializationFlags OnNetSerialize(int frameId, byte[] buffer, ref int bitposition, SerializationFlags writeFlags);

		// Token: 0x06000F64 RID: 3940
		SerializationFlags OnNetDeserialize(int originFrameId, byte[] buffer, ref int bitposition, FrameArrival frameArrival);

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x06000F65 RID: 3941
		bool SkipWhenEmpty { get; }
	}
}
