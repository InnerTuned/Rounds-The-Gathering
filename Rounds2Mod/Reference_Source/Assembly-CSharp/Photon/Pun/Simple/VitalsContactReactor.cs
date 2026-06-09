using System;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x02000280 RID: 640
	public class VitalsContactReactor : ContactReactorBase, IOnContactEvent, IVitalsContactReactor, IContactReactor, IOnStateChange
	{
		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x06000DE4 RID: 3556 RVA: 0x0004104C File Offset: 0x0003F24C
		public virtual VitalNameType VitalNameType
		{
			get
			{
				return new VitalNameType(VitalType.None);
			}
		}

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x06000DE5 RID: 3557 RVA: 0x000437BC File Offset: 0x000419BC
		// (set) Token: 0x06000DE6 RID: 3558 RVA: 0x000437C4 File Offset: 0x000419C4
		public double DischargePerSec
		{
			get
			{
				return this.dischargePerSec;
			}
			internal set
			{
				this.valuePerFixed = value * (double)Time.fixedDeltaTime;
				this.dischargePerSec = value;
			}
		}

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x06000DE7 RID: 3559 RVA: 0x000437DB File Offset: 0x000419DB
		// (set) Token: 0x06000DE8 RID: 3560 RVA: 0x000437E3 File Offset: 0x000419E3
		public virtual bool Propagate
		{
			get
			{
				return this.propagate;
			}
			set
			{
				this.propagate = value;
			}
		}

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x06000DE9 RID: 3561 RVA: 0x000437EC File Offset: 0x000419EC
		// (set) Token: 0x06000DEA RID: 3562 RVA: 0x000437F4 File Offset: 0x000419F4
		public virtual bool AllowOverload
		{
			get
			{
				return this.allowOverload;
			}
			set
			{
				this.allowOverload = value;
			}
		}

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x06000DEB RID: 3563 RVA: 0x000437FD File Offset: 0x000419FD
		public override bool IsPickup
		{
			get
			{
				return this.isPickup;
			}
		}

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x06000DEC RID: 3564 RVA: 0x00043805 File Offset: 0x00041A05
		public virtual double Charges
		{
			get
			{
				return this._charges;
			}
		}

		// Token: 0x06000DED RID: 3565 RVA: 0x00043810 File Offset: 0x00041A10
		public virtual Consumption ConsumeCharges(double amount)
		{
			if (amount == 0.0)
			{
				return Consumption.None;
			}
			double num = this._charges - amount;
			double num2 = (this.initialCharges >= 0.0) ? Math.Max(num, 0.0) : Math.Min(num, 0.0);
			this._charges = num2;
			if (num2 == this.initialCharges)
			{
				return Consumption.None;
			}
			if (num2 != 0.0)
			{
				return Consumption.Partial;
			}
			return Consumption.All;
		}

		// Token: 0x06000DEE RID: 3566 RVA: 0x00043888 File Offset: 0x00041A88
		protected virtual void Consume(Consumption consumed)
		{
			if (consumed == Consumption.None)
			{
				return;
			}
			if (this.consumeDespawn != Consumption.None && this.syncState != null)
			{
				if (consumed == Consumption.All)
				{
					this.syncState.Despawn(false);
					return;
				}
				if (this.consumeDespawn == Consumption.Partial)
				{
					this.syncState.Despawn(false);
				}
			}
		}

		// Token: 0x06000DEF RID: 3567 RVA: 0x000438D5 File Offset: 0x00041AD5
		public override void OnAwakeInitialize(bool isNetObject)
		{
			base.OnAwakeInitialize(isNetObject);
			this.valuePerFixed = this.dischargePerSec * (double)Time.fixedDeltaTime;
		}

		// Token: 0x06000DF0 RID: 3568 RVA: 0x000438F4 File Offset: 0x00041AF4
		protected override Consumption ProcessContactEvent(ContactEvent contactEvent)
		{
			IVitalsSystem vitalsSystem = contactEvent.contactSystem as IVitalsSystem;
			if (vitalsSystem == null)
			{
				return Consumption.None;
			}
			double valueForTriggerType = this.GetValueForTriggerType(contactEvent.contactType);
			double num = vitalsSystem.Vitals.ApplyCharges(this.vitalNameType, valueForTriggerType, this.allowOverload, this.propagate);
			Consumption consumption;
			if (this.useCharges)
			{
				consumption = this.ConsumeCharges(num);
			}
			else
			{
				if (num == 0.0)
				{
					return Consumption.None;
				}
				if (num == valueForTriggerType)
				{
					consumption = Consumption.All;
				}
				else
				{
					consumption = Consumption.Partial;
				}
				this.Consume(consumption);
			}
			if (this.isPickup && consumption != Consumption.None)
			{
				Mount mount = vitalsSystem.TryPickup(this, contactEvent);
				if (mount)
				{
					this.syncState.HardMount(mount);
				}
			}
			return consumption;
		}

		// Token: 0x06000DF1 RID: 3569 RVA: 0x0004399D File Offset: 0x00041B9D
		public void OnStateChange(ObjState newState, ObjState previousState, Transform attachmentTransform, Mount attachTo = null, bool isReady = true)
		{
			if (previousState == ObjState.Despawned && (newState & ObjState.Visible) != ObjState.Despawned)
			{
				this._charges = this.initialCharges;
			}
		}

		// Token: 0x06000DF2 RID: 3570 RVA: 0x000439B4 File Offset: 0x00041BB4
		public double DischargeValue(ContactType contactType = ContactType.Undefined)
		{
			double num;
			switch (contactType)
			{
			case ContactType.Enter:
				num = this.dischargeOnEnter;
				goto IL_4C;
			case ContactType.Stay:
				num = this.dischargePerSec;
				goto IL_4C;
			case (ContactType)3:
				break;
			case ContactType.Exit:
				num = this.dischargeOnExit;
				goto IL_4C;
			default:
				if (contactType == ContactType.Hitscan)
				{
					num = this.dischargeOnScan;
					goto IL_4C;
				}
				break;
			}
			num = 0.0;
			IL_4C:
			if (!this.useCharges)
			{
				return num;
			}
			if (num >= 0.0)
			{
				return Math.Min(num, this._charges);
			}
			return Math.Max(num, this._charges);
		}

		// Token: 0x06000DF3 RID: 3571 RVA: 0x00043A3C File Offset: 0x00041C3C
		protected double GetValueForTriggerType(ContactType collideType)
		{
			switch (collideType)
			{
			case ContactType.Enter:
				return this.dischargeOnEnter;
			case ContactType.Stay:
				return this.valuePerFixed;
			case (ContactType)3:
				break;
			case ContactType.Exit:
				return this.dischargeOnExit;
			default:
				if (collideType == ContactType.Hitscan)
				{
					return this.dischargeOnScan;
				}
				break;
			}
			return 0.0;
		}

		// Token: 0x04000D25 RID: 3365
		[SerializeField]
		[HideInInspector]
		protected VitalNameType vitalNameType = new VitalNameType(VitalType.Health);

		// Token: 0x04000D26 RID: 3366
		[HideInInspector]
		public double dischargeOnEnter = 20.0;

		// Token: 0x04000D27 RID: 3367
		[HideInInspector]
		public double dischargeOnExit = 20.0;

		// Token: 0x04000D28 RID: 3368
		[HideInInspector]
		public double dischargeOnScan = 20.0;

		// Token: 0x04000D29 RID: 3369
		[SerializeField]
		[HideInInspector]
		protected double dischargePerSec = 20.0;

		// Token: 0x04000D2A RID: 3370
		[Tooltip("Unconsumed values (remainders) should be passed through to the next vital in the stack of vitals.")]
		public bool propagate;

		// Token: 0x04000D2B RID: 3371
		public bool allowOverload;

		// Token: 0x04000D2C RID: 3372
		[SerializeField]
		protected bool isPickup = true;

		// Token: 0x04000D2D RID: 3373
		public bool useCharges;

		// Token: 0x04000D2E RID: 3374
		public double _charges = 50.0;

		// Token: 0x04000D2F RID: 3375
		[Tooltip("The Charges value that will be set on initialization, and whenever this object respawns.")]
		[SerializeField]
		protected double initialCharges = 50.0;

		// Token: 0x04000D30 RID: 3376
		public Consumption consumeDespawn;

		// Token: 0x04000D31 RID: 3377
		protected double valuePerFixed;
	}
}
