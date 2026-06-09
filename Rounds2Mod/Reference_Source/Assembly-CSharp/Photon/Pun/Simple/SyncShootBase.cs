using System;
using Photon.Compression;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x02000273 RID: 627
	public abstract class SyncShootBase : SyncObject<SyncShootBase.Frame>, IOnNetSerialize, IOnPostSimulate, IOnPreUpdate, IOnIncrementFrame, IOnSnapshot, IOnInterpolate
	{
		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x06000D94 RID: 3476 RVA: 0x000426F5 File Offset: 0x000408F5
		public IContactTrigger ContactTrigger
		{
			get
			{
				return this.contactTrigger;
			}
		}

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x06000D95 RID: 3477 RVA: 0x00042601 File Offset: 0x00040801
		public override int ApplyOrder
		{
			get
			{
				return 17;
			}
		}

		// Token: 0x06000D96 RID: 3478 RVA: 0x00042700 File Offset: 0x00040900
		public override void OnAwakeInitialize(bool isNetObject)
		{
			base.OnAwakeInitialize(isNetObject);
			this.contactTrigger = NestedComponentUtilities.GetNestedComponentInParents<IContactTrigger, NetObject>(base.transform);
			this.hasSyncContact = (this.contactTrigger.SyncContact != null);
			if (this.origin == null)
			{
				this.origin = base.transform;
			}
		}

		// Token: 0x06000D97 RID: 3479 RVA: 0x00042753 File Offset: 0x00040953
		public virtual void OnPreUpdate()
		{
			if (base.IsMine && Input.GetKeyDown(this.triggerKey))
			{
				this.QueueTrigger();
			}
		}

		// Token: 0x06000D98 RID: 3480 RVA: 0x00042770 File Offset: 0x00040970
		public virtual void QueueTrigger()
		{
			if (base.enabled && base.gameObject.activeInHierarchy)
			{
				this.triggerQueued = true;
			}
		}

		// Token: 0x06000D99 RID: 3481 RVA: 0x00042790 File Offset: 0x00040990
		public virtual SerializationFlags OnNetSerialize(int frameId, byte[] buffer, ref int bitposition, SerializationFlags writeFlags)
		{
			SyncShootBase.Frame frame = this.frames[frameId];
			int sendEveryXTick = TickEngineSettings.sendEveryXTick;
			if (frame.triggerMask != 0U)
			{
				ArraySerializeExt.WriteBool(buffer, true, ref bitposition);
				ArraySerializeExt.Write(buffer, (ulong)frame.triggerMask, ref bitposition, sendEveryXTick);
				return SerializationFlags.HasContent;
			}
			ArraySerializeExt.WriteBool(buffer, false, ref bitposition);
			return SerializationFlags.None;
		}

		// Token: 0x06000D9A RID: 3482 RVA: 0x000427D8 File Offset: 0x000409D8
		public virtual SerializationFlags OnNetDeserialize(int originFrameId, byte[] buffer, ref int bitposition, FrameArrival arrival)
		{
			SyncShootBase.Frame frame = this.frames[originFrameId];
			int sendEveryXTick = TickEngineSettings.sendEveryXTick;
			if (ArraySerializeExt.ReadBool(buffer, ref bitposition))
			{
				frame.triggerMask = ArraySerializeExt.ReadUInt32(buffer, ref bitposition, sendEveryXTick);
				frame.content = FrameContents.Complete;
				return SerializationFlags.HasContent;
			}
			frame.triggerMask = 0U;
			frame.content = FrameContents.Empty;
			return SerializationFlags.None;
		}

		// Token: 0x06000D9B RID: 3483 RVA: 0x00042824 File Offset: 0x00040A24
		public virtual void OnPostSimulate(int frameId, int subFrameId, bool isNetTick)
		{
			if (!base.IsMine)
			{
				return;
			}
			SyncShootBase.Frame frame = this.frames[frameId];
			if (subFrameId == 0)
			{
				frame.Clear();
			}
			if (this.triggerQueued)
			{
				frame.triggerMask |= 1U << subFrameId;
				if (this.Trigger(frame, subFrameId, 0f))
				{
					this.TriggerCosmetic(frame, subFrameId, 0f);
				}
				this.triggerQueued = false;
				frame.content = FrameContents.Complete;
			}
		}

		// Token: 0x06000D9C RID: 3484 RVA: 0x00042894 File Offset: 0x00040A94
		public virtual void OnIncrementFrame(int newFrameId, int newSubFrameId, int previousFrameId, int prevSubFrameId)
		{
			if (base.IsMine)
			{
				return;
			}
			if (this.targFrame == null)
			{
				return;
			}
			if (this.targFrame.content == FrameContents.Complete)
			{
				int offset = (newSubFrameId == 0) ? (TickEngineSettings.sendEveryXTick - 1) : (newSubFrameId - 1);
				this.ApplySubframe(newFrameId, newSubFrameId, offset);
			}
		}

		// Token: 0x06000D9D RID: 3485 RVA: 0x000428DA File Offset: 0x00040ADA
		protected virtual void ApplySubframe(int newFrameId, int newSubFrameId, int offset)
		{
			if (((ulong)this.targFrame.triggerMask & (ulong)(1L << (offset & 31))) != 0UL && this.Trigger(this.targFrame, newSubFrameId, NetMaster.RTT))
			{
				this.TriggerCosmetic(this.targFrame, newSubFrameId, NetMaster.RTT);
			}
		}

		// Token: 0x06000D9E RID: 3486
		protected abstract bool Trigger(SyncShootBase.Frame frame, int subFrameId, float timeshift = 0f);

		// Token: 0x06000D9F RID: 3487 RVA: 0x000027C8 File Offset: 0x000009C8
		protected virtual void TriggerCosmetic(SyncShootBase.Frame frame, int subFrameId, float timeshift = 0f)
		{
		}

		// Token: 0x04000CF0 RID: 3312
		[Tooltip("Specify the transform hitscans/projectiles will originate from. If null this gameObject will be used as the origin.")]
		[SerializeField]
		protected Transform origin;

		// Token: 0x04000CF1 RID: 3313
		[SerializeField]
		public KeyCode triggerKey;

		// Token: 0x04000CF2 RID: 3314
		protected IContactTrigger contactTrigger;

		// Token: 0x04000CF3 RID: 3315
		protected bool hasSyncContact;

		// Token: 0x04000CF4 RID: 3316
		protected bool triggerQueued;

		// Token: 0x020003C9 RID: 969
		public class Frame : FrameBase
		{
			// Token: 0x060013D0 RID: 5072 RVA: 0x0005A3E9 File Offset: 0x000585E9
			public Frame()
			{
			}

			// Token: 0x060013D1 RID: 5073 RVA: 0x0005A3F1 File Offset: 0x000585F1
			public Frame(int frameId) : base(frameId)
			{
			}

			// Token: 0x060013D2 RID: 5074 RVA: 0x0005A3FA File Offset: 0x000585FA
			public override void CopyFrom(FrameBase sourceFrame)
			{
				this.triggerMask = 0U;
			}

			// Token: 0x060013D3 RID: 5075 RVA: 0x0005A403 File Offset: 0x00058603
			public override void Clear()
			{
				base.Clear();
				this.triggerMask = 0U;
			}

			// Token: 0x040012E8 RID: 4840
			public uint triggerMask;
		}
	}
}
