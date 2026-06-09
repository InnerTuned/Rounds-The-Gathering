using System;
using Photon.Compression;

namespace Photon.Pun.Simple
{
	// Token: 0x020002D6 RID: 726
	public interface IOnTickPreSerialization
	{
		// Token: 0x06000F84 RID: 3972
		SerializationFlags OnPreSerializeTick(int frameId, byte[] buffer, ref int bitposition);
	}
}
