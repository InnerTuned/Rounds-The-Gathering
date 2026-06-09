using System;

namespace Photon.Compression.Internal
{
	// Token: 0x0200022B RID: 555
	[PackSupportedTypes(typeof(byte))]
	public interface IPackByte
	{
		// Token: 0x06000C45 RID: 3141
		SerializationFlags Pack(ref byte value, byte prevvalue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);

		// Token: 0x06000C46 RID: 3142
		SerializationFlags Unpack(ref byte value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);
	}
}
