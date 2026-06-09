using System;
using System.Collections.Generic;
using Photon.Pun.Simple.Pooling;
using Photon.Utilities;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x02000271 RID: 625
	public class SyncCannon : SyncShootBase, IProjectileCannon
	{
		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x06000D8B RID: 3467 RVA: 0x00042504 File Offset: 0x00040704
		public override int ApplyOrder
		{
			get
			{
				return 19;
			}
		}

		// Token: 0x06000D8C RID: 3468 RVA: 0x00042508 File Offset: 0x00040708
		protected override void Reset()
		{
			base.Reset();
		}

		// Token: 0x06000D8D RID: 3469 RVA: 0x00042510 File Offset: 0x00040710
		public override void OnAwake()
		{
			base.OnAwake();
			if (this.projPrefab == null)
			{
				this.projPrefab = ProjectileHelpers.GetPlaceholderProj();
				Pool.AddPrefabToPool(this.projPrefab, 8, 8, null, true);
				return;
			}
			Pool.AddPrefabToPool(this.projPrefab, 8, 8, null, false);
		}

		// Token: 0x06000D8E RID: 3470 RVA: 0x00042550 File Offset: 0x00040750
		protected override bool Trigger(SyncShootBase.Frame frame, int subFrameId, float timeshift = 0f)
		{
			Pool pool = Pool.Spawn(this.projPrefab, this.origin, 5f);
			pool.GetComponent<IContactTrigger>().Proxy = this.contactTrigger;
			IProjectile component = pool.GetComponent<IProjectile>();
			component.Initialize(this, frame.frameId, subFrameId, this.velocity, this.terminateOn, this.damageOn, this.lagCompensate * timeshift);
			component.Owner = this;
			return true;
		}

		// Token: 0x04000CE6 RID: 3302
		[SerializeField]
		public GameObject projPrefab;

		// Token: 0x04000CE7 RID: 3303
		[SerializeField]
		public Vector3 velocity = new Vector3(0f, 0f, 10f);

		// Token: 0x04000CE8 RID: 3304
		[SerializeField]
		[EnumMask(false, null)]
		public RespondTo terminateOn = (RespondTo)12;

		// Token: 0x04000CE9 RID: 3305
		[SerializeField]
		[EnumMask(false, null)]
		public RespondTo damageOn = (RespondTo)12;

		// Token: 0x04000CEA RID: 3306
		[Tooltip("Projectiles are advanced (lagCompensate * RTT) ms into the future on non-owner clients. This will better time align projectiles to the local players time frame (For example dodging a projectile locally is more likely to be how the shooter saw events as well). 0 = Fully in shooters time frame and 1 = Fully in the local players time frame.")]
		[Range(0f, 1f)]
		[SerializeField]
		public float lagCompensate = 1f;

		// Token: 0x04000CEB RID: 3307
		protected static List<NetObject> reusableNetObjects = new List<NetObject>();
	}
}
