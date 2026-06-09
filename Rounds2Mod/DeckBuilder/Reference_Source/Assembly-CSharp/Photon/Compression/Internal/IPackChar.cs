using System;

namespace Photon.Compression.Internal
{
	// Token: 0x0200022A RID: 554
	[PackSupportedTypes(typeof(char))]
	public interface IPackChar
	{
		// Token: 0x06000C43 RID: 3139
		SerializationFlags Pack(ref char value, char prevvalue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);

		// Token: 0x06000C44 RID: 3140
		SerializationFlags Unpack(ref char value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);
	}
}
