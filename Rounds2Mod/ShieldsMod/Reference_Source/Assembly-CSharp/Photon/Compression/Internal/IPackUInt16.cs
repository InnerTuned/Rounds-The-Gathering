using System;

namespace Photon.Compression.Internal
{
	// Token: 0x0200022D RID: 557
	[PackSupportedTypes(typeof(ushort))]
	public interface IPackUInt16
	{
		// Token: 0x06000C49 RID: 3145
		SerializationFlags Pack(ref ushort value, ushort preValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);

		// Token: 0x06000C4A RID: 3146
		SerializationFlags Unpack(ref ushort value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);
	}
}
