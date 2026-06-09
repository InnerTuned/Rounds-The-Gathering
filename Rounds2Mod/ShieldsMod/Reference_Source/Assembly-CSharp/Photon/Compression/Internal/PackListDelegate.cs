using System;
using System.Collections.Generic;

namespace Photon.Compression.Internal
{
	// Token: 0x02000226 RID: 550
	// (Invoke) Token: 0x06000C38 RID: 3128
	public delegate SerializationFlags PackListDelegate<T>(ref List<T> value, List<T> prevvalue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags) where T : struct;
}
