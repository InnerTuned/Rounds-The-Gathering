using System;
using Photon.Compression;
using Photon.Utilities;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x020002AD RID: 685
	[DisallowMultipleComponent]
	[RequireComponent(typeof(SyncState))]
	public class SyncSpawnTimer : SyncObject<SyncSpawnTimer.Frame>, ISpawnController, ISerializationOptional, IOnNetSerialize, IUseKeyframes, IOnSnapshot, IOnCaptureState, IOnStateChange
	{
		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x06000EF8 RID: 3832 RVA: 0x000486CA File Offset: 0x000468CA
		public override int ApplyOrder
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x06000EF9 RID: 3833 RVA: 0x000486D0 File Offset: 0x000468D0
		public override void OnAwake()
		{
			base.OnAwake();
			if (this.netObj)
			{
				this.syncState = this.netObj.GetComponent<SyncState>();
			}
			this.spawnWaitAsTicks = base.ConvertSecsToTicks(this.initialDelay);
			this.respawnWaitAsTicks = base.ConvertSecsToTicks(this.respawnDelay);
			this.despawnWaitAsTicks = base.ConvertSecsToTicks(this.despawnDelay);
			this.bitsForTicksUntilRespawn = Math.Max(this.respawnWaitAsTicks, this.spawnWaitAsTicks).GetBitsForMaxValue();
			this.bitsForTicksUntilDespawn = this.despawnWaitAsTicks.GetBitsForMaxValue();
		}

		// Token: 0x06000EFA RID: 3834 RVA: 0x00048764 File Offset: 0x00046964
		public bool AllowNetObjectReadyCallback(bool ready)
		{
			return !ready || this.spawnWaitAsTicks < 0;
		}

		// Token: 0x06000EFB RID: 3835 RVA: 0x00048775 File Offset: 0x00046975
		public override void OnJoinedRoom()
		{
			base.OnJoinedRoom();
			if (this.photonView.IsMine)
			{
				this.ticksUntilRespawn = this.spawnWaitAsTicks;
				return;
			}
			this.ticksUntilRespawn = -1;
		}

		// Token: 0x06000EFC RID: 3836 RVA: 0x0004879E File Offset: 0x0004699E
		public override void OnStart()
		{
			base.OnStart();
			this.ticksUntilRespawn = this.spawnWaitAsTicks;
		}

		// Token: 0x06000EFD RID: 3837 RVA: 0x000487B4 File Offset: 0x000469B4
		public void OnStateChange(ObjState newState, ObjState previousState, Transform attachmentTransform, Mount attachTo = null, bool isReady = true)
		{
			if (newState == previousState)
			{
				return;
			}
			if (this.respawnEnable)
			{
				if (newState == ObjState.Despawned)
				{
					this.ticksUntilRespawn = this.respawnWaitAsTicks;
				}
				else if ((previousState & this.respawnOn) == ObjState.Despawned && (newState & this.respawnOn) != ObjState.Despawned)
				{
					global::Debug.Log(string.Concat(new object[]
					{
						Time.time,
						" ",
						base.name,
						" ",
						this.photonView.OwnerActorNr,
						" <b>Reset </b> ",
						previousState,
						" <> ",
						newState
					}));
					this.ticksUntilRespawn = this.respawnWaitAsTicks;
				}
			}
			if (this.despawnEnable && (previousState & this.despawnOn) == ObjState.Despawned && (newState & this.despawnOn) != ObjState.Despawned)
			{
				this.ticksUntilDespawn = this.despawnWaitAsTicks;
			}
		}

		// Token: 0x06000EFE RID: 3838 RVA: 0x0004889C File Offset: 0x00046A9C
		public virtual void OnCaptureCurrentState(int frameId)
		{
			SyncSpawnTimer.Frame frame = this.frames[frameId];
			if (!this.hadInitialSpawn || this.respawnEnable)
			{
				if (this.ticksUntilRespawn == 0)
				{
					this.syncState.Respawn(false);
					this.hadInitialSpawn = true;
				}
				this.ticksUntilRespawn--;
				frame.ticksUntilRespawn = this.ticksUntilRespawn;
			}
			if (this.despawnEnable)
			{
				if (this.ticksUntilDespawn == 0)
				{
					this.syncState.Despawn(false);
				}
				this.ticksUntilDespawn--;
				frame.ticksUntilDespawn = this.ticksUntilDespawn;
			}
		}

		// Token: 0x06000EFF RID: 3839 RVA: 0x00048930 File Offset: 0x00046B30
		public virtual SerializationFlags OnNetSerialize(int frameId, byte[] buffer, ref int bitposition, SerializationFlags writeFlags)
		{
			SyncSpawnTimer.Frame frame = this.frames[frameId];
			SerializationFlags serializationFlags = SerializationFlags.None;
			if (!base.IsKeyframe(frameId))
			{
				return serializationFlags;
			}
			if (this.respawnEnable)
			{
				int num = frame.ticksUntilRespawn;
				if (num >= 0)
				{
					ArraySerializeExt.WriteBool(buffer, true, ref bitposition);
					ArraySerializeExt.Write(buffer, (ulong)((long)num), ref bitposition, this.bitsForTicksUntilRespawn);
					serializationFlags |= SerializationFlags.HasContent;
				}
				else
				{
					ArraySerializeExt.WriteBool(buffer, false, ref bitposition);
				}
			}
			if (this.despawnEnable)
			{
				int num2 = frame.ticksUntilDespawn;
				if (num2 >= 0)
				{
					ArraySerializeExt.WriteBool(buffer, true, ref bitposition);
					ArraySerializeExt.Write(buffer, (ulong)((long)num2), ref bitposition, this.bitsForTicksUntilDespawn);
					serializationFlags |= SerializationFlags.HasContent;
				}
				else
				{
					ArraySerializeExt.WriteBool(buffer, false, ref bitposition);
				}
			}
			return serializationFlags;
		}

		// Token: 0x06000F00 RID: 3840 RVA: 0x000489C4 File Offset: 0x00046BC4
		public SerializationFlags OnNetDeserialize(int originFrameId, byte[] buffer, ref int bitposition, FrameArrival frameArrival)
		{
			SyncSpawnTimer.Frame frame = this.frames[originFrameId];
			SerializationFlags serializationFlags = SerializationFlags.None;
			if (!base.IsKeyframe(originFrameId))
			{
				frame.content = FrameContents.Empty;
				return serializationFlags;
			}
			if (this.respawnEnable)
			{
				if (ArraySerializeExt.ReadBool(buffer, ref bitposition))
				{
					frame.ticksUntilRespawn = (int)ArraySerializeExt.Read(buffer, ref bitposition, this.bitsForTicksUntilRespawn);
					serializationFlags |= SerializationFlags.HasContent;
				}
				else
				{
					frame.ticksUntilRespawn = -1;
				}
			}
			if (this.despawnEnable)
			{
				if (ArraySerializeExt.ReadBool(buffer, ref bitposition))
				{
					frame.ticksUntilDespawn = (int)ArraySerializeExt.Read(buffer, ref bitposition, this.bitsForTicksUntilDespawn);
					serializationFlags |= SerializationFlags.HasContent;
				}
				else
				{
					frame.ticksUntilDespawn = -1;
				}
			}
			frame.content = FrameContents.Complete;
			return serializationFlags;
		}

		// Token: 0x06000F01 RID: 3841 RVA: 0x00048A5C File Offset: 0x00046C5C
		protected override void ApplySnapshot(SyncSpawnTimer.Frame snapframe, SyncSpawnTimer.Frame targframe, bool snapIsValid, bool targIsValid)
		{
			if (snapIsValid && snapframe.content > FrameContents.Empty)
			{
				if (this.respawnEnable)
				{
					this.ticksUntilRespawn = snapframe.ticksUntilRespawn;
				}
				if (this.despawnEnable)
				{
					this.ticksUntilDespawn = snapframe.ticksUntilDespawn;
				}
			}
			else
			{
				if (this.respawnEnable)
				{
					this.ticksUntilRespawn--;
					targframe.ticksUntilRespawn = this.ticksUntilRespawn;
				}
				if (this.despawnEnable)
				{
					this.ticksUntilDespawn--;
					targframe.ticksUntilDespawn = this.ticksUntilDespawn;
				}
			}
			if (this.photonView.IsMine)
			{
				if (this.respawnEnable && this.ticksUntilRespawn == 0)
				{
					this.syncState.Respawn(false);
				}
				if (this.despawnEnable && this.ticksUntilDespawn == 0)
				{
					this.syncState.Despawn(false);
				}
			}
		}

		// Token: 0x04000E05 RID: 3589
		[HideInInspector]
		[SerializeField]
		public float initialDelay;

		// Token: 0x04000E06 RID: 3590
		[HideInInspector]
		[SerializeField]
		public bool respawnEnable = true;

		// Token: 0x04000E07 RID: 3591
		[EnumMask(true, typeof(ObjStateEditor))]
		[HideInInspector]
		[SerializeField]
		public ObjState despawnOn = ObjState.Mounted;

		// Token: 0x04000E08 RID: 3592
		[Tooltip("Number of seconds after respawn trigger before respawn occurs.")]
		[HideInInspector]
		[SerializeField]
		public float despawnDelay = 5f;

		// Token: 0x04000E09 RID: 3593
		[HideInInspector]
		[SerializeField]
		public bool despawnEnable;

		// Token: 0x04000E0A RID: 3594
		[EnumMask(true, typeof(ObjStateEditor))]
		[HideInInspector]
		[SerializeField]
		public ObjState respawnOn;

		// Token: 0x04000E0B RID: 3595
		[Tooltip("Number of seconds after respawn trigger before respawn occurs.")]
		[HideInInspector]
		[SerializeField]
		public float respawnDelay = 5f;

		// Token: 0x04000E0C RID: 3596
		protected SyncState syncState;

		// Token: 0x04000E0D RID: 3597
		[NonSerialized]
		protected int ticksUntilRespawn = -1;

		// Token: 0x04000E0E RID: 3598
		[NonSerialized]
		protected int ticksUntilDespawn = -1;

		// Token: 0x04000E0F RID: 3599
		[NonSerialized]
		protected int spawnWaitAsTicks;

		// Token: 0x04000E10 RID: 3600
		[NonSerialized]
		protected int respawnWaitAsTicks;

		// Token: 0x04000E11 RID: 3601
		[NonSerialized]
		protected int despawnWaitAsTicks;

		// Token: 0x04000E12 RID: 3602
		[NonSerialized]
		protected bool hadInitialSpawn;

		// Token: 0x04000E13 RID: 3603
		protected int bitsForTicksUntilRespawn;

		// Token: 0x04000E14 RID: 3604
		protected int bitsForTicksUntilDespawn;

		// Token: 0x020003D5 RID: 981
		public class Frame : FrameBase
		{
			// Token: 0x060013ED RID: 5101 RVA: 0x0005A3E9 File Offset: 0x000585E9
			public Frame()
			{
			}

			// Token: 0x060013EE RID: 5102 RVA: 0x0005A3F1 File Offset: 0x000585F1
			public Frame(int frameId) : base(frameId)
			{
			}

			// Token: 0x060013EF RID: 5103 RVA: 0x0005A83C File Offset: 0x00058A3C
			public override void CopyFrom(FrameBase sourceFrame)
			{
				base.CopyFrom(sourceFrame);
				SyncSpawnTimer.Frame frame = sourceFrame as SyncSpawnTimer.Frame;
				this.ticksUntilRespawn = frame.ticksUntilRespawn;
				this.ticksUntilDespawn = frame.ticksUntilDespawn;
			}

			// Token: 0x060013F0 RID: 5104 RVA: 0x0005A86F File Offset: 0x00058A6F
			public bool Compare(SyncSpawnTimer.Frame otherFrame)
			{
				return this.ticksUntilRespawn == otherFrame.ticksUntilRespawn && this.ticksUntilDespawn == otherFrame.ticksUntilDespawn;
			}

			// Token: 0x04001309 RID: 4873
			public int ticksUntilRespawn;

			// Token: 0x0400130A RID: 4874
			public int ticksUntilDespawn;
		}
	}
}
