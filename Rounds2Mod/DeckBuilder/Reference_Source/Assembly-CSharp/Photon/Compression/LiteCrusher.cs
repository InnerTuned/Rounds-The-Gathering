using System;
using UnityEngine;

namespace Photon.Compression
{
	// Token: 0x020001F1 RID: 497
	[Serializable]
	public abstract class LiteCrusher
	{
		// Token: 0x060009C4 RID: 2500 RVA: 0x00031F14 File Offset: 0x00030114
		public static int GetBitsForMaxValue(uint maxvalue)
		{
			for (int i = 0; i < 32; i++)
			{
				if (maxvalue >> i == 0U)
				{
					return i;
				}
			}
			return 32;
		}

		// Token: 0x04000B2D RID: 2861
		[SerializeField]
		protected int bits;
	}
}
