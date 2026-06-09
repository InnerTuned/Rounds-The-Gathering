using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;

namespace Photon.Pun.Simple.Internal
{
	// Token: 0x02000308 RID: 776
	public class TickManager : IInRoomCallbacks
	{
		// Token: 0x06001082 RID: 4226 RVA: 0x0004FA2B File Offset: 0x0004DC2B
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		public static void Bootstrap()
		{
			TickManager.single = new TickManager();
			PhotonNetwork.NetworkingClient.AddCallbackTarget(TickManager.single);
		}

		// Token: 0x06001083 RID: 4227 RVA: 0x0004FA46 File Offset: 0x0004DC46
		public void OnPlayerEnteredRoom(Player newPlayer)
		{
			TickManager.AddConnection(newPlayer.ActorNumber, null);
		}

		// Token: 0x06001084 RID: 4228 RVA: 0x0004FA54 File Offset: 0x0004DC54
		public void OnPlayerLeftRoom(Player otherPlayer)
		{
			TickManager.RemoveConnection(otherPlayer.ActorNumber);
		}

		// Token: 0x06001085 RID: 4229 RVA: 0x000027C8 File Offset: 0x000009C8
		public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
		}

		// Token: 0x06001086 RID: 4230 RVA: 0x000027C8 File Offset: 0x000009C8
		public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
		{
		}

		// Token: 0x06001087 RID: 4231 RVA: 0x000027C8 File Offset: 0x000009C8
		public void OnMasterClientSwitched(Player newMasterClient)
		{
		}

		// Token: 0x06001088 RID: 4232 RVA: 0x0004FA64 File Offset: 0x0004DC64
		public static void PreSnapshot(int currentFrameId)
		{
			for (int i = 0; i < TickManager.connections.Count; i++)
			{
				if (TickManager.perConnOffsets[TickManager.connections[i]] != null)
				{
					ConnectionTickOffsets connectionTickOffsets = TickManager.perConnOffsets[TickManager.connections[i]];
					int num = connectionTickOffsets.ConvertFrameLocalToOrigin(currentFrameId);
					float num2 = connectionTickOffsets.validFrameMask[num] ? (Time.time - connectionTickOffsets.frameArriveTime[num]) : -1f;
					connectionTickOffsets.frameTimeBeforeConsumption[num] = num2;
					connectionTickOffsets.SnapshotAdvance();
				}
			}
		}

		// Token: 0x06001089 RID: 4233 RVA: 0x0004FAF0 File Offset: 0x0004DCF0
		public static void PostSnapshot(int currentFrameId)
		{
			for (int i = 0; i < TickManager.connections.Count; i++)
			{
				if (TickManager.perConnOffsets[TickManager.connections[i]] != null)
				{
					TickManager.perConnOffsets[TickManager.connections[i]].PostSnapshot();
				}
			}
		}

		// Token: 0x0600108A RID: 4234 RVA: 0x0004FB44 File Offset: 0x0004DD44
		public static ConnectionTickOffsets LogIncomingFrame(int connId, int originFrameId, out FrameArrival arrival)
		{
			int frameCount = TickEngineSettings.frameCount;
			ConnectionTickOffsets connectionTickOffsets;
			if (!TickManager.perConnOffsets.TryGetValue(connId, ref connectionTickOffsets) || connectionTickOffsets == null)
			{
				TickManager.LogNewConnection(connId, originFrameId, frameCount, out connectionTickOffsets);
			}
			int num = originFrameId + connectionTickOffsets.originToLocalFrame;
			if (num >= frameCount)
			{
				num -= frameCount;
			}
			connectionTickOffsets.frameArriveTime[originFrameId] = Time.time;
			int currentFrameId = NetMaster.CurrentFrameId;
			int num2;
			if (num == currentFrameId)
			{
				num2 = 0;
			}
			else
			{
				num2 = currentFrameId - num;
				if (num2 < 0)
				{
					num2 += frameCount;
				}
				if (num2 >= TickEngineSettings.halfFrameCount)
				{
					num2 -= frameCount;
				}
			}
			if (num2 >= 0)
			{
				if (num2 != 0 && num2 != 1 && num2 >= TickEngineSettings.halfFrameCount)
				{
				}
			}
			else
			{
				int num3 = -TickEngineSettings.halfFrameCount;
			}
			arrival = (FrameArrival)num2;
			bool flag = num2 <= 0;
			connectionTickOffsets.frameArrivedTooLate |= !flag;
			connectionTickOffsets.validFrameMask.Set(originFrameId, true);
			return connectionTickOffsets;
		}

		// Token: 0x0600108B RID: 4235 RVA: 0x0004FC10 File Offset: 0x0004DE10
		private static void LogNewConnection(int connId, int originFrameId, int frameCount, out ConnectionTickOffsets offsets)
		{
			int i;
			for (i = NetMaster.CurrentFrameId + TickEngineSettings.targetBufferSize; i >= frameCount; i -= frameCount)
			{
			}
			int num = i - originFrameId;
			if (num < 0)
			{
				num += frameCount;
			}
			int num2 = frameCount - num;
			if (num2 < 0)
			{
				num2 += frameCount;
			}
			offsets = new ConnectionTickOffsets(connId, num, num2);
			TickManager.AddConnection(connId, offsets);
		}

		// Token: 0x0600108C RID: 4236 RVA: 0x0004FC60 File Offset: 0x0004DE60
		private static void AddConnection(int connId, ConnectionTickOffsets offsets = null)
		{
			if (PhotonNetwork.LocalPlayer.ActorNumber == connId)
			{
				return;
			}
			if (!TickManager.connections.Contains(connId))
			{
				TickManager.perConnOffsets.Add(connId, offsets);
				TickManager.connections.Add(connId);
				NetMsgSends.newPlayers.Add(connId);
				TickManager.needToSendInitialForNewConn = true;
				return;
			}
			TickManager.perConnOffsets[connId] = offsets;
		}

		// Token: 0x0600108D RID: 4237 RVA: 0x0004FCBE File Offset: 0x0004DEBE
		public static void RemoveConnection(int connId)
		{
			if (TickManager.perConnOffsets.ContainsKey(connId))
			{
				TickManager.perConnOffsets.Remove(connId);
				TickManager.connections.Remove(connId);
			}
		}

		// Token: 0x04000F91 RID: 3985
		public static readonly Dictionary<int, ConnectionTickOffsets> perConnOffsets = new Dictionary<int, ConnectionTickOffsets>();

		// Token: 0x04000F92 RID: 3986
		public static readonly List<int> connections = new List<int>();

		// Token: 0x04000F93 RID: 3987
		public static TickManager single;

		// Token: 0x04000F94 RID: 3988
		public static bool needToSendInitialForNewConn;
	}
}
