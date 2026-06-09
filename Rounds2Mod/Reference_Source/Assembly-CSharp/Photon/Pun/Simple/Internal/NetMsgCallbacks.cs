using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

namespace Photon.Pun.Simple.Internal
{
	// Token: 0x0200030A RID: 778
	public static class NetMsgCallbacks
	{
		// Token: 0x06001091 RID: 4241 RVA: 0x0004FD5A File Offset: 0x0004DF5A
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void RegisterOnEventListener()
		{
			PhotonNetwork.NetworkingClient.EventReceived += new Action<EventData>(NetMsgCallbacks.OnEvent);
		}

		// Token: 0x06001092 RID: 4242 RVA: 0x0004FD74 File Offset: 0x0004DF74
		public static void OnEvent(EventData photonEvent)
		{
			byte code = photonEvent.Code;
			if (!NetMsgCallbacks.callbacks.ContainsKey((int)code))
			{
				return;
			}
			bool useByteArraySlicePoolForEvents = PhotonNetwork.NetworkingClient.LoadBalancingPeer.UseByteArraySlicePoolForEvents;
			ByteArraySlice byteArraySlice;
			byte[] buffer;
			if (useByteArraySlicePoolForEvents)
			{
				byteArraySlice = (photonEvent.CustomData as ByteArraySlice);
				buffer = byteArraySlice.Buffer;
			}
			else
			{
				byteArraySlice = null;
				buffer = (photonEvent.CustomData as byte[]);
			}
			NetMsgCallbacks.CallbackLists callbackLists = NetMsgCallbacks.callbacks[(int)code];
			if (callbackLists.bufferCallbacks != null && callbackLists.bufferCallbacks.Count > 0)
			{
				foreach (NetMsgCallbacks.ByteBufferCallback byteBufferCallback in callbackLists.bufferCallbacks)
				{
					byteBufferCallback(null, photonEvent.Sender, buffer);
				}
			}
			if (useByteArraySlicePoolForEvents)
			{
				byteArraySlice.Release();
			}
		}

		// Token: 0x06001093 RID: 4243 RVA: 0x0004FE48 File Offset: 0x0004E048
		[Obsolete("Removed the asServer from UNET side, killing it here as well.")]
		public static void RegisterCallback(byte msgid, NetMsgCallbacks.ByteBufferCallback callback, bool asServer)
		{
			if (!NetMsgCallbacks.callbacks.ContainsKey((int)msgid))
			{
				NetMsgCallbacks.callbacks.Add((int)msgid, new NetMsgCallbacks.CallbackLists());
			}
			if (NetMsgCallbacks.callbacks[(int)msgid].bufferCallbacks == null)
			{
				NetMsgCallbacks.callbacks[(int)msgid].bufferCallbacks = new List<NetMsgCallbacks.ByteBufferCallback>();
			}
			List<NetMsgCallbacks.ByteBufferCallback> bufferCallbacks = NetMsgCallbacks.callbacks[(int)msgid].bufferCallbacks;
			if (!bufferCallbacks.Contains(callback))
			{
				bufferCallbacks.Add(callback);
			}
		}

		// Token: 0x06001094 RID: 4244 RVA: 0x0004FEBA File Offset: 0x0004E0BA
		public static void RegisterCallback(NetMsgCallbacks.ByteBufferCallback callback)
		{
			NetMsgCallbacks.RegisterCallback(215, callback);
		}

		// Token: 0x06001095 RID: 4245 RVA: 0x0004FEC8 File Offset: 0x0004E0C8
		public static void RegisterCallback(byte msgid, NetMsgCallbacks.ByteBufferCallback callback)
		{
			if (!NetMsgCallbacks.callbacks.ContainsKey((int)msgid))
			{
				NetMsgCallbacks.callbacks.Add((int)msgid, new NetMsgCallbacks.CallbackLists());
			}
			if (NetMsgCallbacks.callbacks[(int)msgid].bufferCallbacks == null)
			{
				NetMsgCallbacks.callbacks[(int)msgid].bufferCallbacks = new List<NetMsgCallbacks.ByteBufferCallback>();
			}
			List<NetMsgCallbacks.ByteBufferCallback> bufferCallbacks = NetMsgCallbacks.callbacks[(int)msgid].bufferCallbacks;
			if (!bufferCallbacks.Contains(callback))
			{
				bufferCallbacks.Add(callback);
			}
		}

		// Token: 0x06001096 RID: 4246 RVA: 0x0004FF3A File Offset: 0x0004E13A
		[Obsolete("Removed the asServer from UNET side, killing it here as well.")]
		public static void UnregisterCallback(byte msgid, NetMsgCallbacks.ByteBufferCallback callback, bool asServer)
		{
			if (NetMsgCallbacks.callbacks.ContainsKey((int)msgid))
			{
				NetMsgCallbacks.CallbackLists callbackLists = NetMsgCallbacks.callbacks[(int)msgid];
				callbackLists.bufferCallbacks.Remove(callback);
				if (callbackLists.bufferCallbacks.Count == 0)
				{
					NetMsgCallbacks.callbacks.Remove((int)msgid);
				}
			}
		}

		// Token: 0x06001097 RID: 4247 RVA: 0x0004FF79 File Offset: 0x0004E179
		public static void UnregisterCallback(NetMsgCallbacks.ByteBufferCallback callback)
		{
			NetMsgCallbacks.UnregisterCallback(215, callback);
		}

		// Token: 0x06001098 RID: 4248 RVA: 0x0004FF3A File Offset: 0x0004E13A
		public static void UnregisterCallback(byte msgid, NetMsgCallbacks.ByteBufferCallback callback)
		{
			if (NetMsgCallbacks.callbacks.ContainsKey((int)msgid))
			{
				NetMsgCallbacks.CallbackLists callbackLists = NetMsgCallbacks.callbacks[(int)msgid];
				callbackLists.bufferCallbacks.Remove(callback);
				if (callbackLists.bufferCallbacks.Count == 0)
				{
					NetMsgCallbacks.callbacks.Remove((int)msgid);
				}
			}
		}

		// Token: 0x04000F95 RID: 3989
		private static Dictionary<int, NetMsgCallbacks.CallbackLists> callbacks = new Dictionary<int, NetMsgCallbacks.CallbackLists>();

		// Token: 0x04000F96 RID: 3990
		public const byte DEF_MSG_ID = 215;

		// Token: 0x020003DF RID: 991
		// (Invoke) Token: 0x06001403 RID: 5123
		public delegate void ByteBufferCallback(object conn, int connId, byte[] buffer);

		// Token: 0x020003E0 RID: 992
		private class CallbackLists
		{
			// Token: 0x04001332 RID: 4914
			public List<NetMsgCallbacks.ByteBufferCallback> bufferCallbacks;
		}
	}
}
