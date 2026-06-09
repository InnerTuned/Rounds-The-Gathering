using System;

namespace Photon.Compression.Internal
{
	// Token: 0x02000232 RID: 562
	[PackSupportedTypes(typeof(long))]
	public interface IPackInt64
	{
		// Token: 0x06000C53 RID: 3155
		SerializationFlags Pack(ref long value, long preValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);

		// Token: 0x06000C54 RID: 3156
		SerializationFlags Unpack(ref long value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);
	}
}
