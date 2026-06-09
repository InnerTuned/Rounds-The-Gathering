using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Compression;
using Photon.Realtime;
using Photon.Utilities;
using UnityEngine;

namespace Photon.Pun.Simple.Internal
{
	// Token: 0x0200030C RID: 780
	public static class NetMsgSends
	{
		// Token: 0x0600109A RID: 4250 RVA: 0x0004FF92 File Offset: 0x0004E192
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void CacheSettings()
		{
			NetMsgSends.unreliableCapable = (PhotonNetwork.NetworkingClient.LoadBalancingPeer.UsedProtocol == 0);
		}

		// Token: 0x0600109B RID: 4251 RVA: 0x0004FFAC File Offset: 0x0004E1AC
		public static void Send(this byte[] buffer, int bitposition, Object refObj, SerializationFlags flags, bool flush = false)
		{
			Room currentRoom = PhotonNetwork.CurrentRoom;
			if (PhotonNetwork.OfflineMode || currentRoom == null || currentRoom.Players == null)
			{
				return;
			}
			bool flag = (flags & SerializationFlags.SendToSelf) > SerializationFlags.None;
			if (!flag && !SettingsScriptableObject<TickEngineSettings>.single.sendWhenSolo && currentRoom.Players.Count <= 1)
			{
				return;
			}
			ReceiveGroup receiveGroup = flag ? ReceiveGroup.All : ReceiveGroup.Others;
			int num = bitposition + 7 >> 3;
			LoadBalancingClient networkingClient = PhotonNetwork.NetworkingClient;
			DeliveryMode deliveryMode;
			if (NetMsgSends.newPlayers.Count > 0)
			{
				deliveryMode = 1;
				NetMsgSends.newPlayers.Clear();
			}
			else
			{
				bool flag2 = (flags & SerializationFlags.ForceReliable) > SerializationFlags.None;
				deliveryMode = (NetMsgSends.unreliableCapable ? (flag2 ? 3 : 0) : 1);
			}
			SendOptions sendOptions = default(SendOptions);
			sendOptions.DeliveryMode = deliveryMode;
			SendOptions sendOptions2 = sendOptions;
			ByteArraySlice byteArraySlice = PhotonNetwork.NetworkingClient.LoadBalancingPeer.ByteArraySlicePool.Acquire(buffer, 0, num);
			networkingClient.OpRaiseEvent(215, byteArraySlice, NetMsgSends.opts[(int)receiveGroup], sendOptions2);
			if (flush)
			{
				networkingClient.Service();
			}
		}

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x0600109C RID: 4252 RVA: 0x00050095 File Offset: 0x0004E295
		public static bool ReadyToSend
		{
			get
			{
				return PhotonNetwork.NetworkClientState == 9;
			}
		}

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x0600109D RID: 4253 RVA: 0x000429F6 File Offset: 0x00040BF6
		public static bool AmActiveServer
		{
			get
			{
				return false;
			}
		}

		// Token: 0x04000F9B RID: 3995
		private static bool unreliableCapable;

		// Token: 0x04000F9C RID: 3996
		public static byte[] reusableBuffer = new byte[16384];

		// Token: 0x04000F9D RID: 3997
		public static byte[] reusableNetObjBuffer = new byte[4096];

		// Token: 0x04000F9E RID: 3998
		public static HashSet<int> newPlayers = new HashSet<int>();

		// Token: 0x04000F9F RID: 3999
		private static RaiseEventOptions[] opts = new RaiseEventOptions[]
		{
			new RaiseEventOptions
			{
				Receivers = 0
			},
			new RaiseEventOptions
			{
				Receivers = 1
			},
			new RaiseEventOptions
			{
				Receivers = 2
			}
		};
	}
}
