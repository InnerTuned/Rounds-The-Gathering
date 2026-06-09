using System;
using UnityEngine;

namespace Photon.Compression.Internal
{
	// Token: 0x0200023B RID: 571
	[PackSupportedTypes(typeof(Vector3Int))]
	public interface IPackVector3Int
	{
		// Token: 0x06000C67 RID: 3175
		SerializationFlags Pack(ref Vector3Int value, Vector3Int preValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);

		// Token: 0x06000C68 RID: 3176
		SerializationFlags Unpack(ref Vector3Int value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);
	}
}
