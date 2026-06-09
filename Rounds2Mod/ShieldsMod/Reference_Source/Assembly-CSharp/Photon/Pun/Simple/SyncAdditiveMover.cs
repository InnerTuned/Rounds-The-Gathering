using System;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x02000299 RID: 665
	public class SyncAdditiveMover : NetComponent, ITransformController, IOnPreUpdate, IOnPreSimulate
	{
		// Token: 0x170000DE RID: 222
		// (get) Token: 0x06000E9D RID: 3741 RVA: 0x000422B1 File Offset: 0x000404B1
		public bool HandlesInterpolation
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x06000E9E RID: 3742 RVA: 0x000422B1 File Offset: 0x000404B1
		public bool HandlesExtrapolation
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06000E9F RID: 3743 RVA: 0x000470A7 File Offset: 0x000452A7
		public void OnPreSimulate(int frameId, int subFrameId)
		{
			if (!base.isActiveAndEnabled || (this.photonView && !this.photonView.IsMine))
			{
				return;
			}
			this.AddVector();
		}

		// Token: 0x06000EA0 RID: 3744 RVA: 0x000470D2 File Offset: 0x000452D2
		public void OnPreUpdate()
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			this.AddVector();
		}

		// Token: 0x06000EA1 RID: 3745 RVA: 0x000470E4 File Offset: 0x000452E4
		private void AddVector()
		{
			float mixedDeltaTime = DoubleTime.mixedDeltaTime;
			base.transform.localScale += this.sclDef.addVector * mixedDeltaTime;
			if (this.rotDef.local)
			{
				base.transform.localEulerAngles += this.rotDef.addVector * mixedDeltaTime;
			}
			else
			{
				base.transform.eulerAngles += this.rotDef.addVector * mixedDeltaTime;
			}
			base.transform.Translate(this.posDef.addVector * mixedDeltaTime, this.posDef.local ? Space.Self : Space.World);
		}

		// Token: 0x04000DAE RID: 3502
		[HideInInspector]
		public SyncAdditiveMover.TRSDefinition posDef = new SyncAdditiveMover.TRSDefinition();

		// Token: 0x04000DAF RID: 3503
		[HideInInspector]
		public SyncAdditiveMover.TRSDefinition rotDef = new SyncAdditiveMover.TRSDefinition();

		// Token: 0x04000DB0 RID: 3504
		[HideInInspector]
		public SyncAdditiveMover.TRSDefinition sclDef = new SyncAdditiveMover.TRSDefinition();

		// Token: 0x020003CE RID: 974
		[Serializable]
		public class TRSDefinition : TRSDefinitionBase
		{
			// Token: 0x040012FA RID: 4858
			public Vector3 addVector = new Vector3(0f, 0f, 0f);
		}
	}
}
