using System;
using emotitron.Compression;

namespace Photon.Compression
{
	// Token: 0x0200020A RID: 522
	public static class FloatCrusherExtensions
	{
		// Token: 0x06000AB6 RID: 2742 RVA: 0x00038080 File Offset: 0x00036280
		public static CompressedFloat Write(this FloatCrusher fc, float f, ref ulong buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			int num = fc._bits[bcl];
			CompressedFloat compressedFloat = fc.Compress(f, 0);
			PrimitiveSerializeExt.Inject(compressedFloat.cvalue, ref buffer, ref bitposition, num);
			return compressedFloat;
		}

		// Token: 0x06000AB7 RID: 2743 RVA: 0x000380B0 File Offset: 0x000362B0
		public static CompressedFloat Write(this FloatCrusher fc, float f, ref uint buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			int num = fc._bits[bcl];
			CompressedFloat compressedFloat = fc.Compress(f, 0);
			PrimitiveSerializeExt.Inject(compressedFloat.cvalue, ref buffer, ref bitposition, num);
			return compressedFloat;
		}

		// Token: 0x06000AB8 RID: 2744 RVA: 0x000380E0 File Offset: 0x000362E0
		public static CompressedFloat Write(this FloatCrusher fc, float f, ref ushort buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			int num = fc._bits[bcl];
			CompressedFloat compressedFloat = fc.Compress(f, 0);
			PrimitiveSerializeExt.Inject(compressedFloat.cvalue, ref buffer, ref bitposition, num);
			return compressedFloat;
		}

		// Token: 0x06000AB9 RID: 2745 RVA: 0x00038110 File Offset: 0x00036310
		public static CompressedFloat Write(this FloatCrusher fc, float f, ref byte buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			int num = fc._bits[bcl];
			CompressedFloat compressedFloat = fc.Compress(f, 0);
			PrimitiveSerializeExt.Inject(compressedFloat.cvalue, ref buffer, ref bitposition, num);
			return compressedFloat;
		}

		// Token: 0x06000ABA RID: 2746 RVA: 0x0003813D File Offset: 0x0003633D
		public static CompressedFloat Write(this FloatCrusher fc, CompressedFloat c, ref ulong buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			PrimitiveSerializeExt.Inject(c.cvalue, ref buffer, ref bitposition, fc._bits[bcl]);
			return c;
		}

		// Token: 0x06000ABB RID: 2747 RVA: 0x00038158 File Offset: 0x00036358
		public static float ReadAndDecompress(this FloatCrusher fc, ulong buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			int num = fc._bits[bcl];
			uint num2 = (uint)PrimitiveSerializeExt.Read(buffer, ref bitposition, num);
			return fc.Decompress(num2);
		}

		// Token: 0x06000ABC RID: 2748 RVA: 0x00038180 File Offset: 0x00036380
		public static CompressedFloat Read(this FloatCrusher fc, ulong buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			int num = fc._bits[bcl];
			uint num2 = (uint)PrimitiveSerializeExt.Read(buffer, ref bitposition, num);
			return new CompressedFloat(fc, num2);
		}

		// Token: 0x06000ABD RID: 2749 RVA: 0x000381A8 File Offset: 0x000363A8
		[Obsolete("No reason for buffer to be a ref")]
		public static CompressedFloat Read(this FloatCrusher fc, ref ulong buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			int num = fc._bits[bcl];
			uint num2 = fc.masks[bcl];
			uint num3 = (uint)(buffer >> bitposition & (ulong)num2);
			bitposition += num;
			return new CompressedFloat(fc, num3);
		}

		// Token: 0x06000ABE RID: 2750 RVA: 0x000381E4 File Offset: 0x000363E4
		public static CompressedFloat Write(this FloatCrusher fc, CompressedFloat c, byte[] buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			int num = fc._bits[bcl];
			ArraySerializeExt.Write(buffer, (ulong)c.cvalue, ref bitposition, num);
			return c;
		}

		// Token: 0x06000ABF RID: 2751 RVA: 0x0003820C File Offset: 0x0003640C
		public static CompressedFloat Write(this FloatCrusher fc, CompressedFloat c, uint[] buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			int num = fc._bits[bcl];
			ArraySerializeExt.Write(buffer, (ulong)c.cvalue, ref bitposition, num);
			return c;
		}

		// Token: 0x06000AC0 RID: 2752 RVA: 0x00038234 File Offset: 0x00036434
		public static CompressedFloat Write(this FloatCrusher fc, CompressedFloat c, ulong[] buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			int num = fc._bits[bcl];
			ArraySerializeExt.Write(buffer, (ulong)c.cvalue, ref bitposition, num);
			return c;
		}

		// Token: 0x06000AC1 RID: 2753 RVA: 0x0003825C File Offset: 0x0003645C
		public static CompressedFloat Write(this FloatCrusher fc, uint c, byte[] buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			int num = fc._bits[bcl];
			ArraySerializeExt.Write(buffer, (ulong)c, ref bitposition, num);
			return new CompressedFloat(fc, c);
		}

		// Token: 0x06000AC2 RID: 2754 RVA: 0x00038284 File Offset: 0x00036484
		public static CompressedFloat Write(this FloatCrusher fc, uint c, uint[] buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			int num = fc._bits[bcl];
			ArraySerializeExt.Write(buffer, (ulong)c, ref bitposition, num);
			return new CompressedFloat(fc, c);
		}

		// Token: 0x06000AC3 RID: 2755 RVA: 0x000382AC File Offset: 0x000364AC
		public static CompressedFloat Write(this FloatCrusher fc, uint c, ulong[] buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			int num = fc._bits[bcl];
			ArraySerializeExt.Write(buffer, (ulong)c, ref bitposition, num);
			return new CompressedFloat(fc, c);
		}

		// Token: 0x06000AC4 RID: 2756 RVA: 0x000382D4 File Offset: 0x000364D4
		public static CompressedFloat Write(this FloatCrusher fc, float f, byte[] buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			uint num = fc.Compress(f, 0);
			int num2 = fc._bits[bcl];
			ArraySerializeExt.Write(buffer, (ulong)num, ref bitposition, num2);
			return new CompressedFloat(fc, num);
		}

		// Token: 0x06000AC5 RID: 2757 RVA: 0x0003830C File Offset: 0x0003650C
		public static CompressedFloat Write(this FloatCrusher fc, float f, uint[] buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			int num = fc._bits[bcl];
			uint num2 = fc.Compress(f, 0);
			ArraySerializeExt.Write(buffer, (ulong)num2, ref bitposition, num);
			return new CompressedFloat(fc, num2);
		}

		// Token: 0x06000AC6 RID: 2758 RVA: 0x00038344 File Offset: 0x00036544
		public static CompressedFloat Write(this FloatCrusher fc, float f, ulong[] buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			int num = fc._bits[bcl];
			uint num2 = fc.Compress(f, 0);
			ArraySerializeExt.Write(buffer, (ulong)num2, ref bitposition, num);
			return new CompressedFloat(fc, num2);
		}

		// Token: 0x06000AC7 RID: 2759 RVA: 0x0003837C File Offset: 0x0003657C
		public static CompressedFloat Read(this FloatCrusher fc, byte[] buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			int num = fc._bits[bcl];
			return new CompressedFloat(fc, ArraySerializeExt.ReadUInt32(buffer, ref bitposition, num));
		}

		// Token: 0x06000AC8 RID: 2760 RVA: 0x000383A0 File Offset: 0x000365A0
		public static CompressedFloat Read(this FloatCrusher fc, uint[] buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			int num = fc._bits[bcl];
			return new CompressedFloat(fc, ArraySerializeExt.ReadUInt32(buffer, ref bitposition, num));
		}

		// Token: 0x06000AC9 RID: 2761 RVA: 0x000383C4 File Offset: 0x000365C4
		public static CompressedFloat Read(this FloatCrusher fc, ulong[] buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			int num = fc._bits[bcl];
			return new CompressedFloat(fc, ArraySerializeExt.ReadUInt32(buffer, ref bitposition, num));
		}

		// Token: 0x06000ACA RID: 2762 RVA: 0x000383E8 File Offset: 0x000365E8
		public static float ReadAndDecompress(this FloatCrusher fc, byte[] buffer, ref int bitposition)
		{
			uint num = ArraySerializeExt.ReadUInt32(buffer, ref bitposition, fc._bits[0]);
			return fc.Decompress(num);
		}

		// Token: 0x06000ACB RID: 2763 RVA: 0x0003840C File Offset: 0x0003660C
		public static float ReadAndDecompress(this FloatCrusher fc, uint[] buffer, ref int bitposition)
		{
			uint num = ArraySerializeExt.ReadUInt32(buffer, ref bitposition, fc._bits[0]);
			return fc.Decompress(num);
		}

		// Token: 0x06000ACC RID: 2764 RVA: 0x00038430 File Offset: 0x00036630
		public static float ReadAndDecompress(this FloatCrusher fc, ulong[] buffer, ref int bitposition)
		{
			uint num = ArraySerializeExt.ReadUInt32(buffer, ref bitposition, fc._bits[0]);
			return fc.Decompress(num);
		}
	}
}
