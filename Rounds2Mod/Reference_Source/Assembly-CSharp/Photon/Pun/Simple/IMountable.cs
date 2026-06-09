using System;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x020002A0 RID: 672
	public interface IMountable
	{
		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x06000ECA RID: 3786
		Mount CurrentMount { get; }

		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x06000ECB RID: 3787
		bool IsDroppable { get; }

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x06000ECC RID: 3788
		bool IsThrowable { get; }

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x06000ECD RID: 3789
		Rigidbody Rb { get; }

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x06000ECE RID: 3790
		Rigidbody2D Rb2d { get; }

		// Token: 0x06000ECF RID: 3791
		void ImmediateUnmount();
	}
}
