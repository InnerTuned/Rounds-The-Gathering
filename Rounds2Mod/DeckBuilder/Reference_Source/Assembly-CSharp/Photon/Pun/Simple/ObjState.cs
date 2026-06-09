using System;

namespace Photon.Pun.Simple
{
	// Token: 0x0200029E RID: 670
	public enum ObjState
	{
		// Token: 0x04000DC8 RID: 3528
		Despawned,
		// Token: 0x04000DC9 RID: 3529
		Visible,
		// Token: 0x04000DCA RID: 3530
		Mounted,
		// Token: 0x04000DCB RID: 3531
		AnchoredPosition = 4,
		// Token: 0x04000DCC RID: 3532
		AnchoredRotation = 8,
		// Token: 0x04000DCD RID: 3533
		Anchored = 12,
		// Token: 0x04000DCE RID: 3534
		Dropped = 16,
		// Token: 0x04000DCF RID: 3535
		Transit = 32
	}
}
