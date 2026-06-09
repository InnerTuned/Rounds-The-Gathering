using System;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x0200027D RID: 637
	public abstract class VitalsUISrcBase : MonoBehaviour, IOnChangeOwnedVitals
	{
		// Token: 0x06000DC8 RID: 3528
		public abstract void OnChangeOwnedVitals(IVitalsSystem added, IVitalsSystem removed);

		// Token: 0x06000DC9 RID: 3529 RVA: 0x000430D8 File Offset: 0x000412D8
		protected virtual void Reset()
		{
			this.ApplyVitalsSource(null);
		}

		// Token: 0x06000DCA RID: 3530 RVA: 0x000430E4 File Offset: 0x000412E4
		public virtual IVitalsSystem ApplyVitalsSource(Object srcObj)
		{
			if (this.monitor == VitalsUISrcBase.MonitorSource.Auto)
			{
				if (srcObj == null)
				{
					srcObj = (Object)base.GetComponentInParent<IVitalsSystem>();
					this.monitor = VitalsUISrcBase.MonitorSource.Self;
				}
				if (srcObj == null)
				{
					srcObj = (Object)OwnedIVitals.LastItem;
					this.monitor = VitalsUISrcBase.MonitorSource.Owned;
				}
			}
			Component component;
			GameObject gameObject;
			if (this.monitor == VitalsUISrcBase.MonitorSource.Owned)
			{
				component = (OwnedIVitals.LastItem as Component);
				gameObject = null;
			}
			else if (this.monitor == VitalsUISrcBase.MonitorSource.Self)
			{
				gameObject = base.gameObject;
				component = null;
			}
			else
			{
				this.vitalsSource = srcObj;
				gameObject = (srcObj as GameObject);
				component = (srcObj as Component);
			}
			IVitalsSystem vitalsSystem;
			if (gameObject)
			{
				vitalsSystem = VitalsUISrcBase.FindIVitalComponentOnGameObj(gameObject);
				if (vitalsSystem != null)
				{
					this.vitalsSource = (vitalsSystem as Component).gameObject;
				}
			}
			else if (component)
			{
				vitalsSystem = (component as IVitalsSystem);
				if (this.monitor == VitalsUISrcBase.MonitorSource.GameObject)
				{
					this.vitalsSource = component.gameObject;
				}
			}
			else
			{
				vitalsSystem = null;
				this.vitalsSource = null;
			}
			return vitalsSystem;
		}

		// Token: 0x06000DCB RID: 3531 RVA: 0x000431C8 File Offset: 0x000413C8
		private static IVitalsSystem FindIVitalComponentOnGameObj(GameObject go)
		{
			if (go)
			{
				IVitalsSystem vitalsSystem = go.GetComponentInParent<IVitalsSystem>();
				if (vitalsSystem == null)
				{
					vitalsSystem = go.GetComponentInChildren<IVitalsSystem>();
				}
				return vitalsSystem;
			}
			return null;
		}

		// Token: 0x04000D11 RID: 3345
		[Tooltip("Where this VitalUI will look for Vitals data.")]
		[HideInInspector]
		public VitalsUISrcBase.MonitorSource monitor;

		// Token: 0x04000D12 RID: 3346
		[Tooltip("Object that this VitalUI will search for an IVitalsSystem vitals data source.")]
		[HideInInspector]
		[SerializeField]
		public Object vitalsSource;

		// Token: 0x04000D13 RID: 3347
		[NonSerialized]
		public Vitals vitals;

		// Token: 0x020003CB RID: 971
		public enum MonitorSource
		{
			// Token: 0x040012EB RID: 4843
			Auto,
			// Token: 0x040012EC RID: 4844
			Owned,
			// Token: 0x040012ED RID: 4845
			Self,
			// Token: 0x040012EE RID: 4846
			GameObject
		}
	}
}
