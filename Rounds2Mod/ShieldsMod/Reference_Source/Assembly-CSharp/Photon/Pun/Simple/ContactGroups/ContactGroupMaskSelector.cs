using System;
using UnityEngine;

namespace Photon.Pun.Simple.ContactGroups
{
	// Token: 0x020002FD RID: 765
	[Serializable]
	public struct ContactGroupMaskSelector : IContactGroupMask
	{
		// Token: 0x17000131 RID: 305
		// (get) Token: 0x0600106D RID: 4205 RVA: 0x0004F301 File Offset: 0x0004D501
		// (set) Token: 0x0600106E RID: 4206 RVA: 0x0004F309 File Offset: 0x0004D509
		public int Mask
		{
			get
			{
				return this.mask;
			}
			set
			{
				this.mask = value;
			}
		}

		// Token: 0x0600106F RID: 4207 RVA: 0x0004F309 File Offset: 0x0004D509
		public ContactGroupMaskSelector(int mask)
		{
			this.mask = mask;
		}

		// Token: 0x06001070 RID: 4208 RVA: 0x0004F301 File Offset: 0x0004D501
		public static implicit operator int(ContactGroupMaskSelector selector)
		{
			return selector.mask;
		}

		// Token: 0x06001071 RID: 4209 RVA: 0x0004F312 File Offset: 0x0004D512
		public static implicit operator ContactGroupMaskSelector(int mask)
		{
			return new ContactGroupMaskSelector(mask);
		}

		// Token: 0x04000F62 RID: 3938
		[SerializeField]
		private int mask;
	}
}
