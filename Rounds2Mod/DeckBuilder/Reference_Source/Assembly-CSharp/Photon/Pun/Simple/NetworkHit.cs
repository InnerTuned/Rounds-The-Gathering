using System;
using Photon.Compression;

namespace Photon.Pun.Simple
{
	// Token: 0x020002F7 RID: 759
	public struct NetworkHit
	{
		// Token: 0x0600103F RID: 4159 RVA: 0x0004EA9E File Offset: 0x0004CC9E
		public NetworkHit(int objectID, int hitMask, int colliderId)
		{
			this.netObjId = objectID;
			this.hitMask = hitMask;
			this.colliderId = colliderId;
		}

		// Token: 0x06001040 RID: 4160 RVA: 0x0004EAB5 File Offset: 0x0004CCB5
		public void Serialize(byte[] buffer, ref int bitposition, int bitsForHitmask, int bitsForColliderId)
		{
			ArrayPackBytesExt.WritePackedBytes(buffer, (ulong)((long)this.netObjId), ref bitposition, 32);
			ArraySerializeExt.Write(buffer, (ulong)((long)this.hitMask), ref bitposition, bitsForHitmask);
			ArraySerializeExt.Write(buffer, (ulong)((long)this.colliderId), ref bitposition, bitsForColliderId);
		}

		// Token: 0x06001041 RID: 4161 RVA: 0x0004EAE8 File Offset: 0x0004CCE8
		public static NetworkHit Deserialize(byte[] buffer, ref int bitposition, int bitsForHitmask, int bitsForColliderId)
		{
			int objectID = (int)ArrayPackBytesExt.ReadPackedBytes(buffer, ref bitposition, 32);
			int num = (int)ArraySerializeExt.Read(buffer, ref bitposition, bitsForHitmask);
			int num2 = (int)ArraySerializeExt.Read(buffer, ref bitposition, bitsForColliderId);
			return new NetworkHit(objectID, num, num2);
		}

		// Token: 0x04000F4E RID: 3918
		public readonly int netObjId;

		// Token: 0x04000F4F RID: 3919
		public readonly int hitMask;

		// Token: 0x04000F50 RID: 3920
		public readonly int colliderId;
	}
}
