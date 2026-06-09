using System;

namespace Photon.Compression.Internal
{
	// Token: 0x02000234 RID: 564
	[PackSupportedTypes(typeof(float))]
	public interface IPackSingle
	{
		// Token: 0x06000C59 RID: 3161
		SerializationFlags Pack(ref float value, float preValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);

		// Token: 0x06000C5A RID: 3162
		SerializationFlags Unpack(ref float value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);
	}
}
