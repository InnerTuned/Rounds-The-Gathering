using System;
using UnityEngine;

namespace Photon.Compression.Internal
{
	// Token: 0x02000238 RID: 568
	[PackSupportedTypes(typeof(Vector2))]
	public interface IPackVector2
	{
		// Token: 0x06000C61 RID: 3169
		SerializationFlags Pack(ref Vector2 value, Vector2 preValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);

		// Token: 0x06000C62 RID: 3170
		SerializationFlags Unpack(ref Vector2 value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);
	}
}
