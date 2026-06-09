using System;

namespace Photon.Compression.Internal
{
	// Token: 0x02000235 RID: 565
	[PackSupportedTypes(typeof(double))]
	public interface IPackDouble
	{
		// Token: 0x06000C5B RID: 3163
		SerializationFlags Pack(ref double value, double preValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);

		// Token: 0x06000C5C RID: 3164
		SerializationFlags Unpack(ref double value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);
	}
}
