using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Compression.Internal;

namespace Photon.Compression
{
	// Token: 0x02000200 RID: 512
	public class SyncListAttribute : SyncVarBaseAttribute, IPackList<int>
	{
		// Token: 0x06000A06 RID: 2566 RVA: 0x0003311C File Offset: 0x0003131C
		public SerializationFlags Pack(ref List<int> value, List<int> prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			bool flag = base.IsKeyframe(frameId);
			bool flag2 = (writeFlags & (SerializationFlags)22) > SerializationFlags.None;
			SerializationFlags serializationFlags = SerializationFlags.None;
			int num = bitposition;
			int i = 0;
			int count = value.Count;
			while (i < count)
			{
				int num2 = value[i];
				if (flag)
				{
					goto IL_57;
				}
				if (flag2 || num2 != prevValue[i])
				{
					ArraySerializeExt.WriteBool(buffer, true, ref bitposition);
					goto IL_57;
				}
				ArraySerializeExt.WriteBool(buffer, false, ref bitposition);
				IL_6B:
				i++;
				continue;
				IL_57:
				ArrayPackBytesExt.WriteSignedPackedBytes(buffer, num2, ref bitposition, this.bitCount);
				serializationFlags |= SerializationFlags.HasContent;
				goto IL_6B;
			}
			if (serializationFlags == SerializationFlags.None)
			{
				bitposition = num;
			}
			return serializationFlags;
		}

		// Token: 0x06000A07 RID: 2567 RVA: 0x000331A8 File Offset: 0x000313A8
		public SerializationFlags Unpack(ref List<int> value, BitArray isCompleteMask, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			bool flag = base.IsKeyframe(frameId);
			SerializationFlags serializationFlags = SerializationFlags.None;
			SerializationFlags serializationFlags2 = SerializationFlags.IsComplete;
			int i = 0;
			int count = value.Count;
			while (i < count)
			{
				if (!flag && !ArraySerializeExt.ReadBool(buffer, ref bitposition))
				{
					serializationFlags2 = SerializationFlags.None;
					isCompleteMask[i] = false;
				}
				else
				{
					isCompleteMask[i] = true;
					value[i] = ArrayPackBytesExt.ReadSignedPackedBytes(buffer, ref bitposition, this.bitCount);
					serializationFlags |= SerializationFlags.HasContent;
				}
				i++;
			}
			return serializationFlags | serializationFlags2;
		}

		// Token: 0x06000A08 RID: 2568 RVA: 0x00033218 File Offset: 0x00031418
		public SerializationFlags Pack(ref List<uint> value, List<uint> prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			bool flag = base.IsKeyframe(frameId);
			bool flag2 = (writeFlags & (SerializationFlags)22) > SerializationFlags.None;
			SerializationFlags serializationFlags = SerializationFlags.None;
			int num = bitposition;
			int i = 0;
			int count = value.Count;
			while (i < count)
			{
				uint num2 = value[i];
				if (flag)
				{
					goto IL_57;
				}
				if (flag2 || num2 != prevValue[i])
				{
					ArraySerializeExt.WriteBool(buffer, true, ref bitposition);
					goto IL_57;
				}
				ArraySerializeExt.WriteBool(buffer, false, ref bitposition);
				IL_6C:
				i++;
				continue;
				IL_57:
				ArrayPackBytesExt.WritePackedBytes(buffer, (ulong)num2, ref bitposition, this.bitCount);
				serializationFlags |= SerializationFlags.HasContent;
				goto IL_6C;
			}
			if (serializationFlags == SerializationFlags.None)
			{
				bitposition = num;
			}
			return serializationFlags;
		}

		// Token: 0x06000A09 RID: 2569 RVA: 0x000332A8 File Offset: 0x000314A8
		public SerializationFlags Unpack(ref List<uint> value, BitArray isCompleteMask, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			bool flag = base.IsKeyframe(frameId);
			SerializationFlags serializationFlags = SerializationFlags.None;
			SerializationFlags serializationFlags2 = SerializationFlags.IsComplete;
			int i = 0;
			int count = value.Count;
			while (i < count)
			{
				if (!flag && !ArraySerializeExt.ReadBool(buffer, ref bitposition))
				{
					serializationFlags2 = SerializationFlags.None;
					isCompleteMask[i] = false;
				}
				else
				{
					isCompleteMask[i] = true;
					value[i] = (uint)ArrayPackBytesExt.ReadPackedBytes(buffer, ref bitposition, this.bitCount);
					serializationFlags |= SerializationFlags.HasContent;
				}
				i++;
			}
			return serializationFlags | serializationFlags2;
		}

		// Token: 0x06000A0A RID: 2570 RVA: 0x00033318 File Offset: 0x00031518
		public static void Copy<T>(List<T> src, List<T> trg, BitArray mask) where T : struct
		{
			int i = 0;
			int count = src.Count;
			while (i < count)
			{
				if (mask.Get(i))
				{
					trg[i] = src[i];
				}
				i++;
			}
		}

		// Token: 0x06000A0B RID: 2571 RVA: 0x00033350 File Offset: 0x00031550
		public static void Capture<T>(List<T> src, List<T> trg) where T : struct
		{
			int i = 0;
			int count = src.Count;
			while (i < count)
			{
				trg[i] = src[i];
				i++;
			}
		}
	}
}
