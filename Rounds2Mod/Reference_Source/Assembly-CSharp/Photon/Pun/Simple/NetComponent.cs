using System;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x020002D4 RID: 724
	[HelpURL("https://doc.photonengine.com/en-us/pun/current/gameplay/simple/simpleoverview")]
	public abstract class NetComponent : MonoBehaviour, IOnJoinedRoom, IOnAwake, IOnStart, IOnEnable, IOnDisable, IOnAuthorityChanged
	{
		// Token: 0x17000104 RID: 260
		// (get) Token: 0x06000F6C RID: 3948 RVA: 0x0004B466 File Offset: 0x00049666
		public NetObject NetObj
		{
			get
			{
				return this.netObj;
			}
		}

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x06000F6D RID: 3949 RVA: 0x0004B46E File Offset: 0x0004966E
		// (set) Token: 0x06000F6E RID: 3950 RVA: 0x0004B476 File Offset: 0x00049676
		public RigidbodyType RigidbodyType { get; private set; }

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x06000F6F RID: 3951 RVA: 0x0004B47F File Offset: 0x0004967F
		public int ViewID
		{
			get
			{
				return this.photonView.ViewID;
			}
		}

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x06000F70 RID: 3952 RVA: 0x0004B48C File Offset: 0x0004968C
		public PhotonView PhotonView
		{
			get
			{
				return this.photonView;
			}
		}

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x06000F71 RID: 3953 RVA: 0x0004B494 File Offset: 0x00049694
		public bool IsMine
		{
			get
			{
				return this.photonView.IsMine;
			}
		}

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x06000F72 RID: 3954 RVA: 0x0004B4A1 File Offset: 0x000496A1
		public int ControllerActorNr
		{
			get
			{
				return this.photonView.ControllerActorNr;
			}
		}

		// Token: 0x06000F73 RID: 3955 RVA: 0x000027C8 File Offset: 0x000009C8
		protected virtual void Reset()
		{
		}

		// Token: 0x06000F74 RID: 3956 RVA: 0x000027C8 File Offset: 0x000009C8
		protected virtual void OnValidate()
		{
		}

		// Token: 0x06000F75 RID: 3957 RVA: 0x000027C8 File Offset: 0x000009C8
		public virtual void OnJoinedRoom()
		{
		}

		// Token: 0x06000F76 RID: 3958 RVA: 0x0004B4AE File Offset: 0x000496AE
		public void Awake()
		{
			if (!NestedComponentUtilities.GetParentComponent<NetObject>(base.transform))
			{
				this.OnAwakeInitialize(false);
			}
		}

		// Token: 0x06000F77 RID: 3959 RVA: 0x0004B4C9 File Offset: 0x000496C9
		public virtual void OnAwake()
		{
			this.netObj = NestedComponentUtilities.GetParentComponent<NetObject>(base.transform);
			this.EnsureComponentsDependenciesExist();
			this.OnAwakeInitialize(true);
		}

		// Token: 0x06000F78 RID: 3960 RVA: 0x000027C8 File Offset: 0x000009C8
		public virtual void OnAwakeInitialize(bool isNetObject)
		{
		}

		// Token: 0x06000F79 RID: 3961 RVA: 0x0004B4EC File Offset: 0x000496EC
		protected virtual NetObject EnsureComponentsDependenciesExist()
		{
			if (!this.netObj)
			{
				this.netObj = NestedComponentUtilities.GetParentComponent<NetObject>(base.transform);
			}
			if (this.netObj)
			{
				this.photonView = this.netObj.GetComponent<PhotonView>();
				this.RigidbodyType = (this.netObj.Rb ? RigidbodyType.RB : (this.netObj.Rb2D ? RigidbodyType.RB2D : RigidbodyType.None));
				return this.netObj;
			}
			global::Debug.LogError("NetComponent derived class cannot find a NetObject on '" + base.transform.root.name + "'.");
			return null;
		}

		// Token: 0x06000F7A RID: 3962 RVA: 0x0004B592 File Offset: 0x00049792
		public virtual void Start()
		{
			if (!this.netObj)
			{
				this.OnStartInitialize(false);
			}
		}

		// Token: 0x06000F7B RID: 3963 RVA: 0x0004B5A8 File Offset: 0x000497A8
		public virtual void OnStart()
		{
			this.OnStartInitialize(true);
		}

		// Token: 0x06000F7C RID: 3964 RVA: 0x000027C8 File Offset: 0x000009C8
		public virtual void OnStartInitialize(bool isNetObject)
		{
		}

		// Token: 0x06000F7D RID: 3965 RVA: 0x000027C8 File Offset: 0x000009C8
		public virtual void OnPostEnable()
		{
		}

		// Token: 0x06000F7E RID: 3966 RVA: 0x0004B5B1 File Offset: 0x000497B1
		public virtual void OnPostDisable()
		{
			this.hadFirstAuthorityAssgn = false;
		}

		// Token: 0x06000F7F RID: 3967 RVA: 0x0004B5BA File Offset: 0x000497BA
		public virtual void OnAuthorityChanged(bool isMine, bool controllerChanged)
		{
			if (!controllerChanged)
			{
				return;
			}
			if (!this.hadFirstAuthorityAssgn)
			{
				this.OnFirstAuthorityAssign(isMine, controllerChanged);
				this.hadFirstAuthorityAssgn = true;
			}
		}

		// Token: 0x06000F80 RID: 3968 RVA: 0x000027C8 File Offset: 0x000009C8
		public virtual void OnFirstAuthorityAssign(bool isMine, bool asServer)
		{
		}

		// Token: 0x04000E8C RID: 3724
		[HideInInspector]
		[SerializeField]
		protected int prefabInstanceId;

		// Token: 0x04000E8D RID: 3725
		protected NetObject netObj;

		// Token: 0x04000E8F RID: 3727
		protected PhotonView photonView;

		// Token: 0x04000E90 RID: 3728
		protected bool hadFirstAuthorityAssgn;
	}
}
