using System;

namespace Photon.Utilities
{
	// Token: 0x0200023E RID: 574
	public static class BitUtilities
	{
		// Token: 0x06000C6C RID: 3180 RVA: 0x0003EA58 File Offset: 0x0003CC58
		public static int GetBitsForMaxValue(this int maxvalue)
		{
			for (int i = 0; i < 32; i++)
			{
				if (maxvalue >> i == 0)
				{
					return i;
				}
			}
			return 32;
		}

		// Token: 0x06000C6D RID: 3181 RVA: 0x0003EA80 File Offset: 0x0003CC80
		public static int GetBitsForMaxValue(this uint maxvalue)
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
	}
}
