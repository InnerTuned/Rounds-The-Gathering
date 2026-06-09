using System;
using System.Collections.Generic;
using Photon.Compression;

namespace Photon.Pun.Simple
{
	// Token: 0x020002F8 RID: 760
	public class NetworkHits
	{
		// Token: 0x06001042 RID: 4162 RVA: 0x0004EB1A File Offset: 0x0004CD1A
		public NetworkHits(bool nearestOnly, int bitsForContactGroupMask)
		{
			this.nearestOnly = nearestOnly;
			this.bitsForContactGroupMask = bitsForContactGroupMask;
		}

		// Token: 0x06001043 RID: 4163 RVA: 0x0004EB42 File Offset: 0x0004CD42
		public void Reset(bool nearestOnly, int bitsForContactGroupMask)
		{
			this.nearestOnly = nearestOnly;
			this.bitsForContactGroupMask = bitsForContactGroupMask;
			this.hits.Clear();
			this.nearestIndex = -1;
		}

		// Token: 0x06001044 RID: 4164 RVA: 0x0004EB64 File Offset: 0x0004CD64
		public void Clear()
		{
			this.hits.Clear();
			this.nearestIndex = -1;
		}

		// Token: 0x06001045 RID: 4165 RVA: 0x0004EB78 File Offset: 0x0004CD78
		public SerializationFlags Serialize(byte[] buffer, ref int bitposition, int bitsForColliderId)
		{
			SerializationFlags result = SerializationFlags.None;
			if (this.nearestOnly)
			{
				if (this.nearestIndex != -1)
				{
					ArraySerializeExt.WriteBool(buffer, true, ref bitposition);
					this.hits[this.nearestIndex].Serialize(buffer, ref bitposition, this.bitsForContactGroupMask, bitsForColliderId);
					result = SerializationFlags.HasContent;
				}
				else
				{
					ArraySerializeExt.WriteBool(buffer, false, ref bitposition);
				}
			}
			else
			{
				int i = 0;
				int count = this.hits.Count;
				while (i < this.hits.Count)
				{
					ArraySerializeExt.WriteBool(buffer, true, ref bitposition);
					this.hits[i].Serialize(buffer, ref bitposition, this.bitsForContactGroupMask, bitsForColliderId);
					result = SerializationFlags.HasContent;
					i++;
				}
				ArraySerializeExt.WriteBool(buffer, false, ref bitposition);
			}
			return result;
		}

		// Token: 0x06001046 RID: 4166 RVA: 0x0004EC24 File Offset: 0x0004CE24
		public SerializationFlags Deserialize(byte[] buffer, ref int bitposition, int bitsForColliderId)
		{
			this.hits.Clear();
			SerializationFlags result = SerializationFlags.None;
			if (this.nearestOnly)
			{
				if (ArraySerializeExt.ReadBool(buffer, ref bitposition))
				{
					this.hits.Add(NetworkHit.Deserialize(buffer, ref bitposition, this.bitsForContactGroupMask, bitsForColliderId));
					result = SerializationFlags.HasContent;
					this.nearestIndex = 0;
				}
				else
				{
					this.nearestIndex = -1;
				}
			}
			else
			{
				while (ArraySerializeExt.ReadBool(buffer, ref bitposition))
				{
					this.hits.Add(NetworkHit.Deserialize(buffer, ref bitposition, this.bitsForContactGroupMask, bitsForColliderId));
					result = SerializationFlags.HasContent;
				}
			}
			return result;
		}

		// Token: 0x06001047 RID: 4167 RVA: 0x0004ECA4 File Offset: 0x0004CEA4
		public override string ToString()
		{
			string text = base.GetType().Name;
			for (int i = 0; i < this.hits.Count; i++)
			{
				text = string.Concat(new object[]
				{
					text,
					"\nObj:",
					this.hits[i].netObjId,
					" Mask:",
					this.hits[i].hitMask
				});
			}
			return text;
		}

		// Token: 0x04000F51 RID: 3921
		public readonly List<NetworkHit> hits = new List<NetworkHit>();

		// Token: 0x04000F52 RID: 3922
		public bool nearestOnly;

		// Token: 0x04000F53 RID: 3923
		public int nearestIndex = -1;

		// Token: 0x04000F54 RID: 3924
		public int bitsForContactGroupMask;
	}
}
