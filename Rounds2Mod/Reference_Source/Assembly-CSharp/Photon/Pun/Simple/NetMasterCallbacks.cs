using System;
using System.Collections.Generic;
using Photon.Pun.Simple.Internal;
using UnityEngine;

namespace Photon.Pun.Simple
{
	// Token: 0x020002E2 RID: 738
	public static class NetMasterCallbacks
	{
		// Token: 0x06000FA3 RID: 4003 RVA: 0x0004BD64 File Offset: 0x00049F64
		public static void RegisterCallbackInterfaces(object comp, bool register = true, bool delay = false)
		{
			if (delay || NetMasterCallbacks.callbacksLocked)
			{
				NetMasterCallbacks.pendingRegistrations.Enqueue(new NetMasterCallbacks.DelayedRegistrationItem(comp, register));
				return;
			}
			CallbackUtilities.RegisterInterface<IOnPreUpdate>(NetMasterCallbacks.onPreUpdates, comp, register);
			CallbackUtilities.RegisterInterface<IOnPostUpdate>(NetMasterCallbacks.onPostUpdates, comp, register);
			CallbackUtilities.RegisterInterface<IOnPreLateUpdate>(NetMasterCallbacks.onPreLateUpdates, comp, register);
			CallbackUtilities.RegisterInterface<IOnPostLateUpdate>(NetMasterCallbacks.onPostLateUpdates, comp, register);
			CallbackUtilities.RegisterInterface<IOnIncrementFrame>(NetMasterCallbacks.onIncrementFrames, comp, register);
			CallbackUtilities.RegisterInterface<IOnPreSimulate>(NetMasterCallbacks.onPreSimulates, comp, register);
			CallbackUtilities.RegisterInterface<IOnPostSimulate>(NetMasterCallbacks.onPostSimulates, comp, register);
			CallbackUtilities.RegisterInterface<IOnTickSnapshot>(NetMasterCallbacks.onSnapshots, comp, register);
			CallbackUtilities.RegisterInterface<IOnInterpolate>(NetMasterCallbacks.onInterpolates, comp, register);
			CallbackUtilities.RegisterInterface<IOnPreQuit>(NetMasterCallbacks.onPreQuits, comp, register);
		}

		// Token: 0x17000110 RID: 272
		// (set) Token: 0x06000FA4 RID: 4004 RVA: 0x0004BE10 File Offset: 0x0004A010
		public static bool CallbacksLocked
		{
			set
			{
				NetMasterCallbacks.callbacksLocked = value;
				if (!value)
				{
					while (NetMasterCallbacks.pendingRegistrations.Count > 0)
					{
						NetMasterCallbacks.DelayedRegistrationItem delayedRegistrationItem = NetMasterCallbacks.pendingRegistrations.Dequeue();
						NetMasterCallbacks.RegisterCallbackInterfaces(delayedRegistrationItem.comp, delayedRegistrationItem.register, false);
					}
					while (NetMasterCallbacks.postCallbackActions.Count > 0)
					{
						NetMasterCallbacks.postCallbackActions.Dequeue().Invoke();
					}
				}
			}
		}

		// Token: 0x06000FA5 RID: 4005 RVA: 0x0004BE70 File Offset: 0x0004A070
		public static void OnPreQuitCallbacks()
		{
			NetMasterCallbacks.CallbacksLocked = true;
			int i = 0;
			int count = NetMasterCallbacks.onPreQuits.Count;
			while (i < count)
			{
				NetMasterCallbacks.onPreQuits[i].OnPreQuit();
				i++;
			}
			NetMasterCallbacks.CallbacksLocked = false;
		}

		// Token: 0x06000FA6 RID: 4006 RVA: 0x0004BEB0 File Offset: 0x0004A0B0
		public static void OnPreUpdateCallbacks()
		{
			NetMasterCallbacks.CallbacksLocked = true;
			int i = 0;
			int count = NetMasterCallbacks.onPreUpdates.Count;
			while (i < count)
			{
				NetMasterCallbacks.onPreUpdates[i].OnPreUpdate();
				i++;
			}
			NetMasterCallbacks.CallbacksLocked = false;
		}

		// Token: 0x06000FA7 RID: 4007 RVA: 0x0004BEF0 File Offset: 0x0004A0F0
		public static void OnInterpolateCallbacks(int _prevFrameId, int _currFrameId, float t)
		{
			NetMasterCallbacks.CallbacksLocked = true;
			int i = 0;
			int count = NetMasterCallbacks.onInterpolates.Count;
			while (i < count)
			{
				NetMasterCallbacks.onInterpolates[i].OnInterpolate(_prevFrameId, _currFrameId, t);
				i++;
			}
			NetMasterCallbacks.CallbacksLocked = false;
		}

		// Token: 0x06000FA8 RID: 4008 RVA: 0x0004BF34 File Offset: 0x0004A134
		public static void OnPreLateUpdateCallbacks()
		{
			NetMasterCallbacks.CallbacksLocked = true;
			int i = 0;
			int count = NetMasterCallbacks.onPreLateUpdates.Count;
			while (i < count)
			{
				NetMasterCallbacks.onPreLateUpdates[i].OnPreLateUpdate();
				i++;
			}
			NetMasterCallbacks.CallbacksLocked = false;
		}

		// Token: 0x06000FA9 RID: 4009 RVA: 0x0004BF74 File Offset: 0x0004A174
		public static void OnPostSimulateCallbacks(int _currFrameId, int _currSubFrameId, bool isNetTick)
		{
			NetMasterCallbacks.CallbacksLocked = true;
			int i = 0;
			int count = NetMasterCallbacks.onPostSimulates.Count;
			while (i < count)
			{
				NetMasterCallbacks.onPostSimulates[i].OnPostSimulate(_currFrameId, _currSubFrameId, isNetTick);
				i++;
			}
			NetMasterCallbacks.CallbacksLocked = false;
		}

		// Token: 0x06000FAA RID: 4010 RVA: 0x0004BFB8 File Offset: 0x0004A1B8
		public static void OnIncrementFrameCallbacks(int _currFrameId, int _currSubFrameId, int _prevFrameId, int _prevSubFrameId)
		{
			NetMasterCallbacks.CallbacksLocked = true;
			int i = 0;
			int count = NetMasterCallbacks.onIncrementFrames.Count;
			while (i < count)
			{
				NetMasterCallbacks.onIncrementFrames[i].OnIncrementFrame(_currFrameId, _currSubFrameId, _prevFrameId, _prevSubFrameId);
				i++;
			}
			NetMasterCallbacks.CallbacksLocked = false;
		}

		// Token: 0x06000FAB RID: 4011 RVA: 0x0004BFFC File Offset: 0x0004A1FC
		public static void OnSnapshotCallbacks(int _currFrameId)
		{
			NetMasterCallbacks.CallbacksLocked = true;
			int i = 0;
			int count = NetMasterCallbacks.onSnapshots.Count;
			while (i < count)
			{
				NetMasterCallbacks.onSnapshots[i].OnSnapshot(_currFrameId);
				i++;
			}
			NetMasterCallbacks.CallbacksLocked = false;
			while (NetMasterCallbacks.postSimulateActions.Count > 0)
			{
				NetMasterCallbacks.postSimulateActions.Dequeue().Invoke();
			}
		}

		// Token: 0x06000FAC RID: 4012 RVA: 0x0004C05C File Offset: 0x0004A25C
		public static void OnPreSerializeTickCallbacks(int _currFrameId, byte[] buffer, ref int bitposition)
		{
			NetMasterCallbacks.CallbacksLocked = true;
			int i = 0;
			int count = NetMasterCallbacks.onTickPreSerializations.Count;
			while (i < count)
			{
				NetMasterCallbacks.onTickPreSerializations[i].OnPreSerializeTick(_currFrameId, buffer, ref bitposition);
				i++;
			}
			NetMasterCallbacks.CallbacksLocked = false;
		}

		// Token: 0x06000FAD RID: 4013 RVA: 0x0004C0A0 File Offset: 0x0004A2A0
		public static void OnPreSimulateCallbacks(int currentFrameId, int currentSubFrameId)
		{
			NetMasterCallbacks.CallbacksLocked = true;
			int i = 0;
			int count = NetMasterCallbacks.onPreSimulates.Count;
			while (i < count)
			{
				IOnPreSimulate onPreSimulate = NetMasterCallbacks.onPreSimulates[i];
				Behaviour behaviour = onPreSimulate as Behaviour;
				if (!behaviour || (behaviour.isActiveAndEnabled && behaviour.gameObject.activeInHierarchy))
				{
					onPreSimulate.OnPreSimulate(currentFrameId, currentSubFrameId);
				}
				i++;
			}
			NetMasterCallbacks.CallbacksLocked = false;
		}

		// Token: 0x06000FAE RID: 4014 RVA: 0x0004C108 File Offset: 0x0004A308
		public static void OnPostUpdateCallbacks()
		{
			NetMasterCallbacks.CallbacksLocked = true;
			int i = 0;
			int count = NetMasterCallbacks.onPostUpdates.Count;
			while (i < count)
			{
				IOnPostUpdate onPostUpdate = NetMasterCallbacks.onPostUpdates[i];
				Behaviour behaviour = onPostUpdate as Behaviour;
				if (!behaviour || (behaviour.isActiveAndEnabled && behaviour.gameObject.activeInHierarchy))
				{
					onPostUpdate.OnPostUpdate();
				}
				i++;
			}
			NetMasterCallbacks.CallbacksLocked = false;
		}

		// Token: 0x06000FAF RID: 4015 RVA: 0x0004C170 File Offset: 0x0004A370
		public static void OnPostLateUpdateCallbacks()
		{
			NetMasterCallbacks.CallbacksLocked = true;
			int i = 0;
			int count = NetMasterCallbacks.onPostLateUpdates.Count;
			while (i < count)
			{
				IOnPostLateUpdate onPostLateUpdate = NetMasterCallbacks.onPostLateUpdates[i];
				Behaviour behaviour = onPostLateUpdate as Behaviour;
				if (!behaviour || (behaviour.isActiveAndEnabled && behaviour.gameObject.activeInHierarchy))
				{
					onPostLateUpdate.OnPostLateUpdate();
				}
				i++;
			}
			NetMasterCallbacks.CallbacksLocked = false;
		}

		// Token: 0x04000EA8 RID: 3752
		public static List<IOnTickPreSerialization> onTickPreSerializations = new List<IOnTickPreSerialization>();

		// Token: 0x04000EA9 RID: 3753
		public static List<IOnPreUpdate> onPreUpdates = new List<IOnPreUpdate>();

		// Token: 0x04000EAA RID: 3754
		public static List<IOnPostUpdate> onPostUpdates = new List<IOnPostUpdate>();

		// Token: 0x04000EAB RID: 3755
		public static List<IOnPreLateUpdate> onPreLateUpdates = new List<IOnPreLateUpdate>();

		// Token: 0x04000EAC RID: 3756
		public static List<IOnPostLateUpdate> onPostLateUpdates = new List<IOnPostLateUpdate>();

		// Token: 0x04000EAD RID: 3757
		public static List<IOnIncrementFrame> onIncrementFrames = new List<IOnIncrementFrame>();

		// Token: 0x04000EAE RID: 3758
		public static List<IOnPreSimulate> onPreSimulates = new List<IOnPreSimulate>();

		// Token: 0x04000EAF RID: 3759
		public static List<IOnPostSimulate> onPostSimulates = new List<IOnPostSimulate>();

		// Token: 0x04000EB0 RID: 3760
		public static List<IOnTickSnapshot> onSnapshots = new List<IOnTickSnapshot>();

		// Token: 0x04000EB1 RID: 3761
		public static List<IOnInterpolate> onInterpolates = new List<IOnInterpolate>();

		// Token: 0x04000EB2 RID: 3762
		public static List<IOnPreQuit> onPreQuits = new List<IOnPreQuit>();

		// Token: 0x04000EB3 RID: 3763
		public static Queue<Action> postCallbackActions = new Queue<Action>();

		// Token: 0x04000EB4 RID: 3764
		public static Queue<Action> postSimulateActions = new Queue<Action>();

		// Token: 0x04000EB5 RID: 3765
		public static Queue<Action> postSerializationActions = new Queue<Action>();

		// Token: 0x04000EB6 RID: 3766
		public static Queue<NetMasterCallbacks.DelayedRegistrationItem> pendingRegistrations = new Queue<NetMasterCallbacks.DelayedRegistrationItem>();

		// Token: 0x04000EB7 RID: 3767
		private static bool callbacksLocked;

		// Token: 0x020003D8 RID: 984
		public struct DelayedRegistrationItem
		{
			// Token: 0x060013FF RID: 5119 RVA: 0x0005AC0C File Offset: 0x00058E0C
			public DelayedRegistrationItem(object comp, bool register)
			{
				this.comp = comp;
				this.register = register;
			}

			// Token: 0x04001316 RID: 4886
			public object comp;

			// Token: 0x04001317 RID: 4887
			public bool register;
		}
	}
}
