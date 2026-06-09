using System;
using UnityEngine;

namespace Photon.Pun.Simple.ContactGroups
{
	// Token: 0x020002FC RID: 764
	public class ContactGroupAssign : MonoBehaviour, IContactGroupsAssign, IContactGroupMask
	{
		// Token: 0x1700012F RID: 303
		// (get) Token: 0x0600106A RID: 4202 RVA: 0x0004F2DD File Offset: 0x0004D4DD
		public bool ApplyToChildren
		{
			get
			{
				return this.applyToChildren;
			}
		}

		// Token: 0x17000130 RID: 304
		// (get) Token: 0x0600106B RID: 4203 RVA: 0x0004F2E5 File Offset: 0x0004D4E5
		public int Mask
		{
			get
			{
				return this.contactGroups.Mask;
			}
		}

		// Token: 0x04000F60 RID: 3936
		public ContactGroupMaskSelector contactGroups;

		// Token: 0x04000F61 RID: 3937
		[Tooltip("Will add a ContactGroupAssign to any children that have colliders and no ContactGroupAssign of their own. ")]
		[SerializeField]
		protected bool applyToChildren = true;
	}
}
