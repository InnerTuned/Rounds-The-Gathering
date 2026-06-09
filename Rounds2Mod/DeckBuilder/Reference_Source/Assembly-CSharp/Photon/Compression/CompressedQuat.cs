using System;
using UnityEngine;

namespace Photon.Compression
{
	// Token: 0x02000210 RID: 528
	public struct CompressedQuat
	{
		// Token: 0x06000B57 RID: 2903 RVA: 0x0003A716 File Offset: 0x00038916
		public CompressedQuat(QuatCrusher crusher, ulong cvalue)
		{
			this = default(CompressedQuat);
			this.crusher = crusher;
			this.cvalue = cvalue;
		}

		// Token: 0x06000B58 RID: 2904 RVA: 0x0003A72D File Offset: 0x0003892D
		public CompressedQuat(QuatCrusher crusher, uint cvalue)
		{
			this = default(CompressedQuat);
			this.crusher = crusher;
			this.cvalue = (ulong)cvalue;
		}

		// Token: 0x06000B59 RID: 2905 RVA: 0x0003A72D File Offset: 0x0003892D
		public CompressedQuat(QuatCrusher crusher, ushort cvalue)
		{
			this = default(CompressedQuat);
			this.crusher = crusher;
			this.cvalue = (ulong)cvalue;
		}

		// Token: 0x06000B5A RID: 2906 RVA: 0x0003A72D File Offset: 0x0003892D
		public CompressedQuat(QuatCrusher crusher, byte cvalue)
		{
			this = default(CompressedQuat);
			this.crusher = crusher;
			this.cvalue = (ulong)cvalue;
		}

		// Token: 0x06000B5B RID: 2907 RVA: 0x0003A745 File Offset: 0x00038945
		public static implicit operator ulong(CompressedQuat cv)
		{
			return cv.cvalue;
		}

		// Token: 0x06000B5C RID: 2908 RVA: 0x0003A74D File Offset: 0x0003894D
		public static explicit operator uint(CompressedQuat cv)
		{
			return (uint)cv.cvalue;
		}

		// Token: 0x06000B5D RID: 2909 RVA: 0x0003A756 File Offset: 0x00038956
		public static explicit operator ushort(CompressedQuat cv)
		{
			return (ushort)cv.cvalue;
		}

		// Token: 0x06000B5E RID: 2910 RVA: 0x0003A75F File Offset: 0x0003895F
		public static explicit operator byte(CompressedQuat cv)
		{
			return (byte)cv.cvalue;
		}

		// Token: 0x06000B5F RID: 2911 RVA: 0x0003A768 File Offset: 0x00038968
		public Quaternion Decompress()
		{
			return this.crusher.Decompress(this.cvalue);
		}

		// Token: 0x06000B60 RID: 2912 RVA: 0x0003A77B File Offset: 0x0003897B
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"[CompressedQuat: ",
				this.cvalue,
				" bits: ",
				this.crusher,
				"] "
			});
		}

		// Token: 0x04000C01 RID: 3073
		public readonly QuatCrusher crusher;

		// Token: 0x04000C02 RID: 3074
		public readonly ulong cvalue;
	}
}
