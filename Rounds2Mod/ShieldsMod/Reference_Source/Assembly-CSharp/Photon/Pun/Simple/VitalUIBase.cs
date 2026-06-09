using System;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x0200027F RID: 639
	public abstract class VitalUIBase : VitalsUISrcBase, IOnVitalValueChange, IOnVitalChange, IOnChangeOwnedVitals
	{
		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x06000DD6 RID: 3542 RVA: 0x00043625 File Offset: 0x00041825
		// (set) Token: 0x06000DD7 RID: 3543 RVA: 0x0004362D File Offset: 0x0004182D
		public Vitals Vitals
		{
			get
			{
				return this.vitals;
			}
			set
			{
				this.vitals = value;
				if (value == null)
				{
					this.Vital = null;
					return;
				}
				this.Vital = this.vitals.GetVital(this.targetVital);
			}
		}

		// Token: 0x06000DD8 RID: 3544 RVA: 0x00043658 File Offset: 0x00041858
		public override IVitalsSystem ApplyVitalsSource(Object vs)
		{
			IVitalsSystem vitalsSystem = base.ApplyVitalsSource(vs);
			this.Vitals = ((vitalsSystem != null) ? vitalsSystem.Vitals : null);
			this.vitalIndex = ((vitalsSystem == null) ? -1 : vitalsSystem.Vitals.GetVitalIndex(this.targetVital));
			return vitalsSystem;
		}

		// Token: 0x06000DD9 RID: 3545
		public abstract void Recalculate();

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x06000DDA RID: 3546 RVA: 0x0004369D File Offset: 0x0004189D
		// (set) Token: 0x06000DDB RID: 3547 RVA: 0x000436A5 File Offset: 0x000418A5
		public Vital Vital
		{
			get
			{
				return this.vital;
			}
			private set
			{
				if (this.vital != null)
				{
					this.vital.RemoveIOnVitalChange(this);
				}
				if (value != null)
				{
					value.AddIOnVitalChange(this);
				}
				this.vital = value;
				this.UpdateGraphics(this.vital);
			}
		}

		// Token: 0x06000DDC RID: 3548 RVA: 0x000436D8 File Offset: 0x000418D8
		protected virtual void Awake()
		{
			this.ApplyVitalsSource(this.vitalsSource);
			if (this.monitor == VitalsUISrcBase.MonitorSource.Auto || this.monitor == VitalsUISrcBase.MonitorSource.Owned)
			{
				OwnedIVitals.iOnChangeOwnedVitals.Add(this);
			}
		}

		// Token: 0x06000DDD RID: 3549 RVA: 0x00043703 File Offset: 0x00041903
		protected virtual void Start()
		{
			if (this.vital != null)
			{
				this.UpdateGraphics(this.vital);
			}
		}

		// Token: 0x06000DDE RID: 3550 RVA: 0x00043719 File Offset: 0x00041919
		protected virtual void OnDestroy()
		{
			if (this.monitor == VitalsUISrcBase.MonitorSource.Auto || this.monitor == VitalsUISrcBase.MonitorSource.Owned)
			{
				OwnedIVitals.iOnChangeOwnedVitals.Remove(this);
			}
			if (this.vital != null)
			{
				this.vital.RemoveIOnVitalChange(this);
			}
		}

		// Token: 0x06000DDF RID: 3551 RVA: 0x0004374C File Offset: 0x0004194C
		public override void OnChangeOwnedVitals(IVitalsSystem added, IVitalsSystem removed)
		{
			if (added != null)
			{
				this.vitalsSource = (added as Component);
				this.Vitals = added.Vitals;
				return;
			}
			if (removed.Vitals == this.vitals)
			{
				IVitalsSystem lastItem = OwnedIVitals.LastItem;
				if (lastItem != null)
				{
					this.Vitals = lastItem.Vitals;
				}
			}
		}

		// Token: 0x06000DE0 RID: 3552 RVA: 0x000027C8 File Offset: 0x000009C8
		public void OnVitalParamChange(Vital vital)
		{
		}

		// Token: 0x06000DE1 RID: 3553 RVA: 0x00043798 File Offset: 0x00041998
		public virtual void OnVitalValueChange(Vital vital)
		{
			this.UpdateGraphics(vital);
		}

		// Token: 0x06000DE2 RID: 3554
		public abstract void UpdateGraphics(Vital vital);

		// Token: 0x04000D21 RID: 3361
		[Tooltip("Which vital type will be monitored.")]
		[HideInInspector]
		public VitalNameType targetVital = new VitalNameType(VitalType.Health);

		// Token: 0x04000D22 RID: 3362
		[Tooltip("Which value to track. Typically this is value, but can be other VitalDefinition values like Max/Full.")]
		[HideInInspector]
		public VitalUIBase.TargetField targetField;

		// Token: 0x04000D23 RID: 3363
		[SerializeField]
		[HideInInspector]
		protected int vitalIndex = -1;

		// Token: 0x04000D24 RID: 3364
		[NonSerialized]
		protected Vital vital;

		// Token: 0x020003CC RID: 972
		public enum TargetField
		{
			// Token: 0x040012F0 RID: 4848
			Value,
			// Token: 0x040012F1 RID: 4849
			Max,
			// Token: 0x040012F2 RID: 4850
			MaxOverload
		}
	}
}
