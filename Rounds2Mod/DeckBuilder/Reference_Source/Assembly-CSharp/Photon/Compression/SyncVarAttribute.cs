using System;
using System.Collections.Generic;
using System.Text;
using Photon.Compression.Internal;
using UnityEngine;

namespace Photon.Compression
{
	// Token: 0x02000201 RID: 513
	public class SyncVarAttribute : SyncVarBaseAttribute, IPackByte, IPackSByte, IPackUInt16, IPackInt16, IPackUInt32, IPackInt32, IPackUInt64, IPackInt64, IPackSingle, IPackDouble, IPackString, IPackStringBuilder, IPackVector2, IPackVector3, IPackVector2Int, IPackVector3Int, IPackBoolean, IPackChar
	{
		// Token: 0x06000A0D RID: 2573 RVA: 0x00033386 File Offset: 0x00031586
		public SyncVarAttribute(KeyRate keyRate = KeyRate.UseDefault)
		{
			this.keyRate = keyRate;
		}

		// Token: 0x06000A0E RID: 2574 RVA: 0x00033398 File Offset: 0x00031598
		public override int GetMaxBits(Type fieldType)
		{
			if (fieldType == typeof(string) || fieldType == typeof(StringBuilder))
			{
				return 1030;
			}
			if (fieldType == typeof(Vector2))
			{
				return 64;
			}
			if (fieldType == typeof(Vector2))
			{
				return 96;
			}
			return base.GetMaxBits(fieldType);
		}

		// Token: 0x06000A0F RID: 2575 RVA: 0x00033400 File Offset: 0x00031600
		public bool Compare<T>(List<T> a, List<T> b) where T : struct
		{
			int count = a.Count;
			if (count != b.Count)
			{
				return false;
			}
			for (int i = 0; i < count; i++)
			{
				T t = a[i];
				if (!t.Equals(b[i]))
				{
					return false;
				}
			}
			return false;
		}

		// Token: 0x06000A10 RID: 2576 RVA: 0x00033451 File Offset: 0x00031651
		public SerializationFlags Pack(ref bool value, bool prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (!base.IsForced<bool>(frameId, value, prevValue, writeFlags))
			{
				return SerializationFlags.None;
			}
			ArraySerializeExt.Write(buffer, value ? 1UL : 0UL, ref bitposition, 1);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A11 RID: 2577 RVA: 0x00033479 File Offset: 0x00031679
		public SerializationFlags Unpack(ref bool value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			value = (ArraySerializeExt.Read(buffer, ref bitposition, 1) != 0UL);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A12 RID: 2578 RVA: 0x0003348D File Offset: 0x0003168D
		public SerializationFlags Pack(ref byte value, byte prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (!base.IsForced<byte>(frameId, value, prevValue, writeFlags))
			{
				return SerializationFlags.None;
			}
			ArraySerializeExt.Write(buffer, (ulong)value, ref bitposition, this.bitCount);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A13 RID: 2579 RVA: 0x000334B3 File Offset: 0x000316B3
		public SerializationFlags Unpack(ref byte value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			value = (byte)ArraySerializeExt.Read(buffer, ref bitposition, this.bitCount);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A14 RID: 2580 RVA: 0x000334C7 File Offset: 0x000316C7
		public SerializationFlags Pack(ref sbyte value, sbyte prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (!base.IsForced<sbyte>(frameId, value, prevValue, writeFlags))
			{
				return SerializationFlags.None;
			}
			ArraySerializeExt.WriteSigned(buffer, (int)value, ref bitposition, this.bitCount);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A15 RID: 2581 RVA: 0x000334EC File Offset: 0x000316EC
		public SerializationFlags Unpack(ref sbyte value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			value = (sbyte)ArraySerializeExt.ReadSigned(buffer, ref bitposition, this.bitCount);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A16 RID: 2582 RVA: 0x00033500 File Offset: 0x00031700
		public SerializationFlags Pack(ref ushort value, ushort prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (!base.IsForced<ushort>(frameId, value, prevValue, writeFlags))
			{
				return SerializationFlags.None;
			}
			ArraySerializeExt.Write(buffer, (ulong)value, ref bitposition, this.bitCount);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A17 RID: 2583 RVA: 0x00033526 File Offset: 0x00031726
		public SerializationFlags Unpack(ref ushort value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			value = (ushort)ArraySerializeExt.Read(buffer, ref bitposition, this.bitCount);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A18 RID: 2584 RVA: 0x0003353A File Offset: 0x0003173A
		public SerializationFlags Pack(ref short value, short prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			global::Debug.Log("Pack " + this.bitCount);
			if (!base.IsForced<short>(frameId, value, prevValue, writeFlags))
			{
				return SerializationFlags.None;
			}
			ArraySerializeExt.WriteSigned(buffer, (int)value, ref bitposition, this.bitCount);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A19 RID: 2585 RVA: 0x00033579 File Offset: 0x00031779
		public SerializationFlags Unpack(ref short value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			value = (short)ArraySerializeExt.ReadSigned(buffer, ref bitposition, this.bitCount);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A1A RID: 2586 RVA: 0x0003358D File Offset: 0x0003178D
		public SerializationFlags Pack(ref char value, char prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (!base.IsForced<char>(frameId, value, prevValue, writeFlags))
			{
				return SerializationFlags.None;
			}
			ArraySerializeExt.Write(buffer, (ulong)value, ref bitposition, this.bitCount);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A1B RID: 2587 RVA: 0x00033526 File Offset: 0x00031726
		public SerializationFlags Unpack(ref char value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			value = (char)ArraySerializeExt.Read(buffer, ref bitposition, this.bitCount);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A1C RID: 2588 RVA: 0x000335B3 File Offset: 0x000317B3
		public SerializationFlags Pack(ref uint value, uint prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (!base.IsForced<uint>(frameId, value, prevValue, writeFlags))
			{
				return SerializationFlags.None;
			}
			ArrayPackBytesExt.WritePackedBytes(buffer, (ulong)value, ref bitposition, this.bitCount);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A1D RID: 2589 RVA: 0x000335D9 File Offset: 0x000317D9
		public SerializationFlags Unpack(ref uint value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			value = (uint)ArrayPackBytesExt.ReadPackedBytes(buffer, ref bitposition, this.bitCount);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A1E RID: 2590 RVA: 0x000335ED File Offset: 0x000317ED
		public SerializationFlags Pack(ref int value, int prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (!base.IsForced<int>(frameId, value, prevValue, writeFlags))
			{
				return SerializationFlags.None;
			}
			ArrayPackBytesExt.WriteSignedPackedBytes(buffer, value, ref bitposition, this.bitCount);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A1F RID: 2591 RVA: 0x00033612 File Offset: 0x00031812
		public SerializationFlags Unpack(ref int value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			value = ArrayPackBytesExt.ReadSignedPackedBytes(buffer, ref bitposition, this.bitCount);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A20 RID: 2592 RVA: 0x00033625 File Offset: 0x00031825
		public SerializationFlags Pack(ref ulong value, ulong prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (!base.IsForced<ulong>(frameId, value, prevValue, writeFlags))
			{
				return SerializationFlags.None;
			}
			ArrayPackBytesExt.WritePackedBytes(buffer, value, ref bitposition, this.bitCount);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A21 RID: 2593 RVA: 0x0003364A File Offset: 0x0003184A
		public SerializationFlags Unpack(ref ulong value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			value = ArrayPackBytesExt.ReadPackedBytes(buffer, ref bitposition, this.bitCount);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A22 RID: 2594 RVA: 0x0003365D File Offset: 0x0003185D
		public SerializationFlags Pack(ref long value, long prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (!base.IsForced<long>(frameId, value, prevValue, writeFlags))
			{
				return SerializationFlags.None;
			}
			ArrayPackBytesExt.WriteSignedPackedBytes64(buffer, value, ref bitposition, this.bitCount);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A23 RID: 2595 RVA: 0x00033682 File Offset: 0x00031882
		public SerializationFlags Unpack(ref long value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			value = ArrayPackBytesExt.ReadSignedPackedBytes64(buffer, ref bitposition, this.bitCount);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A24 RID: 2596 RVA: 0x00033698 File Offset: 0x00031898
		public SerializationFlags Pack(ref float value, float prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (this.WholeNumbers)
			{
				int num = (int)Math.Round((double)value);
				int prevValue2 = (int)Math.Round((double)prevValue);
				if (!base.IsForced<int>(frameId, num, prevValue2, writeFlags))
				{
					return SerializationFlags.None;
				}
				ArrayPackBytesExt.WriteSignedPackedBytes(buffer, num, ref bitposition, 32);
				return SerializationFlags.IsComplete;
			}
			else
			{
				if (!base.IsForced<float>(frameId, value, prevValue, writeFlags))
				{
					return SerializationFlags.None;
				}
				ArraySerializeExt.Write(buffer, (ulong)value, ref bitposition, 32);
				return SerializationFlags.IsComplete;
			}
		}

		// Token: 0x06000A25 RID: 2597 RVA: 0x00033708 File Offset: 0x00031908
		public SerializationFlags Unpack(ref float value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (this.WholeNumbers)
			{
				value = (float)ArrayPackBytesExt.ReadSignedPackedBytes(buffer, ref bitposition, 32);
				return SerializationFlags.IsComplete;
			}
			value = ArraySerializeExt.Read(buffer, ref bitposition, 32);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A26 RID: 2598 RVA: 0x00033738 File Offset: 0x00031938
		public SerializationFlags Pack(ref double value, double prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (this.WholeNumbers)
			{
				long num = (long)((int)Math.Round(value));
				long prevValue2 = (long)((int)Math.Round(prevValue));
				if (!base.IsForced<long>(frameId, num, prevValue2, writeFlags))
				{
					return SerializationFlags.None;
				}
				ArrayPackBytesExt.WriteSignedPackedBytes64(buffer, num, ref bitposition, 64);
				return SerializationFlags.IsComplete;
			}
			else
			{
				if (!base.IsForced<double>(frameId, value, prevValue, writeFlags))
				{
					return SerializationFlags.None;
				}
				ArraySerializeExt.Write(buffer, value, ref bitposition, 64);
				return SerializationFlags.IsComplete;
			}
		}

		// Token: 0x06000A27 RID: 2599 RVA: 0x000337A7 File Offset: 0x000319A7
		public SerializationFlags Unpack(ref double value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (this.WholeNumbers)
			{
				value = (double)ArrayPackBytesExt.ReadSignedPackedBytes64(buffer, ref bitposition, 64);
				return SerializationFlags.IsComplete;
			}
			value = ArraySerializeExt.Read(buffer, ref bitposition, 64);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A28 RID: 2600 RVA: 0x000337D8 File Offset: 0x000319D8
		public SerializationFlags Pack(ref string value, string prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (!base.IsForcedClass<string>(frameId, value, prevValue, writeFlags))
			{
				return SerializationFlags.None;
			}
			if (value == null)
			{
				ArraySerializeExt.Write(buffer, 0UL, ref bitposition, 6);
				return SerializationFlags.IsComplete;
			}
			int num = value.Length;
			if (num > 63)
			{
				num = 63;
			}
			ArraySerializeExt.Write(buffer, (ulong)num, ref bitposition, 6);
			for (int i = 0; i < num; i++)
			{
				ArraySerializeExt.Write(buffer, (ulong)value.get_Chars(i), ref bitposition, this.bitCount);
			}
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A29 RID: 2601 RVA: 0x00033848 File Offset: 0x00031A48
		public SerializationFlags Unpack(ref string value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			SyncVarAttribute.sb.Length = 0;
			int num = (int)ArraySerializeExt.Read(buffer, ref bitposition, 6);
			for (int i = 0; i < num; i++)
			{
				SyncVarAttribute.sb.Append((char)ArraySerializeExt.Read(buffer, ref bitposition, this.bitCount));
			}
			value = SyncVarAttribute.sb.ToString();
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A2A RID: 2602 RVA: 0x000338A0 File Offset: 0x00031AA0
		public SerializationFlags Pack(ref StringBuilder value, StringBuilder prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (!base.IsForcedClass<StringBuilder>(frameId, value, prevValue, writeFlags))
			{
				return SerializationFlags.None;
			}
			if (value == null)
			{
				ArraySerializeExt.Write(buffer, 0UL, ref bitposition, 6);
				return SerializationFlags.IsComplete;
			}
			int num = value.Length;
			if (num > 63)
			{
				num = 63;
			}
			ArraySerializeExt.Write(buffer, (ulong)num, ref bitposition, 6);
			for (int i = 0; i < num; i++)
			{
				ArraySerializeExt.Write(buffer, (ulong)value.get_Chars(i), ref bitposition, this.bitCount);
			}
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A2B RID: 2603 RVA: 0x00033910 File Offset: 0x00031B10
		public SerializationFlags Unpack(ref StringBuilder value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (value == null)
			{
				value = new StringBuilder(64);
			}
			else
			{
				value.Length = 0;
			}
			int num = (int)ArraySerializeExt.Read(buffer, ref bitposition, 6);
			for (int i = 0; i < num; i++)
			{
				value.Append((char)ArraySerializeExt.Read(buffer, ref bitposition, this.bitCount));
			}
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A2C RID: 2604 RVA: 0x00033964 File Offset: 0x00031B64
		public SerializationFlags Pack(ref Vector2 value, Vector2 prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (this.WholeNumbers)
			{
				Vector2Int value2 = new Vector2Int((int)value.x, (int)value.y);
				Vector2Int prevValue2 = new Vector2Int((int)prevValue.x, (int)prevValue.y);
				if (!base.IsForced<Vector2Int>(frameId, value2, prevValue2, writeFlags))
				{
					return SerializationFlags.None;
				}
				ArrayPackBytesExt.WriteSignedPackedBytes(buffer, value2.x, ref bitposition, 32);
				ArrayPackBytesExt.WriteSignedPackedBytes(buffer, value2.y, ref bitposition, 32);
				return SerializationFlags.IsComplete;
			}
			else
			{
				if (!base.IsForced<Vector2>(frameId, value, prevValue, writeFlags))
				{
					return SerializationFlags.None;
				}
				ArraySerializeExt.Write(buffer, (ulong)value.x, ref bitposition, 32);
				ArraySerializeExt.Write(buffer, (ulong)value.y, ref bitposition, 32);
				return SerializationFlags.IsComplete;
			}
		}

		// Token: 0x06000A2D RID: 2605 RVA: 0x00033A28 File Offset: 0x00031C28
		public SerializationFlags Unpack(ref Vector2 value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (this.WholeNumbers)
			{
				value = new Vector2((float)ArrayPackBytesExt.ReadSignedPackedBytes(buffer, ref bitposition, 32), (float)ArrayPackBytesExt.ReadSignedPackedBytes(buffer, ref bitposition, 32));
				return SerializationFlags.IsComplete;
			}
			value = new Vector2(ArraySerializeExt.Read(buffer, ref bitposition, 32), ArraySerializeExt.Read(buffer, ref bitposition, 32));
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A2E RID: 2606 RVA: 0x00033A94 File Offset: 0x00031C94
		public SerializationFlags Pack(ref Vector3 value, Vector3 prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (this.WholeNumbers)
			{
				Vector3Int value2 = new Vector3Int((int)value.x, (int)value.y, (int)value.z);
				Vector3Int prevValue2 = new Vector3Int((int)prevValue.x, (int)prevValue.y, (int)prevValue.z);
				if (!base.IsForced<Vector3Int>(frameId, value2, prevValue2, writeFlags))
				{
					return SerializationFlags.None;
				}
				ArrayPackBytesExt.WriteSignedPackedBytes(buffer, value2.x, ref bitposition, 32);
				ArrayPackBytesExt.WriteSignedPackedBytes(buffer, value2.y, ref bitposition, 32);
				ArrayPackBytesExt.WriteSignedPackedBytes(buffer, value2.z, ref bitposition, 32);
				return SerializationFlags.IsComplete;
			}
			else
			{
				if (!base.IsForced<Vector3>(frameId, value, prevValue, writeFlags))
				{
					return SerializationFlags.None;
				}
				ArraySerializeExt.Write(buffer, (ulong)value.x, ref bitposition, 32);
				ArraySerializeExt.Write(buffer, (ulong)value.y, ref bitposition, 32);
				ArraySerializeExt.Write(buffer, (ulong)value.z, ref bitposition, 32);
				return SerializationFlags.IsComplete;
			}
		}

		// Token: 0x06000A2F RID: 2607 RVA: 0x00033B90 File Offset: 0x00031D90
		public SerializationFlags Unpack(ref Vector3 value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (this.WholeNumbers)
			{
				value = new Vector3((float)ArrayPackBytesExt.ReadSignedPackedBytes(buffer, ref bitposition, 32), (float)ArrayPackBytesExt.ReadSignedPackedBytes(buffer, ref bitposition, 32), (float)ArrayPackBytesExt.ReadSignedPackedBytes(buffer, ref bitposition, 32));
				return SerializationFlags.IsComplete;
			}
			value = new Vector3(ArraySerializeExt.Read(buffer, ref bitposition, 32), ArraySerializeExt.Read(buffer, ref bitposition, 32), ArraySerializeExt.Read(buffer, ref bitposition, 32));
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A30 RID: 2608 RVA: 0x00033C17 File Offset: 0x00031E17
		public SerializationFlags Pack(ref Vector2Int value, Vector2Int prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (!base.IsForced<Vector2Int>(frameId, value, prevValue, writeFlags))
			{
				return SerializationFlags.None;
			}
			ArrayPackBytesExt.WriteSignedPackedBytes(buffer, value.x, ref bitposition, 32);
			ArrayPackBytesExt.WriteSignedPackedBytes(buffer, value.y, ref bitposition, 32);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A31 RID: 2609 RVA: 0x00033C50 File Offset: 0x00031E50
		public SerializationFlags Unpack(ref Vector2Int value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			value = new Vector2Int(ArrayPackBytesExt.ReadSignedPackedBytes(buffer, ref bitposition, 32), ArrayPackBytesExt.ReadSignedPackedBytes(buffer, ref bitposition, 32));
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A32 RID: 2610 RVA: 0x00033C74 File Offset: 0x00031E74
		public SerializationFlags Pack(ref Vector3Int value, Vector3Int prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (!base.IsForced<Vector3Int>(frameId, value, prevValue, writeFlags))
			{
				return SerializationFlags.None;
			}
			ArrayPackBytesExt.WriteSignedPackedBytes(buffer, value.x, ref bitposition, 32);
			ArrayPackBytesExt.WriteSignedPackedBytes(buffer, value.y, ref bitposition, 32);
			ArrayPackBytesExt.WriteSignedPackedBytes(buffer, value.z, ref bitposition, 32);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A33 RID: 2611 RVA: 0x00033CC8 File Offset: 0x00031EC8
		public SerializationFlags Unpack(ref Vector3Int value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			value = new Vector3Int(ArrayPackBytesExt.ReadSignedPackedBytes(buffer, ref bitposition, 32), ArrayPackBytesExt.ReadSignedPackedBytes(buffer, ref bitposition, 32), ArrayPackBytesExt.ReadSignedPackedBytes(buffer, ref bitposition, 32));
			return SerializationFlags.IsComplete;
		}

		// Token: 0x04000B96 RID: 2966
		public const int MAX_STR_LEN = 63;

		// Token: 0x04000B97 RID: 2967
		public const int STR_LEN_BITS = 6;

		// Token: 0x04000B98 RID: 2968
		public bool WholeNumbers;

		// Token: 0x04000B99 RID: 2969
		private static StringBuilder sb = new StringBuilder(0);
	}
}
