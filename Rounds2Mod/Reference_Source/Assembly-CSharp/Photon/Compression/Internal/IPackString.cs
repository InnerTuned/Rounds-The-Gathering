using System;

namespace Photon.Compression.Internal
{
	// Token: 0x02000236 RID: 566
	[PackSupportedTypes(typeof(string))]
	public interface IPackString
	{
		// Token: 0x06000C5D RID: 3165
		SerializationFlags Pack(ref string value, string preValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);

		// Token: 0x06000C5E RID: 3166
		SerializationFlags Unpack(ref string value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);
	}
}
