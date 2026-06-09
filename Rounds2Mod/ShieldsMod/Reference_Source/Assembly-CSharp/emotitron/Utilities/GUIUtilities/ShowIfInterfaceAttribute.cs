using System;
using UnityEngine;

namespace emotitron.Utilities.GUIUtilities
{
	// Token: 0x020001ED RID: 493
	public class ShowIfInterfaceAttribute : PropertyAttribute
	{
		// Token: 0x060009B8 RID: 2488 RVA: 0x00031770 File Offset: 0x0002F970
		public ShowIfInterfaceAttribute(Type type, string tooltip)
		{
			this.type = type;
			this.tooltip = tooltip;
		}

		// Token: 0x060009B9 RID: 2489 RVA: 0x00031786 File Offset: 0x0002F986
		public ShowIfInterfaceAttribute(Type type, string tooltip, float min, float max)
		{
			this.type = type;
			this.tooltip = tooltip;
			this.min = min;
			this.max = max;
		}

		// Token: 0x04000B05 RID: 2821
		public readonly Type type;

		// Token: 0x04000B06 RID: 2822
		public readonly string tooltip;

		// Token: 0x04000B07 RID: 2823
		public readonly float min;

		// Token: 0x04000B08 RID: 2824
		public readonly float max;
	}
}
