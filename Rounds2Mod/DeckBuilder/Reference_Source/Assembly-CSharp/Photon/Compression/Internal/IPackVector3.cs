using System;
using UnityEngine;

namespace Photon.Compression.Internal
{
	// Token: 0x02000239 RID: 569
	[PackSupportedTypes(typeof(Vector3))]
	public interface IPackVector3
	{
		// Token: 0x06000C63 RID: 3171
		SerializationFlags Pack(ref Vector3 value, Vector3 preValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);

		// Token: 0x06000C64 RID: 3172
		SerializationFlags Unpack(ref Vector3 value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags);
	}
}
