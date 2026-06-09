using System;
using Photon.Compression.HalfFloat;

namespace Photon.Compression
{
	// Token: 0x020001F9 RID: 505
	public static class NormCompress
	{
		// Token: 0x060009E2 RID: 2530 RVA: 0x00032748 File Offset: 0x00030948
		static NormCompress()
		{
			for (int i = 0; i <= 32; i++)
			{
				uint maxValueForBits = NormCompress.GetMaxValueForBits(i);
				NormCompress.codecForBit[i] = new NormCompress.NormCompressCodec(i, maxValueForBits, 1f / maxValueForBits);
			}
		}

		// Token: 0x060009E3 RID: 2531 RVA: 0x00032794 File Offset: 0x00030994
		public static uint CompressNorm(this float value, int bits)
		{
			value = ((value > 1f) ? 1f : ((value < 0f) ? 0f : value));
			switch (bits)
			{
			case 0:
				return 0U;
			case 1:
				return (uint)value;
			case 2:
				return (uint)(value * 3f);
			case 3:
				return (uint)(value * 7f);
			case 4:
				return (uint)(value * 15f);
			case 5:
				return (uint)(value * 31f);
			case 6:
				return (uint)(value * 63f);
			case 7:
				return (uint)(value * 127f);
			case 8:
				return (uint)(value * 255f);
			case 9:
				return (uint)(value * 511f);
			case 10:
				return (uint)(value * 1023f);
			case 11:
				return (uint)(value * 2047f);
			case 12:
				return (uint)(value * 4095f);
			case 13:
				return (uint)(value * 8191f);
			case 14:
				return (uint)(value * 16383f);
			case 15:
				return (uint)(value * 32767f);
			case 16:
				return (uint)HalfUtilities.Pack(value);
			default:
				return value;
			}
		}

		// Token: 0x060009E4 RID: 2532 RVA: 0x000328A8 File Offset: 0x00030AA8
		public static uint WriteNorm(this byte[] buffer, float value, ref int bitposition, int bits)
		{
			value = ((value > 1f) ? 1f : ((value < 0f) ? 0f : value));
			uint num;
			switch (bits)
			{
			case 0:
				num = 0U;
				break;
			case 1:
				num = (uint)value;
				break;
			case 2:
				num = (uint)(value * 3f);
				break;
			case 3:
				num = (uint)(value * 7f);
				break;
			case 4:
				num = (uint)(value * 15f);
				break;
			case 5:
				num = (uint)(value * 31f);
				break;
			case 6:
				num = (uint)(value * 63f);
				break;
			case 7:
				num = (uint)(value * 127f);
				break;
			case 8:
				num = (uint)(value * 255f);
				break;
			case 9:
				num = (uint)(value * 511f);
				break;
			case 10:
				num = (uint)(value * 1023f);
				break;
			case 11:
				num = (uint)(value * 2047f);
				break;
			case 12:
				num = (uint)(value * 4095f);
				break;
			case 13:
				num = (uint)(value * 8191f);
				break;
			case 14:
				num = (uint)(value * 16383f);
				break;
			case 15:
				num = (uint)(value * 32767f);
				break;
			case 16:
				num = (uint)HalfUtilities.Pack(value);
				break;
			default:
				num = value;
				break;
			}
			ArraySerializeExt.Write(buffer, (ulong)num, ref bitposition, bits);
			return num;
		}

		// Token: 0x060009E5 RID: 2533 RVA: 0x000329FC File Offset: 0x00030BFC
		public static float ReadNorm(this byte[] buffer, ref int bitposition, int bits)
		{
			switch (bits)
			{
			case 0:
				return 0f;
			case 1:
				return ArraySerializeExt.Read(buffer, ref bitposition, 1) * 1f;
			case 2:
				return ArraySerializeExt.Read(buffer, ref bitposition, 2) * 0.33333334f;
			case 3:
				return ArraySerializeExt.Read(buffer, ref bitposition, 3) * 0.14285715f;
			case 4:
				return ArraySerializeExt.Read(buffer, ref bitposition, 4) * 0.06666667f;
			case 5:
				return ArraySerializeExt.Read(buffer, ref bitposition, 5) * 0.032258064f;
			case 6:
				return ArraySerializeExt.Read(buffer, ref bitposition, 6) * 0.015873017f;
			case 7:
				return ArraySerializeExt.Read(buffer, ref bitposition, 7) * 0.003921569f;
			case 8:
				return ArraySerializeExt.Read(buffer, ref bitposition, 8) * 0.003921569f;
			case 9:
				return ArraySerializeExt.Read(buffer, ref bitposition, 9) * 0.0019569471f;
			case 10:
				return ArraySerializeExt.Read(buffer, ref bitposition, 10) * 0.0009775171f;
			case 11:
				return ArraySerializeExt.Read(buffer, ref bitposition, 11) * 0.0004885198f;
			case 12:
				return ArraySerializeExt.Read(buffer, ref bitposition, 12) * 0.00024420026f;
			case 13:
				return ArraySerializeExt.Read(buffer, ref bitposition, 13) * 0.00012208521f;
			case 14:
				return ArraySerializeExt.Read(buffer, ref bitposition, 14) * 6.103888E-05f;
			case 15:
				return ArraySerializeExt.Read(buffer, ref bitposition, 15) * 3.051851E-05f;
			case 16:
				return ArraySerializeHalfExt.ReadHalf(buffer, ref bitposition);
			default:
				return ArraySerializeExt.ReadFloat(buffer, ref bitposition);
			}
		}

		// Token: 0x060009E6 RID: 2534 RVA: 0x00032B73 File Offset: 0x00030D73
		public static uint GetMaxValueForBits(int bitcount)
		{
			return (uint)((1L << bitcount) - 1L);
		}

		// Token: 0x04000B5F RID: 2911
		public static NormCompress.NormCompressCodec[] codecForBit = new NormCompress.NormCompressCodec[33];

		// Token: 0x04000B60 RID: 2912
		private const float NORM_COMP_ENCODE15 = 32767f;

		// Token: 0x04000B61 RID: 2913
		private const float NORM_COMP_DECODE15 = 3.051851E-05f;

		// Token: 0x04000B62 RID: 2914
		private const float NORM_COMP_ENCODE14 = 16383f;

		// Token: 0x04000B63 RID: 2915
		private const float NORM_COMP_DECODE14 = 6.103888E-05f;

		// Token: 0x04000B64 RID: 2916
		private const float NORM_COMP_ENCODE13 = 8191f;

		// Token: 0x04000B65 RID: 2917
		private const float NORM_COMP_DECODE13 = 0.00012208521f;

		// Token: 0x04000B66 RID: 2918
		private const float NORM_COMP_ENCODE12 = 4095f;

		// Token: 0x04000B67 RID: 2919
		private const float NORM_COMP_DECODE12 = 0.00024420026f;

		// Token: 0x04000B68 RID: 2920
		private const float NORM_COMP_ENCODE11 = 2047f;

		// Token: 0x04000B69 RID: 2921
		private const float NORM_COMP_DECODE11 = 0.0004885198f;

		// Token: 0x04000B6A RID: 2922
		private const float NORM_COMP_ENCODE10 = 1023f;

		// Token: 0x04000B6B RID: 2923
		private const float NORM_COMP_DECODE10 = 0.0009775171f;

		// Token: 0x04000B6C RID: 2924
		private const float NORM_COMP_ENCODE9 = 511f;

		// Token: 0x04000B6D RID: 2925
		private const float NORM_COMP_DECODE9 = 0.0019569471f;

		// Token: 0x04000B6E RID: 2926
		private const float NORM_COMP_ENCODE8 = 255f;

		// Token: 0x04000B6F RID: 2927
		private const float NORM_COMP_DECODE8 = 0.003921569f;

		// Token: 0x04000B70 RID: 2928
		private const float NORM_COMP_ENCODE7 = 127f;

		// Token: 0x04000B71 RID: 2929
		private const float NORM_COMP_DECODE7 = 0.003921569f;

		// Token: 0x04000B72 RID: 2930
		private const float NORM_COMP_ENCODE6 = 63f;

		// Token: 0x04000B73 RID: 2931
		private const float NORM_COMP_DECODE6 = 0.015873017f;

		// Token: 0x04000B74 RID: 2932
		private const float NORM_COMP_ENCODE5 = 31f;

		// Token: 0x04000B75 RID: 2933
		private const float NORM_COMP_DECODE5 = 0.032258064f;

		// Token: 0x04000B76 RID: 2934
		private const float NORM_COMP_ENCODE4 = 15f;

		// Token: 0x04000B77 RID: 2935
		private const float NORM_COMP_DECODE4 = 0.06666667f;

		// Token: 0x04000B78 RID: 2936
		private const float NORM_COMP_ENCODE3 = 7f;

		// Token: 0x04000B79 RID: 2937
		private const float NORM_COMP_DECODE3 = 0.14285715f;

		// Token: 0x04000B7A RID: 2938
		private const float NORM_COMP_ENCODE2 = 3f;

		// Token: 0x04000B7B RID: 2939
		private const float NORM_COMP_DECODE2 = 0.33333334f;

		// Token: 0x04000B7C RID: 2940
		private const float NORM_COMP_ENCODE1 = 1f;

		// Token: 0x04000B7D RID: 2941
		private const float NORM_COMP_DECODE1 = 1f;

		// Token: 0x04000B7E RID: 2942
		private const float NORM_COMP_ENCODE0 = 0f;

		// Token: 0x04000B7F RID: 2943
		private const float NORM_COMP_DECODE0 = 0f;

		// Token: 0x020003B3 RID: 947
		public struct NormCompressCodec
		{
			// Token: 0x06001399 RID: 5017 RVA: 0x0005A1DE File Offset: 0x000583DE
			public NormCompressCodec(int bits, float encoder, float decoder)
			{
				this.bits = bits;
				this.encoder = encoder;
				this.decoder = decoder;
			}

			// Token: 0x040012AE RID: 4782
			public readonly int bits;

			// Token: 0x040012AF RID: 4783
			public readonly float encoder;

			// Token: 0x040012B0 RID: 4784
			public readonly float decoder;
		}
	}
}
