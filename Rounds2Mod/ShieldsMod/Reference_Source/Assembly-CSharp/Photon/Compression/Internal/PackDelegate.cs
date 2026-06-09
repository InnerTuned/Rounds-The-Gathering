using System;

namespace Photon.Compression.Internal
{
	// Token: 0x02000224 RID: 548
	// (Invoke) Token: 0x06000C30 RID: 3120
	public delegate SerializationFlags PackDelegate<T>(ref T value, T prevvalue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);
}
