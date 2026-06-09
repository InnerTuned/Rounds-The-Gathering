using System;
using Photon.Pun.Simple.ContactGroups;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x0200026C RID: 620
	public abstract class Inventory<T> : NetComponent, IInventorySystem<T>, IInventorySystem, IContactSystem
	{
		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x06000D77 RID: 3447 RVA: 0x00042300 File Offset: 0x00040500
		// (set) Token: 0x06000D78 RID: 3448 RVA: 0x00042308 File Offset: 0x00040508
		public Mount DefaultMount { get; set; }

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06000D79 RID: 3449 RVA: 0x00042311 File Offset: 0x00040511
		public IContactGroupMask ValidContactGroups
		{
			get
			{
				return this.contactGroups;
			}
		}

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06000D7A RID: 3450 RVA: 0x0004231E File Offset: 0x0004051E
		// (set) Token: 0x06000D7B RID: 3451 RVA: 0x00042326 File Offset: 0x00040526
		public byte SystemIndex { get; set; }

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x06000D7C RID: 3452 RVA: 0x0004232F File Offset: 0x0004052F
		public int ValidMountsMask
		{
			get
			{
				return this.defaultMountingMask;
			}
		}

		// Token: 0x06000D7D RID: 3453 RVA: 0x00042338 File Offset: 0x00040538
		public override void OnAwakeInitialize(bool isNetObject)
		{
			base.OnAwakeInitialize(isNetObject);
			NestedComponentUtilities.EnsureRootComponentExists<ContactManager, NetObject>(base.transform);
			this.mountsLookup = this.netObj.transform.GetComponent<MountsManager>();
			this.defaultMountingMask = 1 << this.defaultMounting.id;
		}

		// Token: 0x06000D7E RID: 3454 RVA: 0x00042384 File Offset: 0x00040584
		public override void OnStart()
		{
			base.OnStart();
			if (this.mountsLookup)
			{
				this.DefaultMount = this.mountsLookup.GetMount(this.defaultMounting);
			}
		}

		// Token: 0x06000D7F RID: 3455 RVA: 0x000423B8 File Offset: 0x000405B8
		public virtual Consumption TryTrigger(IContactReactor reactor, ContactEvent contactEvent, int compatibleMounts)
		{
			if (!(reactor is IInventoryable<T>))
			{
				return Consumption.None;
			}
			if (this.contactGroups != 0)
			{
				IContactGroupsAssign contactGroupsAssign = contactEvent.contactTrigger.ContactGroupsAssign;
				int num = (contactGroupsAssign == null) ? 0 : contactGroupsAssign.Mask;
				if ((this.contactGroups.Mask & num) == 0)
				{
					return Consumption.None;
				}
			}
			if (!this.TestCapacity(reactor as IInventoryable<T>))
			{
				return Consumption.None;
			}
			if (compatibleMounts == this.defaultMountingMask || (compatibleMounts & this.defaultMountingMask) != 0)
			{
				return Consumption.All;
			}
			return Consumption.None;
		}

		// Token: 0x06000D80 RID: 3456 RVA: 0x0004242C File Offset: 0x0004062C
		public virtual Mount TryPickup(IContactReactor reactor, ContactEvent contactEvent)
		{
			return this.DefaultMount;
		}

		// Token: 0x06000D81 RID: 3457 RVA: 0x000422B1 File Offset: 0x000404B1
		public virtual bool TestCapacity(IInventoryable<T> inventoryable)
		{
			return true;
		}

		// Token: 0x04000CDA RID: 3290
		[SerializeField]
		protected MountSelector defaultMounting = new MountSelector(0);

		// Token: 0x04000CDC RID: 3292
		[SerializeField]
		protected ContactGroupMaskSelector contactGroups;

		// Token: 0x04000CDE RID: 3294
		protected MountsManager mountsLookup;

		// Token: 0x04000CDF RID: 3295
		protected int defaultMountingMask;
	}
}
