using System;
using UnityEngine;

namespace emotitron.Utilities.GUIUtilities
{
	// Token: 0x020001EE RID: 494
	[AttributeUsage(256)]
	public class ValueTypeAttribute : PropertyAttribute
	{
		// Token: 0x060009BA RID: 2490 RVA: 0x000317AB File Offset: 0x0002F9AB
		public ValueTypeAttribute(string labeltag, float width = 48f)
		{
			this.labeltag = labeltag;
			this.width = width;
		}

		// Token: 0x04000B09 RID: 2825
		public string labeltag;

		// Token: 0x04000B0A RID: 2826
		public float width;
	}
}
