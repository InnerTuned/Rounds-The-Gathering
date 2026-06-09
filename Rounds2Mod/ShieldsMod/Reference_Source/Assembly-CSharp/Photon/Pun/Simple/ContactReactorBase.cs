using System;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x0200025A RID: 602
	public abstract class ContactReactorBase : NetComponent, IContactReactor
	{
		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000D10 RID: 3344 RVA: 0x000414FE File Offset: 0x0003F6FE
		public ContactType TriggerOn
		{
			get
			{
				return this.triggerOn;
			}
		}

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000D11 RID: 3345
		public abstract bool IsPickup { get; }

		// Token: 0x06000D12 RID: 3346 RVA: 0x00041508 File Offset: 0x0003F708
		public override void OnAwakeInitialize(bool isNetObject)
		{
			if (isNetObject)
			{
				this.syncState = NestedComponentUtilities.GetNestedComponentInParent<SyncState, NetObject>(base.transform);
				this.syncStateMountMask = (this.syncState ? this.syncState.mountableTo.mask : 0);
			}
			base.OnAwakeInitialize(isNetObject);
		}

		// Token: 0x06000D13 RID: 3347 RVA: 0x00041558 File Offset: 0x0003F758
		public virtual Consumption OnContactEvent(ContactEvent contactEvent)
		{
			ContactType contactType = contactEvent.contactType;
			if (this.triggerOn != ContactType.Undefined && (contactType & this.triggerOn) == ContactType.Undefined)
			{
				return Consumption.None;
			}
			if (this.IsPickup)
			{
				int validMountsMask = contactEvent.contactSystem.ValidMountsMask;
				int num = this.syncState.mountableTo;
				if (validMountsMask != 0 && (validMountsMask & num) == 0)
				{
					return Consumption.None;
				}
			}
			return this.ProcessContactEvent(contactEvent);
		}

		// Token: 0x06000D14 RID: 3348
		protected abstract Consumption ProcessContactEvent(ContactEvent contactEvent);

		// Token: 0x04000CBB RID: 3259
		[HideInInspector]
		public ContactType triggerOn = (ContactType)15;

		// Token: 0x04000CBC RID: 3260
		protected SyncState syncState;

		// Token: 0x04000CBD RID: 3261
		protected int syncStateMountMask;
	}
}
