using System;
using Photon.Compression;
using Photon.Realtime;

namespace Photon.Pun.Simple
{
	// Token: 0x020002AB RID: 683
	public class SyncOwner : SyncObject<SyncOwner.Frame>, IOnCaptureState, IOnNetSerialize, IOnSnapshot, IUseKeyframes, IOnIncrementFrame
	{
		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x06000EED RID: 3821 RVA: 0x00048412 File Offset: 0x00046612
		public override int ApplyOrder
		{
			get
			{
				return 21;
			}
		}

		// Token: 0x06000EEE RID: 3822 RVA: 0x00048416 File Offset: 0x00046616
		public override void OnAuthorityChanged(bool isMine, bool controllerChanged)
		{
			base.OnAuthorityChanged(isMine, controllerChanged);
			if (!isMine)
			{
				this.pendingOwnerChange = false;
				this.ticksUntilOwnershipRetry = -1;
			}
		}

		// Token: 0x06000EEF RID: 3823 RVA: 0x00048431 File Offset: 0x00046631
		public void TransferOwner(int newOwnerId)
		{
			if (this.photonView.IsMine)
			{
				this.pendingOwnerChange = true;
				this.pendingOwnerId = newOwnerId;
			}
		}

		// Token: 0x06000EF0 RID: 3824 RVA: 0x00048450 File Offset: 0x00046650
		public void OnCaptureCurrentState(int frameId)
		{
			SyncOwner.Frame frame = this.frames[frameId];
			if (this.pendingOwnerChange)
			{
				if (this.photonView.OwnerActorNr != 0)
				{
					this.ticksUntilOwnershipRetry = TickEngineSettings.frameCount;
				}
				NetMasterCallbacks.postCallbackActions.Enqueue(new Action(this.DeferredOwnerChange));
				frame.ownerActorId = this.pendingOwnerId;
				frame.ownerHasChanged = true;
				this.pendingOwnerChange = false;
				return;
			}
			this.frames[frameId].ownerActorId = this.photonView.OwnerActorNr;
		}

		// Token: 0x06000EF1 RID: 3825 RVA: 0x000484D0 File Offset: 0x000466D0
		public SerializationFlags OnNetSerialize(int frameId, byte[] buffer, ref int bitposition, SerializationFlags writeFlags)
		{
			SyncOwner.Frame frame = this.frames[frameId];
			bool flag = frame.ownerHasChanged && (this.reliableOwnerChange || this.keyframeRate == 0 || (writeFlags & SerializationFlags.NewConnection) > SerializationFlags.None);
			if (!flag && !base.IsKeyframe(frameId))
			{
				ArraySerializeExt.Write(buffer, 0UL, ref bitposition, 1);
				return SerializationFlags.None;
			}
			ArraySerializeExt.Write(buffer, 1UL, ref bitposition, 1);
			SerializationFlags serializationFlags = SerializationFlags.HasContent;
			ArrayPackBytesExt.WritePackedBytes(buffer, (ulong)frame.ownerActorId, ref bitposition, 32);
			if (flag)
			{
				serializationFlags |= SerializationFlags.ForceReliable;
			}
			return serializationFlags;
		}

		// Token: 0x06000EF2 RID: 3826 RVA: 0x0004854C File Offset: 0x0004674C
		public SerializationFlags OnNetDeserialize(int originFrameId, byte[] buffer, ref int bitposition, FrameArrival frameArrival)
		{
			SyncOwner.Frame frame = this.frames[originFrameId];
			bool flag = ArraySerializeExt.Read(buffer, ref bitposition, 1) > 0UL;
			int ownerActorId = flag ? ((int)ArrayPackBytesExt.ReadPackedBytes(buffer, ref bitposition, 32)) : -1;
			if (this.photonView.IsMine)
			{
				return SerializationFlags.None;
			}
			if (flag)
			{
				frame.content = FrameContents.Complete;
				frame.ownerActorId = ownerActorId;
				return SerializationFlags.HasContent;
			}
			frame.content = FrameContents.Empty;
			return SerializationFlags.None;
		}

		// Token: 0x06000EF3 RID: 3827 RVA: 0x000485AC File Offset: 0x000467AC
		protected override void ApplySnapshot(SyncOwner.Frame snapframe, SyncOwner.Frame targframe, bool snapIsValid, bool targIsValid)
		{
			if (snapIsValid && snapframe.content == FrameContents.Complete)
			{
				int ownerActorId = snapframe.ownerActorId;
				this.pendingOwnerId = ownerActorId;
				NetMasterCallbacks.postCallbackActions.Enqueue(new Action(this.DeferredOwnerChange));
				this.ticksUntilOwnershipRetry = -1;
			}
		}

		// Token: 0x06000EF4 RID: 3828 RVA: 0x000485F0 File Offset: 0x000467F0
		protected void DeferredOwnerChange()
		{
			Player player;
			PhotonNetwork.CurrentRoom.Players.TryGetValue(this.pendingOwnerId, ref player);
			this.photonView.SetOwnerInternal(player, this.pendingOwnerId);
		}

		// Token: 0x06000EF5 RID: 3829 RVA: 0x00048628 File Offset: 0x00046828
		public void OnIncrementFrame(int newFrameId, int newSubFrameId, int previousFrameId, int prevSubFrameId)
		{
			if (this.photonView.IsMine)
			{
				return;
			}
			if (newSubFrameId != 0)
			{
				return;
			}
			if (this.ticksUntilOwnershipRetry >= 0)
			{
				if (this.ticksUntilOwnershipRetry == 0)
				{
					Debug.LogError(base.name + " FALLBACK OWNER CHANGE " + this.photonView.ControllerActorNr);
					this.photonView.TransferOwnership(this.photonView.Controller);
					this.ticksUntilOwnershipRetry = TickEngineSettings.frameCount;
					return;
				}
				this.ticksUntilOwnershipRetry--;
			}
		}

		// Token: 0x04000E01 RID: 3585
		public bool reliableOwnerChange = true;

		// Token: 0x04000E02 RID: 3586
		protected bool pendingOwnerChange;

		// Token: 0x04000E03 RID: 3587
		protected int pendingOwnerId = -1;

		// Token: 0x04000E04 RID: 3588
		protected int ticksUntilOwnershipRetry = -1;

		// Token: 0x020003D4 RID: 980
		public class Frame : FrameBase
		{
			// Token: 0x060013EA RID: 5098 RVA: 0x0005A7F3 File Offset: 0x000589F3
			public override void Clear()
			{
				this.ownerActorId = -1;
				this.ownerHasChanged = false;
				base.Clear();
			}

			// Token: 0x060013EB RID: 5099 RVA: 0x0005A80C File Offset: 0x00058A0C
			public override void CopyFrom(FrameBase sourceFrame)
			{
				base.CopyFrom(sourceFrame);
				SyncOwner.Frame frame = sourceFrame as SyncOwner.Frame;
				this.ownerActorId = frame.ownerActorId;
				this.ownerHasChanged = false;
			}

			// Token: 0x04001307 RID: 4871
			public int ownerActorId;

			// Token: 0x04001308 RID: 4872
			public bool ownerHasChanged;
		}
	}
}
