using System;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x02000258 RID: 600
	public class ContactManager : MonoBehaviour
	{
		// Token: 0x06000CFD RID: 3325 RVA: 0x00040F94 File Offset: 0x0003F194
		public void Awake()
		{
			NestedComponentUtilities.GetNestedComponentsInChildren<IContactSystem, NetObject>(base.transform, this.contactSystems, true);
			NestedComponentUtilities.GetNestedComponentsInChildren<IContactTrigger, NetObject>(base.transform, this.contactTriggers, true);
			int count = this.contactSystems.Count;
			if (count > 255)
			{
				throw new IndexOutOfRangeException("NetObjects may not have more than 255 IContactSystem components on them.");
			}
			byte b = 0;
			while ((int)b < count)
			{
				this.contactSystems[(int)b].SystemIndex = b;
				b += 1;
			}
		}

		// Token: 0x06000CFE RID: 3326 RVA: 0x00041005 File Offset: 0x0003F205
		public IContactSystem GetContacting(int index)
		{
			return this.contactSystems[index];
		}

		// Token: 0x04000CA4 RID: 3236
		public List<IContactSystem> contactSystems = new List<IContactSystem>(0);

		// Token: 0x04000CA5 RID: 3237
		public List<IContactTrigger> contactTriggers = new List<IContactTrigger>(0);
	}
}
