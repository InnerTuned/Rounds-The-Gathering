using System;

namespace Photon.Pun.Simple
{
	// Token: 0x0200026B RID: 619
	public abstract class InventoryContactReactors<T> : ContactReactorBase<IInventorySystem<T>>, IInventoryable<T>, IContactable
	{
		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x06000D72 RID: 3442
		public abstract T Size { get; }

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x06000D73 RID: 3443 RVA: 0x000422A9 File Offset: 0x000404A9
		public int Volume
		{
			get
			{
				return this.volume;
			}
		}

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x06000D74 RID: 3444 RVA: 0x000422B1 File Offset: 0x000404B1
		public override bool IsPickup
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06000D75 RID: 3445 RVA: 0x000422B4 File Offset: 0x000404B4
		protected override Consumption ProcessContactEvent(ContactEvent contactEvent)
		{
			IInventorySystem<T> inventorySystem = contactEvent.contactSystem as IInventorySystem<T>;
			if (inventorySystem == null)
			{
				return Consumption.None;
			}
			if (this.IsPickup)
			{
				Mount mount = inventorySystem.TryPickup(this, contactEvent);
				if (mount)
				{
					this.syncState.HardMount(mount);
				}
			}
			return Consumption.All;
		}

		// Token: 0x04000CD9 RID: 3289
		protected int volume;
	}
}
