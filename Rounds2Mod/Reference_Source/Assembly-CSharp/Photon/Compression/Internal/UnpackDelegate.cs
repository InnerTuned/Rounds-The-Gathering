using System;

namespace Photon.Compression.Internal
{
	// Token: 0x02000225 RID: 549
	// (Invoke) Token: 0x06000C34 RID: 3124
	public delegate SerializationFlags UnpackDelegate<T>(ref T value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);
}
