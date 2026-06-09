using System;
using System.Collections.Generic;
using Photon.Compression;
using Photon.Pun.Simple.ContactGroups;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x0200027C RID: 636
	public class SyncVitals : SyncObject<SyncVitals.Frame>, IVitalsSystem, IContactSystem, IOnSnapshot, IOnNetSerialize, IOnAuthorityChanged, IOnPostSimulate, IOnVitalValueChange, IOnVitalChange, IOnCaptureState, IUseKeyframes, IOnStateChange
	{
		// Token: 0x170000BB RID: 187
		// (get) Token: 0x06000DAB RID: 3499 RVA: 0x000429F2 File Offset: 0x00040BF2
		public override int ApplyOrder
		{
			get
			{
				return 15;
			}
		}

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000DAC RID: 3500 RVA: 0x000429F6 File Offset: 0x00040BF6
		public override bool AllowReconstructionOfEmpty
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06000DAD RID: 3501 RVA: 0x000429F9 File Offset: 0x00040BF9
		// (set) Token: 0x06000DAE RID: 3502 RVA: 0x00042A01 File Offset: 0x00040C01
		public byte SystemIndex { get; set; }

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x06000DAF RID: 3503 RVA: 0x00042A0A File Offset: 0x00040C0A
		public Vitals Vitals
		{
			get
			{
				return this.vitals;
			}
		}

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x06000DB0 RID: 3504 RVA: 0x00042A12 File Offset: 0x00040C12
		public IContactGroupMask ValidContactGroups
		{
			get
			{
				return this.contactGroups;
			}
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x06000DB1 RID: 3505 RVA: 0x00042A1F File Offset: 0x00040C1F
		public int ValidMountsMask
		{
			get
			{
				return 1 << this.defaultMounting.id;
			}
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x06000DB2 RID: 3506 RVA: 0x00042A31 File Offset: 0x00040C31
		// (set) Token: 0x06000DB3 RID: 3507 RVA: 0x00042A39 File Offset: 0x00040C39
		public Mount DefaultMount { get; set; }

		// Token: 0x06000DB4 RID: 3508 RVA: 0x00042A44 File Offset: 0x00040C44
		protected override void PopulateFrames()
		{
			int frameCount = TickEngineSettings.frameCount;
			this.frames = new SyncVitals.Frame[frameCount + 1];
			for (int i = 0; i <= frameCount; i++)
			{
				this.frames[i] = new SyncVitals.Frame(i, this.vitals);
			}
		}

		// Token: 0x06000DB5 RID: 3509 RVA: 0x00042A88 File Offset: 0x00040C88
		public override void OnAwake()
		{
			base.OnAwake();
			NestedComponentUtilities.EnsureRootComponentExists<ContactManager, NetObject>(base.transform);
			if (this.netObj)
			{
				this.syncState = this.netObj.GetComponent<SyncState>();
			}
			this.vitalArray = this.vitals.VitalArray;
			this.vitalsCount = this.vitals.vitalDefs.Count;
			this.rootVital = this.vitalArray[0];
			this.vitals.OnVitalValueChangeCallbacks.Add(this);
			this.lastSentData = new VitalsData(this.vitals);
			for (int i = 0; i < this.vitalsCount; i++)
			{
				this.vitalArray[i].ResetValues();
			}
			this.defaultMountingMask = 1 << this.defaultMounting.id;
		}

		// Token: 0x06000DB6 RID: 3510 RVA: 0x00042B50 File Offset: 0x00040D50
		public override void OnStart()
		{
			base.OnStart();
			MountsManager component = base.GetComponent<MountsManager>();
			if (component)
			{
				if (component.mountIdLookup.ContainsKey(this.defaultMounting.id))
				{
					this.DefaultMount = component.mountIdLookup[this.defaultMounting.id];
					return;
				}
				global::Debug.LogWarning(string.Concat(new string[]
				{
					"Sync Vitals has a Default Mount setting of ",
					MountSettings.GetName(this.defaultMounting.id),
					" but no such mount is defined yet on GameObject: '",
					base.name,
					"'. Root mount will be used as a failsafe."
				}));
				this.defaultMounting.id = 0;
				this.DefaultMount = component.mountIdLookup[0];
			}
		}

		// Token: 0x06000DB7 RID: 3511 RVA: 0x00042C0C File Offset: 0x00040E0C
		public override void OnAuthorityChanged(bool isMine, bool controllerChanged)
		{
			base.OnAuthorityChanged(isMine, controllerChanged);
			OwnedIVitals.OnChangeAuthority(this, isMine, controllerChanged);
		}

		// Token: 0x06000DB8 RID: 3512 RVA: 0x00042C20 File Offset: 0x00040E20
		public Consumption TryTrigger(IContactReactor icontactReactor, ContactEvent contactEvent, int compatibleMounts)
		{
			IVitalsContactReactor vitalsContactReactor = icontactReactor as IVitalsContactReactor;
			if (vitalsContactReactor == null)
			{
				return Consumption.None;
			}
			if (this.contactGroups != 0)
			{
				IContactGroupsAssign contactGroupsAssign = contactEvent.contactTrigger.ContactGroupsAssign;
				int num = (contactGroupsAssign == null) ? 0 : contactGroupsAssign.Mask;
				if ((this.contactGroups.Mask & num) == 0)
				{
					return Consumption.None;
				}
			}
			if (compatibleMounts != this.defaultMountingMask && (compatibleMounts & this.defaultMountingMask) == 0)
			{
				return Consumption.None;
			}
			Vital vital = this.vitals.GetVital(vitalsContactReactor.VitalNameType);
			if (vital == null)
			{
				return Consumption.None;
			}
			double amountConsumed;
			if (base.IsMine)
			{
				double discharge = vitalsContactReactor.DischargeValue(contactEvent.contactType);
				amountConsumed = this.vitals.ApplyCharges(discharge, vitalsContactReactor.AllowOverload, vitalsContactReactor.Propagate);
			}
			else
			{
				amountConsumed = vital.TestApplyChange(vitalsContactReactor, contactEvent);
			}
			IVitalsConsumable vitalsConsumable = icontactReactor as IVitalsConsumable;
			Consumption result;
			if (vitalsConsumable != null)
			{
				result = this.TestConsumption(amountConsumed, vitalsConsumable, contactEvent);
			}
			else
			{
				result = Consumption.None;
			}
			return result;
		}

		// Token: 0x06000DB9 RID: 3513 RVA: 0x00042CF8 File Offset: 0x00040EF8
		protected Consumption TestConsumption(double amountConsumed, IVitalsConsumable iva, ContactEvent contactEvent)
		{
			Consumption consumption = iva.Consumption;
			double num = iva.DischargeValue(contactEvent.contactType);
			if (consumption == Consumption.None)
			{
				return Consumption.None;
			}
			if (consumption != Consumption.All)
			{
				Consumption result = (amountConsumed == 0.0) ? Consumption.None : ((num == amountConsumed) ? Consumption.All : Consumption.Partial);
				iva.Charges -= amountConsumed;
				return result;
			}
			if (amountConsumed != 0.0)
			{
				iva.Charges = 0.0;
				return Consumption.All;
			}
			return Consumption.None;
		}

		// Token: 0x06000DBA RID: 3514 RVA: 0x00042D66 File Offset: 0x00040F66
		public Mount TryPickup(IContactReactor reactor, ContactEvent contactEvent)
		{
			return this.DefaultMount;
		}

		// Token: 0x06000DBB RID: 3515 RVA: 0x00042D6E File Offset: 0x00040F6E
		public void OnPostSimulate(int frameId, int subFrameId, bool isNetTick)
		{
			if (isNetTick)
			{
				this.vitals.Simulate();
			}
		}

		// Token: 0x06000DBC RID: 3516 RVA: 0x00042D80 File Offset: 0x00040F80
		public virtual void OnCaptureCurrentState(int frameId)
		{
			VitalData[] datas = this.frames[frameId].vitalsData.datas;
			for (int i = 0; i < this.vitalsCount; i++)
			{
				datas[i] = this.vitalArray[i].VitalData;
			}
		}

		// Token: 0x06000DBD RID: 3517 RVA: 0x00042DC8 File Offset: 0x00040FC8
		public SerializationFlags OnNetSerialize(int frameId, byte[] buffer, ref int bitposition, SerializationFlags writeFlags)
		{
			if (!base.enabled)
			{
				ArraySerializeExt.WriteBool(buffer, false, ref bitposition);
				return SerializationFlags.None;
			}
			SyncVitals.Frame frame = this.frames[frameId];
			ArraySerializeExt.WriteBool(buffer, true, ref bitposition);
			bool keyframe = base.IsKeyframe(frameId);
			return this.vitals.Serialize(frame.vitalsData, this.lastSentData, buffer, ref bitposition, keyframe);
		}

		// Token: 0x06000DBE RID: 3518 RVA: 0x00042E1C File Offset: 0x0004101C
		public SerializationFlags OnNetDeserialize(int originFrameId, byte[] buffer, ref int bitposition, FrameArrival arrival)
		{
			SyncVitals.Frame frame = base.IsMine ? this.offtickFrame : this.frames[originFrameId];
			if (!ArraySerializeExt.ReadBool(buffer, ref bitposition))
			{
				return SerializationFlags.None;
			}
			bool keyframe = base.IsKeyframe(originFrameId);
			SerializationFlags serializationFlags = this.vitals.Deserialize(frame.vitalsData, buffer, ref bitposition, keyframe);
			frame.content = (((serializationFlags & SerializationFlags.IsComplete) != SerializationFlags.None) ? FrameContents.Complete : (((serializationFlags & SerializationFlags.HasContent) != SerializationFlags.None) ? FrameContents.Partial : FrameContents.Empty));
			return serializationFlags;
		}

		// Token: 0x06000DBF RID: 3519 RVA: 0x00042E83 File Offset: 0x00041083
		[Obsolete("Use vitals.ApplyCharges() instead")]
		public double ApplyDamage(double damage)
		{
			if (!base.IsMine)
			{
				return damage;
			}
			if (damage == 0.0)
			{
				return damage;
			}
			return this.vitals.ApplyCharges(damage, false, true);
		}

		// Token: 0x06000DC0 RID: 3520 RVA: 0x00042EAC File Offset: 0x000410AC
		public void OnVitalValueChange(Vital vital)
		{
			if (vital.VitalData.Value <= 0.0)
			{
				this.RootVitalBecameZero(vital);
			}
			int i = 0;
			int count = this.OnVitalsValueChange.Count;
			while (i < count)
			{
				this.OnVitalsValueChange[i].OnVitalValueChange(vital);
				i++;
			}
		}

		// Token: 0x06000DC1 RID: 3521 RVA: 0x00042F04 File Offset: 0x00041104
		public void OnVitalParamChange(Vital vital)
		{
			global::Debug.LogError("Not implemented");
			int i = 0;
			int count = this.OnVitalsParamChange.Count;
			while (i < count)
			{
				this.OnVitalsParamChange[i].OnVitalParamChange(vital);
				i++;
			}
		}

		// Token: 0x06000DC2 RID: 3522 RVA: 0x00042F48 File Offset: 0x00041148
		protected virtual void RootVitalBecameZero(Vital vital)
		{
			int i = 0;
			int count = this.OnRootVitalBecameZero.Count;
			while (i < count)
			{
				this.OnRootVitalBecameZero[i].OnRootVitalBecameZero(vital, null);
				i++;
			}
			if (this.autoDespawn && this.syncState && this.rootVital == vital)
			{
				this.syncState.Despawn(false);
			}
		}

		// Token: 0x06000DC3 RID: 3523 RVA: 0x00042FAC File Offset: 0x000411AC
		public void OnStateChange(ObjState newState, ObjState previousState, Transform attachmentTransform, Mount attachTo = null, bool isReady = true)
		{
			if (this.wasDespawned && newState != ObjState.Despawned)
			{
				for (int i = 0; i < this.vitalsCount; i++)
				{
					this.vitalArray[i].ResetValues();
				}
			}
			this.wasDespawned = (newState == ObjState.Despawned);
		}

		// Token: 0x06000DC4 RID: 3524 RVA: 0x00042FEC File Offset: 0x000411EC
		protected override void ApplySnapshot(SyncVitals.Frame snapframe, SyncVitals.Frame targframe, bool snapIsValid, bool targIsValid)
		{
			if (snapIsValid && snapframe.content >= FrameContents.Extrapolated)
			{
				this.vitals.Apply(this.snapFrame.vitalsData);
			}
		}

		// Token: 0x06000DC5 RID: 3525 RVA: 0x00043010 File Offset: 0x00041210
		protected override void InterpolateFrame(SyncVitals.Frame targframe, SyncVitals.Frame startframe, SyncVitals.Frame endframe, float t)
		{
			targframe.CopyFrom(startframe);
		}

		// Token: 0x06000DC6 RID: 3526 RVA: 0x0004301C File Offset: 0x0004121C
		protected override void ExtrapolateFrame(SyncVitals.Frame prevframe, SyncVitals.Frame snapframe, SyncVitals.Frame targframe)
		{
			VitalData[] datas = snapframe.vitalsData.datas;
			VitalData[] datas2 = targframe.vitalsData.datas;
			for (int i = 0; i < this.vitalsCount; i++)
			{
				datas2[i] = this.vitalArray[i].VitalDef.Extrapolate(datas[i]);
			}
			targframe.content = FrameContents.Extrapolated;
		}

		// Token: 0x04000D00 RID: 3328
		public Vitals vitals = new Vitals();

		// Token: 0x04000D01 RID: 3329
		[SerializeField]
		protected ContactGroupMaskSelector contactGroups;

		// Token: 0x04000D02 RID: 3330
		[Tooltip("Vital triggers/pickups must have this as a valid mount type. When pickups will attach to this mount when picked up.")]
		public MountSelector defaultMounting = new MountSelector(0);

		// Token: 0x04000D04 RID: 3332
		[Tooltip("When root vital <= zero, syncState.Despawn() will be called. This allows for a default handling of object 'death'.")]
		public bool autoDespawn = true;

		// Token: 0x04000D05 RID: 3333
		[Tooltip("When OnStateChange changes from ObjState.Despawned to any other state, vital values will be reset to their starting defaults.")]
		public bool resetOnSpawn = true;

		// Token: 0x04000D06 RID: 3334
		[NonSerialized]
		private VitalsData lastSentData;

		// Token: 0x04000D07 RID: 3335
		public List<IOnVitalsValueChange> OnVitalsValueChange = new List<IOnVitalsValueChange>(0);

		// Token: 0x04000D08 RID: 3336
		public List<IOnVitalsParamChange> OnVitalsParamChange = new List<IOnVitalsParamChange>(0);

		// Token: 0x04000D09 RID: 3337
		public List<IOnRootVitalBecameZero> OnRootVitalBecameZero = new List<IOnRootVitalBecameZero>(0);

		// Token: 0x04000D0A RID: 3338
		protected SyncState syncState;

		// Token: 0x04000D0B RID: 3339
		private Vital[] vitalArray;

		// Token: 0x04000D0C RID: 3340
		protected Vital rootVital;

		// Token: 0x04000D0D RID: 3341
		private int vitalsCount;

		// Token: 0x04000D0E RID: 3342
		protected int defaultMountingMask;

		// Token: 0x04000D0F RID: 3343
		protected bool isPredicted;

		// Token: 0x04000D10 RID: 3344
		private bool wasDespawned;

		// Token: 0x020003CA RID: 970
		public class Frame : FrameBase
		{
			// Token: 0x060013D4 RID: 5076 RVA: 0x0005A3E9 File Offset: 0x000585E9
			public Frame()
			{
			}

			// Token: 0x060013D5 RID: 5077 RVA: 0x0005A3F1 File Offset: 0x000585F1
			public Frame(int frameId) : base(frameId)
			{
			}

			// Token: 0x060013D6 RID: 5078 RVA: 0x0005A412 File Offset: 0x00058612
			public Frame(int frameId, Vitals vitals) : base(frameId)
			{
				this.vitalsData = new VitalsData(vitals);
			}

			// Token: 0x060013D7 RID: 5079 RVA: 0x0005A428 File Offset: 0x00058628
			public override void CopyFrom(FrameBase sourceFrame)
			{
				base.CopyFrom(sourceFrame);
				VitalsData source = (sourceFrame as SyncVitals.Frame).vitalsData;
				this.vitalsData.CopyFrom(source);
			}

			// Token: 0x040012E9 RID: 4841
			public VitalsData vitalsData;
		}
	}
}
