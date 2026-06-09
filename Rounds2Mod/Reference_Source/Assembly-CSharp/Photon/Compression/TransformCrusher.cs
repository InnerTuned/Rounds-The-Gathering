using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using emotitron.Compression;
using UnityEngine;

namespace Photon.Compression
{
	// Token: 0x02000218 RID: 536
	[Serializable]
	public class TransformCrusher : Crusher<TransformCrusher>, ICrusherCopy<TransformCrusher>
	{
		// Token: 0x06000B90 RID: 2960 RVA: 0x0003B864 File Offset: 0x00039A64
		public static TransformCrusher CheckAgainstStatics(TransformCrusher tc, bool CheckElementCrusherAsWell = true)
		{
			if (tc == null)
			{
				return null;
			}
			if (CheckElementCrusherAsWell)
			{
				tc.posCrusher = ElementCrusher.CheckAgainstStatics(tc.posCrusher, true);
				tc.rotCrusher = ElementCrusher.CheckAgainstStatics(tc.rotCrusher, true);
				tc.sclCrusher = ElementCrusher.CheckAgainstStatics(tc.sclCrusher, true);
			}
			int hashCode = tc.GetHashCode();
			if (TransformCrusher.staticTransformCrushers.ContainsKey(hashCode))
			{
				return TransformCrusher.staticTransformCrushers[hashCode];
			}
			TransformCrusher.staticTransformCrushers.Add(hashCode, tc);
			return tc;
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x06000B91 RID: 2961 RVA: 0x0003B8DC File Offset: 0x00039ADC
		// (set) Token: 0x06000B92 RID: 2962 RVA: 0x0003B8E4 File Offset: 0x00039AE4
		public ElementCrusher PosCrusher
		{
			get
			{
				return this.posCrusher;
			}
			set
			{
				if (this.posCrusher == value)
				{
					return;
				}
				if (this.posCrusher != null)
				{
					ElementCrusher elementCrusher = this.posCrusher;
					elementCrusher.OnRecalculated = (Action<ElementCrusher>)Delegate.Remove(elementCrusher.OnRecalculated, new Action<ElementCrusher>(this.OnCrusherChange));
				}
				this.posCrusher = value;
				if (this.posCrusher != null)
				{
					ElementCrusher elementCrusher2 = this.posCrusher;
					elementCrusher2.OnRecalculated = (Action<ElementCrusher>)Delegate.Combine(elementCrusher2.OnRecalculated, new Action<ElementCrusher>(this.OnCrusherChange));
				}
				this.CacheValues();
			}
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x06000B93 RID: 2963 RVA: 0x0003B972 File Offset: 0x00039B72
		// (set) Token: 0x06000B94 RID: 2964 RVA: 0x0003B97C File Offset: 0x00039B7C
		public ElementCrusher RotCrusher
		{
			get
			{
				return this.rotCrusher;
			}
			set
			{
				if (this.rotCrusher == value)
				{
					return;
				}
				if (this.rotCrusher != null)
				{
					ElementCrusher elementCrusher = this.rotCrusher;
					elementCrusher.OnRecalculated = (Action<ElementCrusher>)Delegate.Remove(elementCrusher.OnRecalculated, new Action<ElementCrusher>(this.OnCrusherChange));
				}
				this.rotCrusher = value;
				if (this.rotCrusher != null)
				{
					ElementCrusher elementCrusher2 = this.rotCrusher;
					elementCrusher2.OnRecalculated = (Action<ElementCrusher>)Delegate.Combine(elementCrusher2.OnRecalculated, new Action<ElementCrusher>(this.OnCrusherChange));
				}
				this.CacheValues();
			}
		}

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000B95 RID: 2965 RVA: 0x0003BA0A File Offset: 0x00039C0A
		// (set) Token: 0x06000B96 RID: 2966 RVA: 0x0003BA14 File Offset: 0x00039C14
		public ElementCrusher SclCrusher
		{
			get
			{
				return this.sclCrusher;
			}
			set
			{
				if (this.sclCrusher == value)
				{
					return;
				}
				if (this.sclCrusher != null)
				{
					ElementCrusher elementCrusher = this.sclCrusher;
					elementCrusher.OnRecalculated = (Action<ElementCrusher>)Delegate.Remove(elementCrusher.OnRecalculated, new Action<ElementCrusher>(this.OnCrusherChange));
				}
				this.sclCrusher = value;
				if (this.sclCrusher != null)
				{
					ElementCrusher elementCrusher2 = this.sclCrusher;
					elementCrusher2.OnRecalculated = (Action<ElementCrusher>)Delegate.Combine(elementCrusher2.OnRecalculated, new Action<ElementCrusher>(this.OnCrusherChange));
				}
				this.CacheValues();
			}
		}

		// Token: 0x06000B97 RID: 2967 RVA: 0x0003BAA2 File Offset: 0x00039CA2
		public void OnCrusherChange(ElementCrusher crusher)
		{
			this.CacheValues();
		}

		// Token: 0x06000B98 RID: 2968 RVA: 0x0003BAAA File Offset: 0x00039CAA
		public TransformCrusher()
		{
			this.ConstructDefault(false);
		}

		// Token: 0x06000B99 RID: 2969 RVA: 0x0003BAE9 File Offset: 0x00039CE9
		public TransformCrusher(bool isStatic = false)
		{
			this.ConstructDefault(isStatic);
		}

		// Token: 0x06000B9A RID: 2970 RVA: 0x0003BB28 File Offset: 0x00039D28
		protected virtual void ConstructDefault(bool isStatic = false)
		{
			if (!isStatic)
			{
				this.PosCrusher = new ElementCrusher(0, false);
				this.RotCrusher = new ElementCrusher(1, false)
				{
					XCrusher = new FloatCrusher(12, -90f, 90f, 0, 1, true),
					YCrusher = new FloatCrusher(12, -180f, 180f, 1, 1, true),
					ZCrusher = new FloatCrusher(0, -180f, 180f, 2, 1, true)
				};
				this.SclCrusher = new ElementCrusher(3, false)
				{
					uniformAxes = ElementCrusher.UniformAxes.XYZ,
					UCrusher = new FloatCrusher(8, 0f, 2f, 3, 3, true)
				};
			}
		}

		// Token: 0x06000B9B RID: 2971 RVA: 0x000027C8 File Offset: 0x000009C8
		public override void OnBeforeSerialize()
		{
		}

		// Token: 0x06000B9C RID: 2972 RVA: 0x0003BAA2 File Offset: 0x00039CA2
		public override void OnAfterDeserialize()
		{
			this.CacheValues();
		}

		// Token: 0x06000B9D RID: 2973 RVA: 0x0003BBD0 File Offset: 0x00039DD0
		public virtual void CacheValues()
		{
			for (int i = 0; i < 4; i++)
			{
				this.cached_pBits[i] = ((this.posCrusher == null) ? 0 : this.posCrusher.Cached_TotalBits[i]);
				this.cached_rBits[i] = ((this.rotCrusher == null) ? 0 : this.rotCrusher.Cached_TotalBits[i]);
				this.cached_sBits[i] = ((this.sclCrusher == null) ? 0 : this.sclCrusher.Cached_TotalBits[i]);
				this._cached_total[i] = this.cached_pBits[i] + this.cached_rBits[i] + this.cached_sBits[i];
				this.cached_total = Array.AsReadOnly<int>(this._cached_total);
			}
			this.cached = true;
		}

		// Token: 0x06000B9E RID: 2974 RVA: 0x0003BCA8 File Offset: 0x00039EA8
		public void Write(CompressedMatrix cm, byte[] buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			if (this.cached_pBits[bcl] > 0)
			{
				this.posCrusher.Write(cm.cPos, buffer, ref bitposition, IncludedAxes.XYZ, bcl);
			}
			if (this.cached_rBits[bcl] > 0)
			{
				this.rotCrusher.Write(cm.cRot, buffer, ref bitposition, IncludedAxes.XYZ, bcl);
			}
			if (this.cached_sBits[bcl] > 0)
			{
				this.sclCrusher.Write(cm.cScl, buffer, ref bitposition, IncludedAxes.XYZ, bcl);
			}
		}

		// Token: 0x06000B9F RID: 2975 RVA: 0x0003BD2C File Offset: 0x00039F2C
		public void Write(CompressedMatrix cm, uint[] buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			if (this.cached_pBits[bcl] > 0)
			{
				this.posCrusher.Write(cm.cPos, buffer, ref bitposition, IncludedAxes.XYZ, bcl);
			}
			if (this.cached_rBits[bcl] > 0)
			{
				this.rotCrusher.Write(cm.cRot, buffer, ref bitposition, IncludedAxes.XYZ, bcl);
			}
			if (this.cached_sBits[bcl] > 0)
			{
				this.sclCrusher.Write(cm.cScl, buffer, ref bitposition, IncludedAxes.XYZ, bcl);
			}
		}

		// Token: 0x06000BA0 RID: 2976 RVA: 0x0003BDB0 File Offset: 0x00039FB0
		public void Write(CompressedMatrix cm, ulong[] buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			if (this.cached_pBits[bcl] > 0)
			{
				this.posCrusher.Write(cm.cPos, buffer, ref bitposition, IncludedAxes.XYZ, bcl);
			}
			if (this.cached_rBits[bcl] > 0)
			{
				this.rotCrusher.Write(cm.cRot, buffer, ref bitposition, IncludedAxes.XYZ, bcl);
			}
			if (this.cached_sBits[bcl] > 0)
			{
				this.sclCrusher.Write(cm.cScl, buffer, ref bitposition, IncludedAxes.XYZ, bcl);
			}
		}

		// Token: 0x06000BA1 RID: 2977 RVA: 0x0003BE34 File Offset: 0x0003A034
		public void Write(CompressedMatrix nonalloc, Transform transform, byte[] buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			nonalloc.crusher = this;
			if (this.cached_pBits[bcl] > 0)
			{
				this.posCrusher.Write(nonalloc.cPos, transform, buffer, ref bitposition, bcl);
			}
			if (this.cached_rBits[bcl] > 0)
			{
				this.rotCrusher.Write(nonalloc.cRot, transform, buffer, ref bitposition, bcl);
			}
			if (this.cached_sBits[bcl] > 0)
			{
				this.sclCrusher.Write(nonalloc.cScl, transform, buffer, ref bitposition, bcl);
			}
		}

		// Token: 0x06000BA2 RID: 2978 RVA: 0x0003BEBF File Offset: 0x0003A0BF
		public void Write(Transform transform, byte[] buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			this.Write(CompressedMatrix.reusable, transform, buffer, ref bitposition, bcl);
		}

		// Token: 0x06000BA3 RID: 2979 RVA: 0x0003BED4 File Offset: 0x0003A0D4
		public Matrix ReadAndDecompress(ulong[] array, BitCullingLevel bcl = 0)
		{
			int num = 0;
			return this.ReadAndDecompress(array, ref num, bcl);
		}

		// Token: 0x06000BA4 RID: 2980 RVA: 0x0003BEF0 File Offset: 0x0003A0F0
		public Matrix ReadAndDecompress(uint[] array, BitCullingLevel bcl = 0)
		{
			int num = 0;
			return this.ReadAndDecompress(array, ref num, bcl);
		}

		// Token: 0x06000BA5 RID: 2981 RVA: 0x0003BF0C File Offset: 0x0003A10C
		public Matrix ReadAndDecompress(byte[] array, BitCullingLevel bcl = 0)
		{
			int num = 0;
			return this.ReadAndDecompress(array, ref num, bcl);
		}

		// Token: 0x06000BA6 RID: 2982 RVA: 0x0003BF25 File Offset: 0x0003A125
		public void ReadAndDecompress(Matrix nonalloc, ulong[] array, ref int bitposition, BitCullingLevel bcl = 0)
		{
			this.Read(CompressedMatrix.reusable, array, ref bitposition, bcl);
			this.Decompress(nonalloc, CompressedMatrix.reusable);
		}

		// Token: 0x06000BA7 RID: 2983 RVA: 0x0003BF42 File Offset: 0x0003A142
		public void ReadAndDecompress(Matrix nonalloc, uint[] array, ref int bitposition, BitCullingLevel bcl = 0)
		{
			this.Read(CompressedMatrix.reusable, array, ref bitposition, bcl);
			this.Decompress(nonalloc, CompressedMatrix.reusable);
		}

		// Token: 0x06000BA8 RID: 2984 RVA: 0x0003BF5F File Offset: 0x0003A15F
		public void ReadAndDecompress(Matrix nonalloc, byte[] array, ref int bitposition, BitCullingLevel bcl = 0)
		{
			this.Read(CompressedMatrix.reusable, array, ref bitposition, bcl);
			this.Decompress(nonalloc, CompressedMatrix.reusable);
		}

		// Token: 0x06000BA9 RID: 2985 RVA: 0x0003BF7C File Offset: 0x0003A17C
		public Matrix ReadAndDecompress(ulong[] array, ref int bitposition, BitCullingLevel bcl = 0)
		{
			this.ReadAndDecompress(Matrix.reusable, array, ref bitposition, bcl);
			return Matrix.reusable;
		}

		// Token: 0x06000BAA RID: 2986 RVA: 0x0003BF91 File Offset: 0x0003A191
		public Matrix ReadAndDecompress(uint[] array, ref int bitposition, BitCullingLevel bcl = 0)
		{
			this.ReadAndDecompress(Matrix.reusable, array, ref bitposition, bcl);
			return Matrix.reusable;
		}

		// Token: 0x06000BAB RID: 2987 RVA: 0x0003BFA6 File Offset: 0x0003A1A6
		public Matrix ReadAndDecompress(byte[] array, ref int bitposition, BitCullingLevel bcl = 0)
		{
			this.ReadAndDecompress(Matrix.reusable, array, ref bitposition, bcl);
			return Matrix.reusable;
		}

		// Token: 0x06000BAC RID: 2988 RVA: 0x0003BFBC File Offset: 0x0003A1BC
		public void Read(CompressedMatrix nonalloc, byte[] array, ref int bitposition, BitCullingLevel bcl = 0)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			nonalloc.crusher = this;
			if (this.cached_pBits[bcl] > 0)
			{
				this.posCrusher.Read(nonalloc.cPos, array, ref bitposition, IncludedAxes.XYZ, bcl);
			}
			if (this.cached_rBits[bcl] > 0)
			{
				this.rotCrusher.Read(nonalloc.cRot, array, ref bitposition, IncludedAxes.XYZ, bcl);
			}
			if (this.cached_sBits[bcl] > 0)
			{
				this.sclCrusher.Read(nonalloc.cScl, array, ref bitposition, IncludedAxes.XYZ, bcl);
			}
		}

		// Token: 0x06000BAD RID: 2989 RVA: 0x0003C044 File Offset: 0x0003A244
		public CompressedMatrix Read(ulong[] array, ref int bitposition, BitCullingLevel bcl = 0)
		{
			this.Read(CompressedMatrix.reusable, array, ref bitposition, bcl);
			return CompressedMatrix.reusable;
		}

		// Token: 0x06000BAE RID: 2990 RVA: 0x0003C059 File Offset: 0x0003A259
		public CompressedMatrix Read(uint[] array, ref int bitposition, BitCullingLevel bcl = 0)
		{
			this.Read(CompressedMatrix.reusable, array, ref bitposition, bcl);
			return CompressedMatrix.reusable;
		}

		// Token: 0x06000BAF RID: 2991 RVA: 0x0003C06E File Offset: 0x0003A26E
		public CompressedMatrix Read(byte[] array, ref int bitposition, BitCullingLevel bcl = 0)
		{
			this.Read(CompressedMatrix.reusable, array, ref bitposition, bcl);
			return CompressedMatrix.reusable;
		}

		// Token: 0x06000BB0 RID: 2992 RVA: 0x0003C084 File Offset: 0x0003A284
		public CompressedMatrix Read(ulong[] array, BitCullingLevel bcl = 0)
		{
			int num = 0;
			this.Read(CompressedMatrix.reusable, array, ref num, bcl);
			return CompressedMatrix.reusable;
		}

		// Token: 0x06000BB1 RID: 2993 RVA: 0x0003C0A8 File Offset: 0x0003A2A8
		public CompressedMatrix Read(uint[] array, BitCullingLevel bcl = 0)
		{
			int num = 0;
			this.Read(CompressedMatrix.reusable, array, ref num, bcl);
			return CompressedMatrix.reusable;
		}

		// Token: 0x06000BB2 RID: 2994 RVA: 0x0003C0CC File Offset: 0x0003A2CC
		public CompressedMatrix Read(byte[] array, BitCullingLevel bcl = 0)
		{
			int num = 0;
			this.Read(CompressedMatrix.reusable, array, ref num, bcl);
			return CompressedMatrix.reusable;
		}

		// Token: 0x06000BB3 RID: 2995 RVA: 0x0003C0F0 File Offset: 0x0003A2F0
		public void Read(CompressedMatrix nonalloc, ulong[] buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			nonalloc.crusher = this;
			if (this.cached_pBits[bcl] > 0)
			{
				this.posCrusher.Read(nonalloc.cPos, buffer, ref bitposition, IncludedAxes.XYZ, bcl);
			}
			if (this.cached_rBits[bcl] > 0)
			{
				this.rotCrusher.Read(nonalloc.cRot, buffer, ref bitposition, IncludedAxes.XYZ, bcl);
			}
			if (this.cached_sBits[bcl] > 0)
			{
				this.sclCrusher.Read(nonalloc.cScl, buffer, ref bitposition, IncludedAxes.XYZ, bcl);
			}
		}

		// Token: 0x06000BB4 RID: 2996 RVA: 0x0003C178 File Offset: 0x0003A378
		public void Read(CompressedMatrix nonalloc, uint[] buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			nonalloc.crusher = this;
			if (this.cached_pBits[bcl] > 0)
			{
				this.posCrusher.Read(nonalloc.cPos, buffer, ref bitposition, IncludedAxes.XYZ, bcl);
			}
			if (this.cached_rBits[bcl] > 0)
			{
				this.rotCrusher.Read(nonalloc.cRot, buffer, ref bitposition, IncludedAxes.XYZ, bcl);
			}
			if (this.cached_sBits[bcl] > 0)
			{
				this.sclCrusher.Read(nonalloc.cScl, buffer, ref bitposition, IncludedAxes.XYZ, bcl);
			}
		}

		// Token: 0x06000BB5 RID: 2997 RVA: 0x0003C200 File Offset: 0x0003A400
		public void Write(CompressedMatrix nonalloc, Transform transform, ref ulong buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			nonalloc.crusher = this;
			if (this.cached_pBits[bcl] > 0)
			{
				this.posCrusher.Write(nonalloc.cPos, transform, ref buffer, ref bitposition, bcl);
			}
			if (this.cached_rBits[bcl] > 0)
			{
				this.rotCrusher.Write(nonalloc.cRot, transform, ref buffer, ref bitposition, bcl);
			}
			if (this.cached_sBits[bcl] > 0)
			{
				this.sclCrusher.Write(nonalloc.cScl, transform, ref buffer, ref bitposition, bcl);
			}
		}

		// Token: 0x06000BB6 RID: 2998 RVA: 0x0003C28C File Offset: 0x0003A48C
		public void Write(Transform transform, ref ulong buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			if (this.cached_pBits[bcl] > 0)
			{
				this.posCrusher.Write(transform, ref buffer, ref bitposition, bcl);
			}
			if (this.cached_rBits[bcl] > 0)
			{
				this.rotCrusher.Write(transform, ref buffer, ref bitposition, bcl);
			}
			if (this.cached_sBits[bcl] > 0)
			{
				this.sclCrusher.Write(transform, ref buffer, ref bitposition, bcl);
			}
		}

		// Token: 0x06000BB7 RID: 2999 RVA: 0x0003C2FC File Offset: 0x0003A4FC
		public void Write(CompressedMatrix cm, ref ulong buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			if (this.cached_pBits[bcl] > 0)
			{
				this.posCrusher.Write(cm.cPos, ref buffer, ref bitposition, bcl);
			}
			if (this.cached_rBits[bcl] > 0)
			{
				this.rotCrusher.Write(cm.cRot, ref buffer, ref bitposition, bcl);
			}
			if (this.cached_sBits[bcl] > 0)
			{
				this.sclCrusher.Write(cm.cScl, ref buffer, ref bitposition, bcl);
			}
		}

		// Token: 0x06000BB8 RID: 3000 RVA: 0x0003C370 File Offset: 0x0003A570
		public void Write(CompressedMatrix nonalloc, Transform transform, ref ulong bitstream, BitCullingLevel bcl = 0)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			this.Compress(nonalloc, transform);
			if (this.cached_pBits[bcl] > 0)
			{
				this.posCrusher.Write(nonalloc.cPos, ref bitstream, bcl);
			}
			if (this.cached_rBits[bcl] > 0)
			{
				this.rotCrusher.Write(nonalloc.cRot, ref bitstream, bcl);
			}
			if (this.cached_sBits[bcl] > 0)
			{
				this.sclCrusher.Write(nonalloc.cScl, ref bitstream, bcl);
			}
		}

		// Token: 0x06000BB9 RID: 3001 RVA: 0x0003C3F8 File Offset: 0x0003A5F8
		public void ReadAndDecompress(Matrix nonalloc, ulong buffer, BitCullingLevel bcl = 0)
		{
			int num = 0;
			this.ReadAndDecompress(nonalloc, buffer, ref num, bcl);
		}

		// Token: 0x06000BBA RID: 3002 RVA: 0x0003C414 File Offset: 0x0003A614
		[Obsolete("Use the nonalloc overload instead and supply a target Matrix. Matrix is now a class rather than a struct")]
		public Matrix ReadAndDecompress(ulong buffer, BitCullingLevel bcl = 0)
		{
			int num = 0;
			return this.ReadAndDecompress(buffer, ref num, bcl);
		}

		// Token: 0x06000BBB RID: 3003 RVA: 0x0003C42D File Offset: 0x0003A62D
		public void ReadAndDecompress(Matrix nonalloc, ulong buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			this.Read(CompressedMatrix.reusable, buffer, ref bitposition, bcl);
			this.Decompress(nonalloc, CompressedMatrix.reusable);
		}

		// Token: 0x06000BBC RID: 3004 RVA: 0x0003C44A File Offset: 0x0003A64A
		[Obsolete("Use the nonalloc overload instead and supply a target Matrix. Matrix is now a class rather than a struct")]
		public Matrix ReadAndDecompress(ulong buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			this.Read(CompressedMatrix.reusable, buffer, ref bitposition, bcl);
			return this.Decompress(CompressedMatrix.reusable);
		}

		// Token: 0x06000BBD RID: 3005 RVA: 0x0003C465 File Offset: 0x0003A665
		public void ReadAndApply(Transform target, byte[] buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			this.Read(CompressedMatrix.reusable, buffer, ref bitposition, bcl);
			this.Apply(target, CompressedMatrix.reusable);
		}

		// Token: 0x06000BBE RID: 3006 RVA: 0x0003C484 File Offset: 0x0003A684
		public void Read(CompressedMatrix nonalloc, ulong frag0, ulong frag1 = 0UL, ulong frag2 = 0UL, ulong frag3 = 0UL, ulong frag4 = 0UL, BitCullingLevel bcl = 0)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			int num = 0;
			ArraySerializeExt.Write(TransformCrusher.reusableArray64, frag0, ref num, 64);
			ArraySerializeExt.Write(TransformCrusher.reusableArray64, frag1, ref num, 64);
			ArraySerializeExt.Write(TransformCrusher.reusableArray64, frag2, ref num, 64);
			ArraySerializeExt.Write(TransformCrusher.reusableArray64, frag3, ref num, 64);
			ArraySerializeExt.Write(TransformCrusher.reusableArray64, frag4, ref num, 64);
			num = 0;
			this.Read(nonalloc, TransformCrusher.reusableArray64, ref num, bcl);
		}

		// Token: 0x06000BBF RID: 3007 RVA: 0x0003C501 File Offset: 0x0003A701
		public void ReadAndDecompress(Matrix nonalloc, ulong frag0, ulong frag1 = 0UL, ulong frag2 = 0UL, ulong frag3 = 0UL, ulong frag4 = 0UL, BitCullingLevel bcl = 0)
		{
			this.Read(CompressedMatrix.reusable, frag0, frag1, frag2, frag3, frag4, bcl);
			this.Decompress(nonalloc, CompressedMatrix.reusable);
		}

		// Token: 0x06000BC0 RID: 3008 RVA: 0x0003C524 File Offset: 0x0003A724
		public CompressedMatrix Read(ulong frag0, ulong frag1 = 0UL, ulong frag2 = 0UL, ulong frag3 = 0UL, uint frag4 = 0U, BitCullingLevel bcl = 0)
		{
			this.Read(CompressedMatrix.reusable, frag0, frag1, frag2, frag3, (ulong)frag4, bcl);
			return CompressedMatrix.reusable;
		}

		// Token: 0x06000BC1 RID: 3009 RVA: 0x0003C540 File Offset: 0x0003A740
		public void Read(CompressedMatrix nonalloc, ulong buffer, BitCullingLevel bcl = 0)
		{
			int num = 0;
			this.Read(nonalloc, buffer, ref num, bcl);
		}

		// Token: 0x06000BC2 RID: 3010 RVA: 0x0003C55C File Offset: 0x0003A75C
		public void Read(CompressedMatrix nonalloc, ulong buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			nonalloc.crusher = this;
			if (this.cached_pBits[bcl] > 0)
			{
				this.posCrusher.Read(nonalloc.cPos, buffer, ref bitposition, bcl);
			}
			if (this.cached_rBits[bcl] > 0)
			{
				this.rotCrusher.Read(nonalloc.cRot, buffer, ref bitposition, bcl);
			}
			if (this.cached_sBits[bcl] > 0)
			{
				this.sclCrusher.Read(nonalloc.cScl, buffer, ref bitposition, bcl);
			}
		}

		// Token: 0x06000BC3 RID: 3011 RVA: 0x0003C5E1 File Offset: 0x0003A7E1
		public CompressedMatrix Read(ulong buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			this.Read(CompressedMatrix.reusable, buffer, ref bitposition, bcl);
			return CompressedMatrix.reusable;
		}

		// Token: 0x06000BC4 RID: 3012 RVA: 0x0003C5F6 File Offset: 0x0003A7F6
		[Obsolete("Supply the transform to compress. Default Transform has be deprecated.")]
		public void Compress(CompressedMatrix nonalloc)
		{
			global::Debug.Assert(this.defaultTransform, "The 'defaultTransform' is null and has not be set in the inspector. For non-editor usages of TransformCrusher you need to pass the target transform to this method.");
			this.Compress(nonalloc, this.defaultTransform);
		}

		// Token: 0x06000BC5 RID: 3013 RVA: 0x0003C615 File Offset: 0x0003A815
		[Obsolete("Supply the transform to compress. Default Transform has be deprecated.")]
		public CompressedMatrix Compress()
		{
			global::Debug.Assert(this.defaultTransform, "The 'defaultTransform' is null and has not be set in the inspector. For non-editor usages of TransformCrusher you need to pass the target transform to this method.");
			return this.Compress(this.defaultTransform);
		}

		// Token: 0x06000BC6 RID: 3014 RVA: 0x0003C634 File Offset: 0x0003A834
		public void Compress(CompressedMatrix nonalloc, Matrix matrix)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			nonalloc.crusher = this;
			if (this.cached_pBits[0] > 0)
			{
				this.posCrusher.Compress(nonalloc.cPos, matrix.position, IncludedAxes.XYZ);
			}
			else
			{
				nonalloc.cPos.Clear();
			}
			if (this.cached_rBits[0] > 0)
			{
				this.rotCrusher.Compress(nonalloc.cRot, matrix.rotation);
			}
			else
			{
				nonalloc.cRot.Clear();
			}
			if (this.cached_sBits[0] > 0)
			{
				this.sclCrusher.Compress(nonalloc.cScl, matrix.scale, IncludedAxes.XYZ);
				return;
			}
			nonalloc.cScl.Clear();
		}

		// Token: 0x06000BC7 RID: 3015 RVA: 0x0003C6E4 File Offset: 0x0003A8E4
		public void Compress(CompressedMatrix nonalloc, Transform transform)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			nonalloc.crusher = this;
			if (this.cached_pBits[0] > 0)
			{
				this.posCrusher.Compress(nonalloc.cPos, transform);
			}
			else
			{
				nonalloc.cPos.Clear();
			}
			if (this.cached_rBits[0] > 0)
			{
				this.rotCrusher.Compress(nonalloc.cRot, transform);
			}
			else
			{
				nonalloc.cRot.Clear();
			}
			if (this.cached_sBits[0] > 0)
			{
				this.sclCrusher.Compress(nonalloc.cScl, transform);
				return;
			}
			nonalloc.cScl.Clear();
		}

		// Token: 0x06000BC8 RID: 3016 RVA: 0x0003C783 File Offset: 0x0003A983
		public CompressedMatrix Compress(Transform transform)
		{
			this.Compress(CompressedMatrix.reusable, transform);
			return CompressedMatrix.reusable;
		}

		// Token: 0x06000BC9 RID: 3017 RVA: 0x0003C796 File Offset: 0x0003A996
		public CompressedMatrix Compress(Rigidbody rb)
		{
			this.Compress(CompressedMatrix.reusable, rb);
			return CompressedMatrix.reusable;
		}

		// Token: 0x06000BCA RID: 3018 RVA: 0x0003C7AC File Offset: 0x0003A9AC
		public void Compress(CompressedMatrix nonalloc, Rigidbody rb)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			nonalloc.crusher = this;
			this.posCrusher.Compress(nonalloc.cPos, rb, IncludedAxes.XYZ);
			this.rotCrusher.Compress(nonalloc.cRot, rb, IncludedAxes.XYZ);
			this.sclCrusher.Compress(nonalloc.cScl, rb, IncludedAxes.XYZ);
		}

		// Token: 0x06000BCB RID: 3019 RVA: 0x0003C807 File Offset: 0x0003AA07
		public CompressedMatrix Compress(Rigidbody2D rb2d)
		{
			this.Compress(CompressedMatrix.reusable, rb2d);
			return CompressedMatrix.reusable;
		}

		// Token: 0x06000BCC RID: 3020 RVA: 0x0003C81C File Offset: 0x0003AA1C
		public void Compress(CompressedMatrix nonalloc, Rigidbody2D rb2d)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			nonalloc.crusher = this;
			this.posCrusher.Compress(nonalloc.cPos, rb2d.transform);
			this.rotCrusher.Compress(nonalloc.cRot, rb2d.transform);
			this.sclCrusher.Compress(nonalloc.cScl, rb2d.transform);
		}

		// Token: 0x06000BCD RID: 3021 RVA: 0x0003C884 File Offset: 0x0003AA84
		public void CompressAndWrite(Matrix matrix, byte[] buffer, ref int bitposition)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			if (this.cached_pBits[0] > 0)
			{
				this.posCrusher.CompressAndWrite(matrix.position, buffer, ref bitposition, IncludedAxes.XYZ);
			}
			if (this.cached_rBits[0] > 0)
			{
				this.rotCrusher.CompressAndWrite(matrix.rotation, buffer, ref bitposition);
			}
			if (this.cached_sBits[0] > 0)
			{
				this.sclCrusher.CompressAndWrite(matrix.scale, buffer, ref bitposition, IncludedAxes.XYZ);
			}
		}

		// Token: 0x06000BCE RID: 3022 RVA: 0x0003C8FC File Offset: 0x0003AAFC
		public void CompressAndWrite(Transform transform, byte[] buffer, ref int bitposition)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			if (this.cached_pBits[0] > 0)
			{
				this.posCrusher.CompressAndWrite(transform, buffer, ref bitposition);
			}
			if (this.cached_rBits[0] > 0)
			{
				this.rotCrusher.CompressAndWrite(transform, buffer, ref bitposition);
			}
			if (this.cached_sBits[0] > 0)
			{
				this.sclCrusher.CompressAndWrite(transform, buffer, ref bitposition);
			}
		}

		// Token: 0x06000BCF RID: 3023 RVA: 0x0003C964 File Offset: 0x0003AB64
		public void CompressAndWrite(Rigidbody rb, byte[] buffer, ref int bitposition)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			if (this.cached_pBits[0] > 0)
			{
				Vector3 v = (this.posCrusher.local && rb.transform.parent) ? rb.transform.InverseTransformPoint(rb.position) : rb.position;
				this.posCrusher.CompressAndWrite(v, buffer, ref bitposition, IncludedAxes.XYZ);
			}
			if (this.cached_rBits[0] > 0)
			{
				if (this.rotCrusher.TRSType == 2)
				{
					this.rotCrusher.CompressAndWrite(rb.rotation, buffer, ref bitposition);
				}
				else
				{
					this.rotCrusher.CompressAndWrite(rb.rotation.eulerAngles, buffer, ref bitposition, IncludedAxes.XYZ);
				}
			}
			if (this.cached_sBits[0] > 0)
			{
				this.sclCrusher.CompressAndWrite(rb.transform, buffer, ref bitposition);
			}
		}

		// Token: 0x06000BD0 RID: 3024 RVA: 0x0003CA3C File Offset: 0x0003AC3C
		public void Decompress(Matrix nonalloc, ulong[] buffer, BitCullingLevel bcl = 0)
		{
			int num = 0;
			this.Read(CompressedMatrix.reusable, buffer, ref num, bcl);
			this.Decompress(nonalloc, CompressedMatrix.reusable);
		}

		// Token: 0x06000BD1 RID: 3025 RVA: 0x0003CA68 File Offset: 0x0003AC68
		public void Decompress(Matrix nonalloc, uint[] buffer, BitCullingLevel bcl = 0)
		{
			int num = 0;
			this.Read(CompressedMatrix.reusable, buffer, ref num, bcl);
			this.Decompress(nonalloc, CompressedMatrix.reusable);
		}

		// Token: 0x06000BD2 RID: 3026 RVA: 0x0003CA92 File Offset: 0x0003AC92
		public void Decompress(Matrix nonalloc, ulong compressed, BitCullingLevel bcl = 0)
		{
			this.Read(CompressedMatrix.reusable, compressed, bcl);
			this.Decompress(nonalloc, CompressedMatrix.reusable);
		}

		// Token: 0x06000BD3 RID: 3027 RVA: 0x0003CAB0 File Offset: 0x0003ACB0
		public Matrix Decompress(ulong[] buffer, BitCullingLevel bcl = 0)
		{
			int num = 0;
			this.Read(CompressedMatrix.reusable, buffer, ref num, bcl);
			this.Decompress(Matrix.reusable, CompressedMatrix.reusable);
			return Matrix.reusable;
		}

		// Token: 0x06000BD4 RID: 3028 RVA: 0x0003CAE4 File Offset: 0x0003ACE4
		public Matrix Decompress(uint[] buffer, BitCullingLevel bcl = 0)
		{
			int num = 0;
			this.Read(CompressedMatrix.reusable, buffer, ref num, bcl);
			this.Decompress(Matrix.reusable, CompressedMatrix.reusable);
			return Matrix.reusable;
		}

		// Token: 0x06000BD5 RID: 3029 RVA: 0x0003CB18 File Offset: 0x0003AD18
		public Matrix Decompress(byte[] buffer, BitCullingLevel bcl = 0)
		{
			int num = 0;
			this.Read(CompressedMatrix.reusable, buffer, ref num, bcl);
			this.Decompress(Matrix.reusable, CompressedMatrix.reusable);
			return Matrix.reusable;
		}

		// Token: 0x06000BD6 RID: 3030 RVA: 0x0003CB4B File Offset: 0x0003AD4B
		public Matrix Decompress(ulong compressed, BitCullingLevel bcl = 0)
		{
			this.Read(CompressedMatrix.reusable, compressed, bcl);
			this.Decompress(Matrix.reusable, CompressedMatrix.reusable);
			return Matrix.reusable;
		}

		// Token: 0x06000BD7 RID: 3031 RVA: 0x0003CB70 File Offset: 0x0003AD70
		public void Decompress(Matrix nonalloc, CompressedMatrix compMatrix)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			nonalloc.Set(this, (this.cached_pBits[0] > 0) ? ((Vector3)this.posCrusher.Decompress(compMatrix.cPos)) : default(Vector3), (this.cached_rBits[0] > 0) ? this.rotCrusher.Decompress(compMatrix.cRot) : ((this.rotCrusher.TRSType == 2) ? new Element(new Quaternion(0f, 0f, 0f, 1f)) : new Element(new Vector3(0f, 0f, 0f))), (this.cached_sBits[0] > 0) ? ((Vector3)this.sclCrusher.Decompress(compMatrix.cScl)) : default(Vector3));
		}

		// Token: 0x06000BD8 RID: 3032 RVA: 0x0003CC50 File Offset: 0x0003AE50
		[Obsolete("Use the nonalloc overload instead and supply a target Matrix. Matrix is now a class rather than a struct")]
		public Matrix Decompress(CompressedMatrix compMatrix)
		{
			if (!this.cached)
			{
				this.CacheValues();
			}
			return new Matrix(this, (this.cached_pBits[0] > 0) ? ((Vector3)this.posCrusher.Decompress(compMatrix.cPos)) : default(Vector3), (this.cached_rBits[0] > 0) ? this.rotCrusher.Decompress(compMatrix.cRot) : ((this.rotCrusher.TRSType == 2) ? new Element(new Quaternion(0f, 0f, 0f, 1f)) : new Element(new Vector3(0f, 0f, 0f))), (this.cached_sBits[0] > 0) ? ((Vector3)this.sclCrusher.Decompress(compMatrix.cScl)) : default(Vector3));
		}

		// Token: 0x06000BD9 RID: 3033 RVA: 0x0003CD30 File Offset: 0x0003AF30
		public void Set(Rigidbody rb, CompressedMatrix cmatrix)
		{
			if (this.cached_pBits[0] > 0)
			{
				this.posCrusher.Set(rb, cmatrix.cPos, IncludedAxes.XYZ);
			}
			if (this.cached_rBits[0] > 0)
			{
				this.rotCrusher.Set(rb, cmatrix.cRot, IncludedAxes.XYZ);
			}
			if (this.cached_sBits[0] > 0)
			{
				this.sclCrusher.Apply(rb.transform, cmatrix.cScl, IncludedAxes.XYZ);
			}
		}

		// Token: 0x06000BDA RID: 3034 RVA: 0x0003CD9C File Offset: 0x0003AF9C
		public void Set(Rigidbody rb, Matrix matrix)
		{
			if (this.cached_pBits[0] > 0)
			{
				this.posCrusher.Set(rb, matrix.position, IncludedAxes.XYZ);
			}
			if (this.cached_rBits[0] > 0)
			{
				this.rotCrusher.Set(rb, matrix.rotation, IncludedAxes.XYZ);
			}
			if (this.cached_sBits[0] > 0)
			{
				this.sclCrusher.Apply(rb.transform, matrix.scale, IncludedAxes.XYZ);
			}
		}

		// Token: 0x06000BDB RID: 3035 RVA: 0x0003CE12 File Offset: 0x0003B012
		public void Set(Rigidbody rb, ulong frag0, ulong frag1 = 0UL, ulong frag2 = 0UL, ulong frag3 = 0UL, ulong frag4 = 0UL)
		{
			this.Read(CompressedMatrix.reusable, frag0, frag1, frag2, frag3, frag4, 0);
			this.Set(rb, CompressedMatrix.reusable);
		}

		// Token: 0x06000BDC RID: 3036 RVA: 0x0003CE34 File Offset: 0x0003B034
		public void Set(Rigidbody rb, ulong[] buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			this.Read(CompressedMatrix.reusable, buffer, ref bitposition, bcl);
			this.Set(rb, CompressedMatrix.reusable);
		}

		// Token: 0x06000BDD RID: 3037 RVA: 0x0003CE51 File Offset: 0x0003B051
		public void Set(Rigidbody rb, byte[] buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			this.Read(CompressedMatrix.reusable, buffer, ref bitposition, bcl);
			this.Set(rb, CompressedMatrix.reusable);
		}

		// Token: 0x06000BDE RID: 3038 RVA: 0x0003CE70 File Offset: 0x0003B070
		public void Set(Rigidbody2D rb2d, Matrix matrix)
		{
			if (this.cached_pBits[0] > 0)
			{
				this.posCrusher.Set(rb2d, matrix.position, IncludedAxes.XYZ);
			}
			if (this.cached_rBits[0] > 0)
			{
				this.rotCrusher.Set(rb2d, matrix.rotation, IncludedAxes.XYZ);
			}
			if (this.cached_sBits[0] > 0)
			{
				this.sclCrusher.Apply(rb2d.transform, matrix.scale, IncludedAxes.XYZ);
			}
		}

		// Token: 0x06000BDF RID: 3039 RVA: 0x0003CEE8 File Offset: 0x0003B0E8
		public void Set(Rigidbody2D rb2d, CompressedMatrix cmatrix)
		{
			if (this.cached_pBits[0] > 0)
			{
				this.posCrusher.Set(rb2d, cmatrix.cPos, IncludedAxes.XYZ);
			}
			if (this.cached_rBits[0] > 0)
			{
				this.rotCrusher.Set(rb2d, cmatrix.cRot, IncludedAxes.XYZ);
			}
			if (this.cached_sBits[0] > 0)
			{
				this.sclCrusher.Apply(rb2d.transform, cmatrix.cScl, IncludedAxes.XYZ);
			}
		}

		// Token: 0x06000BE0 RID: 3040 RVA: 0x0003CF54 File Offset: 0x0003B154
		public void Move(Rigidbody rb, CompressedMatrix cmatrix)
		{
			this.Move(rb, cmatrix.Decompress());
			if (this.cached_pBits[0] > 0)
			{
				this.posCrusher.Move(rb, cmatrix.cPos, IncludedAxes.XYZ);
			}
			if (this.cached_rBits[0] > 0)
			{
				this.rotCrusher.Move(rb, cmatrix.cRot, IncludedAxes.XYZ);
			}
			if (this.cached_sBits[0] > 0)
			{
				this.sclCrusher.Apply(rb.transform, cmatrix.cScl, IncludedAxes.XYZ);
			}
		}

		// Token: 0x06000BE1 RID: 3041 RVA: 0x0003CFD0 File Offset: 0x0003B1D0
		public void Move(Rigidbody rb, Matrix matrix)
		{
			if (this.cached_pBits[0] > 0)
			{
				this.posCrusher.Move(rb, matrix.position, IncludedAxes.XYZ);
			}
			if (this.cached_rBits[0] > 0)
			{
				this.rotCrusher.Move(rb, matrix.rotation, IncludedAxes.XYZ);
			}
			if (this.cached_sBits[0] > 0)
			{
				this.sclCrusher.Apply(rb.transform, matrix.scale, IncludedAxes.XYZ);
			}
		}

		// Token: 0x06000BE2 RID: 3042 RVA: 0x0003D046 File Offset: 0x0003B246
		public void Move(Rigidbody rb, ulong frag0, ulong frag1 = 0UL, ulong frag2 = 0UL, ulong frag3 = 0UL, ulong frag4 = 0UL)
		{
			this.Read(CompressedMatrix.reusable, frag0, frag1, frag2, frag3, frag4, 0);
			this.Move(rb, CompressedMatrix.reusable);
		}

		// Token: 0x06000BE3 RID: 3043 RVA: 0x0003D068 File Offset: 0x0003B268
		public void Move(Rigidbody rb, ulong[] buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			this.Read(CompressedMatrix.reusable, buffer, ref bitposition, bcl);
			this.Move(rb, CompressedMatrix.reusable);
		}

		// Token: 0x06000BE4 RID: 3044 RVA: 0x0003D085 File Offset: 0x0003B285
		public void Move(Rigidbody rb, byte[] buffer, ref int bitposition, BitCullingLevel bcl = 0)
		{
			this.Read(CompressedMatrix.reusable, buffer, ref bitposition, bcl);
			this.Move(rb, CompressedMatrix.reusable);
		}

		// Token: 0x06000BE5 RID: 3045 RVA: 0x0003D0A4 File Offset: 0x0003B2A4
		[Obsolete("Apply for Rigidbody has been replaced with Move and Set, to indicate usage of MovePosition/Rotation vs rb.position/rotation.")]
		public void Apply(Rigidbody rb, CompressedMatrix cmatrix)
		{
			if (this.cached_pBits[0] > 0)
			{
				this.posCrusher.Apply(rb, cmatrix.cPos, IncludedAxes.XYZ);
			}
			if (this.cached_rBits[0] > 0)
			{
				this.rotCrusher.Apply(rb, cmatrix.cRot, IncludedAxes.XYZ);
			}
			if (this.cached_sBits[0] > 0)
			{
				this.sclCrusher.Apply(rb.transform, cmatrix.cScl, IncludedAxes.XYZ);
			}
		}

		// Token: 0x06000BE6 RID: 3046 RVA: 0x0003D110 File Offset: 0x0003B310
		[Obsolete("Apply for Rigidbody has been replaced with Move and Set, to indicate usage of MovePosition/Rotation vs rb.position/rotation.")]
		public void Apply(Rigidbody rb, Matrix matrix)
		{
			if (this.cached_pBits[0] > 0)
			{
				this.posCrusher.Apply(rb, matrix.position, IncludedAxes.XYZ);
			}
			if (this.cached_rBits[0] > 0)
			{
				this.rotCrusher.Apply(rb, matrix.rotation, IncludedAxes.XYZ);
			}
			if (this.cached_sBits[0] > 0)
			{
				this.sclCrusher.Apply(rb.transform, matrix.scale, IncludedAxes.XYZ);
			}
		}

		// Token: 0x06000BE7 RID: 3047 RVA: 0x0003D186 File Offset: 0x0003B386
		[Obsolete("Supply the transform to compress. Default Transform has be deprecated.")]
		public void Apply(ulong cvalue)
		{
			global::Debug.Assert(this.defaultTransform, "The 'defaultTransform' is null and has not be set in the inspector. For non-editor usages of TransformCrusher you need to pass the target transform to this method.");
			this.Apply(this.defaultTransform, cvalue);
		}

		// Token: 0x06000BE8 RID: 3048 RVA: 0x0003D1A5 File Offset: 0x0003B3A5
		public void Apply(Transform t, ulong cvalue)
		{
			this.Decompress(Matrix.reusable, cvalue, 0);
			this.Apply(t, Matrix.reusable);
		}

		// Token: 0x06000BE9 RID: 3049 RVA: 0x0003D1C0 File Offset: 0x0003B3C0
		[Obsolete("Supply the transform to compress. Default Transform has be deprecated.")]
		public void Apply(ulong u0, ulong u1, ulong u2, ulong u3, uint u4)
		{
			global::Debug.Assert(this.defaultTransform, "The 'defaultTransform' is null and has not be set in the inspector. For non-editor usages of TransformCrusher you need to pass the target transform to this method.");
			this.Apply(this.defaultTransform, u0, u1, u2, u3, (ulong)u4);
		}

		// Token: 0x06000BEA RID: 3050 RVA: 0x0003D1E6 File Offset: 0x0003B3E6
		public void Apply(Transform t, ulong frag0, ulong frag1 = 0UL, ulong frag2 = 0UL, ulong frag3 = 0UL, ulong frag4 = 0UL)
		{
			this.Read(CompressedMatrix.reusable, frag0, frag1, frag2, frag3, frag4, 0);
			this.Apply(t, CompressedMatrix.reusable);
		}

		// Token: 0x06000BEB RID: 3051 RVA: 0x0003D208 File Offset: 0x0003B408
		[Obsolete("Supply the transform to Apply to. Default Transform has be deprecated.")]
		public void Apply(CompressedMatrix cmatrix)
		{
			global::Debug.Assert(this.defaultTransform, "The 'defaultTransform' is null and has not be set in the inspector. For non-editor usages of TransformCrusher you need to pass the target transform to this method.");
			this.Apply(this.defaultTransform, cmatrix);
		}

		// Token: 0x06000BEC RID: 3052 RVA: 0x0003D228 File Offset: 0x0003B428
		public void Apply(Transform t, CompressedMatrix cmatrix)
		{
			if (this.cached_pBits[0] > 0)
			{
				this.posCrusher.Apply(t, cmatrix.cPos, IncludedAxes.XYZ);
			}
			if (this.cached_rBits[0] > 0)
			{
				this.rotCrusher.Apply(t, cmatrix.cRot, IncludedAxes.XYZ);
			}
			if (this.cached_sBits[0] > 0)
			{
				this.sclCrusher.Apply(t, cmatrix.cScl, IncludedAxes.XYZ);
			}
		}

		// Token: 0x06000BED RID: 3053 RVA: 0x0003D28F File Offset: 0x0003B48F
		[Obsolete("Supply the transform to Apply to. Default Transform has be deprecated.")]
		public void Apply(Matrix matrix)
		{
			global::Debug.Assert(this.defaultTransform, "The 'defaultTransform' is null and has not be set in the inspector. For non-editor usages of TransformCrusher you need to pass the target transform to this method.");
			this.Apply(this.defaultTransform, matrix);
		}

		// Token: 0x06000BEE RID: 3054 RVA: 0x0003D2B0 File Offset: 0x0003B4B0
		public void Apply(Transform transform, Matrix matrix)
		{
			if (this.cached_pBits[0] > 0)
			{
				this.posCrusher.Apply(transform, matrix.position, IncludedAxes.XYZ);
			}
			if (this.cached_rBits[0] > 0)
			{
				this.rotCrusher.Apply(transform, matrix.rotation, IncludedAxes.XYZ);
			}
			if (this.cached_sBits[0] > 0)
			{
				this.sclCrusher.Apply(transform, matrix.scale, IncludedAxes.XYZ);
			}
		}

		// Token: 0x06000BEF RID: 3055 RVA: 0x0003D321 File Offset: 0x0003B521
		public void Capture(Rigidbody rb, CompressedMatrix cm, Matrix m)
		{
			this.Compress(cm, rb);
			this.Decompress(m, cm);
		}

		// Token: 0x06000BF0 RID: 3056 RVA: 0x0003D333 File Offset: 0x0003B533
		public void Capture(Rigidbody2D rb2d, CompressedMatrix cm, Matrix m)
		{
			this.Compress(cm, rb2d);
			this.Decompress(m, cm);
		}

		// Token: 0x06000BF1 RID: 3057 RVA: 0x0003D348 File Offset: 0x0003B548
		public void Capture(Transform tr, CompressedMatrix cm, Matrix m)
		{
			this.posCrusher.Compress(cm.cPos, tr);
			m.position = (Vector3)this.posCrusher.Decompress(cm.cPos);
			this.rotCrusher.Compress(cm.cRot, tr);
			m.rotation = this.rotCrusher.Decompress(cm.cRot);
			this.sclCrusher.Compress(cm.cScl, tr);
			m.scale = (Vector3)this.sclCrusher.Decompress(cm.cScl);
		}

		// Token: 0x06000BF2 RID: 3058 RVA: 0x0003D3DC File Offset: 0x0003B5DC
		public int TallyBits(BitCullingLevel bcl = 0)
		{
			int num = (this.posCrusher != null) ? this.posCrusher.TallyBits(bcl) : 0;
			int num2 = (this.posCrusher != null) ? this.rotCrusher.TallyBits(bcl) : 0;
			int num3 = (this.posCrusher != null) ? this.sclCrusher.TallyBits(bcl) : 0;
			return num + num2 + num3;
		}

		// Token: 0x06000BF3 RID: 3059 RVA: 0x0003D446 File Offset: 0x0003B646
		public void CopyFrom(TransformCrusher source)
		{
			this.posCrusher.CopyFrom(source.posCrusher);
			this.rotCrusher.CopyFrom(source.rotCrusher);
			this.sclCrusher.CopyFrom(source.sclCrusher);
			this.CacheValues();
		}

		// Token: 0x06000BF4 RID: 3060 RVA: 0x0003D481 File Offset: 0x0003B681
		public override bool Equals(object obj)
		{
			return this.Equals(obj as TransformCrusher);
		}

		// Token: 0x06000BF5 RID: 3061 RVA: 0x0003D490 File Offset: 0x0003B690
		public bool Equals(TransformCrusher other)
		{
			if (!(other != null) || !((this.posCrusher == null) ? (other.posCrusher == null) : this.posCrusher.Equals(other.posCrusher)) || !((this.rotCrusher == null) ? (other.rotCrusher == null) : this.rotCrusher.Equals(other.rotCrusher)))
			{
				return false;
			}
			if (!(this.sclCrusher == null))
			{
				return this.sclCrusher.Equals(other.sclCrusher);
			}
			return other.sclCrusher == null;
		}

		// Token: 0x06000BF6 RID: 3062 RVA: 0x0003D538 File Offset: 0x0003B738
		public override int GetHashCode()
		{
			return ((-453726296 * -1521134295 + ((this.posCrusher == null) ? 0 : this.posCrusher.GetHashCode())) * -1521134295 + ((this.rotCrusher == null) ? 0 : this.rotCrusher.GetHashCode())) * -1521134295 + ((this.sclCrusher == null) ? 0 : this.sclCrusher.GetHashCode());
		}

		// Token: 0x06000BF7 RID: 3063 RVA: 0x0003D5B3 File Offset: 0x0003B7B3
		public static bool operator ==(TransformCrusher crusher1, TransformCrusher crusher2)
		{
			return EqualityComparer<TransformCrusher>.Default.Equals(crusher1, crusher2);
		}

		// Token: 0x06000BF8 RID: 3064 RVA: 0x0003D5C1 File Offset: 0x0003B7C1
		public static bool operator !=(TransformCrusher crusher1, TransformCrusher crusher2)
		{
			return !(crusher1 == crusher2);
		}

		// Token: 0x04000C15 RID: 3093
		public const int VersionMajor = 3;

		// Token: 0x04000C16 RID: 3094
		public const int VersionMinor = 5;

		// Token: 0x04000C17 RID: 3095
		public const int VersionRevision = 3;

		// Token: 0x04000C18 RID: 3096
		public const int Build = 3503;

		// Token: 0x04000C19 RID: 3097
		public static Dictionary<int, TransformCrusher> staticTransformCrushers = new Dictionary<int, TransformCrusher>();

		// Token: 0x04000C1A RID: 3098
		[HideInInspector]
		[Obsolete("Default Transform breaks crusher sharing across multiple instances and is now deprecated.")]
		[Tooltip("This is the default assumed transform when no transform or gameobject is given to methods.")]
		public Transform defaultTransform;

		// Token: 0x04000C1B RID: 3099
		[SerializeField]
		protected ElementCrusher posCrusher;

		// Token: 0x04000C1C RID: 3100
		[SerializeField]
		protected ElementCrusher rotCrusher;

		// Token: 0x04000C1D RID: 3101
		[SerializeField]
		protected ElementCrusher sclCrusher;

		// Token: 0x04000C1E RID: 3102
		[NonSerialized]
		protected readonly int[] cached_pBits = new int[4];

		// Token: 0x04000C1F RID: 3103
		[NonSerialized]
		protected readonly int[] cached_rBits = new int[4];

		// Token: 0x04000C20 RID: 3104
		[NonSerialized]
		protected readonly int[] cached_sBits = new int[4];

		// Token: 0x04000C21 RID: 3105
		[NonSerialized]
		protected readonly int[] _cached_total = new int[4];

		// Token: 0x04000C22 RID: 3106
		public ReadOnlyCollection<int> cached_total;

		// Token: 0x04000C23 RID: 3107
		protected bool cached;

		// Token: 0x04000C24 RID: 3108
		public static ulong[] reusableArray64 = new ulong[5];

		// Token: 0x04000C25 RID: 3109
		private const string transformMissingError = "The 'defaultTransform' is null and has not be set in the inspector. For non-editor usages of TransformCrusher you need to pass the target transform to this method.";
	}
}
