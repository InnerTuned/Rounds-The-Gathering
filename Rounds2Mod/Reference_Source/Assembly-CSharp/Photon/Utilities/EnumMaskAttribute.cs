using System;
using UnityEngine;

namespace Photon.Utilities
{
	// Token: 0x0200024D RID: 589
	[AttributeUsage(256)]
	public class EnumMaskAttribute : PropertyAttribute
	{
		// Token: 0x06000CC2 RID: 3266 RVA: 0x0004019F File Offset: 0x0003E39F
		public EnumMaskAttribute(bool definesZero = false, Type castTo = null)
		{
			this.castTo = castTo;
			this.definesZero = definesZero;
		}

		// Token: 0x04000C6F RID: 3183
		public bool definesZero;

		// Token: 0x04000C70 RID: 3184
		public Type castTo;
	}
}
