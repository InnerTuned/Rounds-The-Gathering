using System;
using System.Collections.Generic;
using Photon.Compression;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x02000265 RID: 613
	public class SyncContact : SyncObject<SyncContact.Frame>, ISyncContact, IOnSnapshot, IOnNetSerialize, IOnAuthorityChanged, IOnCaptureState, ISerializationOptional
	{
		// Token: 0x1700009F RID: 159
		// (get) Token: 0x06000D5B RID: 3419 RVA: 0x00041DEB File Offset: 0x0003FFEB
		public bool HasRigidbody
		{
			get
			{
				return this._hasRigidbody;
			}
		}

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x06000D5C RID: 3420 RVA: 0x00041DF3 File Offset: 0x0003FFF3
		public GameObject VisiblePickupObj
		{
			get
			{
				return base.gameObject;
			}
		}

		// Token: 0x06000D5D RID: 3421 RVA: 0x00041DFC File Offset: 0x0003FFFC
		public override void OnAwake()
		{
			base.OnAwake();
			this.contactTrigger = base.GetComponent<IContactTrigger>();
			this.rb = base.GetComponentInParent<Rigidbody>();
			this.rb2d = base.GetComponentInParent<Rigidbody2D>();
			this._hasRigidbody = (this.rb || this.rb2d);
		}

		// Token: 0x06000D5E RID: 3422 RVA: 0x00041E54 File Offset: 0x00040054
		public virtual void SyncContactEvent(ContactEvent contactEvent)
		{
			if (!base.IsMine)
			{
				return;
			}
			this.EnqueueEvent(contactEvent);
		}

		// Token: 0x06000D5F RID: 3423 RVA: 0x00041E67 File Offset: 0x00040067
		protected virtual bool EnqueueEvent(ContactEvent contactEvent)
		{
			this.queuedContactEvents.Enqueue(contactEvent);
			return true;
		}

		// Token: 0x06000D60 RID: 3424 RVA: 0x00041E78 File Offset: 0x00040078
		public virtual void OnCaptureCurrentState(int frameId)
		{
			SyncContact.Frame frame = this.frames[frameId];
			frame.content = FrameContents.Empty;
			while (this.queuedContactEvents.Count > 0)
			{
				ContactEvent contactEvent = this.queuedContactEvents.Dequeue();
				Consumption consumption = this.Contact(contactEvent);
				if (consumption != Consumption.None)
				{
					frame.content = FrameContents.Complete;
					List<SyncContact.ContactRecord> contactRecords = frame.contactRecords;
					SyncContact.ContactRecord contactRecord = new SyncContact.ContactRecord(contactEvent.contactSystem.ViewID, contactEvent.contactSystem.SystemIndex, contactEvent.contactType);
					contactRecords.Add(contactRecord);
				}
				if (consumption == Consumption.All)
				{
					break;
				}
			}
			this.queuedContactEvents.Clear();
		}

		// Token: 0x06000D61 RID: 3425 RVA: 0x00041EFF File Offset: 0x000400FF
		protected virtual Consumption Contact(ContactEvent contactEvent)
		{
			return this.contactTrigger.ContactCallbacks(contactEvent);
		}

		// Token: 0x06000D62 RID: 3426 RVA: 0x000027C8 File Offset: 0x000009C8
		protected virtual void Consume(SyncContact.Frame frame, ContactEvent contactEvent, Consumption consumed)
		{
		}

		// Token: 0x06000D63 RID: 3427 RVA: 0x00041F10 File Offset: 0x00040110
		public virtual SerializationFlags OnNetSerialize(int frameId, byte[] buffer, ref int bitposition, SerializationFlags writeFlags)
		{
			SyncContact.Frame frame = this.frames[frameId];
			List<SyncContact.ContactRecord> contactRecords = frame.contactRecords;
			SerializationFlags result;
			if (frame.content == FrameContents.Complete)
			{
				int i = 0;
				int count = contactRecords.Count;
				while (i < count)
				{
					SyncContact.ContactRecord contactRecord = contactRecords[i];
					ArraySerializeExt.Write(buffer, 1UL, ref bitposition, 1);
					ArrayPackBytesExt.WritePackedBytes(buffer, (ulong)contactRecord.contactSystemViewID, ref bitposition, 32);
					ArrayPackBitsExt.WritePackedBits(buffer, (ulong)contactRecord.contactSystemIndex, ref bitposition, 8);
					ContactType contactType = contactRecord.contactType;
					int num = (contactType == ContactType.Enter) ? 0 : ((contactType == ContactType.Hitscan) ? 3 : ((contactType == ContactType.Stay) ? 1 : 2));
					ArraySerializeExt.Write(buffer, (ulong)num, ref bitposition, 2);
					i++;
				}
				ArraySerializeExt.Write(buffer, 0UL, ref bitposition, 1);
				result = (SerializationFlags)5;
			}
			else
			{
				ArraySerializeExt.WriteBool(buffer, false, ref bitposition);
				result = SerializationFlags.None;
			}
			return result;
		}

		// Token: 0x06000D64 RID: 3428 RVA: 0x00041FC8 File Offset: 0x000401C8
		public SerializationFlags OnNetDeserialize(int originFrameId, byte[] buffer, ref int bitposition, FrameArrival arrival)
		{
			SyncContact.Frame frame = this.frames[originFrameId];
			if (ArraySerializeExt.ReadBool(buffer, ref bitposition))
			{
				do
				{
					List<SyncContact.ContactRecord> contactRecords = frame.contactRecords;
					SyncContact.ContactRecord contactRecord = new SyncContact.ContactRecord((int)ArrayPackBytesExt.ReadPackedBytes(buffer, ref bitposition, 32), (byte)ArrayPackBitsExt.ReadPackedBits(buffer, ref bitposition, 8), (ContactType)(1 << (int)ArraySerializeExt.Read(buffer, ref bitposition, 2)));
					contactRecords.Add(contactRecord);
				}
				while (ArraySerializeExt.ReadBool(buffer, ref bitposition));
				frame.content = FrameContents.Complete;
				return SerializationFlags.HasContent;
			}
			frame.content = FrameContents.Empty;
			return SerializationFlags.None;
		}

		// Token: 0x06000D65 RID: 3429 RVA: 0x00042038 File Offset: 0x00040238
		protected override void ApplySnapshot(SyncContact.Frame snapframe, SyncContact.Frame targframe, bool snapIsValid, bool targIsValid)
		{
			base.ApplySnapshot(snapframe, targframe, snapIsValid, targIsValid);
			if (snapframe.content == FrameContents.Complete)
			{
				List<SyncContact.ContactRecord> contactRecords = snapframe.contactRecords;
				int i = 0;
				int count = contactRecords.Count;
				while (i < count)
				{
					SyncContact.ContactRecord contactRecord = contactRecords[i];
					PhotonView photonView = PhotonNetwork.GetPhotonView(contactRecord.contactSystemViewID);
					if (photonView && photonView.IsMine)
					{
						ContactManager component = photonView.GetComponent<ContactManager>();
						if (component)
						{
							IContactSystem contacting = component.GetContacting((int)contactRecord.contactSystemIndex);
							ContactEvent contactEvent = new ContactEvent(contacting, this.contactTrigger, contactRecord.contactType);
							Consumption consumption = this.Contact(contactEvent);
							if (consumption != Consumption.None)
							{
								this.Consume(snapframe, contactEvent, consumption);
							}
						}
					}
					i++;
				}
			}
		}

		// Token: 0x06000D66 RID: 3430 RVA: 0x000420EC File Offset: 0x000402EC
		protected static int ConvertMaskToIndex(int mask)
		{
			int num = 0;
			if (mask > 32767)
			{
				mask >>= 16;
				num += 16;
			}
			if (mask > 127)
			{
				mask >>= 8;
				num += 8;
			}
			if (mask > 7)
			{
				mask >>= 4;
				num += 4;
			}
			if (mask > 1)
			{
				mask >>= 2;
				num += 2;
			}
			if (mask > 0)
			{
				num++;
			}
			return num;
		}

		// Token: 0x06000D67 RID: 3431 RVA: 0x0004213F File Offset: 0x0004033F
		public static int ConvertIndexToMask(int index)
		{
			if (index == 0)
			{
				return 0;
			}
			return 1 << index - 1;
		}

		// Token: 0x04000CD1 RID: 3281
		protected SyncContact.Frame currentState = new SyncContact.Frame();

		// Token: 0x04000CD2 RID: 3282
		protected IContactTrigger contactTrigger;

		// Token: 0x04000CD3 RID: 3283
		protected Rigidbody rb;

		// Token: 0x04000CD4 RID: 3284
		protected Rigidbody2D rb2d;

		// Token: 0x04000CD5 RID: 3285
		protected bool _hasRigidbody;

		// Token: 0x04000CD6 RID: 3286
		protected Queue<ContactEvent> queuedContactEvents = new Queue<ContactEvent>();

		// Token: 0x020003C7 RID: 967
		public struct ContactRecord
		{
			// Token: 0x060013C9 RID: 5065 RVA: 0x0005A31F File Offset: 0x0005851F
			public ContactRecord(int contactSystemViewID, byte contactSystemIndex, ContactType contactType)
			{
				this.contactSystemViewID = contactSystemViewID;
				this.contactSystemIndex = contactSystemIndex;
				this.contactType = contactType;
			}

			// Token: 0x060013CA RID: 5066 RVA: 0x0005A338 File Offset: 0x00058538
			public override string ToString()
			{
				return string.Concat(new object[]
				{
					this.contactType,
					" view: ",
					this.contactSystemViewID,
					" index:",
					this.contactSystemIndex
				});
			}

			// Token: 0x040012E4 RID: 4836
			public ContactType contactType;

			// Token: 0x040012E5 RID: 4837
			public int contactSystemViewID;

			// Token: 0x040012E6 RID: 4838
			public byte contactSystemIndex;
		}

		// Token: 0x020003C8 RID: 968
		public class Frame : FrameBase
		{
			// Token: 0x060013CB RID: 5067 RVA: 0x0005A38A File Offset: 0x0005858A
			public Frame()
			{
			}

			// Token: 0x060013CC RID: 5068 RVA: 0x0005A39E File Offset: 0x0005859E
			public Frame(int frameId) : base(frameId)
			{
			}

			// Token: 0x060013CD RID: 5069 RVA: 0x0005A3B3 File Offset: 0x000585B3
			public override void CopyFrom(FrameBase sourceFrame)
			{
				base.CopyFrom(sourceFrame);
				this.content = FrameContents.Empty;
				this.contactRecords.Clear();
			}

			// Token: 0x060013CE RID: 5070 RVA: 0x0005A3CE File Offset: 0x000585CE
			public static SyncContact.Frame Construct(int frameId)
			{
				return new SyncContact.Frame(frameId);
			}

			// Token: 0x060013CF RID: 5071 RVA: 0x0005A3D6 File Offset: 0x000585D6
			public override void Clear()
			{
				base.Clear();
				this.contactRecords.Clear();
			}

			// Token: 0x040012E7 RID: 4839
			public List<SyncContact.ContactRecord> contactRecords = new List<SyncContact.ContactRecord>(1);
		}
	}
}
