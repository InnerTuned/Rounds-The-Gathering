using System;
using System.Runtime.InteropServices;
using emotitron.Compression;
using UnityEngine;

namespace Photon.Compression
{
	// Token: 0x0200020D RID: 525
	[StructLayout(2)]
	public class CompressedElement : IEquatable<CompressedElement>
	{
		// Token: 0x06000AF0 RID: 2800 RVA: 0x00038930 File Offset: 0x00036B30
		public void Clear()
		{
			this.crusher = null;
			this.cx = new CompressedFloat(null, 0);
			this.cy = new CompressedFloat(null, 0);
			this.cz = new CompressedFloat(null, 0);
		}

		// Token: 0x06000AF1 RID: 2801 RVA: 0x00038960 File Offset: 0x00036B60
		public ulong[] AsArray64(BitCullingLevel bcl = 0)
		{
			int num = 0;
			ArraySerializeExt.Append(CompressedElement.reusableArray64, (ulong)this.cx.cvalue, ref num, this.cx.crusher._bits[bcl]);
			ArraySerializeExt.Append(CompressedElement.reusableArray64, (ulong)this.cy.cvalue, ref num, this.cy.crusher._bits[bcl]);
			ArraySerializeExt.Append(CompressedElement.reusableArray64, (ulong)this.cz.cvalue, ref num, this.cz.crusher._bits[bcl]);
			ArraySerializeExt.Zero(CompressedElement.reusableArray64, num + 63 >> 6);
			return CompressedElement.reusableArray64;
		}

		// Token: 0x06000AF2 RID: 2802 RVA: 0x00038A04 File Offset: 0x00036C04
		public void AsArray64(ulong[] nonalloc, BitCullingLevel bcl = 0)
		{
			int num = 0;
			ArraySerializeExt.Append(nonalloc, (ulong)this.cx.cvalue, ref num, this.cx.crusher._bits[bcl]);
			ArraySerializeExt.Append(nonalloc, (ulong)this.cy.cvalue, ref num, this.cy.crusher._bits[bcl]);
			ArraySerializeExt.Append(nonalloc, (ulong)this.cz.cvalue, ref num, this.cz.crusher._bits[bcl]);
			ArraySerializeExt.Zero(nonalloc, num + 63 >> 6);
		}

		// Token: 0x06000AF3 RID: 2803 RVA: 0x00038A94 File Offset: 0x00036C94
		public uint[] AsArray32(BitCullingLevel bcl = 0)
		{
			int num = 0;
			ArraySerializeExt.Append(CompressedElement.reusableArray64, (ulong)this.cx.cvalue, ref num, this.cx.crusher._bits[bcl]);
			ArraySerializeExt.Append(CompressedElement.reusableArray64, (ulong)this.cy.cvalue, ref num, this.cy.crusher._bits[bcl]);
			ArraySerializeExt.Append(CompressedElement.reusableArray64, (ulong)this.cz.cvalue, ref num, this.cz.crusher._bits[bcl]);
			ArraySerializeExt.Zero(CompressedElement.reusableArray64, num + 31 >> 5);
			return CompressedElement.reusableArray32;
		}

		// Token: 0x06000AF4 RID: 2804 RVA: 0x00038B38 File Offset: 0x00036D38
		public void AsArray32(uint[] nonalloc, BitCullingLevel bcl = 0)
		{
			int num = 0;
			ArraySerializeExt.Append(nonalloc, this.cx.cvalue, ref num, this.cx.crusher._bits[bcl]);
			ArraySerializeExt.Append(nonalloc, this.cy.cvalue, ref num, this.cy.crusher._bits[bcl]);
			ArraySerializeExt.Append(nonalloc, this.cz.cvalue, ref num, this.cz.crusher._bits[bcl]);
			ArraySerializeExt.Zero(nonalloc, num + 31 >> 5);
		}

		// Token: 0x06000AF5 RID: 2805 RVA: 0x00038BC4 File Offset: 0x00036DC4
		public byte[] AsArray8(BitCullingLevel bcl = 0)
		{
			int num = 0;
			ArraySerializeExt.Append(CompressedElement.reusableArray8, (ulong)this.cx.cvalue, ref num, this.cx.crusher._bits[bcl]);
			ArraySerializeExt.Append(CompressedElement.reusableArray8, (ulong)this.cy.cvalue, ref num, this.cy.crusher._bits[bcl]);
			ArraySerializeExt.Append(CompressedElement.reusableArray8, (ulong)this.cz.cvalue, ref num, this.cz.crusher._bits[bcl]);
			ArraySerializeExt.Zero(CompressedElement.reusableArray64, num + 7 >> 3);
			return CompressedElement.reusableArray8;
		}

		// Token: 0x06000AF6 RID: 2806 RVA: 0x00038C68 File Offset: 0x00036E68
		public void AsArray8(byte[] nonalloc, BitCullingLevel bcl = 0)
		{
			int num = 0;
			ArraySerializeExt.Append(nonalloc, (ulong)this.cx.cvalue, ref num, this.cx.crusher._bits[bcl]);
			ArraySerializeExt.Append(nonalloc, (ulong)this.cy.cvalue, ref num, this.cy.crusher._bits[bcl]);
			ArraySerializeExt.Append(nonalloc, (ulong)this.cz.cvalue, ref num, this.cz.crusher._bits[bcl]);
			ArraySerializeExt.Zero(nonalloc, num + 7 >> 3);
		}

		// Token: 0x06000AF7 RID: 2807 RVA: 0x00038CF4 File Offset: 0x00036EF4
		public static explicit operator ulong(CompressedElement ce)
		{
			ulong result = 0UL;
			ce.crusher.Write(ce, ref result, 0);
			return result;
		}

		// Token: 0x06000AF8 RID: 2808 RVA: 0x00038D18 File Offset: 0x00036F18
		public static explicit operator uint(CompressedElement ce)
		{
			ulong num = 0UL;
			ce.crusher.Write(ce, ref num, 0);
			return (uint)num;
		}

		// Token: 0x06000AF9 RID: 2809 RVA: 0x00038D3C File Offset: 0x00036F3C
		public static explicit operator ushort(CompressedElement ce)
		{
			ulong num = 0UL;
			ce.crusher.Write(ce, ref num, 0);
			return (ushort)num;
		}

		// Token: 0x06000AFA RID: 2810 RVA: 0x00038D60 File Offset: 0x00036F60
		public static explicit operator byte(CompressedElement ce)
		{
			ulong num = 0UL;
			ce.crusher.Write(ce, ref num, 0);
			return (byte)num;
		}

		// Token: 0x06000AFB RID: 2811 RVA: 0x00038D82 File Offset: 0x00036F82
		public static explicit operator ulong[](CompressedElement ce)
		{
			ce.AsArray64(CompressedElement.reusableArray64, 0);
			return CompressedElement.reusableArray64;
		}

		// Token: 0x06000AFC RID: 2812 RVA: 0x00038D95 File Offset: 0x00036F95
		public static explicit operator uint[](CompressedElement ce)
		{
			ce.AsArray32(CompressedElement.reusableArray32, 0);
			return CompressedElement.reusableArray32;
		}

		// Token: 0x06000AFD RID: 2813 RVA: 0x00038DA8 File Offset: 0x00036FA8
		public static explicit operator byte[](CompressedElement ce)
		{
			ce.AsArray8(CompressedElement.reusableArray8, 0);
			return CompressedElement.reusableArray8;
		}

		// Token: 0x06000AFE RID: 2814 RVA: 0x00038DBB File Offset: 0x00036FBB
		public static explicit operator Element(CompressedElement ce)
		{
			return ce.Decompress();
		}

		// Token: 0x06000AFF RID: 2815 RVA: 0x00038DC4 File Offset: 0x00036FC4
		public static explicit operator Vector3(CompressedElement ce)
		{
			Element element = ce.Decompress();
			if (ce.crusher.TRSType == 2)
			{
				global::Debug.LogWarning("Casting CompressedElement of type Quaternion to a Vector3 using quaternion.eulerAngles. Is this intentional? Cast to Quaternion and convert to eulerAnges yourself to silence this warning.");
				return element.quat.eulerAngles;
			}
			return element.v;
		}

		// Token: 0x06000B00 RID: 2816 RVA: 0x00038E04 File Offset: 0x00037004
		public static explicit operator Quaternion(CompressedElement ce)
		{
			Element element = ce.Decompress();
			TRSType trstype = ce.crusher.TRSType;
			if (trstype == 2)
			{
				return element.quat;
			}
			if (trstype == 1)
			{
				global::Debug.LogWarning("Casting a CompressedElement of TRSType.Euler to a Quaternion using Quaternion.Euler(). Is this intentional? Cast to Vector3 and convert to Quaternion yourself to silence this warning.");
				return Quaternion.Euler(element.v);
			}
			global::Debug.LogError("Trying to cast a CompresedElement of " + trstype + " to a quaternion, even though it is not a rotation type. Are you using the correct ElementCrusher to compressed this value?");
			return element.quat;
		}

		// Token: 0x06000B01 RID: 2817 RVA: 0x00003CCC File Offset: 0x00001ECC
		public CompressedElement()
		{
		}

		// Token: 0x06000B02 RID: 2818 RVA: 0x00038E69 File Offset: 0x00037069
		public CompressedElement(ElementCrusher crusher, CompressedFloat cx, CompressedFloat cy, CompressedFloat cz)
		{
			this.crusher = crusher;
			this.cx = cx;
			this.cy = cy;
			this.cz = cz;
		}

		// Token: 0x06000B03 RID: 2819 RVA: 0x00038E90 File Offset: 0x00037090
		public CompressedElement(ElementCrusher crusher, uint cx, uint cy, uint cz)
		{
			UnityEngine.Debug.LogWarning("CE Construct");
			this.crusher = crusher;
			this.cx = new CompressedFloat(crusher.XCrusher, cx);
			this.cy = new CompressedFloat(crusher.YCrusher, cy);
			this.cz = new CompressedFloat(crusher.ZCrusher, cz);
		}

		// Token: 0x06000B04 RID: 2820 RVA: 0x00038EEB File Offset: 0x000370EB
		public CompressedElement(ElementCrusher crusher, uint cUniform)
		{
			UnityEngine.Debug.LogWarning("CE Construct");
			this.crusher = crusher;
			this.cUniform = new CompressedFloat(crusher.UCrusher, cUniform);
		}

		// Token: 0x06000B05 RID: 2821 RVA: 0x00038F16 File Offset: 0x00037116
		public CompressedElement(ElementCrusher crusher, ulong cQuat)
		{
			UnityEngine.Debug.LogWarning("CE Construct");
			this.crusher = crusher;
			this.cQuat = new CompressedQuat(crusher.QCrusher, cQuat);
		}

		// Token: 0x06000B06 RID: 2822 RVA: 0x00038F41 File Offset: 0x00037141
		public void Set(ElementCrusher crusher, CompressedFloat cx, CompressedFloat cy, CompressedFloat cz)
		{
			this.crusher = crusher;
			this.cx = cx;
			this.cy = cy;
			this.cz = cz;
		}

		// Token: 0x06000B07 RID: 2823 RVA: 0x00038F60 File Offset: 0x00037160
		public void Set(ElementCrusher crusher, uint cx, uint cy, uint cz)
		{
			this.crusher = crusher;
			this.cx = new CompressedFloat(crusher.XCrusher, cx);
			this.cy = new CompressedFloat(crusher.YCrusher, cy);
			this.cz = new CompressedFloat(crusher.ZCrusher, cz);
		}

		// Token: 0x06000B08 RID: 2824 RVA: 0x00038FA0 File Offset: 0x000371A0
		public void Set(ElementCrusher crusher, uint cUniform)
		{
			this.crusher = crusher;
			this.cUniform = new CompressedFloat(crusher.UCrusher, cUniform);
		}

		// Token: 0x06000B09 RID: 2825 RVA: 0x00038FBB File Offset: 0x000371BB
		public void Set(ElementCrusher crusher, ulong cQuat)
		{
			this.crusher = crusher;
			this.cQuat = new CompressedQuat(crusher.QCrusher, cQuat);
		}

		// Token: 0x06000B0A RID: 2826 RVA: 0x00038FD6 File Offset: 0x000371D6
		public void CopyTo(CompressedElement copyTarget)
		{
			copyTarget.crusher = this.crusher;
			copyTarget.cx = this.cx;
			copyTarget.cy = this.cy;
			copyTarget.cz = this.cz;
		}

		// Token: 0x06000B0B RID: 2827 RVA: 0x00039008 File Offset: 0x00037208
		public void CopyFrom(CompressedElement copySource)
		{
			this.crusher = copySource.crusher;
			this.cx = copySource.cx;
			this.cy = copySource.cy;
			this.cz = copySource.cz;
		}

		// Token: 0x1700006A RID: 106
		public uint this[int axis]
		{
			get
			{
				return (axis == 0) ? this.cx : ((axis == 1) ? this.cy : this.cz);
			}
		}

		// Token: 0x06000B0D RID: 2829 RVA: 0x0003903A File Offset: 0x0003723A
		public uint GetUInt(int axis)
		{
			return (axis == 0) ? this.cx : ((axis == 1) ? this.cy : this.cz);
		}

		// Token: 0x06000B0E RID: 2830 RVA: 0x0003905E File Offset: 0x0003725E
		public Element Decompress()
		{
			return this.crusher.Decompress(this);
		}

		// Token: 0x06000B0F RID: 2831 RVA: 0x0003906C File Offset: 0x0003726C
		public void Serialize(byte[] buffer, ref int bitposition, IncludedAxes ia, BitCullingLevel bcl = 0)
		{
			this.crusher.Write(this, buffer, ref bitposition, ia, bcl);
		}

		// Token: 0x06000B10 RID: 2832 RVA: 0x0003907F File Offset: 0x0003727F
		[Obsolete("Use a.Compare(b) now instead.")]
		public static bool Compare(CompressedElement a, CompressedElement b)
		{
			return a.Equals(b);
		}

		// Token: 0x06000B11 RID: 2833 RVA: 0x00039088 File Offset: 0x00037288
		public static int HighestDifferentBit(uint a, uint b)
		{
			int result = 0;
			for (int i = 0; i < 32; i++)
			{
				uint num = 1U << i;
				if ((a & num) == (b & num))
				{
					result = i;
				}
			}
			return result;
		}

		// Token: 0x06000B12 RID: 2834 RVA: 0x000390B8 File Offset: 0x000372B8
		public static void Extrapolate(ElementCrusher crusher, CompressedElement target, CompressedElement curr, CompressedElement prev, int divisor = 2)
		{
			target.Set(crusher, (uint)((ulong)curr.cx + ((ulong)curr.cx - (ulong)prev.cx) / (ulong)((long)divisor)), (uint)((ulong)curr.cy + ((ulong)curr.cy - (ulong)prev.cy) / (ulong)((long)divisor)), (uint)((ulong)curr.cz + ((ulong)curr.cz - (ulong)prev.cz) / (ulong)((long)divisor)));
		}

		// Token: 0x06000B13 RID: 2835 RVA: 0x00039150 File Offset: 0x00037350
		[Obsolete]
		public static CompressedElement Extrapolate(ElementCrusher crusher, CompressedElement curr, CompressedElement prev, int divisor = 2)
		{
			return new CompressedElement(crusher, (uint)((ulong)curr.cx + ((ulong)curr.cx - (ulong)prev.cx) / (ulong)((long)divisor)), (uint)((ulong)curr.cy + ((ulong)curr.cy - (ulong)prev.cy) / (ulong)((long)divisor)), (uint)((ulong)curr.cz + ((ulong)curr.cz - (ulong)prev.cz) / (ulong)((long)divisor)));
		}

		// Token: 0x06000B14 RID: 2836 RVA: 0x000391E4 File Offset: 0x000373E4
		public static void Extrapolate(CompressedElement target, CompressedElement curr, CompressedElement prev, int divisor = 2)
		{
			target.Set(curr.crusher, (uint)((ulong)curr.cx + ((ulong)curr.cx - (ulong)prev.cx) / (ulong)((long)divisor)), (uint)((ulong)curr.cy + ((ulong)curr.cy - (ulong)prev.cy) / (ulong)((long)divisor)), (uint)((ulong)curr.cz + ((ulong)curr.cz - (ulong)prev.cz) / (ulong)((long)divisor)));
		}

		// Token: 0x06000B15 RID: 2837 RVA: 0x0003927C File Offset: 0x0003747C
		[Obsolete]
		public static CompressedElement Extrapolate(CompressedElement curr, CompressedElement prev, int divisor = 2)
		{
			return new CompressedElement(curr.crusher, (uint)((ulong)curr.cx + ((ulong)curr.cx - (ulong)prev.cx) / (ulong)((long)divisor)), (uint)((ulong)curr.cy + ((ulong)curr.cy - (ulong)prev.cy) / (ulong)((long)divisor)), (uint)((ulong)curr.cz + ((ulong)curr.cz - (ulong)prev.cz) / (ulong)((long)divisor)));
		}

		// Token: 0x06000B16 RID: 2838 RVA: 0x00039314 File Offset: 0x00037514
		[Obsolete]
		public static void Extrapolate(ElementCrusher crusher, CompressedElement target, CompressedElement curr, CompressedElement prev, float amount = 0.5f)
		{
			target.Set(crusher, (uint)(curr.cx + (float)((ulong)curr.cx - (ulong)prev.cx) * amount), (uint)(curr.cy + (float)((ulong)curr.cy - (ulong)prev.cy) * amount), (uint)(curr.cz + (float)((ulong)curr.cz - (ulong)prev.cz) * amount));
		}

		// Token: 0x06000B17 RID: 2839 RVA: 0x000393AC File Offset: 0x000375AC
		[Obsolete]
		public static CompressedElement Extrapolate(ElementCrusher crusher, CompressedElement curr, CompressedElement prev, float amount = 0.5f)
		{
			return new CompressedElement(crusher, (uint)(curr.cx + (float)((ulong)curr.cx - (ulong)prev.cx) * amount), (uint)(curr.cy + (float)((ulong)curr.cy - (ulong)prev.cy) * amount), (uint)(curr.cz + (float)((ulong)curr.cz - (ulong)prev.cz) * amount));
		}

		// Token: 0x06000B18 RID: 2840 RVA: 0x00039440 File Offset: 0x00037640
		public static void Extrapolate(CompressedElement target, CompressedElement curr, CompressedElement prev, float amount = 0.5f)
		{
			target.Set(curr.crusher, (uint)(curr.cx + (float)((ulong)curr.cx - (ulong)prev.cx) * amount), (uint)(curr.cy + (float)((ulong)curr.cy - (ulong)prev.cy) * amount), (uint)(curr.cz + (float)((ulong)curr.cz - (ulong)prev.cz) * amount));
		}

		// Token: 0x06000B19 RID: 2841 RVA: 0x000394DC File Offset: 0x000376DC
		[Obsolete]
		public static CompressedElement Extrapolate(CompressedElement curr, CompressedElement prev, float amount = 0.5f)
		{
			return new CompressedElement(curr.crusher, (uint)(curr.cx + (float)((ulong)curr.cx - (ulong)prev.cx) * amount), (uint)(curr.cy + (float)((ulong)curr.cy - (ulong)prev.cy) * amount), (uint)(curr.cz + (float)((ulong)curr.cz - (ulong)prev.cz) * amount));
		}

		// Token: 0x06000B1A RID: 2842 RVA: 0x00039578 File Offset: 0x00037778
		public static BitCullingLevel GetGuessableBitCullLevel(CompressedElement a, CompressedElement b, BitCullingLevel maxCullLvl)
		{
			for (BitCullingLevel bitCullingLevel = maxCullLvl; bitCullingLevel > 0; bitCullingLevel--)
			{
				a.ZeroLowerBits(CompressedElement.uppers, bitCullingLevel);
				b.ZeroUpperBits(CompressedElement.lowers, bitCullingLevel);
				if ((CompressedElement.uppers.cx | CompressedElement.lowers.cx) == b.cx && (CompressedElement.uppers.cy | CompressedElement.lowers.cy) == b.cy && (CompressedElement.uppers.cz | CompressedElement.lowers.cz) == b.cz)
				{
					return bitCullingLevel;
				}
			}
			return 0;
		}

		// Token: 0x06000B1B RID: 2843 RVA: 0x00039638 File Offset: 0x00037838
		[Obsolete]
		public static BitCullingLevel GetGuessableBitCullLevel(CompressedElement oldComp, CompressedElement newComp, ElementCrusher ec, BitCullingLevel maxCullLvl)
		{
			for (BitCullingLevel bitCullingLevel = maxCullLvl; bitCullingLevel > 0; bitCullingLevel--)
			{
				oldComp.ZeroLowerBits(CompressedElement.uppers, bitCullingLevel);
				newComp.ZeroUpperBits(CompressedElement.lowers, bitCullingLevel);
				if ((CompressedElement.uppers.cx | CompressedElement.lowers.cx) == newComp.cx && (CompressedElement.uppers.cy | CompressedElement.lowers.cy) == newComp.cy && (CompressedElement.uppers.cz | CompressedElement.lowers.cz) == newComp.cz)
				{
					return bitCullingLevel;
				}
			}
			return 0;
		}

		// Token: 0x06000B1C RID: 2844 RVA: 0x000396F8 File Offset: 0x000378F8
		public static BitCullingLevel FindBestBitCullLevel(CompressedElement a, CompressedElement b, BitCullingLevel maxCulling)
		{
			ElementCrusher elementCrusher = a.crusher;
			if (elementCrusher == null)
			{
				UnityEngine.Debug.Log("NUL CE CRUSHER FindBestBitCullLevel");
				return 0;
			}
			if (elementCrusher.TRSType == 2)
			{
				if (a.cQuat == b.cQuat)
				{
					return 3;
				}
				return 0;
			}
			else
			{
				if (maxCulling == null || !CompressedElement.TestMatchingUpper(a, b, 1))
				{
					return 0;
				}
				if (maxCulling == 1 || !CompressedElement.TestMatchingUpper(a, b, 2))
				{
					return 1;
				}
				if (maxCulling == 2 || !CompressedElement.TestMatchingUpper(a, b, 3))
				{
					return 2;
				}
				return 3;
			}
		}

		// Token: 0x06000B1D RID: 2845 RVA: 0x00039778 File Offset: 0x00037978
		[Obsolete]
		public static BitCullingLevel FindBestBitCullLevel(CompressedElement a, CompressedElement b, ElementCrusher ec, BitCullingLevel maxCulling)
		{
			if (ec.TRSType == 2)
			{
				if (a.cQuat == b.cQuat)
				{
					return 3;
				}
				return 0;
			}
			else
			{
				if (maxCulling == null || !CompressedElement.TestMatchingUpper(a, b, ec, 1))
				{
					return 0;
				}
				if (maxCulling == 1 || !CompressedElement.TestMatchingUpper(a, b, ec, 2))
				{
					return 1;
				}
				if (maxCulling == 2 || !CompressedElement.TestMatchingUpper(a, b, ec, 3))
				{
					return 2;
				}
				return 3;
			}
		}

		// Token: 0x06000B1E RID: 2846 RVA: 0x000397DD File Offset: 0x000379DD
		[Obsolete]
		public static BitCullingLevel FindBestBitCullLevel(CompressedElement a, CompressedElement b, FloatCrusher[] ec, BitCullingLevel maxCulling)
		{
			if (maxCulling == null || !CompressedElement.TestMatchingUpper(a, b, ec, 1))
			{
				return 0;
			}
			if (maxCulling == 1 || !CompressedElement.TestMatchingUpper(a, b, ec, 2))
			{
				return 1;
			}
			if (maxCulling == 2 || !CompressedElement.TestMatchingUpper(a, b, ec, 3))
			{
				return 2;
			}
			return 3;
		}

		// Token: 0x06000B1F RID: 2847 RVA: 0x00039812 File Offset: 0x00037A12
		private static bool TestMatchingUpper(uint a, uint b, int lowerbits)
		{
			return a >> lowerbits << lowerbits == b >> lowerbits << lowerbits;
		}

		// Token: 0x06000B20 RID: 2848 RVA: 0x0003982C File Offset: 0x00037A2C
		public static bool TestMatchingUpper(CompressedElement a, CompressedElement b, BitCullingLevel bcl)
		{
			ElementCrusher elementCrusher = a.crusher;
			return CompressedElement.TestMatchingUpper(a.cx, b.cx, elementCrusher.XCrusher.GetBits(bcl)) && CompressedElement.TestMatchingUpper(a.cy, b.cy, elementCrusher.YCrusher.GetBits(bcl)) && CompressedElement.TestMatchingUpper(a.cz, b.cz, elementCrusher.ZCrusher.GetBits(bcl));
		}

		// Token: 0x06000B21 RID: 2849 RVA: 0x000398BC File Offset: 0x00037ABC
		[Obsolete]
		public static bool TestMatchingUpper(CompressedElement a, CompressedElement b, ElementCrusher ec, BitCullingLevel bcl)
		{
			return CompressedElement.TestMatchingUpper(a.cx, b.cx, ec[0].GetBits(bcl)) && CompressedElement.TestMatchingUpper(a.cy, b.cy, ec[1].GetBits(bcl)) && CompressedElement.TestMatchingUpper(a.cz, b.cz, ec[2].GetBits(bcl));
		}

		// Token: 0x06000B22 RID: 2850 RVA: 0x00039948 File Offset: 0x00037B48
		[Obsolete]
		public static bool TestMatchingUpper(CompressedElement a, CompressedElement b, FloatCrusher[] ec, BitCullingLevel bcl)
		{
			return CompressedElement.TestMatchingUpper(a.cx, b.cx, ec[0].GetBitsAtCullLevel(bcl)) && CompressedElement.TestMatchingUpper(a.cy, b.cy, ec[1].GetBitsAtCullLevel(bcl)) && CompressedElement.TestMatchingUpper(a.cz, b.cz, ec[2].GetBitsAtCullLevel(bcl));
		}

		// Token: 0x06000B23 RID: 2851 RVA: 0x000399C8 File Offset: 0x00037BC8
		public override string ToString()
		{
			if (this.crusher == null)
			{
				return "[Empty CompElement]";
			}
			if (this.crusher.TRSType == 2)
			{
				return string.Concat(new object[]
				{
					this.crusher.TRSType,
					" [",
					this.cQuat.cvalue,
					"]"
				});
			}
			if (this.crusher.TRSType == 3 && this.crusher.uniformAxes != ElementCrusher.UniformAxes.NonUniform)
			{
				return string.Concat(new object[]
				{
					this.crusher.TRSType,
					" [",
					this.crusher.uniformAxes,
					" : ",
					this.cUniform.cvalue,
					"]"
				});
			}
			return string.Concat(new object[]
			{
				this.crusher.TRSType,
				" [x:",
				this.cx.cvalue,
				" y:",
				this.cy.cvalue,
				" z:",
				this.cz.cvalue,
				"]"
			});
		}

		// Token: 0x06000B24 RID: 2852 RVA: 0x00039B28 File Offset: 0x00037D28
		public static bool operator ==(CompressedElement a, CompressedElement b)
		{
			return a != null && a.Equals(b);
		}

		// Token: 0x06000B25 RID: 2853 RVA: 0x00039B36 File Offset: 0x00037D36
		public static bool operator !=(CompressedElement a, CompressedElement b)
		{
			return a == null || !a.Equals(b);
		}

		// Token: 0x06000B26 RID: 2854 RVA: 0x00039B47 File Offset: 0x00037D47
		public override bool Equals(object obj)
		{
			return this.Equals(obj as CompressedElement);
		}

		// Token: 0x06000B27 RID: 2855 RVA: 0x00039B58 File Offset: 0x00037D58
		public bool Equals(CompressedElement other)
		{
			return other != null && this.cx.cvalue == other.cx.cvalue && this.cy.cvalue == other.cy.cvalue && this.cz.cvalue == other.cz.cvalue && this.cUniform.cvalue == other.cUniform.cvalue && this.cQuat.cvalue == other.cQuat.cvalue;
		}

		// Token: 0x06000B28 RID: 2856 RVA: 0x00039BE4 File Offset: 0x00037DE4
		public override int GetHashCode()
		{
			return ((((-1337834834 * -1521134295 + this.cx.GetHashCode()) * -1521134295 + this.cy.GetHashCode()) * -1521134295 + this.cz.GetHashCode()) * -1521134295 + this.cUniform.GetHashCode()) * -1521134295 + this.cQuat.GetHashCode();
		}

		// Token: 0x04000BEB RID: 3051
		public static CompressedElement reusable = new CompressedElement();

		// Token: 0x04000BEC RID: 3052
		[FieldOffset(0)]
		public CompressedFloat cx;

		// Token: 0x04000BED RID: 3053
		[FieldOffset(16)]
		public CompressedFloat cy;

		// Token: 0x04000BEE RID: 3054
		[FieldOffset(32)]
		public CompressedFloat cz;

		// Token: 0x04000BEF RID: 3055
		[FieldOffset(0)]
		public CompressedFloat cUniform;

		// Token: 0x04000BF0 RID: 3056
		[FieldOffset(0)]
		public CompressedQuat cQuat;

		// Token: 0x04000BF1 RID: 3057
		[FieldOffset(48)]
		public ElementCrusher crusher;

		// Token: 0x04000BF2 RID: 3058
		private static readonly ulong[] reusableArray64 = new ulong[2];

		// Token: 0x04000BF3 RID: 3059
		private static readonly uint[] reusableArray32 = new uint[4];

		// Token: 0x04000BF4 RID: 3060
		private static readonly byte[] reusableArray8 = new byte[16];

		// Token: 0x04000BF5 RID: 3061
		[Obsolete("Compressed Element is now a class and no longer a struct. Where this used to be used, now compressedElement.Clear() should be used instead.")]
		public static readonly CompressedElement Empty = new CompressedElement();

		// Token: 0x04000BF6 RID: 3062
		private static CompressedElement uppers = new CompressedElement();

		// Token: 0x04000BF7 RID: 3063
		private static CompressedElement lowers = new CompressedElement();
	}
}
