using System;

namespace Photon.Compression
{
	// Token: 0x02000216 RID: 534
	public static class MatrixExtensions
	{
		// Token: 0x06000B8E RID: 2958 RVA: 0x0003B831 File Offset: 0x00039A31
		public static void CopyFrom(this Matrix target, Matrix src)
		{
			target.crusher = src.crusher;
			target.position = src.position;
			target.rotation = src.rotation;
			target.scale = src.scale;
		}
	}
}
