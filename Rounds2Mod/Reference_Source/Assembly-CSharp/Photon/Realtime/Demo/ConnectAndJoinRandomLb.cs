using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Realtime.Demo
{
	// Token: 0x0200030F RID: 783
	public class ConnectAndJoinRandomLb : MonoBehaviour, IConnectionCallbacks, IMatchmakingCallbacks, ILobbyCallbacks
	{
		// Token: 0x060010A2 RID: 4258 RVA: 0x000502BC File Offset: 0x0004E4BC
		public void Start()
		{
			this.lbc = new LoadBalancingClient(0);
			this.lbc.AddCallbackTarget(this);
			this.lbc.SerializationProtocol = 0;
			if (!this.lbc.ConnectUsingSettings(this.appSettings))
			{
				global::Debug.LogError("Error while connecting");
			}
			this.ch = base.gameObject.GetComponent<ConnectionHandler>();
			if (this.ch != null)
			{
				this.ch.Client = this.lbc;
				this.ch.StartFallbackSendAckThread();
			}
		}

		// Token: 0x060010A3 RID: 4259 RVA: 0x00050348 File Offset: 0x0004E548
		public void Update()
		{
			LoadBalancingClient loadBalancingClient = this.lbc;
			if (loadBalancingClient != null)
			{
				loadBalancingClient.Service();
				Text stateUiText = this.StateUiText;
				string text = loadBalancingClient.State.ToString();
				if (stateUiText != null && !stateUiText.text.Equals(text))
				{
					stateUiText.text = "State: " + text;
				}
			}
		}

		// Token: 0x060010A4 RID: 4260 RVA: 0x000027C8 File Offset: 0x000009C8
		public void OnConnected()
		{
		}

		// Token: 0x060010A5 RID: 4261 RVA: 0x000503A9 File Offset: 0x0004E5A9
		public void OnConnectedToMaster()
		{
			global::Debug.Log("OnConnectedToMaster");
			this.lbc.OpJoinRandomRoom(null);
		}

		// Token: 0x060010A6 RID: 4262 RVA: 0x000503C2 File Offset: 0x0004E5C2
		public void OnDisconnected(DisconnectCause cause)
		{
			global::Debug.Log("OnDisconnected(" + cause + ")");
		}

		// Token: 0x060010A7 RID: 4263 RVA: 0x000027C8 File Offset: 0x000009C8
		public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
		{
		}

		// Token: 0x060010A8 RID: 4264 RVA: 0x000027C8 File Offset: 0x000009C8
		public void OnCustomAuthenticationFailed(string debugMessage)
		{
		}

		// Token: 0x060010A9 RID: 4265 RVA: 0x000503DE File Offset: 0x0004E5DE
		public void OnRegionListReceived(RegionHandler regionHandler)
		{
			global::Debug.Log("OnRegionListReceived");
			regionHandler.PingMinimumOfRegions(new Action<RegionHandler>(this.OnRegionPingCompleted), null);
		}

		// Token: 0x060010AA RID: 4266 RVA: 0x000027C8 File Offset: 0x000009C8
		public void OnRoomListUpdate(List<RoomInfo> roomList)
		{
		}

		// Token: 0x060010AB RID: 4267 RVA: 0x000027C8 File Offset: 0x000009C8
		public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
		{
		}

		// Token: 0x060010AC RID: 4268 RVA: 0x000027C8 File Offset: 0x000009C8
		public void OnJoinedLobby()
		{
		}

		// Token: 0x060010AD RID: 4269 RVA: 0x000027C8 File Offset: 0x000009C8
		public void OnLeftLobby()
		{
		}

		// Token: 0x060010AE RID: 4270 RVA: 0x000027C8 File Offset: 0x000009C8
		public void OnFriendListUpdate(List<FriendInfo> friendList)
		{
		}

		// Token: 0x060010AF RID: 4271 RVA: 0x000027C8 File Offset: 0x000009C8
		public void OnCreatedRoom()
		{
		}

		// Token: 0x060010B0 RID: 4272 RVA: 0x000027C8 File Offset: 0x000009C8
		public void OnCreateRoomFailed(short returnCode, string message)
		{
		}

		// Token: 0x060010B1 RID: 4273 RVA: 0x000503FE File Offset: 0x0004E5FE
		public void OnJoinedRoom()
		{
			global::Debug.Log("OnJoinedRoom");
		}

		// Token: 0x060010B2 RID: 4274 RVA: 0x000027C8 File Offset: 0x000009C8
		public void OnJoinRoomFailed(short returnCode, string message)
		{
		}

		// Token: 0x060010B3 RID: 4275 RVA: 0x0005040A File Offset: 0x0004E60A
		public void OnJoinRandomFailed(short returnCode, string message)
		{
			global::Debug.Log("OnJoinRandomFailed");
			this.lbc.OpCreateRoom(new EnterRoomParams());
		}

		// Token: 0x060010B4 RID: 4276 RVA: 0x000027C8 File Offset: 0x000009C8
		public void OnLeftRoom()
		{
		}

		// Token: 0x060010B5 RID: 4277 RVA: 0x00050428 File Offset: 0x0004E628
		private void OnRegionPingCompleted(RegionHandler regionHandler)
		{
			global::Debug.Log("OnRegionPingCompleted " + regionHandler.BestRegion);
			global::Debug.Log("RegionPingSummary: " + regionHandler.SummaryToCache);
			this.lbc.ConnectToRegionMaster(regionHandler.BestRegion.Code);
		}

		// Token: 0x04000FA9 RID: 4009
		[SerializeField]
		private AppSettings appSettings = new AppSettings();

		// Token: 0x04000FAA RID: 4010
		private LoadBalancingClient lbc;

		// Token: 0x04000FAB RID: 4011
		private ConnectionHandler ch;

		// Token: 0x04000FAC RID: 4012
		public Text StateUiText;
	}
}
