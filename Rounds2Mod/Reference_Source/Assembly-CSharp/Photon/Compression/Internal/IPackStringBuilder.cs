using System;
using System.Text;

namespace Photon.Compression.Internal
{
	// Token: 0x02000237 RID: 567
	[PackSupportedTypes(typeof(StringBuilder))]
	public interface IPackStringBuilder
	{
		// Token: 0x06000C5F RID: 3167
		SerializationFlags Pack(ref StringBuilder value, StringBuilder preValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);

		// Token: 0x06000C60 RID: 3168
		SerializationFlags Unpack(ref StringBuilder value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);
	}
}
