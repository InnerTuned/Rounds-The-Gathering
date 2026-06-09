using System;
using System.Collections.Generic;
using Photon.Utilities;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x02000259 RID: 601
	public class ContactProjectile : MonoBehaviour, IProjectile, IOnPreSimulate, IOnPreUpdate
	{
		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000D00 RID: 3328 RVA: 0x00041033 File Offset: 0x0003F233
		// (set) Token: 0x06000D01 RID: 3329 RVA: 0x0004103B File Offset: 0x0003F23B
		public IProjectileCannon Owner
		{
			get
			{
				return this.owner;
			}
			set
			{
				this.owner = value;
			}
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000D02 RID: 3330 RVA: 0x00041044 File Offset: 0x0003F244
		public bool HasRigidbody
		{
			get
			{
				return this._hasRigidBody;
			}
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000D03 RID: 3331 RVA: 0x0004104C File Offset: 0x0003F24C
		public VitalNameType VitalNameType
		{
			get
			{
				return new VitalNameType(VitalType.None);
			}
		}

		// Token: 0x06000D04 RID: 3332 RVA: 0x00041054 File Offset: 0x0003F254
		private void Reset()
		{
			this.localContactTrigger = base.GetComponent<IContactTrigger>();
			if (this.localContactTrigger == null)
			{
				this.localContactTrigger = base.gameObject.AddComponent<ContactTrigger>();
			}
		}

		// Token: 0x06000D05 RID: 3333 RVA: 0x0004107C File Offset: 0x0003F27C
		private void Awake()
		{
			this.rb = base.GetComponentInParent<Rigidbody>();
			this.rb2d = base.GetComponentInParent<Rigidbody2D>();
			this._hasRigidBody = (this.rb || this.rb2d);
			this.useRbForces = (this.rb && !this.rb.isKinematic);
			this.useRb2dForces = (this.rb2d && !this.rb2d.isKinematic);
			this.useRBGravity = ((this.rb && this.rb.useGravity) || (this.rb2d && this.rb.useGravity));
			this.needsSnapshot = (!this._hasRigidBody || (this.rb && this.rb.isKinematic) || (this.rb2d && this.rb2d.isKinematic));
			this.localContactTrigger = base.GetComponent<IContactTrigger>();
			if (this.needsSnapshot)
			{
				NetMasterCallbacks.RegisterCallbackInterfaces(this, true, false);
			}
			if (this._hasRigidBody)
			{
				NetMasterCallbacks.onPreUpdates.Remove(this);
			}
			base.GetComponents<IOnNetworkHit>(this.onHit);
			base.GetComponents<IOnTerminate>(this.onTerminate);
		}

		// Token: 0x06000D06 RID: 3334 RVA: 0x0002A58A File Offset: 0x0002878A
		private void OnDestroy()
		{
			NetMasterCallbacks.RegisterCallbackInterfaces(this, false, true);
		}

		// Token: 0x06000D07 RID: 3335 RVA: 0x000411D8 File Offset: 0x0003F3D8
		public virtual void LagCompensate(float timeshift)
		{
			this.snapPos = base.transform.position + this.velocity * timeshift;
			if (this.rb && this.rb.useGravity)
			{
				this.velocity += Physics.gravity * timeshift;
				return;
			}
			if (this.rb2d)
			{
				this.velocity += Physics.gravity * timeshift;
			}
		}

		// Token: 0x06000D08 RID: 3336 RVA: 0x00041268 File Offset: 0x0003F468
		public void Initialize(IProjectileCannon owner, int frameId, int subFrameId, Vector3 localVelocity, RespondTo terminateOn, RespondTo damageOn, float timeshift = 0f)
		{
			this.owner = owner;
			this.velocity = base.transform.TransformDirection(localVelocity);
			this.terminateOn = terminateOn;
			this.damageOn = damageOn;
			this.frameId = frameId;
			this.subFrameId = subFrameId;
			if (timeshift != 0f)
			{
				this.LagCompensate(timeshift);
			}
			else
			{
				this.snapPos = base.transform.position;
			}
			if (this.useRbForces)
			{
				this.rb.MovePosition(this.snapPos);
				this.rb.velocity = this.velocity;
			}
			else if (this.rb2d)
			{
				this.rb2d.MovePosition(this.snapPos);
				this.rb2d.velocity = this.velocity;
			}
			else
			{
				base.transform.position = this.snapPos;
				this.targPos = this.snapPos + this.velocity * Time.fixedDeltaTime;
				base.transform.position = this.snapPos;
			}
			this.localContactTrigger.Proxy = owner.ContactTrigger;
		}

		// Token: 0x06000D09 RID: 3337 RVA: 0x0004138C File Offset: 0x0003F58C
		public void OnPreSimulate(int frameId, int subFrameId)
		{
			if (!this.useRbForces && !this.useRb2dForces)
			{
				this.SimulateTime(Time.fixedDeltaTime);
			}
		}

		// Token: 0x06000D0A RID: 3338 RVA: 0x000413AC File Offset: 0x0003F5AC
		public virtual void SimulateTime(float t)
		{
			Vector3 a = this.targPos;
			this.targPos = a + this.velocity * t;
			this.snapPos = a;
			if (this.useRb2dForces)
			{
				this.velocity += Physics.gravity * this.rb2d.gravityScale * t;
			}
			else
			{
				this.velocity += Physics.gravity * t;
			}
			this.Interpolate(0f);
		}

		// Token: 0x06000D0B RID: 3339 RVA: 0x0004143C File Offset: 0x0003F63C
		public virtual void OnPreUpdate()
		{
			if (!this.useRbForces && !this.useRb2dForces)
			{
				this.Interpolate(NetMaster.NormTimeSinceFixed);
			}
		}

		// Token: 0x06000D0C RID: 3340 RVA: 0x00041459 File Offset: 0x0003F659
		protected void Interpolate(float t)
		{
			base.transform.position = Vector3.Lerp(this.snapPos, this.targPos, t);
		}

		// Token: 0x06000D0D RID: 3341 RVA: 0x00041478 File Offset: 0x0003F678
		protected virtual void Terminate()
		{
			global::Debug.Log("Terminate");
			int i = 0;
			int count = this.onTerminate.Count;
			while (i < count)
			{
				this.onTerminate[i].OnTerminate();
				i++;
			}
			base.gameObject.SetActive(false);
		}

		// Token: 0x04000CA6 RID: 3238
		protected IProjectileCannon owner;

		// Token: 0x04000CA7 RID: 3239
		[NonSerialized]
		public Vector3 velocity;

		// Token: 0x04000CA8 RID: 3240
		[NonSerialized]
		public int frameId;

		// Token: 0x04000CA9 RID: 3241
		[NonSerialized]
		public int subFrameId;

		// Token: 0x04000CAA RID: 3242
		[SerializeField]
		[EnumMask(false, null)]
		protected RespondTo terminateOn = (RespondTo)14;

		// Token: 0x04000CAB RID: 3243
		[SerializeField]
		[EnumMask(false, null)]
		protected RespondTo damageOn = (RespondTo)14;

		// Token: 0x04000CAC RID: 3244
		protected Rigidbody rb;

		// Token: 0x04000CAD RID: 3245
		protected Rigidbody2D rb2d;

		// Token: 0x04000CAE RID: 3246
		protected bool _hasRigidBody;

		// Token: 0x04000CAF RID: 3247
		protected bool needsSnapshot;

		// Token: 0x04000CB0 RID: 3248
		protected IContactTrigger localContactTrigger;

		// Token: 0x04000CB1 RID: 3249
		protected bool useRbForces;

		// Token: 0x04000CB2 RID: 3250
		protected bool useRb2dForces;

		// Token: 0x04000CB3 RID: 3251
		protected bool useRBGravity;

		// Token: 0x04000CB4 RID: 3252
		public List<IOnNetworkHit> onHit = new List<IOnNetworkHit>();

		// Token: 0x04000CB5 RID: 3253
		public List<IOnTerminate> onTerminate = new List<IOnTerminate>();

		// Token: 0x04000CB6 RID: 3254
		private Vector3 snapPos;

		// Token: 0x04000CB7 RID: 3255
		private Vector3 targPos;

		// Token: 0x04000CB8 RID: 3256
		private Quaternion snapRot;

		// Token: 0x04000CB9 RID: 3257
		private Quaternion targRot;

		// Token: 0x04000CBA RID: 3258
		protected static List<NetObject> reusableNetObjects = new List<NetObject>();
	}
}
