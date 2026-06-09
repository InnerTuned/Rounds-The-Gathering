using System;

namespace UnityEngine.UI.ProceduralImage
{
	// Token: 0x020001E5 RID: 485
	public struct ProceduralImageInfo
	{
		// Token: 0x060009A6 RID: 2470 RVA: 0x00031530 File Offset: 0x0002F730
		public ProceduralImageInfo(float width, float height, float fallOffDistance, float pixelSize, Vector4 radius, float borderWidth)
		{
			this.width = Mathf.Abs(width);
			this.height = Mathf.Abs(height);
			this.fallOffDistance = Mathf.Max(0f, fallOffDistance);
			this.radius = radius;
			this.borderWidth = Mathf.Max(borderWidth, 0f);
			this.pixelSize = Mathf.Max(0f, pixelSize);
		}

		// Token: 0x04000AEC RID: 2796
		public float width;

		// Token: 0x04000AED RID: 2797
		public float height;

		// Token: 0x04000AEE RID: 2798
		public float fallOffDistance;

		// Token: 0x04000AEF RID: 2799
		public Vector4 radius;

		// Token: 0x04000AF0 RID: 2800
		public float borderWidth;

		// Token: 0x04000AF1 RID: 2801
		public float pixelSize;
	}
}
