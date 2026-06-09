using System;
using System.Collections.Generic;
using Photon.Compression;
using Photon.Realtime;
using Photon.Utilities;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x020002AE RID: 686
	[DisallowMultipleComponent]
	public class SyncState : SyncObject<SyncState.Frame>, IMountable, IOnCaptureState, IOnNetSerialize, IOnSnapshot, IReadyable, IOnNetObjReady, IUseKeyframes
	{
		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x06000F03 RID: 3843 RVA: 0x00048B62 File Offset: 0x00046D62
		public override int ApplyOrder
		{
			get
			{
				return 5;
			}
		}

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x06000F04 RID: 3844 RVA: 0x00048B65 File Offset: 0x00046D65
		// (set) Token: 0x06000F05 RID: 3845 RVA: 0x00048B6D File Offset: 0x00046D6D
		public Mount CurrentMount
		{
			get
			{
				return this.currentMount;
			}
			set
			{
				this.currentMount = value;
			}
		}

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x06000F06 RID: 3846 RVA: 0x000422B1 File Offset: 0x000404B1
		public bool IsThrowable
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x06000F07 RID: 3847 RVA: 0x000422B1 File Offset: 0x000404B1
		public bool IsDroppable
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x06000F08 RID: 3848 RVA: 0x00048B76 File Offset: 0x00046D76
		public Rigidbody Rb
		{
			get
			{
				return this.netObj.Rb;
			}
		}

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x06000F09 RID: 3849 RVA: 0x00048B83 File Offset: 0x00046D83
		public Rigidbody2D Rb2d
		{
			get
			{
				return this.netObj.Rb2D;
			}
		}

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x06000F0A RID: 3850 RVA: 0x000429F6 File Offset: 0x00040BF6
		public override bool AllowReconstructionOfEmpty
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000F0B RID: 3851 RVA: 0x00048B90 File Offset: 0x00046D90
		public override void OnAwake()
		{
			base.OnAwake();
			this.iSpawnController = base.GetComponent<ISpawnController>();
			this.syncTransform = base.GetComponent<SyncTransform>();
			this.syncOwner = base.GetComponent<SyncOwner>();
			NestedComponentUtilities.GetNestedComponentsInChildren<IOnStateChange, NetObject>(base.transform, this.onStateChangeCallbacks, true);
			base.transform.GetComponents<IFlagTeleport>(this.flagTeleportCallbacks);
			this.mountsLookup = this.netObj.GetComponent<MountsManager>();
		}

		// Token: 0x06000F0C RID: 3852 RVA: 0x00048BFC File Offset: 0x00046DFC
		public override void OnStart()
		{
			this.ChangeState(new StateChangeInfo(this.initialState, base.transform.parent ? base.transform.parent.GetComponent<Mount>() : null, true));
			base.OnStart();
			this.respawnStateInfo = new StateChangeInfo(this.respawnState, base.transform.parent ? base.transform.parent.GetComponent<Mount>() : null, new Vector3?(base.transform.localPosition), new Quaternion?(base.transform.localRotation), default(Vector3?), true);
			int num = BitCounter.CountTrueBits(this.mountableTo.mask, ref this.indexToMountTypeId, MountSettings.mountTypeCount);
			this.bitsForMountType = num.GetBitsForMaxValue();
			for (int i = 0; i < num; i++)
			{
				this.mountTypeIdToIndex.Add(this.indexToMountTypeId[i], i);
			}
		}

		// Token: 0x06000F0D RID: 3853 RVA: 0x00048CF0 File Offset: 0x00046EF0
		public override void OnJoinedRoom()
		{
			base.OnJoinedRoom();
			if (this.iSpawnController != null)
			{
				return;
			}
			this.ChangeState(new StateChangeInfo(this.initialState, base.transform.parent ? base.transform.parent.GetComponent<Mount>() : null, true));
		}

		// Token: 0x06000F0E RID: 3854 RVA: 0x00048D43 File Offset: 0x00046F43
		public override void OnAuthorityChanged(bool isMine, bool controllerChanged)
		{
			base.OnAuthorityChanged(isMine, controllerChanged);
			this.stateChangeQueue.Clear();
			this.prevSerializedFrame = null;
		}

		// Token: 0x06000F0F RID: 3855 RVA: 0x00048D60 File Offset: 0x00046F60
		public virtual void OnNetObjReadyChange(bool ready)
		{
			this.netObjIsReady = ready;
			if (base.IsMine && this.iSpawnController != null && !this.iSpawnController.AllowNetObjectReadyCallback(ready))
			{
				return;
			}
			if (ready)
			{
				this.ChangeState(new StateChangeInfo(this.readyState, this.currentMount, true));
				return;
			}
			this.ChangeState(new StateChangeInfo(this.unreadyState, this.currentMount, true));
		}

		// Token: 0x06000F10 RID: 3856 RVA: 0x00048DC8 File Offset: 0x00046FC8
		public void SoftMount(Mount attachTo)
		{
			ObjState itemState = attachTo ? ((this.currentState.state & (ObjState)(-49)) | ObjState.Mounted) : (this.currentState.state & (ObjState)(-3));
			this.ChangeState(new StateChangeInfo(itemState, attachTo, false));
		}

		// Token: 0x06000F11 RID: 3857 RVA: 0x00048E0C File Offset: 0x0004700C
		public void HardMount(Mount mountTo)
		{
			ObjState itemState = mountTo ? ((this.currentState.state & (ObjState)(-49)) | (ObjState)14) : (this.currentState.state & (ObjState)(-15));
			this.ChangeState(new StateChangeInfo(itemState, mountTo, false));
		}

		// Token: 0x06000F12 RID: 3858 RVA: 0x000027C8 File Offset: 0x000009C8
		public void Spawn()
		{
		}

		// Token: 0x06000F13 RID: 3859 RVA: 0x00048E51 File Offset: 0x00047051
		public void Respawn(bool immediate)
		{
			if (immediate)
			{
				this.ChangeState(this.respawnStateInfo);
				return;
			}
			this.stateChangeQueue.Enqueue(this.respawnStateInfo);
		}

		// Token: 0x06000F14 RID: 3860 RVA: 0x00048E74 File Offset: 0x00047074
		public void Despawn(bool immediate)
		{
			if (immediate)
			{
				this.ChangeState(new StateChangeInfo(ObjState.Despawned, null, true));
				return;
			}
			this.stateChangeQueue.Enqueue(new StateChangeInfo(ObjState.Despawned, null, true));
		}

		// Token: 0x06000F15 RID: 3861 RVA: 0x00048E9B File Offset: 0x0004709B
		public void ImmediateUnmount()
		{
			this.stateChangeQueue.Clear();
			this.ChangeState(new StateChangeInfo((ObjState)49, null, true));
		}

		// Token: 0x06000F16 RID: 3862 RVA: 0x00048EB7 File Offset: 0x000470B7
		public void Drop(Mount newMount, bool force = false)
		{
			this.stateChangeQueue.Enqueue(new StateChangeInfo((ObjState)17, newMount, force));
		}

		// Token: 0x06000F17 RID: 3863 RVA: 0x00048ECD File Offset: 0x000470CD
		public void Throw(Vector3 position, Quaternion rotation, Vector3 velocity)
		{
			this.stateChangeQueue.Enqueue(new StateChangeInfo((ObjState)49, null, new Vector3?(position), new Quaternion?(rotation), new Vector3?(velocity), false));
		}

		// Token: 0x06000F18 RID: 3864 RVA: 0x00048EF5 File Offset: 0x000470F5
		public void ThrowLocal(Transform origin, Vector3 offset, Vector3 velocity)
		{
			this.stateChangeQueue.Enqueue(new StateChangeInfo((ObjState)49, null, new Vector3?(origin.TransformPoint(offset)), new Vector3?(origin.TransformPoint(velocity)), false));
		}

		// Token: 0x06000F19 RID: 3865 RVA: 0x00048F24 File Offset: 0x00047124
		public virtual void QueueStateChange(ObjState newState, Mount newMount, bool force)
		{
			this.stateChangeQueue.Enqueue(new StateChangeInfo(newState, newMount, default(Vector3?), default(Vector3?), force));
		}

		// Token: 0x06000F1A RID: 3866 RVA: 0x00048F56 File Offset: 0x00047156
		public virtual void QueueStateChange(ObjState newState, Mount newMount, Vector3 offset, Vector3 velocity, bool force)
		{
			this.stateChangeQueue.Enqueue(new StateChangeInfo(newState, newMount, new Vector3?(offset), new Vector3?(velocity), force));
		}

		// Token: 0x06000F1B RID: 3867 RVA: 0x00048F7C File Offset: 0x0004717C
		protected virtual void DequeueStateChanges()
		{
			while (this.stateChangeQueue.Count > 0)
			{
				StateChangeInfo stateChangeInfo = this.stateChangeQueue.Dequeue();
				this.ChangeState(stateChangeInfo);
			}
		}

		// Token: 0x06000F1C RID: 3868 RVA: 0x00048FAC File Offset: 0x000471AC
		protected virtual void ChangeState(StateChangeInfo stateChangeInfo)
		{
			if (!base.gameObject)
			{
				global::Debug.LogWarning(base.name + " has been destroyed. Will not try to change state.");
				return;
			}
			ObjState state = this.currentState.state;
			Mount mount = this.currentMount;
			ObjState objState = stateChangeInfo.objState;
			Mount mount2 = stateChangeInfo.mount;
			bool flag;
			if (this.autoReset && state == ObjState.Despawned && objState != ObjState.Despawned && (objState & ObjState.Anchored) == ObjState.Despawned)
			{
				stateChangeInfo = new StateChangeInfo(this.respawnStateInfo)
				{
					objState = stateChangeInfo.objState
				};
				flag = true;
			}
			else
			{
				flag = false;
			}
			bool force = stateChangeInfo.force;
			bool flag2 = objState != state;
			bool flag3 = mount != mount2;
			Transform parent = base.transform.parent;
			if (!force && !flag2 && !flag3)
			{
				return;
			}
			this.currentState.state = objState;
			Mount prevMount = this.currentMount;
			this.currentMount = mount2;
			bool flag4 = (objState & ObjState.Mounted) > ObjState.Despawned;
			bool flag5 = flag4 && mount2 == null;
			if (flag4 && !flag5 && mount2 == null)
			{
				global::Debug.LogError("Invalid Mount!");
				this.InvalidMountHandler(objState, mount2, force);
				return;
			}
			if (base.IsMine && (flag3 || flag))
			{
				for (int i = 0; i < this.flagTeleportCallbacks.Count; i++)
				{
					this.flagTeleportCallbacks[i].FlagTeleport();
				}
			}
			if (flag3)
			{
				if (flag4)
				{
					this.currentState.mountToViewID = new int?(mount2.ViewID);
					this.currentState.mountTypeId = new int?(mount2.mountType.id);
					base.transform.parent = mount2.transform;
					if ((objState & ObjState.AnchoredPosition) != ObjState.Despawned)
					{
						base.transform.localPosition = default(Vector3);
					}
					if ((objState & ObjState.AnchoredRotation) != ObjState.Despawned)
					{
						base.transform.localRotation = default(Quaternion);
					}
				}
				else
				{
					this.currentState.mountToViewID = default(int?);
					this.currentState.mountTypeId = default(int?);
					base.transform.parent = null;
				}
				if (this.autoOwnerChange && mount2 && mount != mount2)
				{
					this.ChangeOwnerToParentMountsOwner();
				}
				Mount.ChangeMounting(this, prevMount, mount2);
			}
			Vector3? offsetPos = stateChangeInfo.offsetPos;
			Quaternion? offsetRot = stateChangeInfo.offsetRot;
			Vector3? velocity = stateChangeInfo.velocity;
			if (offsetRot != null)
			{
				base.transform.rotation = offsetRot.Value;
			}
			if (offsetPos != null)
			{
				base.transform.position = offsetPos.Value;
			}
			if (velocity != null)
			{
				Rigidbody rb = this.netObj.Rb;
				if (rb)
				{
					rb.velocity = velocity.Value;
				}
				else
				{
					Rigidbody2D rb2D = this.netObj.Rb2D;
					if (rb2D)
					{
						rb2D.velocity = velocity.Value;
					}
				}
			}
			if (flag3 || flag2 || force)
			{
				for (int j = 0; j < this.onStateChangeCallbacks.Count; j++)
				{
					this.onStateChangeCallbacks[j].OnStateChange(objState, state, base.transform, this.currentMount, this.netObjIsReady);
				}
			}
		}

		// Token: 0x06000F1D RID: 3869 RVA: 0x000492C0 File Offset: 0x000474C0
		private void ChangeOwnerToParentMountsOwner()
		{
			if (!base.IsMine)
			{
				return;
			}
			if (!this.syncOwner)
			{
				global::Debug.LogWarning(base.name + " cannot automatically change owner without a " + typeof(SyncOwner).Name + " component.");
				return;
			}
			if (this.currentMount == null)
			{
				return;
			}
			PhotonView photonView = this.currentMount.PhotonView;
			if (photonView)
			{
				Player owner = photonView.Owner;
				int newOwnerId = (owner == null) ? 0 : owner.ActorNumber;
				if (this.autoOwnerChange)
				{
					this.syncOwner.TransferOwner(newOwnerId);
					return;
				}
			}
			else if (this.autoOwnerChange)
			{
				base.GetComponent<SyncOwner>().TransferOwner(0);
			}
		}

		// Token: 0x06000F1E RID: 3870 RVA: 0x0004936D File Offset: 0x0004756D
		protected virtual void InvalidMountHandler(ObjState newState, Mount newMount, bool force)
		{
			global::Debug.LogWarning("Invalid Mount Handled!!");
			this.ChangeState(new StateChangeInfo(ObjState.Visible, null, true));
		}

		// Token: 0x06000F1F RID: 3871 RVA: 0x00049388 File Offset: 0x00047588
		public virtual bool ChangeMount(int newMountId)
		{
			if (this.currentMount == null)
			{
				global::Debug.LogWarning("'" + base.name + "' is not currently mounted, so we cannot change to a different mount.");
				return false;
			}
			if ((this.mountableTo & 1 << newMountId) == 0)
			{
				global::Debug.LogWarning(string.Concat(new string[]
				{
					"'",
					base.name,
					"' is trying to switch to a mount '",
					MountSettings.GetName(newMountId),
					"' , but mount is not set as valid in SyncState."
				}));
				return false;
			}
			Dictionary<int, Mount> mountIdLookup = this.currentMount.mountsLookup.mountIdLookup;
			if (!mountIdLookup.ContainsKey(newMountId))
			{
				global::Debug.LogWarning(string.Concat(new string[]
				{
					"'",
					base.name,
					"' doesn't contain a mount for '",
					MountSettings.GetName(newMountId),
					"'."
				}));
				return false;
			}
			Mount mount = mountIdLookup[newMountId];
			this.ChangeState(new StateChangeInfo(this.currentState.state, mount, false));
			return true;
		}

		// Token: 0x06000F20 RID: 3872 RVA: 0x00049480 File Offset: 0x00047680
		public void OnCaptureCurrentState(int frameId)
		{
			this.DequeueStateChanges();
			this.frames[frameId].CopyFrom(this.currentState);
		}

		// Token: 0x06000F21 RID: 3873 RVA: 0x0004949C File Offset: 0x0004769C
		public SerializationFlags OnNetSerialize(int frameId, byte[] buffer, ref int bitposition, SerializationFlags writeFlags)
		{
			if (!base.enabled)
			{
				ArraySerializeExt.WriteBool(buffer, false, ref bitposition);
				return SerializationFlags.None;
			}
			SyncState.Frame frame = this.frames[frameId];
			bool flag = base.IsKeyframe(frameId);
			bool flag2 = (writeFlags & SerializationFlags.NewConnection) > SerializationFlags.None;
			bool flag3 = this.prevSerializedFrame == null;
			bool flag4 = flag3 || frame.state != this.prevSerializedFrame.state;
			bool flag5;
			if (!flag3)
			{
				int? num = frame.mountTypeId;
				int? num2 = this.prevSerializedFrame.mountTypeId;
				if (num.GetValueOrDefault() == num2.GetValueOrDefault() & num != null == (num2 != null))
				{
					num2 = frame.mountToViewID;
					num = this.prevSerializedFrame.mountToViewID;
					flag5 = !(num2.GetValueOrDefault() == num.GetValueOrDefault() & num2 != null == (num != null));
					goto IL_C9;
				}
			}
			flag5 = true;
			IL_C9:
			bool flag6 = flag5;
			bool flag7 = flag2 || flag || flag3;
			bool flag8 = flag4 || flag6;
			bool flag9 = flag7 || flag8;
			bool flag10 = flag3 || flag2 || (this.keyframeRate == 0 && flag9) || (this.mountReliable && flag6);
			if (!flag9 && !flag10)
			{
				ArraySerializeExt.WriteBool(buffer, false, ref bitposition);
				return SerializationFlags.None;
			}
			ArraySerializeExt.WriteBool(buffer, true, ref bitposition);
			SerializationFlags serializationFlags = SerializationFlags.HasContent;
			if (flag10)
			{
				serializationFlags |= SerializationFlags.ForceReliable;
			}
			ArraySerializeExt.Write(buffer, (ulong)((long)frame.state), ref bitposition, 6);
			if ((frame.state & ObjState.Mounted) != ObjState.Despawned)
			{
				if (flag6 || flag7)
				{
					if (!flag)
					{
						ArraySerializeExt.Write(buffer, 1UL, ref bitposition, 1);
					}
					ArrayPackBytesExt.WritePackedBytes(buffer, (ulong)frame.mountToViewID.Value, ref bitposition, 32);
					if (this.bitsForMountType > 0)
					{
						int num3 = this.mountTypeIdToIndex[frame.mountTypeId.Value];
						ArraySerializeExt.Write(buffer, (ulong)num3, ref bitposition, this.bitsForMountType);
					}
				}
				else if (!flag)
				{
					ArraySerializeExt.Write(buffer, 0UL, ref bitposition, 1);
				}
			}
			this.prevSerializedFrame = frame;
			return serializationFlags;
		}

		// Token: 0x06000F22 RID: 3874 RVA: 0x0004965C File Offset: 0x0004785C
		public SerializationFlags OnNetDeserialize(int originFrameId, byte[] buffer, ref int bitposition, FrameArrival arrival)
		{
			SyncState.Frame frame = this.frames[originFrameId];
			if (!ArraySerializeExt.ReadBool(buffer, ref bitposition))
			{
				return SerializationFlags.None;
			}
			int result = 1;
			frame.state = (ObjState)ArraySerializeExt.Read(buffer, ref bitposition, 6);
			if ((frame.state & ObjState.Mounted) <= ObjState.Despawned)
			{
				frame.content = FrameContents.Complete;
				return (SerializationFlags)result;
			}
			if (base.IsKeyframe(originFrameId) || ArraySerializeExt.Read(buffer, ref bitposition, 1) == 1UL)
			{
				if ((frame.state & ObjState.Mounted) != ObjState.Despawned)
				{
					frame.mountToViewID = new int?((int)ArrayPackBytesExt.ReadPackedBytes(buffer, ref bitposition, 32));
					if (this.bitsForMountType > 0)
					{
						int num = (int)ArraySerializeExt.Read(buffer, ref bitposition, this.bitsForMountType);
						int num2 = this.indexToMountTypeId[num];
						frame.mountTypeId = new int?(num2);
					}
					else
					{
						frame.mountTypeId = new int?(0);
					}
				}
				frame.content = FrameContents.Complete;
				return (SerializationFlags)result;
			}
			frame.mountToViewID = default(int?);
			frame.mountTypeId = default(int?);
			frame.content = FrameContents.Partial;
			return (SerializationFlags)result;
		}

		// Token: 0x06000F23 RID: 3875 RVA: 0x00049738 File Offset: 0x00047938
		protected override void ApplySnapshot(SyncState.Frame snapframe, SyncState.Frame targframe, bool snapIsValid, bool targIsValid)
		{
			if (snapframe.content == FrameContents.Empty)
			{
				return;
			}
			if (targframe.content == FrameContents.Complete)
			{
				Transform newParent;
				if (targframe.mountToViewID != null)
				{
					Mount mount = SyncState.GetMount(targframe.mountToViewID, targframe.mountTypeId);
					newParent = (mount ? mount.transform.parent : null);
				}
				else
				{
					newParent = null;
				}
				if (this.syncTransform)
				{
					this.syncTransform.UpdateParent(targframe.state, newParent);
				}
			}
			if (snapframe.content >= FrameContents.Extrapolated)
			{
				this.ApplyFrame(snapframe);
			}
		}

		// Token: 0x06000F24 RID: 3876 RVA: 0x000497C0 File Offset: 0x000479C0
		private void ApplyFrame(SyncState.Frame frame)
		{
			ObjState state = frame.state;
			bool flag = (state & ObjState.Mounted) > ObjState.Despawned;
			bool force = false;
			Mount mount2;
			if (flag)
			{
				int? mountToViewID = frame.mountToViewID;
				if (mountToViewID != null)
				{
					int? mountTypeId = frame.mountTypeId;
					Mount mount = SyncState.GetMount(mountToViewID, mountTypeId);
					if (mount == this.currentMount && state == this.currentState.state)
					{
						return;
					}
					if (mount)
					{
						mount2 = mount;
						base.ReadyState = ReadyStateEnum.Ready;
						force = true;
					}
					else
					{
						mount2 = this.currentMount;
					}
				}
				else if (this.currentMount == null)
				{
					mount2 = null;
				}
				else
				{
					base.ReadyState = ReadyStateEnum.Ready;
					mount2 = this.currentMount;
					force = true;
				}
			}
			else
			{
				base.ReadyState = ReadyStateEnum.Ready;
				force = true;
				mount2 = null;
			}
			this.ChangeState(new StateChangeInfo(state, mount2, force));
		}

		// Token: 0x06000F25 RID: 3877 RVA: 0x00049878 File Offset: 0x00047A78
		public static Mount GetMount(int? viewID, int? mountId)
		{
			if (viewID == null || mountId == null)
			{
				return null;
			}
			PhotonView photonView = PhotonNetwork.GetPhotonView(viewID.Value);
			MountsManager mountsManager = photonView ? photonView.GetComponent<MountsManager>() : null;
			if (mountsManager)
			{
				return mountsManager.mountIdLookup[mountId.Value];
			}
			return null;
		}

		// Token: 0x04000E15 RID: 3605
		[EnumMask(true, typeof(ObjStateEditor))]
		public ObjState initialState;

		// Token: 0x04000E16 RID: 3606
		[EnumMask(true, typeof(ObjStateEditor))]
		public ObjState respawnState = ObjState.Visible;

		// Token: 0x04000E17 RID: 3607
		[EnumMask(true, typeof(ObjStateEditor))]
		public ObjState readyState = ObjState.Visible;

		// Token: 0x04000E18 RID: 3608
		[EnumMask(true, typeof(ObjStateEditor))]
		public ObjState unreadyState;

		// Token: 0x04000E19 RID: 3609
		[Tooltip("Mount types this NetObject can be attached to.")]
		public MountMaskSelector mountableTo;

		// Token: 0x04000E1A RID: 3610
		[Tooltip("Automatically return this object to its starting position and attach to original parent when ObjState changes from Despawned to any other state.")]
		public bool autoReset = true;

		// Token: 0x04000E1B RID: 3611
		[Tooltip("Automatically will request ownership transfer to the owner of NetObjects this becomes attached to.")]
		public bool autoOwnerChange = true;

		// Token: 0x04000E1C RID: 3612
		[Tooltip("Parent Mount changes will force the entire update from this client to send Reliable. This ensures a keyframe of parenting and position reaches all clients (which isn't certain with packetloss), but can cause a visible hanging if packetloss is present on the network.")]
		public bool mountReliable = true;

		// Token: 0x04000E1D RID: 3613
		[NonSerialized]
		protected SyncState.Frame currentState = new SyncState.Frame();

		// Token: 0x04000E1E RID: 3614
		[NonSerialized]
		protected Mount currentMount;

		// Token: 0x04000E1F RID: 3615
		[NonSerialized]
		protected bool netObjIsReady;

		// Token: 0x04000E20 RID: 3616
		protected MountsManager mountsLookup;

		// Token: 0x04000E21 RID: 3617
		protected SyncTransform syncTransform;

		// Token: 0x04000E22 RID: 3618
		protected SyncOwner syncOwner;

		// Token: 0x04000E23 RID: 3619
		protected ISpawnController iSpawnController;

		// Token: 0x04000E24 RID: 3620
		protected Dictionary<int, int> mountTypeIdToIndex = new Dictionary<int, int>();

		// Token: 0x04000E25 RID: 3621
		protected int[] indexToMountTypeId;

		// Token: 0x04000E26 RID: 3622
		protected int bitsForMountType;

		// Token: 0x04000E27 RID: 3623
		protected StateChangeInfo respawnStateInfo;

		// Token: 0x04000E28 RID: 3624
		[NonSerialized]
		public List<IOnStateChange> onStateChangeCallbacks = new List<IOnStateChange>();

		// Token: 0x04000E29 RID: 3625
		[NonSerialized]
		public List<IFlagTeleport> flagTeleportCallbacks = new List<IFlagTeleport>();

		// Token: 0x04000E2A RID: 3626
		protected Queue<StateChangeInfo> stateChangeQueue = new Queue<StateChangeInfo>();

		// Token: 0x04000E2B RID: 3627
		protected SyncState.Frame prevSerializedFrame;

		// Token: 0x020003D6 RID: 982
		public class Frame : FrameBase
		{
			// Token: 0x060013F1 RID: 5105 RVA: 0x0005A3E9 File Offset: 0x000585E9
			public Frame()
			{
			}

			// Token: 0x060013F2 RID: 5106 RVA: 0x0005A3F1 File Offset: 0x000585F1
			public Frame(int frameId) : base(frameId)
			{
			}

			// Token: 0x060013F3 RID: 5107 RVA: 0x0005A890 File Offset: 0x00058A90
			public override void CopyFrom(FrameBase sourceFrame)
			{
				base.CopyFrom(sourceFrame);
				SyncState.Frame frame = sourceFrame as SyncState.Frame;
				this.state = frame.state;
				this.mountToViewID = frame.mountToViewID;
				this.mountTypeId = frame.mountTypeId;
			}

			// Token: 0x060013F4 RID: 5108 RVA: 0x0005A8CF File Offset: 0x00058ACF
			public override void Clear()
			{
				base.Clear();
				this.state = ObjState.Despawned;
				this.mountToViewID = default(int?);
				this.mountTypeId = default(int?);
			}

			// Token: 0x060013F5 RID: 5109 RVA: 0x0005A8F8 File Offset: 0x00058AF8
			public bool Compare(SyncState.Frame otherFrame)
			{
				if (this.state == otherFrame.state)
				{
					int? num = this.mountToViewID;
					int? num2 = otherFrame.mountToViewID;
					if (num.GetValueOrDefault() == num2.GetValueOrDefault() & num != null == (num2 != null))
					{
						num2 = this.mountTypeId;
						num = otherFrame.mountTypeId;
						if (num2.GetValueOrDefault() == num.GetValueOrDefault() & num2 != null == (num != null))
						{
							return true;
						}
					}
				}
				return false;
			}

			// Token: 0x0400130B RID: 4875
			public ObjState state;

			// Token: 0x0400130C RID: 4876
			public int? mountToViewID;

			// Token: 0x0400130D RID: 4877
			public int? mountTypeId;

			// Token: 0x020003F0 RID: 1008
			public enum Changes
			{
				// Token: 0x0400134E RID: 4942
				None,
				// Token: 0x0400134F RID: 4943
				MountIdChange
			}
		}
	}
}
