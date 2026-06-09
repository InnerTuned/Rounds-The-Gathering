using System;
using System.Text;
using emotitron.Compression;

namespace Photon.Compression
{
	// Token: 0x0200020E RID: 526
	public static class CompressedElementExt
	{
		// Token: 0x06000B2A RID: 2858 RVA: 0x00039CC8 File Offset: 0x00037EC8
		public static StringBuilder AppendSB(this StringBuilder strb, CompressedElement ce)
		{
			if (ce == null)
			{
				strb.Append("[Null CompElement]");
			}
			else
			{
				ElementCrusher crusher = ce.crusher;
				if (crusher == null)
				{
					strb.Append("[CE Null Crusher]");
				}
				else if (crusher.TRSType == 2)
				{
					strb.Append(crusher.TRSType).Append(" cQuat: [").Append(ce.cQuat.cvalue).Append("]");
				}
				else if (crusher.TRSType == 3 && crusher.uniformAxes != ElementCrusher.UniformAxes.NonUniform)
				{
					strb.Append(crusher.TRSType).Append(" cUni: [").Append(crusher.uniformAxes).Append(" : ").Append(ce.cUniform.cvalue).Append("]");
				}
				else
				{
					strb.Append(crusher.TRSType).Append(" cXYZ: [x:").Append(ce.cx.cvalue).Append(" y:").Append(ce.cy.cvalue).Append(" z:").Append(ce.cz.cvalue).Append("]");
				}
			}
			return strb;
		}

		// Token: 0x06000B2B RID: 2859 RVA: 0x00039E1C File Offset: 0x0003801C
		public static void GetChangeAmount(uint[] results, CompressedElement a, CompressedElement b)
		{
			for (int i = 0; i < 3; i++)
			{
				results[i] = (uint)Math.Abs((long)((ulong)(a[i] - b[0])));
			}
		}

		// Token: 0x06000B2C RID: 2860 RVA: 0x00039E50 File Offset: 0x00038050
		[Obsolete]
		public static uint[] GetChangeAmount(CompressedElement a, CompressedElement b)
		{
			for (int i = 0; i < 3; i++)
			{
				CompressedElementExt.reusableInts[i] = (uint)Math.Abs((long)((ulong)(a[i] - b[0])));
			}
			return CompressedElementExt.reusableInts;
		}

		// Token: 0x06000B2D RID: 2861 RVA: 0x00039E8C File Offset: 0x0003808C
		public static void GuessUpperBits(this CompressedElement newcpos, ElementCrusher ec, CompressedElement oldcpos, BitCullingLevel bcl)
		{
			newcpos.Set(ec, ec.XCrusher.GuessUpperBits(newcpos[0], oldcpos[0], bcl), ec.YCrusher.GuessUpperBits(newcpos[1], oldcpos[1], bcl), ec.ZCrusher.GuessUpperBits(newcpos[2], oldcpos[2], bcl));
		}

		// Token: 0x06000B2E RID: 2862 RVA: 0x00039EF0 File Offset: 0x000380F0
		[Obsolete]
		public static CompressedElement GuessUpperBits(this CompressedElement newcpos, CompressedElement oldcpos, ElementCrusher ec, BitCullingLevel bcl)
		{
			return new CompressedElement(ec, ec.XCrusher.GuessUpperBits(newcpos[0], oldcpos[0], bcl), ec.YCrusher.GuessUpperBits(newcpos[1], oldcpos[1], bcl), ec.ZCrusher.GuessUpperBits(newcpos[2], oldcpos[2], bcl));
		}

		// Token: 0x06000B2F RID: 2863 RVA: 0x00039F54 File Offset: 0x00038154
		public static void OverwriteUpperBits(this CompressedElement low, CompressedElement uppers, BitCullingLevel bcl)
		{
			ElementCrusher crusher = low.crusher;
			low.Set(crusher, crusher.XCrusher.OverwriteUpperBits(low.cx, uppers.cx, bcl), crusher.YCrusher.OverwriteUpperBits(low.cy, uppers.cy, bcl), crusher.ZCrusher.OverwriteUpperBits(low.cz, uppers.cz, bcl));
		}

		// Token: 0x06000B30 RID: 2864 RVA: 0x00039FD8 File Offset: 0x000381D8
		[Obsolete]
		public static CompressedElement OverwriteUpperBits(this CompressedElement low, CompressedElement up, ElementCrusher ec, BitCullingLevel bcl)
		{
			return new CompressedElement(ec, ec[0].OverwriteUpperBits(low.cx, up.cx, bcl), ec[1].OverwriteUpperBits(low.cy, up.cy, bcl), ec[2].OverwriteUpperBits(low.cz, up.cz, bcl));
		}

		// Token: 0x06000B31 RID: 2865 RVA: 0x0003A054 File Offset: 0x00038254
		public static void ZeroLowerBits(this CompressedElement fullpos, CompressedElement target, BitCullingLevel bcl)
		{
			ElementCrusher crusher = fullpos.crusher;
			target.Set(crusher, crusher.XCrusher.ZeroLowerBits(fullpos.cx, bcl), crusher.YCrusher.ZeroLowerBits(fullpos.cy, bcl), crusher.ZCrusher.ZeroLowerBits(fullpos.cz, bcl));
		}

		// Token: 0x06000B32 RID: 2866 RVA: 0x0003A0B4 File Offset: 0x000382B4
		[Obsolete]
		public static CompressedElement ZeroLowerBits(this CompressedElement fullpos, ElementCrusher ec, BitCullingLevel bcl)
		{
			return new CompressedElement(ec, ec[0].ZeroLowerBits(fullpos.cx, bcl), ec[1].ZeroLowerBits(fullpos.cy, bcl), ec[2].ZeroLowerBits(fullpos.cz, bcl));
		}

		// Token: 0x06000B33 RID: 2867 RVA: 0x0003A110 File Offset: 0x00038310
		public static void ZeroUpperBits(this CompressedElement fullpos, CompressedElement target, BitCullingLevel bcl)
		{
			ElementCrusher crusher = fullpos.crusher;
			target.Set(crusher, crusher.XCrusher.ZeroUpperBits(fullpos.cx, bcl), crusher.YCrusher.ZeroUpperBits(fullpos.cy, bcl), crusher.ZCrusher.ZeroUpperBits(fullpos.cz, bcl));
		}

		// Token: 0x06000B34 RID: 2868 RVA: 0x0003A170 File Offset: 0x00038370
		[Obsolete]
		public static CompressedElement ZeroUpperBits(this CompressedElement fullpos, ElementCrusher ec, BitCullingLevel bcl)
		{
			return new CompressedElement(ec, ec[0].ZeroUpperBits(fullpos.cx, bcl), ec[1].ZeroUpperBits(fullpos.cy, bcl), ec[2].ZeroUpperBits(fullpos.cz, bcl));
		}

		// Token: 0x04000BF8 RID: 3064
		public static uint[] reusableInts = new uint[3];
	}
}
