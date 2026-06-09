using System;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Pun
{
	// Token: 0x02000250 RID: 592
	[AddComponentMenu("Photon Networking/Photon Transform View")]
	[RequireComponent(typeof(PhotonView))]
	public class SyncPlayerMovement : MonoBehaviour, IPunObservable
	{
		// Token: 0x06000CD7 RID: 3287 RVA: 0x00040608 File Offset: 0x0003E808
		public void Awake()
		{
			this.photonView = base.GetComponent<PhotonView>();
			this.data = base.GetComponent<CharacterData>();
			if (!this.photonView.IsMine)
			{
				this.data.input.controlledElseWhere = true;
				base.GetComponent<PlayerCollision>().checkForGoThroughWall = false;
				return;
			}
			PlayerJump jump = this.data.jump;
			jump.JumpAction = (Action)Delegate.Combine(jump.JumpAction, new Action(this.DoJump));
			this.data.weaponHandler.gun.AddAttackAction(new Action(this.SendShoot));
			Block block = this.data.block;
			block.BlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Combine(block.BlockAction, new Action<BlockTrigger.BlockTriggerType>(this.SendBlock));
		}

		// Token: 0x06000CD8 RID: 3288 RVA: 0x000406D1 File Offset: 0x0003E8D1
		private void OnEnable()
		{
			this.syncPackages.Clear();
		}

		// Token: 0x06000CD9 RID: 3289 RVA: 0x000406DE File Offset: 0x0003E8DE
		public void SetDontSyncFor(float t)
		{
			this.sincePushed = t * 0.5f;
		}

		// Token: 0x06000CDA RID: 3290 RVA: 0x000406F0 File Offset: 0x0003E8F0
		private void Update()
		{
			if (this.syncPackages.Count > 0)
			{
				if (this.syncPackages[0].timeDelta > 0f)
				{
					this.syncPackages[0].timeDelta -= Time.deltaTime * 1.5f * (1f + (float)this.syncPackages.Count * 0.1f);
					return;
				}
				while (this.syncPackages.Count > 5)
				{
					this.syncPackages.RemoveAt(0);
				}
				this.sincePushed -= Time.deltaTime;
				float t = Mathf.Clamp(1f - this.sincePushed * 5f, 0f, 1f);
				base.transform.position = Vector2.Lerp(base.transform.position, this.syncPackages[0].pos, t);
				this.data.playerVel.velocity = Vector2.Lerp(this.data.playerVel.velocity, this.syncPackages[0].vel, t);
				this.data.input.direction = this.syncPackages[0].dir;
				this.data.input.aimDirection = this.syncPackages[0].aim;
				this.data.input.jumpIsPressed = this.syncPackages[0].holdJump;
				this.data.sinceGrounded = this.syncPackages[0].sinceGrounded;
				if (this.syncPackages[0].jump)
				{
					this.Jump();
				}
				this.syncPackages.RemoveAt(0);
			}
		}

		// Token: 0x06000CDB RID: 3291 RVA: 0x000408C8 File Offset: 0x0003EAC8
		private void SendBlock(BlockTrigger.BlockTriggerType blockTrigger)
		{
			this.SendBlock(blockTrigger, false);
		}

		// Token: 0x06000CDC RID: 3292 RVA: 0x000408D2 File Offset: 0x0003EAD2
		public void SendBlock(BlockTrigger.BlockTriggerType blockTrigger, bool toAll = false)
		{
			if (blockTrigger == BlockTrigger.BlockTriggerType.Default)
			{
				if (toAll)
				{
					this.data.view.RPC("RPCAO_DoBlock", 0, Array.Empty<object>());
					return;
				}
				this.data.view.RPC("RPCAO_DoBlock", 1, Array.Empty<object>());
			}
		}

		// Token: 0x06000CDD RID: 3293 RVA: 0x00040914 File Offset: 0x0003EB14
		[PunRPC]
		public void RPCAO_DoBlock()
		{
			this.data.block.RPCA_DoBlock(true, false, BlockTrigger.BlockTriggerType.Default, default(Vector3), false);
		}

		// Token: 0x06000CDE RID: 3294 RVA: 0x0004093E File Offset: 0x0003EB3E
		public void SendBlock(BlockTrigger.BlockTriggerType blockTrigger, bool firstBlock, bool dontSetCD)
		{
			this.data.view.RPC("RPCA_DoBlock", 0, new object[]
			{
				(int)blockTrigger,
				firstBlock,
				dontSetCD
			});
		}

		// Token: 0x06000CDF RID: 3295 RVA: 0x00040978 File Offset: 0x0003EB78
		[PunRPC]
		public void RPCA_DoBlock(int blocktTrigger, bool firstBlock, bool dontSetCD)
		{
			this.data.block.RPCA_DoBlock(firstBlock, dontSetCD, (BlockTrigger.BlockTriggerType)blocktTrigger, default(Vector3), false);
		}

		// Token: 0x06000CE0 RID: 3296 RVA: 0x000409A2 File Offset: 0x0003EBA2
		public void SendShoot()
		{
			this.photonView.RPC("Shoot", 1, Array.Empty<object>());
		}

		// Token: 0x06000CE1 RID: 3297 RVA: 0x000409BA File Offset: 0x0003EBBA
		[PunRPC]
		public void Shoot()
		{
			this.data.weaponHandler.gun.Attack(0f, true, 1f, 1f, true);
		}

		// Token: 0x06000CE2 RID: 3298 RVA: 0x000409E3 File Offset: 0x0003EBE3
		public void DoJump()
		{
			this.didJump = true;
		}

		// Token: 0x06000CE3 RID: 3299 RVA: 0x000409EC File Offset: 0x0003EBEC
		public void SendJump()
		{
			this.photonView.RPC("Jump", 1, Array.Empty<object>());
		}

		// Token: 0x06000CE4 RID: 3300 RVA: 0x00040A04 File Offset: 0x0003EC04
		[PunRPC]
		public void Jump()
		{
			this.data.jump.Jump(true, 1f);
		}

		// Token: 0x06000CE5 RID: 3301 RVA: 0x00040A1C File Offset: 0x0003EC1C
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.IsWriting)
			{
				stream.SendNext(base.transform.position);
				stream.SendNext(this.data.input.direction);
				stream.SendNext(this.data.input.aimDirection);
				stream.SendNext(this.data.input.jumpIsPressed);
				stream.SendNext(this.data.playerVel.velocity);
				stream.SendNext(this.didJump);
				stream.SendNext(this.data.sinceGrounded);
				this.didJump = false;
				if (this.lastTime == 0f)
				{
					this.lastTime = Time.time;
				}
				stream.SendNext(Time.time - this.lastTime);
				this.lastTime = Time.time;
				return;
			}
			PlayerSyncPackage playerSyncPackage = new PlayerSyncPackage();
			playerSyncPackage.pos = (Vector3)stream.ReceiveNext();
			playerSyncPackage.dir = (Vector3)stream.ReceiveNext();
			playerSyncPackage.aim = (Vector3)stream.ReceiveNext();
			playerSyncPackage.holdJump = (bool)stream.ReceiveNext();
			playerSyncPackage.vel = (Vector2)stream.ReceiveNext();
			playerSyncPackage.jump = (bool)stream.ReceiveNext();
			playerSyncPackage.sinceGrounded = (float)stream.ReceiveNext();
			playerSyncPackage.timeDelta = (float)stream.ReceiveNext();
			playerSyncPackage.timeDelta = Mathf.Clamp(playerSyncPackage.timeDelta, 0f, 0.1f);
			this.syncPackages.Add(playerSyncPackage);
		}

		// Token: 0x04000C85 RID: 3205
		private PhotonView photonView;

		// Token: 0x04000C86 RID: 3206
		private CharacterData data;

		// Token: 0x04000C87 RID: 3207
		private float sincePushed;

		// Token: 0x04000C88 RID: 3208
		private bool didJump;

		// Token: 0x04000C89 RID: 3209
		private float lastTime;

		// Token: 0x04000C8A RID: 3210
		private List<PlayerSyncPackage> syncPackages = new List<PlayerSyncPackage>();
	}
}
