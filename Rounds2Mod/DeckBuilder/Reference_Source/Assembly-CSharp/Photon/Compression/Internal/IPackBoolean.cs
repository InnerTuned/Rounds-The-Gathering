using System;

namespace Photon.Compression.Internal
{
	// Token: 0x02000229 RID: 553
	[PackSupportedTypes(typeof(bool))]
	public interface IPackBoolean
	{
		// Token: 0x06000C41 RID: 3137
		SerializationFlags Pack(ref bool value, bool prevvalue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);

		// Token: 0x06000C42 RID: 3138
		SerializationFlags Unpack(ref bool value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);
	}
}
