using System;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x020002A9 RID: 681
	public class OnStateChangeToggle : NetComponent, IOnStateChange
	{
		// Token: 0x06000EE5 RID: 3813 RVA: 0x00048194 File Offset: 0x00046394
		public override void OnAwake()
		{
			base.OnAwake();
			if (this.toggle == DisplayToggle.Renderer)
			{
				if (this._renderer == null)
				{
					this._renderer = base.GetComponent<Renderer>();
				}
			}
			else if (this.toggle == DisplayToggle.Component)
			{
				this.monob = (this.component as MonoBehaviour);
			}
			else if (this._gameObject == null)
			{
				this._gameObject = base.gameObject;
			}
			this.stateLogic.RecalculateMasks();
			this.reactToAttached = ((this.stateLogic.notMask & 2) == 0 && (this.stateLogic.stateMask & 2) != 0);
		}

		// Token: 0x06000EE6 RID: 3814 RVA: 0x00048238 File Offset: 0x00046438
		public void OnStateChange(ObjState newState, ObjState previousState, Transform pickup, Mount attachedTo = null, bool isReady = true)
		{
			if (!isReady)
			{
				this.show = false;
			}
			else if (this.stateLogic.Evaluate((int)newState))
			{
				this.show = true;
				if (this.reactToAttached && attachedTo == null && (newState & ObjState.Mounted) != ObjState.Despawned)
				{
					this.show = false;
				}
			}
			else
			{
				this.show = false;
			}
			this.DeferredEnable();
		}

		// Token: 0x06000EE7 RID: 3815 RVA: 0x00048294 File Offset: 0x00046494
		private void DeferredEnable()
		{
			switch (this.toggle)
			{
			case DisplayToggle.GameObject:
				this._gameObject.SetActive(this.show);
				return;
			case DisplayToggle.Component:
				if (this.monob)
				{
					this.monob.enabled = this.show;
					return;
				}
				break;
			case DisplayToggle.Renderer:
				if (this._renderer)
				{
					this._renderer.enabled = this.show;
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x04000DF3 RID: 3571
		[HideInInspector]
		[Tooltip("How this object should be toggled. GameObject toggles gameObject.SetActive(), Renderer toggles renderer.enabled, and Component toggles component.enabled.")]
		public DisplayToggle toggle;

		// Token: 0x04000DF4 RID: 3572
		[Tooltip("User specified component to toggle enabled.")]
		[HideInInspector]
		public Component component;

		// Token: 0x04000DF5 RID: 3573
		[HideInInspector]
		public GameObject _gameObject;

		// Token: 0x04000DF6 RID: 3574
		[HideInInspector]
		public Renderer _renderer;

		// Token: 0x04000DF7 RID: 3575
		[HideInInspector]
		public ObjStateLogic stateLogic = new ObjStateLogic();

		// Token: 0x04000DF8 RID: 3576
		private bool reactToAttached;

		// Token: 0x04000DF9 RID: 3577
		private MonoBehaviour monob;

		// Token: 0x04000DFA RID: 3578
		private bool show;
	}
}
