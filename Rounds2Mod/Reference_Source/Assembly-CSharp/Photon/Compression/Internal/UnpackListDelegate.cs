using System;
using System.Collections;
using System.Collections.Generic;

namespace Photon.Compression.Internal
{
	// Token: 0x02000227 RID: 551
	// (Invoke) Token: 0x06000C3C RID: 3132
	public delegate SerializationFlags UnpackListDelegate<T>(ref List<T> value, BitArray mask, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags) where T : struct;
}
