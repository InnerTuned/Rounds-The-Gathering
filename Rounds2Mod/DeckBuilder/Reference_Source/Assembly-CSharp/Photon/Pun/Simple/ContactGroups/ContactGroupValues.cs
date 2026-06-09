using System;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Pun.Simple.ContactGroups
{
	// Token: 0x02000300 RID: 768
	[Serializable]
	public class ContactGroupValues
	{
		// Token: 0x06001077 RID: 4215 RVA: 0x0004F47C File Offset: 0x0004D67C
		public ContactGroupValues()
		{
			List<float> list = new List<float>();
			list.Add(1f);
			this.values = list;
			base..ctor();
		}

		// Token: 0x04000F69 RID: 3945
		[SerializeField]
		public List<float> values;
	}
}
