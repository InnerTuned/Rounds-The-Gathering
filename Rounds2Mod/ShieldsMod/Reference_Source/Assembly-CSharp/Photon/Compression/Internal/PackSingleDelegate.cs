using System;

namespace Photon.Compression.Internal
{
	// Token: 0x02000233 RID: 563
	// (Invoke) Token: 0x06000C56 RID: 3158
	public delegate SerializationFlags PackSingleDelegate(ref float value, float preValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);
}
