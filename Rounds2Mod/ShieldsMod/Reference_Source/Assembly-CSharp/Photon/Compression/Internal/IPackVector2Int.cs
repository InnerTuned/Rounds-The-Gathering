using System;
using UnityEngine;

namespace Photon.Compression.Internal
{
	// Token: 0x0200023A RID: 570
	[PackSupportedTypes(typeof(Vector2Int))]
	public interface IPackVector2Int
	{
		// Token: 0x06000C65 RID: 3173
		SerializationFlags Pack(ref Vector2Int value, Vector2Int preValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);

		// Token: 0x06000C66 RID: 3174
		SerializationFlags Unpack(ref Vector2Int value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);
	}
}
