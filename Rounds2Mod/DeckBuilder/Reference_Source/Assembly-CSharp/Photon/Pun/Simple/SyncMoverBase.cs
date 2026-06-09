using System;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x0200029B RID: 667
	[Serializable]
	public abstract class SyncMoverBase<TTRSDef, TFrame> : SyncObject<TFrame>, ITransformController, IOnPreSimulate, IOnPreUpdate where TTRSDef : TRSDefinitionBase, new() where TFrame : FrameBase, new()
	{
		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x06000EA4 RID: 3748 RVA: 0x000422B1 File Offset: 0x000404B1
		public virtual bool HandlesInterpolation
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x06000EA5 RID: 3749 RVA: 0x000422B1 File Offset: 0x000404B1
		public virtual bool HandlesExtrapolation
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06000EA6 RID: 3750 RVA: 0x000471E0 File Offset: 0x000453E0
		public override void OnAwakeInitialize(bool isNetObject)
		{
			if (!isNetObject)
			{
				NetMasterCallbacks.onPreSimulates.Add(this);
				NetMasterCallbacks.onPreUpdates.Add(this);
			}
			this.rb = base.GetComponent<Rigidbody>();
			this.rb2d = base.GetComponent<Rigidbody2D>();
			if ((this.rb && !this.rb.isKinematic) || (this.rb2d && !this.rb2d.isKinematic))
			{
				global::Debug.LogWarning(base.GetType().Name + " doesn't work with non-kinematic rigidbodies. Setting to kinematic.");
				if (this.rb)
				{
					this.rb.isKinematic = true;
				}
				else
				{
					this.rb2d.isKinematic = true;
				}
			}
			this.syncTransform = base.GetComponent<SyncTransform>();
			this.Recalculate();
		}

		// Token: 0x06000EA7 RID: 3751 RVA: 0x000472A5 File Offset: 0x000454A5
		public override void OnStartInitialize(bool isNetObject)
		{
			this.InitializeTRS(this.posDef, TRS.Position);
			this.InitializeTRS(this.rotDef, TRS.Rotation);
			this.InitializeTRS(this.sclDef, TRS.Scale);
		}

		// Token: 0x06000EA8 RID: 3752 RVA: 0x000472CE File Offset: 0x000454CE
		private void OnDestroy()
		{
			if (!this.netObj)
			{
				NetMasterCallbacks.onPreSimulates.Remove(this);
				NetMasterCallbacks.onPreUpdates.Remove(this);
			}
		}

		// Token: 0x06000EA9 RID: 3753 RVA: 0x000027C8 File Offset: 0x000009C8
		public virtual void Recalculate()
		{
		}

		// Token: 0x06000EAA RID: 3754
		protected abstract void InitializeTRS(TTRSDef def, TRS type);

		// Token: 0x06000EAB RID: 3755
		public abstract void OnPreSimulate(int frameId, int subFrameId);

		// Token: 0x06000EAC RID: 3756
		public abstract void OnPreUpdate();

		// Token: 0x04000DB2 RID: 3506
		[HideInInspector]
		public TTRSDef posDef = Activator.CreateInstance<TTRSDef>();

		// Token: 0x04000DB3 RID: 3507
		[HideInInspector]
		public TTRSDef rotDef = Activator.CreateInstance<TTRSDef>();

		// Token: 0x04000DB4 RID: 3508
		[HideInInspector]
		public TTRSDef sclDef = Activator.CreateInstance<TTRSDef>();

		// Token: 0x04000DB5 RID: 3509
		protected Rigidbody rb;

		// Token: 0x04000DB6 RID: 3510
		protected Rigidbody2D rb2d;

		// Token: 0x04000DB7 RID: 3511
		[NonSerialized]
		public SyncTransform syncTransform;

		// Token: 0x020003CF RID: 975
		public enum MovementRelation
		{
			// Token: 0x040012FC RID: 4860
			Absolute,
			// Token: 0x040012FD RID: 4861
			Relative
		}
	}
}
