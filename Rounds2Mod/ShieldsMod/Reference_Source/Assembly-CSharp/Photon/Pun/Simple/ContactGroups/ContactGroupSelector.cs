using System;

namespace Photon.Pun.Simple.ContactGroups
{
	// Token: 0x020002FE RID: 766
	[Serializable]
	public struct ContactGroupSelector : IContactGroupMask
	{
		// Token: 0x17000132 RID: 306
		// (get) Token: 0x06001072 RID: 4210 RVA: 0x0004F31A File Offset: 0x0004D51A
		public int Mask
		{
			get
			{
				if (this.index != 0)
				{
					return 1 << this.index - 1;
				}
				return 0;
			}
		}

		// Token: 0x04000F63 RID: 3939
		public int index;
	}
}
