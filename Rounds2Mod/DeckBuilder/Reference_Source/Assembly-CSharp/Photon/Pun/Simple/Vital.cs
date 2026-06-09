using System;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x0200028C RID: 652
	[Serializable]
	public class Vital
	{
		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x06000E15 RID: 3605 RVA: 0x00043FD3 File Offset: 0x000421D3
		public VitalDefinition VitalDef
		{
			get
			{
				return this.vitalDef;
			}
		}

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x06000E16 RID: 3606 RVA: 0x00043FDB File Offset: 0x000421DB
		// (set) Token: 0x06000E17 RID: 3607 RVA: 0x00043FE3 File Offset: 0x000421E3
		public VitalData VitalData
		{
			get
			{
				return this.vitalData;
			}
			private set
			{
				this.vitalData = value;
			}
		}

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x06000E18 RID: 3608 RVA: 0x00043FEC File Offset: 0x000421EC
		// (set) Token: 0x06000E19 RID: 3609 RVA: 0x00043FFC File Offset: 0x000421FC
		public double Value
		{
			get
			{
				return this.vitalData.Value;
			}
			set
			{
				if (value == double.NegativeInfinity)
				{
					return;
				}
				double value2 = this.vitalData.Value;
				double num = Math.Max(Math.Min(value, this.vitalDef.MaxValue), 0.0);
				this.vitalData.Value = num;
				if (value2 != num)
				{
					for (int i = 0; i < this.OnValueChangeCallbacks.Count; i++)
					{
						this.OnValueChangeCallbacks[i].OnVitalValueChange(this);
					}
				}
			}
		}

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x06000E1A RID: 3610 RVA: 0x00044078 File Offset: 0x00042278
		// (set) Token: 0x06000E1B RID: 3611 RVA: 0x00044085 File Offset: 0x00042285
		public int TicksUntilDecay
		{
			get
			{
				return this.vitalData.ticksUntilDecay;
			}
			set
			{
				this.vitalData.ticksUntilDecay = value;
			}
		}

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x06000E1C RID: 3612 RVA: 0x00044093 File Offset: 0x00042293
		// (set) Token: 0x06000E1D RID: 3613 RVA: 0x000440A0 File Offset: 0x000422A0
		public int TicksUntilRegen
		{
			get
			{
				return this.vitalData.ticksUntilRegen;
			}
			set
			{
				this.vitalData.ticksUntilRegen = value;
			}
		}

		// Token: 0x06000E1E RID: 3614 RVA: 0x000440AE File Offset: 0x000422AE
		public Vital(VitalDefinition vitalDef)
		{
			this.vitalDef = vitalDef;
		}

		// Token: 0x06000E1F RID: 3615 RVA: 0x000440D5 File Offset: 0x000422D5
		public void Initialize(float tickDuration)
		{
			this.vitalDef.Initialize(tickDuration);
			this.ResetValues();
		}

		// Token: 0x06000E20 RID: 3616 RVA: 0x000440EC File Offset: 0x000422EC
		public void ResetValues()
		{
			this.vitalData = this.vitalDef.GetDefaultData();
			for (int i = 0; i < this.OnValueChangeCallbacks.Count; i++)
			{
				this.OnValueChangeCallbacks[i].OnVitalValueChange(this);
			}
		}

		// Token: 0x06000E21 RID: 3617 RVA: 0x00044134 File Offset: 0x00042334
		public void AddIOnVitalChange(IOnVitalChange cb)
		{
			IOnVitalValueChange onVitalValueChange = cb as IOnVitalValueChange;
			if (onVitalValueChange != null)
			{
				this.OnValueChangeCallbacks.Add(onVitalValueChange);
			}
			IOnVitalParamChange onVitalParamChange = cb as IOnVitalParamChange;
			if (onVitalParamChange != null)
			{
				this.OnParamChangeCallbacks.Add(onVitalParamChange);
			}
		}

		// Token: 0x06000E22 RID: 3618 RVA: 0x00044170 File Offset: 0x00042370
		public void RemoveIOnVitalChange(IOnVitalChange cb)
		{
			IOnVitalValueChange onVitalValueChange = cb as IOnVitalValueChange;
			if (onVitalValueChange != null)
			{
				this.OnValueChangeCallbacks.Remove(onVitalValueChange);
			}
			IOnVitalParamChange onVitalParamChange = cb as IOnVitalParamChange;
			if (onVitalParamChange != null)
			{
				this.OnParamChangeCallbacks.Remove(onVitalParamChange);
			}
		}

		// Token: 0x06000E23 RID: 3619 RVA: 0x000441AB File Offset: 0x000423AB
		public void Apply(VitalData vdata)
		{
			this.Value = vdata.Value;
			this.vitalData.ticksUntilDecay = vdata.ticksUntilDecay;
			this.vitalData.ticksUntilRegen = vdata.ticksUntilRegen;
		}

		// Token: 0x06000E24 RID: 3620 RVA: 0x000441DC File Offset: 0x000423DC
		public double ApplyCharges(double amt, bool allowOverload, bool ignoreAbsorbtion)
		{
			double num = ignoreAbsorbtion ? amt : (amt * this.vitalDef.Absorbtion);
			double value = this.Value;
			double num2 = value + num;
			if (!allowOverload)
			{
				num2 = Math.Min(num2, this.vitalDef.FullValue);
			}
			this.Value = num2;
			return this.Value - value;
		}

		// Token: 0x06000E25 RID: 3621 RVA: 0x0004422C File Offset: 0x0004242C
		public bool IsFull(bool allowOverload)
		{
			return this.Value >= (allowOverload ? this.vitalDef.MaxValue : this.vitalDef.FullValue);
		}

		// Token: 0x06000E26 RID: 3622 RVA: 0x00044254 File Offset: 0x00042454
		public double ApplyChange(IVitalsContactReactor iVitalsAffector, ContactEvent contectEvent)
		{
			double amount = iVitalsAffector.DischargeValue(contectEvent.contactType);
			return this.ApplyChange(amount, iVitalsAffector);
		}

		// Token: 0x06000E27 RID: 3623 RVA: 0x00044278 File Offset: 0x00042478
		public double ApplyChange(double amount, IVitalsContactReactor reactor = null)
		{
			double value = this.vitalData.Value;
			double num = this.vitalData.Value + amount;
			if (reactor != null && reactor.AllowOverload)
			{
				double maxValue = this.vitalDef.MaxValue;
				if (value >= maxValue)
				{
					return 0.0;
				}
				if (num > maxValue)
				{
					num = maxValue;
				}
			}
			else
			{
				double fullValue = this.vitalDef.FullValue;
				if (value >= fullValue)
				{
					return 0.0;
				}
				if (num > fullValue)
				{
					num = fullValue;
				}
			}
			this.Value = num;
			return num - value;
		}

		// Token: 0x06000E28 RID: 3624 RVA: 0x000442F6 File Offset: 0x000424F6
		public void DisruptRegen()
		{
			this.vitalData.ticksUntilRegen = this.vitalDef.RegenDelayInTicks;
		}

		// Token: 0x06000E29 RID: 3625 RVA: 0x0004430E File Offset: 0x0004250E
		public void DisruptDecay()
		{
			this.vitalData.ticksUntilDecay = this.vitalDef.DecayDelayInTicks;
		}

		// Token: 0x06000E2A RID: 3626 RVA: 0x00044328 File Offset: 0x00042528
		public double TestApplyChange(IVitalsContactReactor iVitalsAffector, ContactEvent contactEvent)
		{
			double charge = iVitalsAffector.DischargeValue(contactEvent.contactType);
			return this.TestApplyChange(charge, iVitalsAffector);
		}

		// Token: 0x06000E2B RID: 3627 RVA: 0x0004434C File Offset: 0x0004254C
		public double TestApplyChange(double charge, IVitalsContactReactor iVitalsAffector)
		{
			double value = this.vitalData.Value;
			double num = this.vitalData.Value + charge;
			if (iVitalsAffector != null && iVitalsAffector.AllowOverload)
			{
				double maxValue = this.vitalDef.MaxValue;
				if (value >= maxValue)
				{
					return 0.0;
				}
				if (num > maxValue)
				{
					num = maxValue;
				}
			}
			else
			{
				double maxValue2 = this.vitalDef.MaxValue;
				if (value >= maxValue2)
				{
					return 0.0;
				}
				if (num > maxValue2)
				{
					num = maxValue2;
				}
			}
			return num - value;
		}

		// Token: 0x06000E2C RID: 3628 RVA: 0x000443C4 File Offset: 0x000425C4
		public void Simulate()
		{
			if (this.vitalData.ticksUntilRegen > 0)
			{
				this.vitalData.ticksUntilRegen = this.vitalData.ticksUntilRegen - 1;
			}
			else if (this.vitalData.Value < this.vitalDef.FullValue)
			{
				this.Value = Math.Min(this.vitalData.Value + this.vitalDef.RegenPerTick, this.vitalDef.FullValue);
			}
			if (this.vitalData.ticksUntilDecay > 0)
			{
				this.vitalData.ticksUntilDecay = this.vitalData.ticksUntilDecay - 1;
				return;
			}
			if (this.vitalData.Value > this.vitalDef.FullValue)
			{
				this.Value = Math.Max(this.vitalData.Value - this.vitalDef.DecayPerTick, this.vitalDef.FullValue);
			}
		}

		// Token: 0x04000D55 RID: 3413
		[SerializeField]
		private VitalDefinition vitalDef;

		// Token: 0x04000D56 RID: 3414
		[NonSerialized]
		private VitalData vitalData;

		// Token: 0x04000D57 RID: 3415
		public List<IOnVitalValueChange> OnValueChangeCallbacks = new List<IOnVitalValueChange>(0);

		// Token: 0x04000D58 RID: 3416
		public List<IOnVitalParamChange> OnParamChangeCallbacks = new List<IOnVitalParamChange>(0);
	}
}
