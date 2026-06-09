using System;

namespace Photon.Pun.Simple
{
	// Token: 0x02000292 RID: 658
	[Serializable]
	public struct MountSelector
	{
		// Token: 0x06000E4F RID: 3663 RVA: 0x00044BBE File Offset: 0x00042DBE
		public MountSelector(int index)
		{
			this.id = index;
		}

		// Token: 0x06000E50 RID: 3664 RVA: 0x00044BC7 File Offset: 0x00042DC7
		public static implicit operator int(MountSelector selector)
		{
			return selector.id;
		}

		// Token: 0x06000E51 RID: 3665 RVA: 0x00044BCF File Offset: 0x00042DCF
		public static implicit operator MountSelector(int id)
		{
			return new MountSelector(id);
		}

		// Token: 0x04000D6C RID: 3436
		public int id;
	}
}
