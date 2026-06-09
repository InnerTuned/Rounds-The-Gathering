using System;
using System.Collections.Generic;
using emotitron.Compression;
using UnityEngine;

namespace Photon.Compression
{
	// Token: 0x0200020F RID: 527
	public class CompressedMatrix : IEquatable<CompressedMatrix>
	{
		// Token: 0x06000B36 RID: 2870 RVA: 0x0003A1D8 File Offset: 0x000383D8
		public CompressedMatrix()
		{
		}

		// Token: 0x06000B37 RID: 2871 RVA: 0x0003A201 File Offset: 0x00038401
		public CompressedMatrix(TransformCrusher crusher)
		{
			this.crusher = crusher;
		}

		// Token: 0x06000B38 RID: 2872 RVA: 0x0003A234 File Offset: 0x00038434
		public CompressedMatrix(TransformCrusher crusher, CompressedElement cPos, CompressedElement cRot, CompressedElement cScl)
		{
			this.crusher = crusher;
			this.cPos = cPos;
			this.cRot = cRot;
			this.cScl = cScl;
		}

		// Token: 0x06000B39 RID: 2873 RVA: 0x0003A288 File Offset: 0x00038488
		public CompressedMatrix(TransformCrusher crusher, ref CompressedElement cPos, ref CompressedElement cRot, ref CompressedElement cScl, int pBits, int rBits, int sBits)
		{
			this.crusher = crusher;
			this.cPos = cPos;
			this.cRot = cRot;
			this.cScl = cScl;
		}

		// Token: 0x06000B3A RID: 2874 RVA: 0x0003A2DC File Offset: 0x000384DC
		public void CopyTo(CompressedMatrix copyTarget)
		{
			this.cPos.CopyTo(copyTarget.cPos);
			this.cRot.CopyTo(copyTarget.cRot);
			this.cScl.CopyTo(copyTarget.cScl);
		}

		// Token: 0x06000B3B RID: 2875 RVA: 0x0003A311 File Offset: 0x00038511
		public void CopyFrom(CompressedMatrix copySource)
		{
			this.cPos.CopyFrom(copySource.cPos);
			this.cRot.CopyFrom(copySource.cRot);
			this.cScl.CopyFrom(copySource.cScl);
		}

		// Token: 0x06000B3C RID: 2876 RVA: 0x0003A346 File Offset: 0x00038546
		public void Clear()
		{
			this.crusher = null;
			this.cPos.Clear();
			this.cRot.Clear();
			this.cScl.Clear();
		}

		// Token: 0x06000B3D RID: 2877 RVA: 0x0003A370 File Offset: 0x00038570
		public ulong[] AsArray64(BitCullingLevel bcl = 0)
		{
			int num = 0;
			this.crusher.Write(this, CompressedMatrix.reusableArray64, ref num, bcl);
			ArraySerializeExt.Zero(CompressedMatrix.reusableArray64, num + 63 >> 6);
			return CompressedMatrix.reusableArray64;
		}

		// Token: 0x06000B3E RID: 2878 RVA: 0x0003A3A8 File Offset: 0x000385A8
		public void AsArray64(ulong[] nonalloc, BitCullingLevel bcl = 0)
		{
			int num = 0;
			this.crusher.Write(this, nonalloc, ref num, bcl);
			ArraySerializeExt.Zero(nonalloc, num + 63 >> 6);
		}

		// Token: 0x06000B3F RID: 2879 RVA: 0x0003A3D4 File Offset: 0x000385D4
		public uint[] AsArray32(BitCullingLevel bcl = 0)
		{
			int num = 0;
			this.crusher.Write(this, CompressedMatrix.reusableArray32, ref num, bcl);
			ArraySerializeExt.Zero(CompressedMatrix.reusableArray32, num + 31 >> 5);
			return CompressedMatrix.reusableArray32;
		}

		// Token: 0x06000B40 RID: 2880 RVA: 0x0003A40C File Offset: 0x0003860C
		public void AsArray32(uint[] nonalloc, BitCullingLevel bcl = 0)
		{
			int num = 0;
			this.crusher.Write(this, nonalloc, ref num, bcl);
			ArraySerializeExt.Zero(nonalloc, num + 31 >> 5);
		}

		// Token: 0x06000B41 RID: 2881 RVA: 0x0003A438 File Offset: 0x00038638
		public byte[] AsArray8(BitCullingLevel bcl = 0)
		{
			int num = 0;
			this.crusher.Write(this, CompressedMatrix.reusableArray64, ref num, bcl);
			ArraySerializeExt.Zero(CompressedMatrix.reusableArray8, num + 7 >> 3);
			return CompressedMatrix.reusableArray8;
		}

		// Token: 0x06000B42 RID: 2882 RVA: 0x0003A470 File Offset: 0x00038670
		public void AsArray8(byte[] nonalloc, BitCullingLevel bcl = 0)
		{
			int num = 0;
			this.crusher.Write(this, nonalloc, ref num, bcl);
			ArraySerializeExt.Zero(nonalloc, num + 7 >> 3);
		}

		// Token: 0x06000B43 RID: 2883 RVA: 0x0003A49C File Offset: 0x0003869C
		public static explicit operator ulong(CompressedMatrix cm)
		{
			ulong result = 0UL;
			int num = 0;
			cm.crusher.Write(cm, ref result, ref num, 0);
			return result;
		}

		// Token: 0x06000B44 RID: 2884 RVA: 0x0003A4C0 File Offset: 0x000386C0
		public static explicit operator uint(CompressedMatrix cm)
		{
			ulong num = 0UL;
			int num2 = 0;
			cm.crusher.Write(cm, ref num, ref num2, 0);
			return (uint)num;
		}

		// Token: 0x06000B45 RID: 2885 RVA: 0x0003A4E8 File Offset: 0x000386E8
		public static explicit operator ushort(CompressedMatrix cm)
		{
			ulong num = 0UL;
			int num2 = 0;
			cm.crusher.Write(cm, ref num, ref num2, 0);
			return (ushort)num;
		}

		// Token: 0x06000B46 RID: 2886 RVA: 0x0003A510 File Offset: 0x00038710
		public static explicit operator byte(CompressedMatrix cm)
		{
			ulong num = 0UL;
			int num2 = 0;
			cm.crusher.Write(cm, ref num, ref num2, 0);
			return (byte)num;
		}

		// Token: 0x06000B47 RID: 2887 RVA: 0x0003A535 File Offset: 0x00038735
		public static explicit operator ulong[](CompressedMatrix cm)
		{
			return cm.AsArray64(0);
		}

		// Token: 0x06000B48 RID: 2888 RVA: 0x0003A53E File Offset: 0x0003873E
		public static explicit operator uint[](CompressedMatrix cm)
		{
			return cm.AsArray32(0);
		}

		// Token: 0x06000B49 RID: 2889 RVA: 0x0003A547 File Offset: 0x00038747
		public static explicit operator byte[](CompressedMatrix cm)
		{
			return cm.AsArray8(0);
		}

		// Token: 0x06000B4A RID: 2890 RVA: 0x0003A550 File Offset: 0x00038750
		public void Decompress(Matrix nonalloc)
		{
			if (this.crusher != null)
			{
				this.crusher.Decompress(nonalloc, this);
				return;
			}
			nonalloc.Clear();
		}

		// Token: 0x06000B4B RID: 2891 RVA: 0x0003A574 File Offset: 0x00038774
		public Matrix Decompress()
		{
			this.crusher.Decompress(Matrix.reusable, this);
			return Matrix.reusable;
		}

		// Token: 0x06000B4C RID: 2892 RVA: 0x0003A58C File Offset: 0x0003878C
		[Obsolete("Supply the transform to Compress. Default Transform has been deprecated to allow shared TransformCrushers.")]
		public void Apply()
		{
			if (this.crusher != null)
			{
				this.crusher.Apply(this);
			}
		}

		// Token: 0x06000B4D RID: 2893 RVA: 0x0003A5A8 File Offset: 0x000387A8
		public void Apply(Transform t)
		{
			if (this.crusher != null)
			{
				this.crusher.Apply(t, this);
			}
		}

		// Token: 0x06000B4E RID: 2894 RVA: 0x0003A5C5 File Offset: 0x000387C5
		[Obsolete("Apply for Rigidbody has been replaced with Move and Set, to indicate usage of MovePosition/Rotation vs rb.position/rotation.")]
		public void Apply(Rigidbody rb)
		{
			if (this.crusher != null)
			{
				this.crusher.Apply(rb, this);
			}
		}

		// Token: 0x06000B4F RID: 2895 RVA: 0x0003A5E2 File Offset: 0x000387E2
		public void Set(Rigidbody rb)
		{
			if (this.crusher != null)
			{
				this.crusher.Set(rb, this);
			}
		}

		// Token: 0x06000B50 RID: 2896 RVA: 0x0003A5FF File Offset: 0x000387FF
		public void Move(Rigidbody rb)
		{
			if (this.crusher != null)
			{
				this.crusher.Move(rb, this);
			}
		}

		// Token: 0x06000B51 RID: 2897 RVA: 0x0003A61C File Offset: 0x0003881C
		public static bool operator ==(CompressedMatrix a, CompressedMatrix b)
		{
			return a != null && a.Equals(b);
		}

		// Token: 0x06000B52 RID: 2898 RVA: 0x0003A62A File Offset: 0x0003882A
		public static bool operator !=(CompressedMatrix a, CompressedMatrix b)
		{
			return a == null || !a.Equals(b);
		}

		// Token: 0x06000B53 RID: 2899 RVA: 0x0003A63B File Offset: 0x0003883B
		public override bool Equals(object obj)
		{
			return this.Equals(obj as CompressedMatrix);
		}

		// Token: 0x06000B54 RID: 2900 RVA: 0x0003A649 File Offset: 0x00038849
		public bool Equals(CompressedMatrix other)
		{
			return other != null && this.cPos.Equals(other.cPos) && this.cRot.Equals(other.cRot) && this.cScl.Equals(other.cScl);
		}

		// Token: 0x06000B55 RID: 2901 RVA: 0x0003A688 File Offset: 0x00038888
		public override int GetHashCode()
		{
			return (((94804922 * -1521134295 + this.cPos.GetHashCode()) * -1521134295 + this.cRot.GetHashCode()) * -1521134295 + this.cScl.GetHashCode()) * -1521134295 + EqualityComparer<TransformCrusher>.Default.GetHashCode(this.crusher);
		}

		// Token: 0x04000BF9 RID: 3065
		public CompressedElement cPos = new CompressedElement();

		// Token: 0x04000BFA RID: 3066
		public CompressedElement cRot = new CompressedElement();

		// Token: 0x04000BFB RID: 3067
		public CompressedElement cScl = new CompressedElement();

		// Token: 0x04000BFC RID: 3068
		public TransformCrusher crusher;

		// Token: 0x04000BFD RID: 3069
		public static CompressedMatrix reusable = new CompressedMatrix();

		// Token: 0x04000BFE RID: 3070
		protected static readonly ulong[] reusableArray64 = new ulong[6];

		// Token: 0x04000BFF RID: 3071
		protected static readonly uint[] reusableArray32 = new uint[12];

		// Token: 0x04000C00 RID: 3072
		protected static readonly byte[] reusableArray8 = new byte[24];
	}
}
