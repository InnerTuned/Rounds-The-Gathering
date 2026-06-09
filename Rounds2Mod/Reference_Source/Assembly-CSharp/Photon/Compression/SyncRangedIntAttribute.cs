using System;
using Photon.Compression.Internal;
using Photon.Utilities;

namespace Photon.Compression
{
	// Token: 0x020001FF RID: 511
	public class SyncRangedIntAttribute : SyncVarBaseAttribute, IPackByte, IPackSByte, IPackUInt16, IPackInt16, IPackUInt32, IPackInt32, IPackUInt64, IPackInt64, IPackSingle, IPackDouble
	{
		// Token: 0x060009EE RID: 2542 RVA: 0x00032CB4 File Offset: 0x00030EB4
		public SyncRangedIntAttribute(int min, int max, IndicatorBits indicatorBits = IndicatorBits.None, KeyRate keyRate = KeyRate.UseDefault)
		{
			this.min = min;
			this.max = max;
			this.indicatorBits = indicatorBits;
			this.keyRate = keyRate;
			if (min < max)
			{
				this.smallest = min;
				this.biggest = max;
			}
			else
			{
				this.smallest = max;
				this.biggest = min;
			}
			uint maxvalue = (uint)(this.biggest - this.smallest);
			this.bitCount = maxvalue.GetBitsForMaxValue();
		}

		// Token: 0x060009EF RID: 2543 RVA: 0x00032D20 File Offset: 0x00030F20
		public override int GetMaxBits(Type fieldType)
		{
			IndicatorBits indicatorBits = this.indicatorBits;
			if (indicatorBits == IndicatorBits.IsZero)
			{
				return this.bitCount + 1;
			}
			if (indicatorBits != IndicatorBits.IsZeroMidMinMax)
			{
				return this.bitCount;
			}
			return this.bitCount + 2;
		}

		// Token: 0x060009F0 RID: 2544 RVA: 0x00032D58 File Offset: 0x00030F58
		private SerializationFlags Write(int value, int prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			int num = (value > this.biggest) ? this.biggest : ((value < this.smallest) ? this.smallest : value);
			if (!base.IsForced<int>(frameId, num, prevValue, writeFlags))
			{
				return SerializationFlags.None;
			}
			if (this.indicatorBits == IndicatorBits.IsZero)
			{
				if (num == 0)
				{
					ArraySerializeExt.Write(buffer, 0UL, ref bitposition, 1);
					return SerializationFlags.IsComplete;
				}
				ArraySerializeExt.Write(buffer, 1UL, ref bitposition, 1);
			}
			else if (this.indicatorBits == IndicatorBits.IsZeroMidMinMax)
			{
				if (num == 0)
				{
					ArraySerializeExt.Write(buffer, 0UL, ref bitposition, 1);
					return SerializationFlags.IsComplete;
				}
				if (num == this.min)
				{
					ArraySerializeExt.Write(buffer, 1UL, ref bitposition, 1);
					return SerializationFlags.IsComplete;
				}
				if (num == this.max)
				{
					ArraySerializeExt.Write(buffer, 3UL, ref bitposition, 1);
					return SerializationFlags.IsComplete;
				}
				ArraySerializeExt.Write(buffer, 3UL, ref bitposition, 1);
			}
			if (this.bitCount < 16)
			{
				ArraySerializeExt.Write(buffer, (ulong)((long)(num - this.smallest)), ref bitposition, this.bitCount);
			}
			else
			{
				ArrayPackBytesExt.WritePackedBytes(buffer, (ulong)((long)(num - this.smallest)), ref bitposition, this.bitCount);
			}
			return SerializationFlags.IsComplete;
		}

		// Token: 0x060009F1 RID: 2545 RVA: 0x00032E50 File Offset: 0x00031050
		private int Read(byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (this.indicatorBits == IndicatorBits.IsZero)
			{
				if (ArraySerializeExt.Read(buffer, ref bitposition, 1) == 0UL)
				{
					return 0;
				}
			}
			else if (this.indicatorBits == IndicatorBits.IsZeroMidMinMax)
			{
				ulong num = ArraySerializeExt.Read(buffer, ref bitposition, 2);
				ulong num2 = num;
				if (num2 <= 2UL)
				{
					switch ((uint)num2)
					{
					case 0U:
						return 0;
					case 1U:
						return this.min;
					case 2U:
						return this.max;
					}
				}
			}
			if (this.bitCount < 16)
			{
				return (int)ArraySerializeExt.Read(buffer, ref bitposition, this.bitCount) + this.smallest;
			}
			return (int)(ArrayPackBytesExt.ReadPackedBytes(buffer, ref bitposition, this.bitCount) + (ulong)((long)this.smallest));
		}

		// Token: 0x060009F2 RID: 2546 RVA: 0x00032EE7 File Offset: 0x000310E7
		public SerializationFlags Pack(ref byte value, byte prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (!base.IsForced<byte>(frameId, value, prevValue, writeFlags))
			{
				return SerializationFlags.None;
			}
			return this.Write((int)value, (int)prevValue, buffer, ref bitposition, frameId, writeFlags);
		}

		// Token: 0x060009F3 RID: 2547 RVA: 0x00032F0A File Offset: 0x0003110A
		public SerializationFlags Unpack(ref byte value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			value = (byte)this.Read(buffer, ref bitposition, frameId, writeFlags);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x060009F4 RID: 2548 RVA: 0x00032F1D File Offset: 0x0003111D
		public SerializationFlags Pack(ref sbyte value, sbyte prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (!base.IsForced<sbyte>(frameId, value, prevValue, writeFlags))
			{
				return SerializationFlags.None;
			}
			return this.Write((int)value, (int)prevValue, buffer, ref bitposition, frameId, writeFlags);
		}

		// Token: 0x060009F5 RID: 2549 RVA: 0x00032F40 File Offset: 0x00031140
		public SerializationFlags Unpack(ref sbyte value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			value = (sbyte)this.Read(buffer, ref bitposition, frameId, writeFlags);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x060009F6 RID: 2550 RVA: 0x00032F53 File Offset: 0x00031153
		public SerializationFlags Pack(ref ushort value, ushort prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (!base.IsForced<ushort>(frameId, value, prevValue, writeFlags))
			{
				return SerializationFlags.None;
			}
			return this.Write((int)value, (int)prevValue, buffer, ref bitposition, frameId, writeFlags);
		}

		// Token: 0x060009F7 RID: 2551 RVA: 0x00032F76 File Offset: 0x00031176
		public SerializationFlags Unpack(ref ushort value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			value = (ushort)this.Read(buffer, ref bitposition, frameId, writeFlags);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x060009F8 RID: 2552 RVA: 0x00032F89 File Offset: 0x00031189
		public SerializationFlags Pack(ref short value, short prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (!base.IsForced<short>(frameId, value, prevValue, writeFlags))
			{
				return SerializationFlags.None;
			}
			return this.Write((int)value, (int)prevValue, buffer, ref bitposition, frameId, writeFlags);
		}

		// Token: 0x060009F9 RID: 2553 RVA: 0x00032FAC File Offset: 0x000311AC
		public SerializationFlags Unpack(ref short value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			value = (short)this.Read(buffer, ref bitposition, frameId, writeFlags);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x060009FA RID: 2554 RVA: 0x00032FBF File Offset: 0x000311BF
		public SerializationFlags Pack(ref uint value, uint prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (!base.IsForced<uint>(frameId, value, prevValue, writeFlags))
			{
				return SerializationFlags.None;
			}
			return this.Write((int)value, (int)prevValue, buffer, ref bitposition, frameId, writeFlags);
		}

		// Token: 0x060009FB RID: 2555 RVA: 0x00032FE2 File Offset: 0x000311E2
		public SerializationFlags Unpack(ref uint value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			value = (uint)this.Read(buffer, ref bitposition, frameId, writeFlags);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x060009FC RID: 2556 RVA: 0x00032FF4 File Offset: 0x000311F4
		public SerializationFlags Pack(ref int value, int prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (!base.IsForced<int>(frameId, value, prevValue, writeFlags))
			{
				return SerializationFlags.None;
			}
			return this.Write(value, prevValue, buffer, ref bitposition, frameId, writeFlags);
		}

		// Token: 0x060009FD RID: 2557 RVA: 0x00032FE2 File Offset: 0x000311E2
		public SerializationFlags Unpack(ref int value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			value = this.Read(buffer, ref bitposition, frameId, writeFlags);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x060009FE RID: 2558 RVA: 0x00033017 File Offset: 0x00031217
		public SerializationFlags Pack(ref ulong value, ulong prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (!base.IsForced<ulong>(frameId, value, prevValue, writeFlags))
			{
				return SerializationFlags.None;
			}
			return this.Write((int)value, (int)prevValue, buffer, ref bitposition, frameId, writeFlags);
		}

		// Token: 0x060009FF RID: 2559 RVA: 0x0003303C File Offset: 0x0003123C
		public SerializationFlags Unpack(ref ulong value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			value = (ulong)((long)this.Read(buffer, ref bitposition, frameId, writeFlags));
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A00 RID: 2560 RVA: 0x0003304F File Offset: 0x0003124F
		public SerializationFlags Pack(ref long value, long prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (!base.IsForced<long>(frameId, value, prevValue, writeFlags))
			{
				return SerializationFlags.None;
			}
			return this.Write((int)value, (int)prevValue, buffer, ref bitposition, frameId, writeFlags);
		}

		// Token: 0x06000A01 RID: 2561 RVA: 0x0003303C File Offset: 0x0003123C
		public SerializationFlags Unpack(ref long value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			value = (long)this.Read(buffer, ref bitposition, frameId, writeFlags);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A02 RID: 2562 RVA: 0x00033074 File Offset: 0x00031274
		public SerializationFlags Pack(ref float value, float prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			int num = (int)Math.Round((double)value);
			int num2 = (int)Math.Round((double)prevValue);
			if (!base.IsForced(frameId, writeFlags) && num == num2)
			{
				return SerializationFlags.None;
			}
			return this.Write(num, num2, buffer, ref bitposition, frameId, writeFlags);
		}

		// Token: 0x06000A03 RID: 2563 RVA: 0x000330B5 File Offset: 0x000312B5
		public SerializationFlags Unpack(ref float value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			value = (float)this.Read(buffer, ref bitposition, frameId, writeFlags);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x06000A04 RID: 2564 RVA: 0x000330C8 File Offset: 0x000312C8
		public SerializationFlags Pack(ref double value, double prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			int num = (int)Math.Round(value);
			int num2 = (int)Math.Round(prevValue);
			if (!base.IsForced(frameId, writeFlags) && num == num2)
			{
				return SerializationFlags.None;
			}
			return this.Write(num, num2, buffer, ref bitposition, frameId, writeFlags);
		}

		// Token: 0x06000A05 RID: 2565 RVA: 0x00033107 File Offset: 0x00031307
		public SerializationFlags Unpack(ref double value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			value = (double)this.Read(buffer, ref bitposition, frameId, writeFlags);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x04000B91 RID: 2961
		private int min;

		// Token: 0x04000B92 RID: 2962
		private int max;

		// Token: 0x04000B93 RID: 2963
		private readonly int smallest;

		// Token: 0x04000B94 RID: 2964
		private readonly int biggest;

		// Token: 0x04000B95 RID: 2965
		private readonly IndicatorBits indicatorBits;
	}
}
