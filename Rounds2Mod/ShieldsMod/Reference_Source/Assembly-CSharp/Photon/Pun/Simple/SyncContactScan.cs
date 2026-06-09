using System;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x02000272 RID: 626
	public class SyncContactScan : SyncShootBase, IOnSnapshot, IOnAuthorityChanged
	{
		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x06000D91 RID: 3473 RVA: 0x00042601 File Offset: 0x00040801
		public override int ApplyOrder
		{
			get
			{
				return 17;
			}
		}

		// Token: 0x06000D92 RID: 3474 RVA: 0x00042608 File Offset: 0x00040808
		protected override bool Trigger(SyncShootBase.Frame frame, int subFrameId, float timeshift = 0f)
		{
			if (base.GetComponent<SyncContact>() && !this.photonView.IsMine)
			{
				this.hitscanDefinition.VisualizeHitscan(this.origin, 0.5f);
				return true;
			}
			int num = -1;
			RaycastHit[] array;
			Collider[] array2;
			int num2 = this.hitscanDefinition.GenericHitscanNonAlloc(this.origin, out array, out array2, ref num, this.visualizeHitscan, false);
			if (num2 <= 0)
			{
				return true;
			}
			for (int i = 0; i < num2; i++)
			{
				IContactTrigger nestedComponentInParents = NestedComponentUtilities.GetNestedComponentInParents<IContactTrigger, NetObject>(array2[i].transform);
				if (nestedComponentInParents != null && !(nestedComponentInParents.NetObj == this.contactTrigger.NetObj))
				{
					if (this.poke)
					{
						this.contactTrigger.OnContact(nestedComponentInParents, ContactType.Hitscan);
					}
					if (this.grab)
					{
						nestedComponentInParents.OnContact(this.contactTrigger, ContactType.Hitscan);
					}
				}
			}
			return true;
		}

		// Token: 0x04000CEC RID: 3308
		public bool poke = true;

		// Token: 0x04000CED RID: 3309
		public bool grab = true;

		// Token: 0x04000CEE RID: 3310
		public HitscanDefinition hitscanDefinition;

		// Token: 0x04000CEF RID: 3311
		[Tooltip("Render widgets that represent the shape of the hitscan when triggered.")]
		public bool visualizeHitscan = true;
	}
}
