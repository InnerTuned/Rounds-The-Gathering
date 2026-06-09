using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Compression;
using Photon.Compression.Internal;
using Photon.Pun.Simple.Internal;
using Photon.Realtime;
using Photon.Utilities;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x020002E5 RID: 741
	[DisallowMultipleComponent]
	[HelpURL("https://doc.photonengine.com/en-us/pun/current/gameplay/simple/simpleoverview")]
	[RequireComponent(typeof(PhotonView))]
	public class NetObject : MonoBehaviour, IMatchmakingCallbacks, IOnPhotonViewPreNetDestroy, IPhotonViewCallback, IOnPhotonViewOwnerChange, IOnPhotonViewControllerChange, IOnPreUpdate, IOnPreSimulate, IOnPostSimulate, IOnQuantize, IOnIncrementFrame, IOnPreQuit
	{
		// Token: 0x17000111 RID: 273
		// (get) Token: 0x06000FB6 RID: 4022 RVA: 0x0004C33D File Offset: 0x0004A53D
		public bool SkipWhenEmpty
		{
			get
			{
				return this.skipWhenEmpty;
			}
		}

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x06000FB7 RID: 4023 RVA: 0x0004C345 File Offset: 0x0004A545
		public bool IgnoreNonControllerUpdates
		{
			get
			{
				return this.ignoreNonControllerUpdates;
			}
		}

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x06000FB8 RID: 4024 RVA: 0x0004C34D File Offset: 0x0004A54D
		public bool ResimulateLateArrivals
		{
			get
			{
				return this.resimulateLateArrivals;
			}
		}

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x06000FB9 RID: 4025 RVA: 0x0004C355 File Offset: 0x0004A555
		public Rigidbody Rb
		{
			get
			{
				return this._rigidbody;
			}
		}

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x06000FBA RID: 4026 RVA: 0x0004C35D File Offset: 0x0004A55D
		public Rigidbody2D Rb2D
		{
			get
			{
				return this._rigidbody2D;
			}
		}

		// Token: 0x17000116 RID: 278
		// (set) Token: 0x06000FBB RID: 4027 RVA: 0x0004C368 File Offset: 0x0004A568
		public static bool NetObjDictsLocked
		{
			set
			{
				NetObject.netObjDictsLocked = value;
				if (!value)
				{
					int i = 0;
					int count = NetObject.pendingActiveNetObjDictChanges.Count;
					while (i < count)
					{
						NetObject netObject = NetObject.pendingActiveNetObjDictChanges.Dequeue();
						netObject.DetermineActiveAndControlled(netObject.photonView.IsMine);
						i++;
					}
				}
			}
		}

		// Token: 0x06000FBC RID: 4028 RVA: 0x0004C3B0 File Offset: 0x0004A5B0
		public void OnSyncObjReadyChange(SyncObject sobj, ReadyStateEnum readyState)
		{
			int syncObjIndex = sobj.SyncObjIndex;
			if (readyState != ReadyStateEnum.Unready)
			{
				this.syncObjReadyMask[syncObjIndex] = true;
			}
			else
			{
				this.syncObjReadyMask[syncObjIndex] = false;
			}
			this.AllObjsAreReady = (this.syncObjReadyMask.AllAreTrue && this.packObjReadyMask.AllAreTrue);
		}

		// Token: 0x06000FBD RID: 4029 RVA: 0x0004C404 File Offset: 0x0004A604
		public void OnPackObjReadyChange(Component pobj, ReadyStateEnum readyState)
		{
			int bit = this.packObjIndexLookup[pobj];
			if (readyState != ReadyStateEnum.Unready)
			{
				this.packObjReadyMask[bit] = true;
			}
			else
			{
				this.packObjReadyMask[bit] = false;
			}
			this.AllObjsAreReady = (this.syncObjReadyMask.AllAreTrue && this.packObjReadyMask.AllAreTrue);
		}

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x06000FBE RID: 4030 RVA: 0x0004C45E File Offset: 0x0004A65E
		// (set) Token: 0x06000FBF RID: 4031 RVA: 0x0004C478 File Offset: 0x0004A678
		public bool AllObjsAreReady
		{
			get
			{
				return this.photonView.IsMine || this._allObjsAreReady;
			}
			private set
			{
				if (this._allObjsAreReady == value)
				{
					return;
				}
				this._allObjsAreReady = value;
				for (int i = 0; i < this.onNetObjReadyCallbacks.Count; i++)
				{
					this.onNetObjReadyCallbacks[i].OnNetObjReadyChange(value);
				}
				this.packObjReadyMask.SetAllTrue();
				this.syncObjReadyMask.SetAllTrue();
			}
		}

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x06000FC0 RID: 4032 RVA: 0x0004C4D4 File Offset: 0x0004A6D4
		public int ViewID
		{
			get
			{
				return this.viewID;
			}
		}

		// Token: 0x06000FC1 RID: 4033 RVA: 0x0004C4DC File Offset: 0x0004A6DC
		protected void Awake()
		{
			this.frameValidMask = new FastBitMask128(TickEngineSettings.frameCount);
			this.originHistory = new int[TickEngineSettings.frameCount];
			int i = 0;
			int frameCount = TickEngineSettings.frameCount;
			while (i < frameCount)
			{
				this.originHistory[i] = -1;
				i++;
			}
			if (!this._rigidbody)
			{
				this._rigidbody = NestedComponentUtilities.GetNestedComponentInChildren<Rigidbody, NetObject>(base.transform, true);
			}
			if (!this._rigidbody)
			{
				this._rigidbody2D = NestedComponentUtilities.GetNestedComponentInChildren<Rigidbody2D, NetObject>(base.transform, true);
			}
			this.photonView = base.GetComponent<PhotonView>();
			if (this.photonView == null)
			{
				global::Debug.LogWarning("PhotonView missing from NetObject on GameObject '" + base.name + "'. One will be added to suppress errors, but this object will likely not be networked correctly.");
				this.photonView = base.gameObject.AddComponent<PhotonView>();
			}
			this.CollectAndReorderInterfaces();
			this.IndexColliders();
			NestedComponentUtilities.GetNestedComponentsInChildren<IOnAwake, NetObject>(base.transform, NetObject.onAwakeCallbacks, true);
			int j = 0;
			int count = NetObject.onAwakeCallbacks.Count;
			while (j < count)
			{
				NetObject.onAwakeCallbacks[j].OnAwake();
				j++;
			}
		}

		// Token: 0x06000FC2 RID: 4034 RVA: 0x0004C5EC File Offset: 0x0004A7EC
		private void Start()
		{
			NestedComponentUtilities.GetNestedComponentsInChildren<IOnStart, NetObject>(base.transform, NetObject.onStartCallbacks, true);
			int i = 0;
			int count = NetObject.onStartCallbacks.Count;
			while (i < count)
			{
				NetObject.onStartCallbacks[i].OnStart();
				i++;
			}
			if (PhotonNetwork.IsConnectedAndReady)
			{
				this.OnChangeAuthority(this.photonView.IsMine, true);
			}
			this.viewID = this.photonView.ViewID;
		}

		// Token: 0x06000FC3 RID: 4035 RVA: 0x0004C65C File Offset: 0x0004A85C
		private void OnEnable()
		{
			NetMasterCallbacks.RegisterCallbackInterfaces(this, true, false);
			PhotonNetwork.AddCallbackTarget(this);
			this.photonView.AddCallbackTarget(this);
			this.DetermineActiveAndControlled(this.photonView.IsMine);
			int i = 0;
			int count = this.onEnableCallbacks.Count;
			while (i < count)
			{
				this.onEnableCallbacks[i].OnPostEnable();
				i++;
			}
		}

		// Token: 0x06000FC4 RID: 4036 RVA: 0x0004C6C0 File Offset: 0x0004A8C0
		private void OnDisable()
		{
			PhotonNetwork.RemoveCallbackTarget(this);
			this.photonView.RemoveCallbackTarget(this);
			NetMasterCallbacks.RegisterCallbackInterfaces(this, false, false);
			this.DetermineActiveAndControlled(this.photonView.IsMine);
			int i = 0;
			int count = this.onDisableCallbacks.Count;
			while (i < count)
			{
				this.onDisableCallbacks[i].OnPostDisable();
				i++;
			}
		}

		// Token: 0x06000FC5 RID: 4037 RVA: 0x0004C724 File Offset: 0x0004A924
		public void OnPreQuit()
		{
			int i = 0;
			int count = this.onPreQuitCallbacks.Count;
			while (i < this.onPreQuitCallbacks.Count)
			{
				this.onPreQuitCallbacks[i].OnPreQuit();
				i++;
			}
		}

		// Token: 0x06000FC6 RID: 4038 RVA: 0x0004C764 File Offset: 0x0004A964
		public void OnPreNetDestroy(PhotonView rootView)
		{
			NetObject component = rootView.GetComponent<NetObject>();
			if (component == null)
			{
				return;
			}
			int i = 0;
			int count = this.onPreNetDestroyCallbacks.Count;
			while (i < count)
			{
				this.onPreNetDestroyCallbacks[i].OnPreNetDestroy(component);
				i++;
			}
		}

		// Token: 0x06000FC7 RID: 4039 RVA: 0x0004C7AC File Offset: 0x0004A9AC
		private void OnDestroy()
		{
			if (NetObject.activeControlledNetObjs.ContainsKey(this.photonView.ViewID))
			{
				NetObject.activeControlledNetObjs.Remove(this.photonView.ViewID);
			}
			if (NetObject.activeUncontrolledNetObjs.ContainsKey(this.photonView.ViewID))
			{
				NetObject.activeUncontrolledNetObjs.Remove(this.photonView.ViewID);
			}
		}

		// Token: 0x06000FC8 RID: 4040 RVA: 0x0004C814 File Offset: 0x0004AA14
		public virtual void PrepareForDestroy()
		{
			MountsManager component = base.GetComponent<MountsManager>();
			if (component)
			{
				component.UnmountAll();
			}
		}

		// Token: 0x06000FC9 RID: 4041 RVA: 0x0004C836 File Offset: 0x0004AA36
		public void OnOwnerChange(Player newOwner, Player previousOwner)
		{
			this.OnChangeAuthority(this.photonView.IsMine, true);
		}

		// Token: 0x06000FCA RID: 4042 RVA: 0x0004C836 File Offset: 0x0004AA36
		public void OnControllerChange(Player newController, Player previousController)
		{
			this.OnChangeAuthority(this.photonView.IsMine, true);
		}

		// Token: 0x06000FCB RID: 4043 RVA: 0x000027C8 File Offset: 0x000009C8
		public void OnFriendListUpdate(List<FriendInfo> friendList)
		{
		}

		// Token: 0x06000FCC RID: 4044 RVA: 0x000027C8 File Offset: 0x000009C8
		public void OnCreatedRoom()
		{
		}

		// Token: 0x06000FCD RID: 4045 RVA: 0x000027C8 File Offset: 0x000009C8
		public void OnCreateRoomFailed(short returnCode, string message)
		{
		}

		// Token: 0x06000FCE RID: 4046 RVA: 0x0004C84C File Offset: 0x0004AA4C
		public void OnJoinedRoom()
		{
			NestedComponentUtilities.GetNestedComponentsInChildren<IOnJoinedRoom, NetObject>(base.transform, NetObject.onJoinedRoomCallbacks, true);
			int i = 0;
			int count = NetObject.onJoinedRoomCallbacks.Count;
			while (i < count)
			{
				NetObject.onJoinedRoomCallbacks[i].OnJoinedRoom();
				i++;
			}
			this.OnChangeAuthority(this.photonView.IsMine, true);
		}

		// Token: 0x06000FCF RID: 4047 RVA: 0x000027C8 File Offset: 0x000009C8
		public void OnJoinRoomFailed(short returnCode, string message)
		{
		}

		// Token: 0x06000FD0 RID: 4048 RVA: 0x000027C8 File Offset: 0x000009C8
		public void OnJoinRandomFailed(short returnCode, string message)
		{
		}

		// Token: 0x06000FD1 RID: 4049 RVA: 0x000027C8 File Offset: 0x000009C8
		public void OnLeftRoom()
		{
		}

		// Token: 0x06000FD2 RID: 4050 RVA: 0x0004C8A4 File Offset: 0x0004AAA4
		private void DetermineActiveAndControlled(bool amController)
		{
			int num = this.photonView.ViewID;
			if (NetObject.netObjDictsLocked)
			{
				NetObject.pendingActiveNetObjDictChanges.Enqueue(this);
				return;
			}
			bool flag = NetObject.activeControlledNetObjs.ContainsKey(num);
			bool flag2 = NetObject.activeUncontrolledNetObjs.ContainsKey(num);
			if (base.isActiveAndEnabled)
			{
				if (amController)
				{
					if (!flag)
					{
						NetObject.activeControlledNetObjs.Add(num, this);
					}
					if (flag2)
					{
						NetObject.activeUncontrolledNetObjs.Remove(num);
						return;
					}
				}
				else
				{
					if (flag)
					{
						NetObject.activeControlledNetObjs.Remove(num);
					}
					if (!flag2)
					{
						NetObject.activeUncontrolledNetObjs.Add(num, this);
						return;
					}
				}
			}
			else
			{
				if (flag)
				{
					NetObject.activeControlledNetObjs.Remove(num);
				}
				if (flag2)
				{
					NetObject.activeUncontrolledNetObjs.Remove(num);
				}
			}
		}

		// Token: 0x06000FD3 RID: 4051 RVA: 0x0004C950 File Offset: 0x0004AB50
		public void OnChangeAuthority(bool isMine, bool controllerHasChanged)
		{
			this.DetermineActiveAndControlled(isMine);
			int i = 0;
			int count = this.onAuthorityChangedCallbacks.Count;
			while (i < count)
			{
				this.onAuthorityChangedCallbacks[i].OnAuthorityChanged(isMine, controllerHasChanged);
				i++;
			}
			if (isMine)
			{
				this.AllObjsAreReady = true;
			}
		}

		// Token: 0x06000FD4 RID: 4052 RVA: 0x0004C99C File Offset: 0x0004AB9C
		private void CollectAndReorderInterfaces()
		{
			NestedComponentUtilities.GetNestedComponentsInChildren<Component, NetObject>(base.transform, NetObject.reusableComponents, true);
			int i = 0;
			int count = NetObject.reusableComponents.Count;
			while (i <= 24)
			{
				for (int j = 0; j < count; j++)
				{
					Component component = NetObject.reusableComponents[j];
					if (!(component == this))
					{
						IApplyOrder applyOrder = component as IApplyOrder;
						if (applyOrder == null)
						{
							if (i == 13)
							{
								this.AddInterfaces(component, false);
								this.AddPackObjects(component);
							}
						}
						else if (applyOrder.ApplyOrder == i)
						{
							this.AddInterfaces(component, false);
						}
					}
				}
				i++;
			}
			this.syncObjReadyMask = new FastBitMask128(this.syncObjects.Count);
			this.packObjReadyMask = new FastBitMask128(this.packObjects.Count);
			for (int k = 0; k < this.syncObjects.Count; k++)
			{
				SyncObject syncObject = this.syncObjects[k];
				syncObject.SyncObjIndex = k;
				this.OnSyncObjReadyChange(syncObject, syncObject.ReadyState);
			}
		}

		// Token: 0x06000FD5 RID: 4053 RVA: 0x0004CA95 File Offset: 0x0004AC95
		public void RemoveInterfaces(Component comp)
		{
			this.AddInterfaces(comp, true);
		}

		// Token: 0x06000FD6 RID: 4054 RVA: 0x0004CAA0 File Offset: 0x0004ACA0
		private void AddInterfaces(Component comp, bool remove = false)
		{
			this.AddInterfaceToList<IOnEnable>(comp, this.onEnableCallbacks, remove, false);
			this.AddInterfaceToList<IOnDisable>(comp, this.onDisableCallbacks, remove, false);
			this.AddInterfaceToList<IOnPreUpdate>(comp, this.onPreUpdateCallbacks, remove, false);
			this.AddInterfaceToList<IOnAuthorityChanged>(comp, this.onAuthorityChangedCallbacks, remove, false);
			this.AddInterfaceToList<IOnCaptureState>(comp, this.onCaptureCurrentStateCallbacks, remove, false);
			this.AddInterfaceToList<IOnNetSerialize>(comp, this.onNetSerializeCallbacks, remove, true);
			this.AddInterfaceToList<IOnQuantize>(comp, this.onQuantizeCallbacks, remove, true);
			this.AddInterfaceToList<IOnIncrementFrame>(comp, this.onIncrementFramesCallbacks, remove, true);
			this.AddInterfaceToList<IOnSnapshot>(comp, this.onSnapshotCallbacks, remove, true);
			this.AddInterfaceToList<IOnCriticallyLateFrame>(comp, this.onCriticallyLateFrameCallbacks, remove, true);
			this.AddInterfaceToList<IOnInterpolate>(comp, this.onInterpolateCallbacks, remove, true);
			this.AddInterfaceToList<IOnPreSimulate>(comp, this.onPreSimulateCallbacks, remove, false);
			this.AddInterfaceToList<IOnPostSimulate>(comp, this.onPostSimulateCallbacks, remove, false);
			this.AddInterfaceToList<IOnPreQuit>(comp, this.onPreQuitCallbacks, remove, false);
			this.AddInterfaceToList<IOnPreNetDestroy>(comp, this.onPreNetDestroyCallbacks, remove, false);
			this.AddInterfaceToList<IOnNetObjReady>(comp, this.onNetObjReadyCallbacks, remove, false);
			this.AddInterfaceToList<SyncObject>(comp, this.syncObjects, remove, false);
		}

		// Token: 0x06000FD7 RID: 4055 RVA: 0x0004CBAC File Offset: 0x0004ADAC
		private void AddInterfaceToList<T>(object comp, List<T> list, bool remove, bool checkSerializationOptional = false) where T : class
		{
			T t = comp as T;
			if (t != null)
			{
				if (checkSerializationOptional)
				{
					ISerializationOptional serializationOptional = t as ISerializationOptional;
					if (serializationOptional != null && !serializationOptional.IncludeInSerialization)
					{
						return;
					}
				}
				T t2 = comp as T;
				if (remove && list.Contains(t2))
				{
					list.Remove(t2);
					return;
				}
				list.Add(t2);
			}
		}

		// Token: 0x06000FD8 RID: 4056 RVA: 0x0004CC14 File Offset: 0x0004AE14
		private void AddPackObjects(Component comp)
		{
			if (comp == null)
			{
				return;
			}
			Type type = comp.GetType();
			if (comp.GetType().GetCustomAttributes(typeof(PackObjectAttribute), false).Length != 0)
			{
				PackObjectDatabase.PackObjectInfo packObjectInfo = PackObjectDatabase.GetPackObjectInfo(type);
				if (packObjectInfo == null)
				{
					return;
				}
				NetObject.PackObjRecord packObjRecord = new NetObject.PackObjRecord
				{
					component = comp,
					onReadyCallback = (comp as IPackObjOnReadyChange),
					info = packObjectInfo,
					packFrames = packObjectInfo.FactoryFramesObj.Invoke(comp, TickEngineSettings.frameCount),
					prevReadyMask = new FastBitMask128(packObjectInfo.fieldCount),
					readyMask = new FastBitMask128(packObjectInfo.fieldCount)
				};
				this.packObjIndexLookup.Add(comp, this.packObjects.Count);
				this.packObjects.Add(packObjRecord);
			}
		}

		// Token: 0x06000FD9 RID: 4057 RVA: 0x0004CCD8 File Offset: 0x0004AED8
		public void OnPreUpdate()
		{
			int i = 0;
			int count = this.onPreUpdateCallbacks.Count;
			while (i < count)
			{
				this.onPreUpdateCallbacks[i].OnPreUpdate();
				i++;
			}
		}

		// Token: 0x06000FDA RID: 4058 RVA: 0x0004CD0E File Offset: 0x0004AF0E
		public SerializationFlags GenerateMessage(int frameId, byte[] buffer, ref int bitposition, SerializationFlags writeFlags)
		{
			this.OnCaptureCurrentState(frameId);
			this.OnQuantize(frameId);
			return this.OnSerialize(frameId, buffer, ref bitposition, writeFlags);
		}

		// Token: 0x06000FDB RID: 4059 RVA: 0x0004CD2C File Offset: 0x0004AF2C
		public SerializationFlags OnSerialize(int frameId, byte[] buffer, ref int bitposition, SerializationFlags writeFlags)
		{
			SerializationFlags serializationFlags = writeFlags;
			int num = ((frameId == 0) ? TickEngineSettings.frameCount : frameId) - 1;
			int count = this.packObjects.Count;
			for (int i = 0; i < count; i++)
			{
				NetObject.PackObjRecord packObjRecord = this.packObjects[i];
				PackFrame packFrame = packObjRecord.packFrames[frameId];
				int fieldCount = packObjRecord.info.fieldCount;
				int num2 = bitposition;
				bitposition += fieldCount;
				int num3 = 0;
				serializationFlags |= packObjRecord.info.PackFrameToBuffer(packFrame, packObjRecord.packFrames[num], ref packFrame.mask, ref num3, buffer, ref bitposition, frameId, writeFlags);
				for (int j = 0; j < fieldCount; j++)
				{
					ArraySerializeExt.WriteBool(buffer, packFrame.mask[j], ref num2);
				}
			}
			int k = 0;
			int count2 = this.onNetSerializeCallbacks.Count;
			while (k < count2)
			{
				serializationFlags |= this.onNetSerializeCallbacks[k].OnNetSerialize(frameId, buffer, ref bitposition, writeFlags);
				k++;
			}
			return serializationFlags;
		}

		// Token: 0x06000FDC RID: 4060 RVA: 0x0004CE2C File Offset: 0x0004B02C
		public void OnDeserialize(int connId, int originFrameId, byte[] buffer, ref int bitposition, bool hasData, FrameArrival arrival)
		{
			if (hasData)
			{
				this.frameValidMask[originFrameId] = true;
				this.originHistory[originFrameId] = connId;
				int count = this.packObjects.Count;
				for (int i = 0; i < count; i++)
				{
					NetObject.PackObjRecord packObjRecord = this.packObjects[i];
					PackFrame packFrame = packObjRecord.packFrames[originFrameId];
					int fieldCount = packObjRecord.info.fieldCount;
					for (int j = 0; j < fieldCount; j++)
					{
						packFrame.mask[j] = ArraySerializeExt.ReadBool(buffer, ref bitposition);
					}
					int num = 0;
					SerializationFlags serializationFlags = packObjRecord.info.UnpackFrameFromBuffer(packFrame, ref packFrame.mask, ref packFrame.isCompleteMask, ref num, buffer, ref bitposition, originFrameId, SerializationFlags.None);
					if (arrival >= FrameArrival.IsSnap && !packObjRecord.readyMask.AllAreTrue && (serializationFlags & SerializationFlags.IsComplete) != SerializationFlags.None)
					{
						packObjRecord.readyMask.OR(packObjRecord.info.defaultReadyMask);
						packObjRecord.readyMask.OR(packFrame.isCompleteMask);
						FastBitMask128 fastBitMask = !packObjRecord.readyMask & packFrame.mask;
						num = 0;
						packObjRecord.info.CopyFrameToObj(packFrame, packObjRecord.component, ref fastBitMask, ref num);
						this.BroadcastReadyMaskChange(packObjRecord);
					}
				}
				int k = 0;
				int count2 = this.onNetSerializeCallbacks.Count;
				while (k < count2)
				{
					this.onNetSerializeCallbacks[k].OnNetDeserialize(originFrameId, buffer, ref bitposition, arrival);
					k++;
				}
				if (this.resimulateLateArrivals && arrival >= FrameArrival.IsSnap)
				{
					int frameCount = TickEngineSettings.frameCount;
					int num2 = originFrameId + 1;
					if (num2 >= frameCount)
					{
						num2 -= frameCount;
					}
					int num3 = originFrameId;
					int num4 = originFrameId - 1;
					if (num4 < 0)
					{
						num4 += frameCount;
					}
					for (int l = 0; l <= (int)arrival; l++)
					{
						int m = 0;
						int count3 = this.onSnapshotCallbacks.Count;
						while (m < count3)
						{
							bool prevIsValid = this.frameValidMask[num4];
							bool snapIsValid = this.frameValidMask[num3];
							bool targIsValid = this.frameValidMask[num2];
							this.onSnapshotCallbacks[m].OnSnapshot(num4, num3, num2, prevIsValid, snapIsValid, targIsValid);
							m++;
						}
						if (l == (int)arrival)
						{
							break;
						}
						num4 = num3;
						num3 = num2;
						num2++;
						if (num2 >= frameCount)
						{
							num2 -= frameCount;
						}
					}
					int n = 0;
					int count4 = this.onCriticallyLateFrameCallbacks.Count;
					while (n < count4)
					{
						this.onCriticallyLateFrameCallbacks[n].HandleCriticallyLateFrame(originFrameId);
						n++;
					}
				}
			}
		}

		// Token: 0x06000FDD RID: 4061 RVA: 0x0004D0B4 File Offset: 0x0004B2B4
		public void OnPreSimulate(int frameId, int _currSubFrameId)
		{
			int i = 0;
			int count = this.onPreSimulateCallbacks.Count;
			while (i < count)
			{
				IOnPreSimulate onPreSimulate = this.onPreSimulateCallbacks[i];
				Behaviour behaviour = onPreSimulate as Behaviour;
				if (behaviour.enabled && behaviour.gameObject.activeInHierarchy)
				{
					onPreSimulate.OnPreSimulate(frameId, _currSubFrameId);
				}
				i++;
			}
		}

		// Token: 0x06000FDE RID: 4062 RVA: 0x0004D10C File Offset: 0x0004B30C
		public void OnPostSimulate(int frameId, int subFrameId, bool isNetTick)
		{
			int i = 0;
			int count = this.onPostSimulateCallbacks.Count;
			while (i < count)
			{
				IOnPostSimulate onPostSimulate = this.onPostSimulateCallbacks[i];
				Behaviour behaviour = onPostSimulate as Behaviour;
				if (behaviour.enabled && behaviour.gameObject.activeInHierarchy)
				{
					onPostSimulate.OnPostSimulate(frameId, subFrameId, isNetTick);
				}
				i++;
			}
		}

		// Token: 0x06000FDF RID: 4063 RVA: 0x0004D164 File Offset: 0x0004B364
		public SerializationFlags OnNetSerialize(int frameId, byte[] buffer, ref int bitposition, SerializationFlags writeFlags)
		{
			if (this.photonView.Group != 0)
			{
				buffer = NetMsgSends.reusableNetObjBuffer;
				int num = 0;
				ArraySerializeExt.Write(buffer, (ulong)frameId, ref num, TickEngineSettings.frameCountBits);
				ArrayPackBytesExt.WritePackedBytes(buffer, (ulong)this.viewID, ref num, 32);
				int num2 = num;
				ArraySerializeExt.WriteBool(buffer, true, ref num);
				int num3 = num;
				num += 16;
				SerializationFlags serializationFlags = this.GenerateMessage(frameId, buffer, ref num, writeFlags);
				if (serializationFlags == SerializationFlags.None)
				{
					if (this.skipWhenEmpty)
					{
						return SerializationFlags.None;
					}
					num = num2;
					ArraySerializeExt.WriteBool(buffer, false, ref bitposition);
				}
				if (serializationFlags != SerializationFlags.None || !this.SkipWhenEmpty)
				{
					ArraySerializeExt.Write(buffer, (ulong)(num - num3), ref num3, 16);
					ArrayPackBytesExt.WritePackedBytes(buffer, 0UL, ref num, 32);
					buffer.Send(num, base.gameObject, serializationFlags, false);
				}
				return SerializationFlags.None;
			}
			return this.GenerateMessage(frameId, buffer, ref bitposition, writeFlags);
		}

		// Token: 0x06000FE0 RID: 4064 RVA: 0x0004D224 File Offset: 0x0004B424
		public void OnCaptureCurrentState(int frameId)
		{
			int count = this.packObjects.Count;
			for (int i = 0; i < count; i++)
			{
				NetObject.PackObjRecord packObjRecord = this.packObjects[i];
				packObjRecord.info.CaptureObj.Invoke(packObjRecord.component, packObjRecord.packFrames[frameId]);
			}
			int j = 0;
			int count2 = this.onCaptureCurrentStateCallbacks.Count;
			while (j < count2)
			{
				this.onCaptureCurrentStateCallbacks[j].OnCaptureCurrentState(frameId);
				j++;
			}
		}

		// Token: 0x06000FE1 RID: 4065 RVA: 0x0004D2A0 File Offset: 0x0004B4A0
		public void OnQuantize(int frameId)
		{
			int i = 0;
			int count = this.onQuantizeCallbacks.Count;
			while (i < count)
			{
				IOnQuantize onQuantize = this.onQuantizeCallbacks[i];
				Behaviour behaviour = onQuantize as Behaviour;
				if (behaviour.enabled && behaviour.gameObject.activeInHierarchy)
				{
					onQuantize.OnQuantize(frameId);
				}
				i++;
			}
		}

		// Token: 0x06000FE2 RID: 4066 RVA: 0x0004D2F8 File Offset: 0x0004B4F8
		public void OnIncrementFrame(int newFrameId, int newSubFrameId, int previousFrameId, int prevSubFrameId)
		{
			int i = 0;
			int count = this.onIncrementFramesCallbacks.Count;
			while (i < count)
			{
				this.onIncrementFramesCallbacks[i].OnIncrementFrame(newFrameId, newSubFrameId, previousFrameId, prevSubFrameId);
				i++;
			}
		}

		// Token: 0x06000FE3 RID: 4067 RVA: 0x0004D334 File Offset: 0x0004B534
		public bool OnSnapshot(int localTargFrameId)
		{
			if (!this.photonView)
			{
				return false;
			}
			if (!this.photonView.enabled)
			{
				return false;
			}
			ConnectionTickOffsets connectionTickOffsets;
			if (!TickManager.perConnOffsets.TryGetValue(this.photonView.ControllerActorNr, ref connectionTickOffsets))
			{
				return false;
			}
			if (connectionTickOffsets == null)
			{
				return false;
			}
			if (!connectionTickOffsets.hadInitialSnapshot)
			{
				return false;
			}
			int advanceCount = connectionTickOffsets.advanceCount;
			if (advanceCount == 0)
			{
				return false;
			}
			int frameCount = TickEngineSettings.frameCount;
			int num = connectionTickOffsets.ConvertFrameLocalToOrigin(localTargFrameId);
			int num2 = num - 1;
			if (num2 < 0)
			{
				num2 += frameCount;
			}
			int num3 = num - 2;
			if (num3 < 0)
			{
				num3 += frameCount;
			}
			int frameCount2 = TickEngineSettings.frameCount;
			for (int i = 0; i < advanceCount; i++)
			{
				int num4 = num + i;
				if (num4 >= frameCount2)
				{
					num4 -= frameCount2;
				}
				int num5 = num4 - TickEngineSettings.halfFrameCount;
				if (num5 < 0)
				{
					num5 += frameCount2;
				}
				bool flag = this.frameValidMask[num2];
				bool flag2 = this.frameValidMask[num4];
				int count = this.packObjects.Count;
				for (int j = 0; j < count; j++)
				{
					NetObject.PackObjRecord packObjRecord = this.packObjects[j];
					PackFrame packFrame = packObjRecord.packFrames[num2];
					PackFrame packFrame2 = packObjRecord.packFrames[num4];
					packObjRecord.readyMask.OR(packObjRecord.info.defaultReadyMask);
					packObjRecord.readyMask.OR(packFrame.isCompleteMask);
					if (!flag2)
					{
						packObjRecord.info.CopyFrameToFrame.Invoke(packFrame, packFrame2);
					}
					int num6 = 0;
					packObjRecord.info.SnapObject(packFrame, packFrame2, packObjRecord.component, ref packObjRecord.readyMask, ref num6);
					if (flag)
					{
						num6 = 0;
						packObjRecord.info.CopyFrameToObj(packFrame, packObjRecord.component, ref packFrame.mask, ref num6);
					}
					if (!packObjRecord.readyMask.Compare(packObjRecord.prevReadyMask))
					{
						this.BroadcastReadyMaskChange(packObjRecord);
					}
				}
				bool prevIsValid = this.frameValidMask[num3];
				bool snapIsValid = this.frameValidMask[num2];
				bool targIsValid = this.frameValidMask[num4];
				int k = 0;
				int count2 = this.onSnapshotCallbacks.Count;
				while (k < count2)
				{
					this.onSnapshotCallbacks[k].OnSnapshot(num3, num2, num4, prevIsValid, snapIsValid, targIsValid);
					k++;
				}
				num3 = num2;
				num2 = num4;
				this.frameValidMask[num5] = false;
			}
			return true;
		}

		// Token: 0x06000FE4 RID: 4068 RVA: 0x0004D5AC File Offset: 0x0004B7AC
		private void BroadcastReadyMaskChange(NetObject.PackObjRecord p)
		{
			this.OnPackObjReadyChange(p.component, p.readyMask.AllAreTrue ? ReadyStateEnum.Ready : ReadyStateEnum.Unready);
			IPackObjOnReadyChange onReadyCallback = p.onReadyCallback;
			if (onReadyCallback != null)
			{
				onReadyCallback.OnPackObjReadyChange(p.readyMask, p.readyMask.AllAreTrue);
			}
			p.prevReadyMask.Copy(p.readyMask);
		}

		// Token: 0x06000FE5 RID: 4069 RVA: 0x0004D608 File Offset: 0x0004B808
		public bool OnInterpolate(int localSnapFrameId, int localTargFrameId, float t)
		{
			ConnectionTickOffsets connectionTickOffsets;
			if (!TickManager.perConnOffsets.TryGetValue(this.photonView.ControllerActorNr, ref connectionTickOffsets))
			{
				return false;
			}
			if (connectionTickOffsets == null)
			{
				return false;
			}
			if (!connectionTickOffsets.hadInitialSnapshot)
			{
				return false;
			}
			int num = connectionTickOffsets.ConvertFrameLocalToOrigin(localSnapFrameId);
			int num2 = connectionTickOffsets.ConvertFrameLocalToOrigin(localTargFrameId);
			if (connectionTickOffsets.validFrameMask[num2])
			{
				int count = this.packObjects.Count;
				for (int i = 0; i < count; i++)
				{
					NetObject.PackObjRecord packObjRecord = this.packObjects[i];
					PackFrame start = packObjRecord.packFrames[num];
					PackFrame end = packObjRecord.packFrames[num2];
					int num3 = 0;
					packObjRecord.info.InterpFrameToObj(start, end, packObjRecord.component, t, ref packObjRecord.readyMask, ref num3);
				}
			}
			int j = 0;
			int count2 = this.onInterpolateCallbacks.Count;
			while (j < count2)
			{
				IOnInterpolate onInterpolate = this.onInterpolateCallbacks[j];
				Behaviour behaviour = onInterpolate as Behaviour;
				if (behaviour.enabled && behaviour.gameObject.activeInHierarchy)
				{
					onInterpolate.OnInterpolate(num, num2, t);
				}
				j++;
			}
			return true;
		}

		// Token: 0x04000EBD RID: 3773
		[SerializeField]
		[HideInInspector]
		[Tooltip("Enabling this will tell the serializer to completely exclude this net object from serialization if none of its content has changed. While this will remove heartbeat data, It may also produce undesirable extrapolation and buffer resizing behavior, as receiving clients will see this as a network failure.")]
		protected bool skipWhenEmpty;

		// Token: 0x04000EBE RID: 3774
		[SerializeField]
		[HideInInspector]
		[Tooltip("Controls if incoming NetObject updates from a current non-owner/controller should be ignored. The exception to this is if the controller is currently null/-1, which indicates that the initial ownership messages from the Master has not yet arrived or been applied, in which case the first arriving updates originating Player will be treated as the current Controller, regardless of this setting.")]
		protected bool ignoreNonControllerUpdates = true;

		// Token: 0x04000EBF RID: 3775
		[SerializeField]
		[HideInInspector]
		[Tooltip("When enabled, if a frame update for this Net Object arrives AFTER that frame number has already been applied (it will have been reconstructed/extrapolated with a best guess), the incoming update will be immediately applied, and all frames between that frame and the current snapshot will be reapplied.")]
		protected bool resimulateLateArrivals = true;

		// Token: 0x04000EC0 RID: 3776
		protected Rigidbody _rigidbody;

		// Token: 0x04000EC1 RID: 3777
		protected Rigidbody2D _rigidbody2D;

		// Token: 0x04000EC2 RID: 3778
		public static NonAllocDictionary<int, NetObject> activeControlledNetObjs = new NonAllocDictionary<int, NetObject>(29U);

		// Token: 0x04000EC3 RID: 3779
		public static NonAllocDictionary<int, NetObject> activeUncontrolledNetObjs = new NonAllocDictionary<int, NetObject>(29U);

		// Token: 0x04000EC4 RID: 3780
		private static Queue<NetObject> pendingActiveNetObjDictChanges = new Queue<NetObject>();

		// Token: 0x04000EC5 RID: 3781
		private static bool netObjDictsLocked;

		// Token: 0x04000EC6 RID: 3782
		[NonSerialized]
		public Dictionary<Component, int> colliderLookup = new Dictionary<Component, int>();

		// Token: 0x04000EC7 RID: 3783
		[NonSerialized]
		public List<Component> indexedColliders = new List<Component>();

		// Token: 0x04000EC8 RID: 3784
		[NonSerialized]
		public int bitsForColliderIndex;

		// Token: 0x04000EC9 RID: 3785
		[NonSerialized]
		public FastBitMask128 frameValidMask;

		// Token: 0x04000ECA RID: 3786
		[NonSerialized]
		public int[] originHistory;

		// Token: 0x04000ECB RID: 3787
		[NonSerialized]
		public FastBitMask128 syncObjReadyMask;

		// Token: 0x04000ECC RID: 3788
		[NonSerialized]
		public FastBitMask128 packObjReadyMask;

		// Token: 0x04000ECD RID: 3789
		[NonSerialized]
		private readonly Dictionary<Component, int> packObjIndexLookup = new Dictionary<Component, int>();

		// Token: 0x04000ECE RID: 3790
		private bool _allObjsAreReady;

		// Token: 0x04000ECF RID: 3791
		protected int viewID;

		// Token: 0x04000ED0 RID: 3792
		[NonSerialized]
		public PhotonView photonView;

		// Token: 0x04000ED1 RID: 3793
		private static List<Component> reusableComponents = new List<Component>();

		// Token: 0x04000ED2 RID: 3794
		private static readonly List<IOnJoinedRoom> onJoinedRoomCallbacks = new List<IOnJoinedRoom>();

		// Token: 0x04000ED3 RID: 3795
		private static readonly List<IOnAwake> onAwakeCallbacks = new List<IOnAwake>();

		// Token: 0x04000ED4 RID: 3796
		private static readonly List<IOnStart> onStartCallbacks = new List<IOnStart>();

		// Token: 0x04000ED5 RID: 3797
		private readonly List<IOnEnable> onEnableCallbacks = new List<IOnEnable>();

		// Token: 0x04000ED6 RID: 3798
		private readonly List<IOnDisable> onDisableCallbacks = new List<IOnDisable>();

		// Token: 0x04000ED7 RID: 3799
		public readonly List<IOnPreUpdate> onPreUpdateCallbacks = new List<IOnPreUpdate>();

		// Token: 0x04000ED8 RID: 3800
		public readonly List<IOnAuthorityChanged> onAuthorityChangedCallbacks = new List<IOnAuthorityChanged>();

		// Token: 0x04000ED9 RID: 3801
		public readonly List<IOnNetSerialize> onNetSerializeCallbacks = new List<IOnNetSerialize>();

		// Token: 0x04000EDA RID: 3802
		public readonly List<IOnCriticallyLateFrame> onCriticallyLateFrameCallbacks = new List<IOnCriticallyLateFrame>();

		// Token: 0x04000EDB RID: 3803
		public readonly List<IOnIncrementFrame> onIncrementFramesCallbacks = new List<IOnIncrementFrame>();

		// Token: 0x04000EDC RID: 3804
		public readonly List<IOnSnapshot> onSnapshotCallbacks = new List<IOnSnapshot>();

		// Token: 0x04000EDD RID: 3805
		public readonly List<IOnQuantize> onQuantizeCallbacks = new List<IOnQuantize>();

		// Token: 0x04000EDE RID: 3806
		public readonly List<IOnInterpolate> onInterpolateCallbacks = new List<IOnInterpolate>();

		// Token: 0x04000EDF RID: 3807
		public readonly List<IOnCaptureState> onCaptureCurrentStateCallbacks = new List<IOnCaptureState>();

		// Token: 0x04000EE0 RID: 3808
		public readonly List<IOnPreSimulate> onPreSimulateCallbacks = new List<IOnPreSimulate>();

		// Token: 0x04000EE1 RID: 3809
		public readonly List<IOnPostSimulate> onPostSimulateCallbacks = new List<IOnPostSimulate>();

		// Token: 0x04000EE2 RID: 3810
		public readonly List<IOnPreQuit> onPreQuitCallbacks = new List<IOnPreQuit>();

		// Token: 0x04000EE3 RID: 3811
		public readonly List<IOnPreNetDestroy> onPreNetDestroyCallbacks = new List<IOnPreNetDestroy>();

		// Token: 0x04000EE4 RID: 3812
		private readonly List<IOnNetObjReady> onNetObjReadyCallbacks = new List<IOnNetObjReady>();

		// Token: 0x04000EE5 RID: 3813
		private readonly List<SyncObject> syncObjects = new List<SyncObject>();

		// Token: 0x04000EE6 RID: 3814
		private readonly List<NetObject.PackObjRecord> packObjects = new List<NetObject.PackObjRecord>();

		// Token: 0x04000EE7 RID: 3815
		private bool processedInitialBacklog;

		// Token: 0x04000EE8 RID: 3816
		private float firstDeserializeTime;

		// Token: 0x020003D9 RID: 985
		private class PackObjRecord
		{
			// Token: 0x04001318 RID: 4888
			public Component component;

			// Token: 0x04001319 RID: 4889
			public PackObjectDatabase.PackObjectInfo info;

			// Token: 0x0400131A RID: 4890
			public PackFrame[] packFrames;

			// Token: 0x0400131B RID: 4891
			public FastBitMask128 prevReadyMask;

			// Token: 0x0400131C RID: 4892
			public FastBitMask128 readyMask;

			// Token: 0x0400131D RID: 4893
			public IPackObjOnReadyChange onReadyCallback;
		}
	}
}
