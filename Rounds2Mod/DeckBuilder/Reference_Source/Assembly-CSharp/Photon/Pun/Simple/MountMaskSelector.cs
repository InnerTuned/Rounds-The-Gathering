using System;

namespace Photon.Pun.Simple
{
	// Token: 0x02000291 RID: 657
	[Serializable]
	public struct MountMaskSelector
	{
		// Token: 0x06000E4B RID: 3659 RVA: 0x00044B92 File Offset: 0x00042D92
		public MountMaskSelector(int mountTypeMask)
		{
			this.mask = mountTypeMask;
		}

		// Token: 0x06000E4C RID: 3660 RVA: 0x00044B9B File Offset: 0x00042D9B
		public MountMaskSelector(bool allTrue)
		{
			this.mask = (allTrue ? MountSettings.AllTrueMask : 0);
		}

		// Token: 0x06000E4D RID: 3661 RVA: 0x00044BAE File Offset: 0x00042DAE
		public static implicit operator int(MountMaskSelector selector)
		{
			return selector.mask;
		}

		// Token: 0x06000E4E RID: 3662 RVA: 0x00044BB6 File Offset: 0x00042DB6
		public static implicit operator MountMaskSelector(int mask)
		{
			return new MountMaskSelector(mask);
		}

		// Token: 0x04000D6B RID: 3435
		public int mask;
	}
}
