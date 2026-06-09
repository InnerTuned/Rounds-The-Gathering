using System;
using UnityEngine;

namespace Photon.Compression
{
	// Token: 0x020001F7 RID: 503
	[Serializable]
	public class LiteIntCrusher : LiteCrusher<uint, int>
	{
		// Token: 0x060009D8 RID: 2520 RVA: 0x0003244C File Offset: 0x0003064C
		public LiteIntCrusher()
		{
			this.compressType = LiteIntCompressType.PackSigned;
			this.min = -128;
			this.max = 127;
			if (this.compressType == LiteIntCompressType.Range)
			{
				LiteIntCrusher.Recalculate(this.min, this.max, ref this.smallest, ref this.biggest, ref this.bits);
			}
		}

		// Token: 0x060009D9 RID: 2521 RVA: 0x000324A2 File Offset: 0x000306A2
		public LiteIntCrusher(LiteIntCompressType compType = LiteIntCompressType.PackSigned, int min = -128, int max = 127)
		{
			this.compressType = compType;
			this.min = min;
			this.max = max;
			if (this.compressType == LiteIntCompressType.Range)
			{
				LiteIntCrusher.Recalculate(min, max, ref this.smallest, ref this.biggest, ref this.bits);
			}
		}

		// Token: 0x060009DA RID: 2522 RVA: 0x000324E4 File Offset: 0x000306E4
		public override uint WriteValue(int val, byte[] buffer, ref int bitposition)
		{
			switch (this.compressType)
			{
			case LiteIntCompressType.PackSigned:
			{
				uint num = (uint)(val << 1 ^ val >> 31);
				ArrayPackBytesExt.WritePackedBytes(buffer, (ulong)num, ref bitposition, 32);
				return num;
			}
			case LiteIntCompressType.PackUnsigned:
				ArrayPackBytesExt.WritePackedBytes(buffer, (ulong)val, ref bitposition, 32);
				return (uint)val;
			case LiteIntCompressType.Range:
			{
				uint num2 = this.Encode(val);
				ArraySerializeExt.Write(buffer, (ulong)num2, ref bitposition, this.bits);
				return num2;
			}
			default:
				return 0U;
			}
		}

		// Token: 0x060009DB RID: 2523 RVA: 0x0003254C File Offset: 0x0003074C
		public override void WriteCValue(uint cval, byte[] buffer, ref int bitposition)
		{
			switch (this.compressType)
			{
			case LiteIntCompressType.PackSigned:
				ArrayPackBytesExt.WritePackedBytes(buffer, (ulong)cval, ref bitposition, 32);
				return;
			case LiteIntCompressType.PackUnsigned:
				ArrayPackBytesExt.WritePackedBytes(buffer, (ulong)cval, ref bitposition, 32);
				return;
			case LiteIntCompressType.Range:
				ArraySerializeExt.Write(buffer, (ulong)cval, ref bitposition, this.bits);
				return;
			default:
				return;
			}
		}

		// Token: 0x060009DC RID: 2524 RVA: 0x0003259C File Offset: 0x0003079C
		public override int ReadValue(byte[] buffer, ref int bitposition)
		{
			switch (this.compressType)
			{
			case LiteIntCompressType.PackSigned:
				return ArrayPackBytesExt.ReadSignedPackedBytes(buffer, ref bitposition, 32);
			case LiteIntCompressType.PackUnsigned:
				return (int)ArrayPackBytesExt.ReadPackedBytes(buffer, ref bitposition, 32);
			case LiteIntCompressType.Range:
			{
				uint val = (uint)ArraySerializeExt.Read(buffer, ref bitposition, this.bits);
				return this.Decode(val);
			}
			default:
				return 0;
			}
		}

		// Token: 0x060009DD RID: 2525 RVA: 0x000325F4 File Offset: 0x000307F4
		public override uint ReadCValue(byte[] buffer, ref int bitposition)
		{
			switch (this.compressType)
			{
			case LiteIntCompressType.PackSigned:
				return (uint)ArrayPackBytesExt.ReadPackedBytes(buffer, ref bitposition, 32);
			case LiteIntCompressType.PackUnsigned:
				return (uint)ArrayPackBytesExt.ReadPackedBytes(buffer, ref bitposition, 32);
			case LiteIntCompressType.Range:
				return (uint)ArraySerializeExt.Read(buffer, ref bitposition, this.bits);
			default:
				return 0U;
			}
		}

		// Token: 0x060009DE RID: 2526 RVA: 0x00032644 File Offset: 0x00030844
		public override uint Encode(int value)
		{
			LiteIntCompressType liteIntCompressType = this.compressType;
			if (liteIntCompressType == LiteIntCompressType.PackSigned)
			{
				return ZigZagExt.ZigZag(value);
			}
			if (liteIntCompressType != LiteIntCompressType.PackUnsigned)
			{
				value = ((value > this.biggest) ? this.biggest : ((value < this.smallest) ? this.smallest : value));
				return (uint)(value - this.smallest);
			}
			return (uint)value;
		}

		// Token: 0x060009DF RID: 2527 RVA: 0x00032698 File Offset: 0x00030898
		public override int Decode(uint cvalue)
		{
			LiteIntCompressType liteIntCompressType = this.compressType;
			if (liteIntCompressType == LiteIntCompressType.PackSigned)
			{
				return ZigZagExt.UnZigZag(cvalue);
			}
			if (liteIntCompressType != LiteIntCompressType.PackUnsigned)
			{
				return (int)((ulong)cvalue + (ulong)((long)this.smallest));
			}
			return (int)cvalue;
		}

		// Token: 0x060009E0 RID: 2528 RVA: 0x000326CC File Offset: 0x000308CC
		public static void Recalculate(int min, int max, LiteIntCrusher crusher)
		{
			if (min < max)
			{
				crusher.smallest = min;
				crusher.biggest = max;
			}
			else
			{
				crusher.smallest = max;
				crusher.biggest = min;
			}
			int maxvalue = crusher.biggest - crusher.smallest;
			crusher.bits = LiteCrusher.GetBitsForMaxValue((uint)maxvalue);
		}

		// Token: 0x060009E1 RID: 2529 RVA: 0x00032718 File Offset: 0x00030918
		public static void Recalculate(int min, int max, ref int smallest, ref int biggest, ref int bits)
		{
			if (min < max)
			{
				smallest = min;
				biggest = max;
			}
			else
			{
				smallest = max;
				biggest = min;
			}
			int maxvalue = biggest - smallest;
			bits = LiteCrusher.GetBitsForMaxValue((uint)maxvalue);
		}

		// Token: 0x04000B4C RID: 2892
		[SerializeField]
		public LiteIntCompressType compressType;

		// Token: 0x04000B4D RID: 2893
		[SerializeField]
		protected int min;

		// Token: 0x04000B4E RID: 2894
		[SerializeField]
		protected int max;

		// Token: 0x04000B4F RID: 2895
		[SerializeField]
		private int smallest;

		// Token: 0x04000B50 RID: 2896
		[SerializeField]
		private int biggest;
	}
}
