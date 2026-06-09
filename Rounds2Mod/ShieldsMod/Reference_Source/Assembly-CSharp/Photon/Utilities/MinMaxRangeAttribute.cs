using System;
using UnityEngine;

namespace Photon.Utilities
{
	// Token: 0x02000241 RID: 577
	public class MinMaxRangeAttribute : PropertyAttribute
	{
		// Token: 0x06000C76 RID: 3190 RVA: 0x0003EF4C File Offset: 0x0003D14C
		public MinMaxRangeAttribute(float min, float max)
		{
			this.min = min;
			this.max = max;
		}

		// Token: 0x04000C50 RID: 3152
		public readonly float max;

		// Token: 0x04000C51 RID: 3153
		public readonly float min;
	}
}
