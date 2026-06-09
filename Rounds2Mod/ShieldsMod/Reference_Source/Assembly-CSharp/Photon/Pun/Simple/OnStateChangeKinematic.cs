using System;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x020002A7 RID: 679
	[DisallowMultipleComponent]
	public class OnStateChangeKinematic : NetComponent, IOnStateChange, IApplyOrder, IAutoKinematic
	{
		// Token: 0x170000EF RID: 239
		// (get) Token: 0x06000EDD RID: 3805 RVA: 0x00044FCC File Offset: 0x000431CC
		public int ApplyOrder
		{
			get
			{
				return 11;
			}
		}

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x06000EDE RID: 3806 RVA: 0x000422B1 File Offset: 0x000404B1
		public bool AutoKinematicEnabled
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06000EDF RID: 3807 RVA: 0x00047E9C File Offset: 0x0004609C
		public override void OnAwake()
		{
			base.OnAwake();
			this.rb = this.netObj.Rb;
			this.rb2d = this.netObj.Rb2D;
			if (this.rb)
			{
				this.kinematicDefault = this.rb.isKinematic;
				this.interpolateDefault = this.rb.interpolation;
			}
			else if (this.rb2d)
			{
				this.kinematicDefault = this.rb2d.isKinematic;
				this.interpolateDefault = this.rb2d.interpolation;
			}
			if (this.autoDestroy && !this.rb && !this.rb2d)
			{
				Object.Destroy(this);
			}
			this.SetOwnedKinematics(this.currentState);
		}

		// Token: 0x06000EE0 RID: 3808 RVA: 0x00047F65 File Offset: 0x00046165
		public override void OnAuthorityChanged(bool isMine, bool controllerChanged)
		{
			base.OnAuthorityChanged(isMine, controllerChanged);
			if (isMine)
			{
				this.SetOwnedKinematics(this.currentState);
				return;
			}
			this.SetUnownedKinematics();
		}

		// Token: 0x06000EE1 RID: 3809 RVA: 0x00047F85 File Offset: 0x00046185
		public void OnStateChange(ObjState newState, ObjState previousState, Transform attachmentTransform, Mount attachTo = null, bool isReady = true)
		{
			if (base.IsMine)
			{
				this.SetOwnedKinematics(newState);
			}
		}

		// Token: 0x06000EE2 RID: 3810 RVA: 0x00047F98 File Offset: 0x00046198
		protected virtual void SetUnownedKinematics()
		{
			if (base.RigidbodyType == RigidbodyType.None)
			{
				return;
			}
			if (base.RigidbodyType == RigidbodyType.RB)
			{
				this.rb.isKinematic = true;
				this.rb.interpolation = 0;
				return;
			}
			this.rb2d.isKinematic = true;
			this.rb2d.interpolation = 0;
		}

		// Token: 0x06000EE3 RID: 3811 RVA: 0x00047FE8 File Offset: 0x000461E8
		protected virtual void SetOwnedKinematics(ObjState state)
		{
			if (base.RigidbodyType == RigidbodyType.None)
			{
				return;
			}
			KinematicSetting kinematicSetting;
			if (state == ObjState.Despawned && this.onDespawned != KinematicSetting.Ignore)
			{
				kinematicSetting = this.onDespawned;
			}
			else if ((state & ObjState.Anchored) != ObjState.Despawned && this.onAnchored != KinematicSetting.Ignore)
			{
				kinematicSetting = this.onAnchored;
			}
			else if ((state & ObjState.Mounted) != ObjState.Despawned && this.onMounted != KinematicSetting.Ignore)
			{
				kinematicSetting = this.onMounted;
			}
			else if ((state & ObjState.Transit) != ObjState.Despawned && this.onTransit != KinematicSetting.Ignore)
			{
				kinematicSetting = this.onTransit;
			}
			else if ((state & ObjState.Dropped) != ObjState.Despawned && this.onDropped != KinematicSetting.Ignore)
			{
				kinematicSetting = this.onDropped;
			}
			else if ((state & ObjState.Visible) != ObjState.Despawned && this.onVisible != KinematicSetting.Ignore)
			{
				kinematicSetting = this.onVisible;
			}
			else
			{
				kinematicSetting = KinematicSetting.Default;
			}
			bool flag = kinematicSetting != KinematicSetting.NonKinematic && (kinematicSetting == KinematicSetting.Kinematic || this.kinematicDefault);
			if (base.RigidbodyType == RigidbodyType.RB)
			{
				this.rb.position = base.transform.position;
				if (flag)
				{
					this.rb.collisionDetectionMode = 3;
					this.rb.isKinematic = true;
				}
				else
				{
					this.rb.isKinematic = false;
				}
				this.rb.interpolation = ((state != ObjState.Despawned && (state & ObjState.Mounted) == ObjState.Despawned) ? this.interpolateDefault : 0);
			}
			else
			{
				this.rb2d.position = base.transform.position;
				this.rb2d.isKinematic = flag;
				this.rb2d.simulated = !flag;
				this.rb2d.interpolation = ((state != ObjState.Despawned && (state & ObjState.Mounted) == ObjState.Despawned) ? this.interpolateDefault : 0);
			}
			this.currentState = state;
		}

		// Token: 0x04000DE3 RID: 3555
		public KinematicSetting onDespawned = KinematicSetting.Kinematic;

		// Token: 0x04000DE4 RID: 3556
		public KinematicSetting onAnchored = KinematicSetting.Kinematic;

		// Token: 0x04000DE5 RID: 3557
		public KinematicSetting onMounted = KinematicSetting.NonKinematic;

		// Token: 0x04000DE6 RID: 3558
		public KinematicSetting onTransit = KinematicSetting.NonKinematic;

		// Token: 0x04000DE7 RID: 3559
		public KinematicSetting onDropped = KinematicSetting.NonKinematic;

		// Token: 0x04000DE8 RID: 3560
		public KinematicSetting onVisible = KinematicSetting.Default;

		// Token: 0x04000DE9 RID: 3561
		[Tooltip("Destroy this component if no Rigidbodies exist on this GameObject.")]
		public bool autoDestroy = true;

		// Token: 0x04000DEA RID: 3562
		private ObjState currentState;

		// Token: 0x04000DEB RID: 3563
		private Rigidbody rb;

		// Token: 0x04000DEC RID: 3564
		private Rigidbody2D rb2d;

		// Token: 0x04000DED RID: 3565
		private bool kinematicDefault;

		// Token: 0x04000DEE RID: 3566
		private int interpolateDefault;
	}
}
