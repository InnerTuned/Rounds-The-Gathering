using System;
using UnityEngine;

namespace Photon.Utilities
{
	// Token: 0x02000242 RID: 578
	public class BitsPerRangeAttribute : PropertyAttribute
	{
		// Token: 0x06000C77 RID: 3191 RVA: 0x0003EF62 File Offset: 0x0003D162
		public BitsPerRangeAttribute(int min, int max, bool show, bool zeroBase = false, string label = "Max:", bool showLabel = true, string tooltip = "")
		{
			this.show = show;
			this.min = min;
			this.max = max;
			this.label = label;
			this.showLabel = showLabel;
			this.tooltip = tooltip;
		}

		// Token: 0x04000C52 RID: 3154
		public readonly int max;

		// Token: 0x04000C53 RID: 3155
		public readonly int min;

		// Token: 0x04000C54 RID: 3156
		public readonly string label;

		// Token: 0x04000C55 RID: 3157
		public readonly bool showLabel;

		// Token: 0x04000C56 RID: 3158
		public readonly string tooltip;

		// Token: 0x04000C57 RID: 3159
		public bool show;
	}
}
