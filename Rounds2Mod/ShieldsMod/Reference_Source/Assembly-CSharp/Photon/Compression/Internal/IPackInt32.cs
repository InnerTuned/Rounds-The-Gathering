using System;

namespace Photon.Compression.Internal
{
	// Token: 0x02000230 RID: 560
	[PackSupportedTypes(typeof(int))]
	public interface IPackInt32
	{
		// Token: 0x06000C4F RID: 3151
		SerializationFlags Pack(ref int value, int preValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);

		// Token: 0x06000C50 RID: 3152
		SerializationFlags Unpack(ref int value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);
	}
}
