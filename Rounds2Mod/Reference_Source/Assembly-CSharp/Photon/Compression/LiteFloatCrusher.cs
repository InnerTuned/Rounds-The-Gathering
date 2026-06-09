using System;
using Photon.Compression.HalfFloat;
using UnityEngine;

namespace Photon.Compression
{
	// Token: 0x020001F5 RID: 501
	[Serializable]
	public class LiteFloatCrusher : LiteCrusher<ulong, float>
	{
		// Token: 0x060009CD RID: 2509 RVA: 0x00031F44 File Offset: 0x00030144
		public LiteFloatCrusher()
		{
			this.compressType = LiteFloatCompressType.Half16;
			this.normalization = LiteFloatCrusher.Normalization.Positive;
			this.min = 0f;
			this.max = 1f;
			this.accurateCenter = true;
			LiteFloatCrusher.Recalculate(this.compressType, this.min, this.max, this.accurateCenter, this);
		}

		// Token: 0x060009CE RID: 2510 RVA: 0x00031FB0 File Offset: 0x000301B0
		public LiteFloatCrusher(LiteFloatCompressType compressType, float min, float max, bool accurateCenter, LiteOutOfBoundsHandling outOfBoundsHandling = LiteOutOfBoundsHandling.Clamp)
		{
			this.compressType = compressType;
			this.normalization = LiteFloatCrusher.Normalization.None;
			if (min == max)
			{
				max += 1f;
				global::Debug.LogWarning(string.Concat(new object[]
				{
					"Float crusher is being given min and max values that are the same. This likely is not intentional. Check your range values. Value is <i>",
					min,
					"</i>, changing the max to ",
					max,
					" to avoid division by zero errors."
				}));
			}
			if (min < max)
			{
				this.min = min;
				this.max = max;
			}
			else
			{
				this.min = max;
				this.max = min;
			}
			this.accurateCenter = accurateCenter;
			this.outOfBoundsHandling = outOfBoundsHandling;
			LiteFloatCrusher.Recalculate(compressType, min, max, accurateCenter, this);
		}

		// Token: 0x060009CF RID: 2511 RVA: 0x00032064 File Offset: 0x00030264
		public LiteFloatCrusher(LiteFloatCompressType compressType, LiteFloatCrusher.Normalization normalization = LiteFloatCrusher.Normalization.None, LiteOutOfBoundsHandling outOfBoundsHandling = LiteOutOfBoundsHandling.Clamp)
		{
			this.compressType = compressType;
			this.normalization = normalization;
			switch (normalization)
			{
			case LiteFloatCrusher.Normalization.None:
				this.min = 0f;
				this.max = 1f;
				this.accurateCenter = false;
				break;
			case LiteFloatCrusher.Normalization.Positive:
				this.min = 0f;
				this.max = 1f;
				this.accurateCenter = false;
				break;
			case LiteFloatCrusher.Normalization.Negative:
				this.min = -1f;
				this.max = 1f;
				this.accurateCenter = true;
				break;
			}
			this.outOfBoundsHandling = outOfBoundsHandling;
			LiteFloatCrusher.Recalculate(compressType, this.min, this.max, this.accurateCenter, this);
		}

		// Token: 0x060009D0 RID: 2512 RVA: 0x00032124 File Offset: 0x00030324
		public static void Recalculate(LiteFloatCompressType compressType, float min, float max, bool accurateCenter, LiteFloatCrusher crusher)
		{
			crusher.bits = (int)compressType;
			float num = max - min;
			ulong num2 = (compressType == (LiteFloatCompressType)64) ? ulong.MaxValue : ((1UL << (int)compressType) - 1UL);
			if (accurateCenter && num2 != 0UL)
			{
				num2 -= 1UL;
			}
			crusher.encoder = ((num == 0f) ? 0f : (num2 / num));
			crusher.decoder = ((num2 == 0UL) ? 0f : (num / num2));
			crusher.maxCVal = num2;
		}

		// Token: 0x060009D1 RID: 2513 RVA: 0x00032198 File Offset: 0x00030398
		public static void Recalculate(LiteFloatCompressType compressType, float min, float max, bool accurateCenter, ref int bits, ref float encoder, ref float decoder, ref ulong maxCVal)
		{
			bits = (int)compressType;
			float num = max - min;
			ulong num2 = (bits == 64) ? ulong.MaxValue : ((1UL << bits) - 1UL);
			if (accurateCenter && num2 != 0UL)
			{
				num2 -= 1UL;
			}
			encoder = ((num == 0f) ? 0f : (num2 / num));
			decoder = ((num2 == 0UL) ? 0f : (num / num2));
			maxCVal = num2;
		}

		// Token: 0x060009D2 RID: 2514 RVA: 0x00032200 File Offset: 0x00030400
		public override ulong Encode(float val)
		{
			if (this.compressType == LiteFloatCompressType.Half16)
			{
				return (ulong)HalfUtilities.Pack(val);
			}
			if (this.compressType == LiteFloatCompressType.Full32)
			{
				return (ulong)val.uint32;
			}
			float num = (val - this.min) * this.encoder + 0.5f;
			if (num < 0f)
			{
				if (this.outOfBoundsHandling == LiteOutOfBoundsHandling.Clamp)
				{
					return 0UL;
				}
				return this.maxCVal + (ulong)((long)num % 10L);
			}
			else if (num > this.maxCVal)
			{
				if (this.outOfBoundsHandling == LiteOutOfBoundsHandling.Clamp)
				{
					return this.maxCVal;
				}
				return (ulong)(num % this.maxCVal);
			}
			else
			{
				ulong num2 = (ulong)num;
				if (num2 <= this.maxCVal)
				{
					return num2;
				}
				return this.maxCVal;
			}
		}

		// Token: 0x060009D3 RID: 2515 RVA: 0x000322A8 File Offset: 0x000304A8
		public override float Decode(uint cval)
		{
			if (this.compressType == LiteFloatCompressType.Half16)
			{
				return HalfUtilities.Unpack((ushort)cval);
			}
			if (this.compressType == LiteFloatCompressType.Full32)
			{
				return cval.float32;
			}
			if (cval == 0U)
			{
				return this.min;
			}
			if ((ulong)cval == this.maxCVal)
			{
				return this.max;
			}
			return cval * this.decoder + this.min;
		}

		// Token: 0x060009D4 RID: 2516 RVA: 0x0003230C File Offset: 0x0003050C
		public override ulong WriteValue(float val, byte[] buffer, ref int bitposition)
		{
			if (this.compressType == LiteFloatCompressType.Half16)
			{
				ulong num = (ulong)HalfUtilities.Pack(val);
				ArraySerializeExt.Write(buffer, num, ref bitposition, 16);
				return num;
			}
			if (this.compressType == LiteFloatCompressType.Full32)
			{
				ulong num2 = (ulong)val.uint32;
				ArraySerializeExt.Write(buffer, num2, ref bitposition, 32);
				return num2;
			}
			ulong num3 = this.Encode(val);
			ArraySerializeExt.Write(buffer, num3, ref bitposition, (int)this.compressType);
			return num3;
		}

		// Token: 0x060009D5 RID: 2517 RVA: 0x00032371 File Offset: 0x00030571
		public override void WriteCValue(uint cval, byte[] buffer, ref int bitposition)
		{
			if (this.compressType == LiteFloatCompressType.Half16)
			{
				ArraySerializeExt.Write(buffer, (ulong)cval, ref bitposition, 16);
				return;
			}
			if (this.compressType == LiteFloatCompressType.Full32)
			{
				ArraySerializeExt.Write(buffer, (ulong)cval, ref bitposition, 32);
				return;
			}
			ArraySerializeExt.Write(buffer, (ulong)cval, ref bitposition, (int)this.compressType);
		}

		// Token: 0x060009D6 RID: 2518 RVA: 0x000323B0 File Offset: 0x000305B0
		public override float ReadValue(byte[] buffer, ref int bitposition)
		{
			if (this.compressType == LiteFloatCompressType.Half16)
			{
				return HalfUtilities.Unpack((ushort)ArraySerializeExt.Read(buffer, ref bitposition, 16));
			}
			if (this.compressType == LiteFloatCompressType.Full32)
			{
				return ((uint)ArraySerializeExt.Read(buffer, ref bitposition, 32)).float32;
			}
			uint val = (uint)ArraySerializeExt.Read(buffer, ref bitposition, (int)this.compressType);
			return this.Decode(val);
		}

		// Token: 0x060009D7 RID: 2519 RVA: 0x0003240C File Offset: 0x0003060C
		public override ulong ReadCValue(byte[] buffer, ref int bitposition)
		{
			if (this.compressType == LiteFloatCompressType.Half16)
			{
				return (ulong)((ushort)ArraySerializeExt.Read(buffer, ref bitposition, 16));
			}
			if (this.compressType == LiteFloatCompressType.Full32)
			{
				return (ulong)((uint)ArraySerializeExt.Read(buffer, ref bitposition, 32));
			}
			return (ulong)((uint)ArraySerializeExt.Read(buffer, ref bitposition, (int)this.compressType));
		}

		// Token: 0x04000B3F RID: 2879
		[SerializeField]
		public LiteFloatCrusher.Normalization normalization;

		// Token: 0x04000B40 RID: 2880
		[SerializeField]
		protected float min;

		// Token: 0x04000B41 RID: 2881
		[SerializeField]
		protected float max;

		// Token: 0x04000B42 RID: 2882
		[SerializeField]
		public LiteFloatCompressType compressType = LiteFloatCompressType.Half16;

		// Token: 0x04000B43 RID: 2883
		[SerializeField]
		public LiteOutOfBoundsHandling outOfBoundsHandling;

		// Token: 0x04000B44 RID: 2884
		[SerializeField]
		private bool accurateCenter = true;

		// Token: 0x04000B45 RID: 2885
		[SerializeField]
		private float encoder;

		// Token: 0x04000B46 RID: 2886
		[SerializeField]
		private float decoder;

		// Token: 0x04000B47 RID: 2887
		[SerializeField]
		private ulong maxCVal;

		// Token: 0x020003B2 RID: 946
		public enum Normalization
		{
			// Token: 0x040012AB RID: 4779
			None,
			// Token: 0x040012AC RID: 4780
			Positive,
			// Token: 0x040012AD RID: 4781
			Negative
		}
	}
}
