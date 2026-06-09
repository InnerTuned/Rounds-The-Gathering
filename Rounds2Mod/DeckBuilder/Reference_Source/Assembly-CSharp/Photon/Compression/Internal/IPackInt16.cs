using System;

namespace Photon.Compression.Internal
{
	// Token: 0x0200022E RID: 558
	[PackSupportedTypes(typeof(short))]
	public interface IPackInt16
	{
		// Token: 0x06000C4B RID: 3147
		SerializationFlags Pack(ref short value, short preValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);

		// Token: 0x06000C4C RID: 3148
		SerializationFlags Unpack(ref short value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);
	}
}
