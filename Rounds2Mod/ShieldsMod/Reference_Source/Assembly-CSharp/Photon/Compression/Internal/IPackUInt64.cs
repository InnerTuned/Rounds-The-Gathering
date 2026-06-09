using System;

namespace Photon.Compression.Internal
{
	// Token: 0x02000231 RID: 561
	[PackSupportedTypes(typeof(ulong))]
	public interface IPackUInt64
	{
		// Token: 0x06000C51 RID: 3153
		SerializationFlags Pack(ref ulong value, ulong preValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);

		// Token: 0x06000C52 RID: 3154
		SerializationFlags Unpack(ref ulong value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);
	}
}
