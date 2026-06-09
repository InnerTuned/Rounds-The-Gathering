using System;
using Photon.Compression.HalfFloat;
using Photon.Compression.Internal;

namespace Photon.Compression
{
	// Token: 0x020001FE RID: 510
	public class SyncHalfFloatAttribute : SyncVarBaseAttribute, IPackSingle, IPackDouble
	{
		// Token: 0x060009E8 RID: 2536 RVA: 0x00032B96 File Offset: 0x00030D96
		public SyncHalfFloatAttribute(IndicatorBit indicatorBit = IndicatorBit.None, KeyRate keyRate = KeyRate.UseDefault)
		{
			this.indicatorBit = indicatorBit;
			this.keyRate = keyRate;
		}

		// Token: 0x060009E9 RID: 2537 RVA: 0x00032BAC File Offset: 0x00030DAC
		public override int GetMaxBits(Type fieldType)
		{
			return 16 + ((this.indicatorBit == IndicatorBit.None) ? 0 : 1);
		}

		// Token: 0x060009EA RID: 2538 RVA: 0x00032BC0 File Offset: 0x00030DC0
		public SerializationFlags Pack(ref float value, float prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			ushort num = HalfUtilities.Pack(value);
			if (!base.IsForced<float>(frameId, value, prevValue, writeFlags) && num == HalfUtilities.Pack(prevValue))
			{
				return SerializationFlags.None;
			}
			if (this.indicatorBit == IndicatorBit.IsZero)
			{
				if (value == 0f)
				{
					ArraySerializeExt.Write(buffer, 1UL, ref bitposition, 1);
					return SerializationFlags.IsComplete;
				}
				ArraySerializeExt.Write(buffer, 0UL, ref bitposition, 1);
			}
			ArraySerializeExt.Write(buffer, (ulong)num, ref bitposition, 16);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x060009EB RID: 2539 RVA: 0x00032C28 File Offset: 0x00030E28
		public SerializationFlags Unpack(ref float value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			if (this.indicatorBit == IndicatorBit.IsZero && ArraySerializeExt.Read(buffer, ref bitposition, 1) == 0UL)
			{
				value = 0f;
				return SerializationFlags.None;
			}
			ushort num = (ushort)ArraySerializeExt.Read(buffer, ref bitposition, 16);
			value = HalfUtilities.Unpack(num);
			return SerializationFlags.IsComplete;
		}

		// Token: 0x060009EC RID: 2540 RVA: 0x00032C68 File Offset: 0x00030E68
		public SerializationFlags Pack(ref double value, double prevValue, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			float num = (float)value;
			return this.Pack(ref num, (float)prevValue, buffer, ref bitposition, frameId, writeFlags);
		}

		// Token: 0x060009ED RID: 2541 RVA: 0x00032C8C File Offset: 0x00030E8C
		public SerializationFlags Unpack(ref double value, byte[] buffer, ref int bitposition, int frameId, SerializationFlags writeFlags)
		{
			float num = 0f;
			SerializationFlags result = this.Unpack(ref num, buffer, ref bitposition, frameId, writeFlags);
			value = (double)num;
			return result;
		}

		// Token: 0x04000B90 RID: 2960
		private readonly IndicatorBit indicatorBit;
	}
}
