using System;
using UnityEngine;

namespace Photon.Utilities
{
	// Token: 0x02000245 RID: 581
	public class VersaMaskAttribute : PropertyAttribute
	{
		// Token: 0x06000C7E RID: 3198 RVA: 0x0003EFC6 File Offset: 0x0003D1C6
		public VersaMaskAttribute(bool definesZero = false, Type castTo = null)
		{
			this.definesZero = definesZero;
			this.castTo = castTo;
		}

		// Token: 0x04000C59 RID: 3161
		public bool definesZero;

		// Token: 0x04000C5A RID: 3162
		public Type castTo;
	}
}
