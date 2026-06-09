using System;

namespace Photon.Compression.Internal
{
	// Token: 0x0200022C RID: 556
	[PackSupportedTypes(typeof(sbyte))]
	public interface IPackSByte
	{
		// Token: 0x06000C47 RID: 3143
		SerializationFlags Pack(ref sbyte value, sbyte prevvalue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);

		// Token: 0x06000C48 RID: 3144
		SerializationFlags Unpack(ref sbyte value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);
	}
}
