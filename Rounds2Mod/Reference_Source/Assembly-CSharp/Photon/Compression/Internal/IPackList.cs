using System;
using System.Collections;
using System.Collections.Generic;

namespace Photon.Compression.Internal
{
	// Token: 0x02000228 RID: 552
	[PackSupportedTypes(typeof(List))]
	public interface IPackList<T> where T : struct
	{
		// Token: 0x06000C3F RID: 3135
		SerializationFlags Pack(ref List<T> value, List<T> prevvalue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);

		// Token: 0x06000C40 RID: 3136
		SerializationFlags Unpack(ref List<T> value, BitArray mask, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);
	}
}
