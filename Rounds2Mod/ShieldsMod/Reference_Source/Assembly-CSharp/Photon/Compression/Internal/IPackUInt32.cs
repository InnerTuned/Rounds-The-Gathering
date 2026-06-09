using System;

namespace Photon.Compression.Internal
{
	// Token: 0x0200022F RID: 559
	[PackSupportedTypes(typeof(uint))]
	public interface IPackUInt32
	{
		// Token: 0x06000C4D RID: 3149
		SerializationFlags Pack(ref uint value, uint preValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);

		// Token: 0x06000C4E RID: 3150
		SerializationFlags Unpack(ref uint value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);
	}
}
