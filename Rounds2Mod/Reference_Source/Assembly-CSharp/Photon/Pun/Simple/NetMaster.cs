using System;
using ExitGames.Client.Photon;
using Photon.Compression;
using Photon.Pun.Simple.Internal;
using Photon.Utilities;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x020002E1 RID: 737
	[HelpURL("https://doc.photonengine.com/en-us/pun/current/gameplay/simple/simpleoverview")]
	public class NetMaster : MonoBehaviour
	{
		// Token: 0x1700010A RID: 266
		// (get) Token: 0x06000F8F RID: 3983 RVA: 0x0004B71D File Offset: 0x0004991D
		public static int CurrentFrameId
		{
			get
			{
				return NetMaster._currFrameId;
			}
		}

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x06000F90 RID: 3984 RVA: 0x0004B724 File Offset: 0x00049924
		public static int CurrentSubFrameId
		{
			get
			{
				return NetMaster._currSubFrameId;
			}
		}

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x06000F91 RID: 3985 RVA: 0x0004B72B File Offset: 0x0004992B
		public static int PreviousFrameId
		{
			get
			{
				return NetMaster._prevFrameId;
			}
		}

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x06000F92 RID: 3986 RVA: 0x0004B732 File Offset: 0x00049932
		public static int PreviousSubFrameId
		{
			get
			{
				return NetMaster._prevSubFrameId;
			}
		}

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x06000F93 RID: 3987 RVA: 0x0004B739 File Offset: 0x00049939
		// (set) Token: 0x06000F94 RID: 3988 RVA: 0x0004B740 File Offset: 0x00049940
		public static float NormTimeSinceFixed { get; private set; }

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x06000F95 RID: 3989 RVA: 0x0004B748 File Offset: 0x00049948
		public static float RTT
		{
			get
			{
				return NetMaster.rtt;
			}
		}

		// Token: 0x06000F96 RID: 3990 RVA: 0x0004B750 File Offset: 0x00049950
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void EnsureExistsInScene()
		{
			if (!SettingsScriptableObject<TickEngineSettings>.Single.enableTickEngine)
			{
				return;
			}
			GameObject gameObject = null;
			if (NetMaster.single)
			{
				gameObject = NetMaster.single.gameObject;
			}
			else
			{
				if (NetMasterLate.single)
				{
					gameObject = NetMasterLate.single.gameObject;
				}
				NetMaster.single = Object.FindObjectOfType<NetMaster>();
				if (NetMaster.single)
				{
					gameObject = NetMaster.single.gameObject;
				}
				else
				{
					if (!gameObject)
					{
						gameObject = new GameObject("Net Master");
					}
					NetMaster.single = gameObject.AddComponent<NetMaster>();
				}
			}
			if (!NetMasterLate.single)
			{
				NetMasterLate.single = Object.FindObjectOfType<NetMasterLate>();
				if (!NetMasterLate.single)
				{
					NetMasterLate.single = gameObject.AddComponent<NetMasterLate>();
				}
			}
			NetMsgCallbacks.RegisterCallback(new NetMsgCallbacks.ByteBufferCallback(NetMaster.ReceiveMessage));
		}

		// Token: 0x06000F97 RID: 3991 RVA: 0x0004B81C File Offset: 0x00049A1C
		private void Awake()
		{
			if (NetMaster.single && NetMaster.single != this)
			{
				Object.Destroy(NetMaster.single);
			}
			NetMaster.single = this;
			Object.DontDestroyOnLoad(this);
			NetMaster._prevFrameId = TickEngineSettings.frameCount - 1;
			NetMaster._prevSubFrameId = TickEngineSettings.sendEveryXTick - 1;
		}

		// Token: 0x06000F98 RID: 3992 RVA: 0x0004B870 File Offset: 0x00049A70
		private void OnApplicationQuit()
		{
			NetMaster.isShuttingDown = true;
			NetMasterCallbacks.OnPreQuitCallbacks();
		}

		// Token: 0x06000F99 RID: 3993 RVA: 0x0004B880 File Offset: 0x00049A80
		private void FixedUpdate()
		{
			if (NetObject.activeControlledNetObjs.Count == 0 && NetObject.activeUncontrolledNetObjs.Count == 0)
			{
				return;
			}
			if (!SettingsScriptableObject<TickEngineSettings>.single.enableTickEngine)
			{
				return;
			}
			if (!NetMsgSends.ReadyToSend)
			{
				DoubleTime.SnapFixed();
				return;
			}
			if (this.simulationHasRun)
			{
				this.PostSimulate();
			}
			DoubleTime.SnapFixed();
			bool flag = true;
			while (PhotonNetwork.InRoom && PhotonNetwork.IsMessageQueueRunning && flag)
			{
				flag = PhotonNetwork.NetworkingClient.LoadBalancingPeer.DispatchIncomingCommands();
			}
			NetMaster.rtt = (float)PhotonNetwork.GetPing() * 0.001f;
			this.simulationHasRun = true;
		}

		// Token: 0x06000F9A RID: 3994 RVA: 0x0004B914 File Offset: 0x00049B14
		private void Update()
		{
			if (NetObject.activeControlledNetObjs.Count == 0 && NetObject.activeUncontrolledNetObjs.Count == 0)
			{
				return;
			}
			if (!SettingsScriptableObject<TickEngineSettings>.single.enableTickEngine)
			{
				return;
			}
			if (this.simulationHasRun)
			{
				this.PostSimulate();
			}
			DoubleTime.SnapUpdate();
			NetMaster.NormTimeSinceFixed = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
			NetMasterCallbacks.OnPreUpdateCallbacks();
			float t = (Time.time - NetMaster.lastSentTickTime) / TickEngineSettings.netTickInterval;
			NetObject.NetObjDictsLocked = true;
			foreach (NetObject netObject in NetObject.activeUncontrolledNetObjs.Values)
			{
				netObject.OnInterpolate(NetMaster._prevFrameId, NetMaster._currFrameId, t);
			}
			NetObject.NetObjDictsLocked = false;
			NetMasterCallbacks.OnInterpolateCallbacks(NetMaster._prevFrameId, NetMaster._currFrameId, t);
		}

		// Token: 0x06000F9B RID: 3995 RVA: 0x0004B9D9 File Offset: 0x00049BD9
		private void LateUpdate()
		{
			NetMasterCallbacks.OnPreLateUpdateCallbacks();
		}

		// Token: 0x06000F9C RID: 3996 RVA: 0x0004B9E0 File Offset: 0x00049BE0
		private void PostSimulate()
		{
			bool flag = NetMaster._currSubFrameId == TickEngineSettings.sendEveryXTick - 1;
			NetMasterCallbacks.OnPostSimulateCallbacks(NetMaster._currFrameId, NetMaster._currSubFrameId, flag);
			if (flag)
			{
				NetMaster.SerializeAllAndSend();
			}
			NetMaster.IncrementFrameId();
			this.simulationHasRun = false;
		}

		// Token: 0x06000F9D RID: 3997 RVA: 0x0004BA20 File Offset: 0x00049C20
		private static void IncrementFrameId()
		{
			NetMaster._prevSubFrameId = NetMaster._currSubFrameId;
			NetMaster._currSubFrameId++;
			if (NetMaster._currSubFrameId >= TickEngineSettings.sendEveryXTick)
			{
				NetMaster._currSubFrameId = 0;
				NetMaster._prevFrameId = NetMaster._currFrameId;
				NetMaster._currFrameId++;
				if (NetMaster._currFrameId >= TickEngineSettings.frameCount)
				{
					NetMaster._currFrameId = 0;
				}
			}
			NetMasterCallbacks.OnIncrementFrameCallbacks(NetMaster._currFrameId, NetMaster._currSubFrameId, NetMaster._prevFrameId, NetMaster._prevSubFrameId);
			if (NetMaster._currSubFrameId == 0)
			{
				TickManager.PreSnapshot(NetMaster._currFrameId);
				NetObject.NetObjDictsLocked = true;
				foreach (NetObject netObject in NetObject.activeUncontrolledNetObjs.Values)
				{
					netObject.OnSnapshot(NetMaster._currFrameId);
				}
				NetObject.NetObjDictsLocked = false;
				NetMasterCallbacks.OnSnapshotCallbacks(NetMaster._currFrameId);
				TickManager.PostSnapshot(NetMaster._currFrameId);
				NetMaster.lastSentTickTime = Time.fixedTime;
			}
		}

		// Token: 0x06000F9E RID: 3998 RVA: 0x0004BB04 File Offset: 0x00049D04
		private static void SerializeAllAndSend()
		{
			byte[] reusableBuffer = NetMsgSends.reusableBuffer;
			int bitposition = 0;
			SerializationFlags writeFlags;
			SerializationFlags serializationFlags;
			if (TickManager.needToSendInitialForNewConn)
			{
				writeFlags = (SerializationFlags)22;
				serializationFlags = SerializationFlags.HasContent;
				TickManager.needToSendInitialForNewConn = false;
			}
			else
			{
				writeFlags = SerializationFlags.None;
				serializationFlags = SerializationFlags.None;
			}
			ArraySerializeExt.Write(reusableBuffer, (ulong)NetMaster._currFrameId, ref bitposition, TickEngineSettings.frameCountBits);
			NetMasterCallbacks.OnPreSerializeTickCallbacks(NetMaster._currFrameId, reusableBuffer, ref bitposition);
			NetObject.NetObjDictsLocked = true;
			NetMaster.SerializeNetObjDict(NetObject.activeControlledNetObjs, reusableBuffer, ref bitposition, ref serializationFlags, writeFlags);
			NetObject.NetObjDictsLocked = false;
			while (NetMasterCallbacks.postSerializationActions.Count > 0)
			{
				NetMasterCallbacks.postSerializationActions.Dequeue().Invoke();
			}
			if (serializationFlags == SerializationFlags.None)
			{
				return;
			}
			ArrayPackBytesExt.WritePackedBytes(reusableBuffer, 0UL, ref bitposition, 32);
			reusableBuffer.Send(bitposition, null, serializationFlags, true);
		}

		// Token: 0x06000F9F RID: 3999 RVA: 0x0004BBA8 File Offset: 0x00049DA8
		private static void SerializeNetObjDict(NonAllocDictionary<int, NetObject> dict, byte[] buffer, ref int bitposition, ref SerializationFlags flags, SerializationFlags writeFlags)
		{
			foreach (NetObject netObject in dict.Values)
			{
				int num = bitposition;
				ArrayPackBytesExt.WritePackedBytes(buffer, (ulong)netObject.ViewID, ref bitposition, 32);
				int num2 = bitposition;
				ArraySerializeExt.WriteBool(buffer, true, ref bitposition);
				int num3 = bitposition;
				bitposition += 16;
				SerializationFlags serializationFlags = netObject.OnNetSerialize(NetMaster._currFrameId, buffer, ref bitposition, writeFlags);
				if (serializationFlags == SerializationFlags.None)
				{
					if (netObject.SkipWhenEmpty)
					{
						bitposition = num;
					}
					else
					{
						bitposition = num2;
						ArraySerializeExt.WriteBool(buffer, false, ref bitposition);
					}
				}
				else
				{
					flags |= serializationFlags;
					int num4 = bitposition - num3;
					ArraySerializeExt.Write(buffer, (ulong)num4, ref num3, 16);
				}
			}
		}

		// Token: 0x06000FA0 RID: 4000 RVA: 0x0004BC50 File Offset: 0x00049E50
		public static void ReceiveMessage(object conn, int connId, byte[] buffer)
		{
			int frameCount = TickEngineSettings.frameCount;
			int num = 0;
			int originFrameId = (int)ArraySerializeExt.Read(buffer, ref num, TickEngineSettings.frameCountBits);
			FrameArrival arrival;
			TickManager.LogIncomingFrame(connId, originFrameId, out arrival);
			for (;;)
			{
				int num2 = (int)ArrayPackBytesExt.ReadPackedBytes(buffer, ref num, 32);
				if (num2 == 0)
				{
					break;
				}
				bool flag = ArraySerializeExt.ReadBool(buffer, ref num);
				if (flag)
				{
					int num3 = num;
					int num4 = (int)ArraySerializeExt.Read(buffer, ref num, 16);
					int num5 = num3 + num4;
					PhotonView photonView = PhotonNetwork.GetPhotonView(num2);
					NetObject netObject = photonView ? photonView.GetComponent<NetObject>() : null;
					if (netObject == null)
					{
						num = num5;
					}
					else
					{
						if (netObject.IgnoreNonControllerUpdates)
						{
							int controllerActorNr = photonView.ControllerActorNr;
							int ownerActorNr = photonView.OwnerActorNr;
							if (controllerActorNr == -1)
							{
								photonView.SetControllerInternal(connId);
							}
							else if (controllerActorNr != connId && ownerActorNr != connId)
							{
								num = num5;
								continue;
							}
						}
						netObject.OnDeserialize(connId, originFrameId, buffer, ref num, flag, arrival);
						num = num5;
					}
				}
			}
		}

		// Token: 0x06000FA1 RID: 4001 RVA: 0x0004BD2C File Offset: 0x00049F2C
		public static FrameArrival CheckFrameArrival(int incomingFrame)
		{
			int num = incomingFrame - NetMaster._prevFrameId;
			if (num == 0)
			{
				return FrameArrival.IsSnap;
			}
			if (num < 0)
			{
				num += TickEngineSettings.frameCount;
			}
			if (num == 1)
			{
				return FrameArrival.IsTarget;
			}
			if (num >= TickEngineSettings.halfFrameCount)
			{
				return FrameArrival.IsLate;
			}
			return FrameArrival.IsFuture;
		}

		// Token: 0x04000E9D RID: 3741
		public static NetMaster single;

		// Token: 0x04000E9E RID: 3742
		public static bool isShuttingDown;

		// Token: 0x04000E9F RID: 3743
		protected static float lastSentTickTime;

		// Token: 0x04000EA0 RID: 3744
		private static int _currFrameId;

		// Token: 0x04000EA1 RID: 3745
		private static int _currSubFrameId;

		// Token: 0x04000EA2 RID: 3746
		private static int _prevFrameId;

		// Token: 0x04000EA3 RID: 3747
		private static int _prevSubFrameId;

		// Token: 0x04000EA5 RID: 3749
		protected static float rtt;

		// Token: 0x04000EA6 RID: 3750
		private bool simulationHasRun;

		// Token: 0x04000EA7 RID: 3751
		public const int BITS_FOR_NETOBJ_SIZE = 16;
	}
}
