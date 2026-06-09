using System;
using UnityEngine;

namespace AmplifyColor
{
	// Token: 0x02000312 RID: 786
	[Serializable]
	public struct RenderLayer
	{
		// Token: 0x060010B7 RID: 4279 RVA: 0x00050489 File Offset: 0x0004E689
		public RenderLayer(LayerMask mask, Color color)
		{
			this.mask = mask;
			this.color = color;
		}

		// Token: 0x04000FB5 RID: 4021
		public LayerMask mask;

		// Token: 0x04000FB6 RID: 4022
		public Color color;
	}
}
