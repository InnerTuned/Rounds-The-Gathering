using System;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Pun
{
	// Token: 0x02000252 RID: 594
	[AddComponentMenu("Photon Networking/Photon Transform View")]
	[RequireComponent(typeof(PhotonView))]
	public class SyncProjectile : MonoBehaviour, IPunObservable
	{
		// Token: 0x06000CE8 RID: 3304 RVA: 0x00040BE7 File Offset: 0x0003EDE7
		public void Awake()
		{
			this.photonView = base.GetComponent<PhotonView>();
			this.move = base.GetComponent<MoveTransform>();
			bool isMine = this.photonView.IsMine;
		}

		// Token: 0x06000CE9 RID: 3305 RVA: 0x00040C10 File Offset: 0x0003EE10
		private void Update()
		{
			if (!this.active)
			{
				return;
			}
			if (this.syncPackages.Count > 0)
			{
				if (this.syncPackages[0].timeDelta > 0f)
				{
					this.syncPackages[0].timeDelta -= Time.deltaTime * 1.5f * (1f + (float)this.syncPackages.Count * 0.5f);
					return;
				}
				if (this.syncPackages.Count > 2)
				{
					this.syncPackages.RemoveAt(0);
				}
				base.transform.position = this.syncPackages[0].pos;
				this.move.velocity = this.syncPackages[0].vel;
				this.syncPackages.RemoveAt(0);
			}
		}

		// Token: 0x06000CEA RID: 3306 RVA: 0x00040CF0 File Offset: 0x0003EEF0
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (!this.active)
			{
				return;
			}
			if (stream.IsWriting)
			{
				stream.SendNext(base.transform.position);
				stream.SendNext(this.move.velocity);
				if (this.lastTime == 0f)
				{
					this.lastTime = Time.time;
				}
				stream.SendNext(Time.time - this.lastTime);
				this.lastTime = Time.time;
				return;
			}
			ProjectileSyncPackage projectileSyncPackage = new ProjectileSyncPackage();
			projectileSyncPackage.pos = (Vector3)stream.ReceiveNext();
			projectileSyncPackage.vel = (Vector3)stream.ReceiveNext();
			projectileSyncPackage.timeDelta = (float)stream.ReceiveNext();
			this.syncPackages.Add(projectileSyncPackage);
		}

		// Token: 0x04000C93 RID: 3219
		public bool active;

		// Token: 0x04000C94 RID: 3220
		private PhotonView photonView;

		// Token: 0x04000C95 RID: 3221
		private MoveTransform move;

		// Token: 0x04000C96 RID: 3222
		private float lastTime;

		// Token: 0x04000C97 RID: 3223
		private List<ProjectileSyncPackage> syncPackages = new List<ProjectileSyncPackage>();
	}
}
