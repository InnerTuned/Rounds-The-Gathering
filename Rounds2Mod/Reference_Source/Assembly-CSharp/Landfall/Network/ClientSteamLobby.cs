using System;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Steamworks;

namespace Landfall.Network
{
	// Token: 0x0200031D RID: 797
	public class ClientSteamLobby
	{
		// Token: 0x17000137 RID: 311
		// (get) Token: 0x060010F2 RID: 4338 RVA: 0x00051AD1 File Offset: 0x0004FCD1
		public bool IsMaster
		{
			get
			{
				return SteamMatchmaking.GetLobbyOwner(this.CurrentLobby) == SteamUser.GetSteamID();
			}
		}

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x060010F3 RID: 4339 RVA: 0x00051AE8 File Offset: 0x0004FCE8
		public bool IsInsideLobby
		{
			get
			{
				return this.CurrentLobby.IsValid() && this.CurrentLobby.IsLobby();
			}
		}

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x060010F4 RID: 4340 RVA: 0x00051B15 File Offset: 0x0004FD15
		public int NumberOfMembers
		{
			get
			{
				if (!this.m_IsActive)
				{
					return 0;
				}
				return SteamMatchmaking.GetNumLobbyMembers(this.CurrentLobby);
			}
		}

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x060010F5 RID: 4341 RVA: 0x00051B2C File Offset: 0x0004FD2C
		// (set) Token: 0x060010F6 RID: 4342 RVA: 0x00051B34 File Offset: 0x0004FD34
		public CSteamID CurrentLobby { get; private set; }

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x060010F7 RID: 4343 RVA: 0x00051B3D File Offset: 0x0004FD3D
		// (set) Token: 0x060010F8 RID: 4344 RVA: 0x00051B45 File Offset: 0x0004FD45
		public bool IsActive
		{
			get
			{
				return this.m_IsActive;
			}
			set
			{
				if (!SteamManager.Initialized)
				{
					this.m_IsActive = false;
					return;
				}
				this.m_IsActive = value;
			}
		}

		// Token: 0x060010F9 RID: 4345 RVA: 0x00051B60 File Offset: 0x0004FD60
		public ClientSteamLobby()
		{
			this.m_IsActive = SteamManager.Initialized;
			if (this.m_IsActive)
			{
				this.CreateCallbacks();
			}
			this.CurrentLobby = CSteamID.Nil;
			this.CheckForCommandLine();
		}

		// Token: 0x060010FA RID: 4346 RVA: 0x00051BB0 File Offset: 0x0004FDB0
		private void CheckForCommandLine()
		{
			string text;
			int launchCommandLine = SteamApps.GetLaunchCommandLine(ref text, 260);
			if (text == null)
			{
				text = "";
			}
			Debug.LogError(string.Concat(new object[]
			{
				"Command Line: ret: ",
				launchCommandLine,
				" : ",
				text
			}));
		}

		// Token: 0x060010FB RID: 4347 RVA: 0x00051C00 File Offset: 0x0004FE00
		private void CreateCallbacks()
		{
			this.m_OnLobbyCreatedCallresult = CallResult<LobbyCreated_t>.Create(new CallResult<LobbyCreated_t>.APIDispatchDelegate(this.OnLobbyCreated));
			this.m_OnLobbyEnteredCallresult = CallResult<LobbyEnter_t>.Create(new CallResult<LobbyEnter_t>.APIDispatchDelegate(this.OnLobbyEnter));
			this.m_OnLobbyJoinRequest = Callback<GameLobbyJoinRequested_t>.Create(new Callback<GameLobbyJoinRequested_t>.DispatchDelegate(this.OnLobbyJoinRequest));
			this.m_OnLobbyUpdated = Callback<LobbyChatUpdate_t>.Create(new Callback<LobbyChatUpdate_t>.DispatchDelegate(this.OnLobbyMembersUpdated));
			this.m_OnLobbyMemberDataUpdated = Callback<LobbyDataUpdate_t>.Create(new Callback<LobbyDataUpdate_t>.DispatchDelegate(this.OnLobbyMemberDataUpdated));
			this.m_OnLobbyChatMessage = Callback<LobbyChatMsg_t>.Create(new Callback<LobbyChatMsg_t>.DispatchDelegate(this.OnLobbyChatMessage));
			this.m_OnInviteCallBack = Callback<LobbyInvite_t>.Create(new Callback<LobbyInvite_t>.DispatchDelegate(this.OnInviteToLobby));
		}

		// Token: 0x060010FC RID: 4348 RVA: 0x00051CB0 File Offset: 0x0004FEB0
		public void JoinedRoom(string roomName)
		{
			if (!this.m_IsActive)
			{
				return;
			}
			if (this.IsMaster)
			{
				byte[] roomData = this.GetRoomData(roomName);
				Debug.Log("Joined room: " + roomName + " And is master!");
				if (this.SendDataThroughLobby(roomData))
				{
					Debug.Log("Sent message to others in lobby about room");
				}
			}
			this.m_IsActive = false;
		}

		// Token: 0x060010FD RID: 4349 RVA: 0x00051D08 File Offset: 0x0004FF08
		private byte[] GetRequestStartGameData()
		{
			if (!this.m_IsActive)
			{
				return new byte[0];
			}
			byte b = 3;
			byte[] array = new byte[2];
			using (MemoryStream memoryStream = new MemoryStream(array))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(b);
				}
			}
			return array;
		}

		// Token: 0x060010FE RID: 4350 RVA: 0x00051D78 File Offset: 0x0004FF78
		private byte[] GetRoomData(string roomName)
		{
			if (!this.m_IsActive)
			{
				return new byte[0];
			}
			byte b = 1;
			byte[] array = new byte[2 * roomName.Length + 1];
			using (MemoryStream memoryStream = new MemoryStream(array))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(b);
					binaryWriter.Write(roomName);
				}
			}
			return array;
		}

		// Token: 0x060010FF RID: 4351 RVA: 0x00051DF8 File Offset: 0x0004FFF8
		public void InviteFriend(CSteamID id)
		{
			bool flag = SteamMatchmaking.InviteUserToLobby(this.CurrentLobby, id);
			Debug.Log("Sent invite to friend: " + id + (flag ? " SUCCESS!" : " FAIL"));
		}

		// Token: 0x06001100 RID: 4352 RVA: 0x00051E36 File Offset: 0x00050036
		private bool SendDataThroughLobby(byte[] data)
		{
			return SteamMatchmaking.SendLobbyChatMsg(this.CurrentLobby, data, data.Length);
		}

		// Token: 0x06001101 RID: 4353 RVA: 0x00051E47 File Offset: 0x00050047
		public void ShowInviteScreenWhenConnected()
		{
			this.AddOnLobbyCreatedAction(delegate
			{
				Debug.Log("Activating SteamOverlay for INVITE: With Lobby: " + this.CurrentLobby);
				SteamFriends.ActivateGameOverlayInviteDialog(this.CurrentLobby);
			});
		}

		// Token: 0x06001102 RID: 4354 RVA: 0x00051E5C File Offset: 0x0005005C
		public void GetFriendsPlayingROUNDS(Action<List<CSteamID>> a)
		{
			List<CSteamID> list = new List<CSteamID>();
			for (int i = 0; i < this.NumberOfMembers; i++)
			{
				list.Add(SteamMatchmaking.GetLobbyMemberByIndex(this.CurrentLobby, i));
			}
			List<CSteamID> list2 = new List<CSteamID>();
			int friendCount = SteamFriends.GetFriendCount(4);
			for (int j = 0; j < friendCount; j++)
			{
				CSteamID friendByIndex = SteamFriends.GetFriendByIndex(j, 4);
				FriendGameInfo_t friendGameInfo_t;
				if (SteamFriends.GetFriendGamePlayed(friendByIndex, ref friendGameInfo_t) && friendGameInfo_t.m_gameID.AppID() == this.ROUNDS_APPID && !list.Contains(friendByIndex))
				{
					Debug.Log(string.Concat(new object[]
					{
						"Friend: ",
						j,
						" : ",
						SteamFriends.GetFriendPersonaName(friendByIndex),
						" Is playing ROUNDS!"
					}));
					list2.Add(friendByIndex);
				}
			}
			a.Invoke(list2);
		}

		// Token: 0x06001103 RID: 4355 RVA: 0x00051F38 File Offset: 0x00050138
		public void CreateLobby(int maxPlayers, Action<string> OnSuccess)
		{
			if (!this.m_IsActive)
			{
				return;
			}
			this.m_OnLobbyCreatedActionString = OnSuccess;
			SteamAPICall_t steamAPICall_t = SteamMatchmaking.CreateLobby(0, maxPlayers);
			this.m_OnLobbyCreatedCallresult.Set(steamAPICall_t, null);
		}

		// Token: 0x06001104 RID: 4356 RVA: 0x00051F6A File Offset: 0x0005016A
		private void AddOnLobbyCreatedAction(Action a)
		{
			this.m_OnLobbyCreatedAction = (Action)Delegate.Combine(this.m_OnLobbyCreatedAction, a);
		}

		// Token: 0x06001105 RID: 4357 RVA: 0x00051F83 File Offset: 0x00050183
		public void HideLobby()
		{
			if (!this.m_IsActive)
			{
				return;
			}
			if (!this.IsMaster)
			{
				return;
			}
			SteamMatchmaking.SetLobbyJoinable(this.CurrentLobby, false);
		}

		// Token: 0x06001106 RID: 4358 RVA: 0x00051FA4 File Offset: 0x000501A4
		public void OpenLobby()
		{
			if (!this.m_IsActive)
			{
				return;
			}
			if (!this.IsMaster)
			{
				return;
			}
			SteamMatchmaking.SetLobbyJoinable(this.CurrentLobby, true);
		}

		// Token: 0x06001107 RID: 4359 RVA: 0x00051FC8 File Offset: 0x000501C8
		private void JoinLobby(CSteamID lobbyID)
		{
			if (!this.m_IsActive)
			{
				return;
			}
			SteamAPICall_t steamAPICall_t = SteamMatchmaking.JoinLobby(lobbyID);
			this.m_OnLobbyEnteredCallresult.Set(steamAPICall_t, null);
		}

		// Token: 0x06001108 RID: 4360 RVA: 0x00008DB5 File Offset: 0x00006FB5
		private bool CheckAppVersion(CSteamID lobbyID)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001109 RID: 4361 RVA: 0x00051FF4 File Offset: 0x000501F4
		private void LeaveCurrentLobby()
		{
			if (!this.m_IsActive)
			{
				return;
			}
			if (this.CurrentLobby.IsLobby())
			{
				SteamMatchmaking.LeaveLobby(this.CurrentLobby);
			}
		}

		// Token: 0x0600110A RID: 4362 RVA: 0x00052025 File Offset: 0x00050225
		public void GoOffline()
		{
			this.LeaveCurrentLobby();
			this.m_IsActive = false;
		}

		// Token: 0x0600110B RID: 4363 RVA: 0x00052034 File Offset: 0x00050234
		public void LeaveLobby()
		{
			this.LeaveCurrentLobby();
		}

		// Token: 0x0600110C RID: 4364 RVA: 0x0005203C File Offset: 0x0005023C
		public string[] GetPhotonIDsOfMembers()
		{
			if (!this.m_IsActive)
			{
				return new string[0];
			}
			int numberOfMembers = this.NumberOfMembers;
			string[] result = new string[numberOfMembers];
			Debug.Log("Reserving spots for : " + numberOfMembers + " Players!");
			for (int i = 0; i < numberOfMembers; i++)
			{
				string friendPersonaName = SteamFriends.GetFriendPersonaName(SteamMatchmaking.GetLobbyMemberByIndex(this.CurrentLobby, i));
				Debug.Log(string.Concat(new object[]
				{
					"Member: ",
					i,
					" : ",
					friendPersonaName
				}));
			}
			return result;
		}

		// Token: 0x0600110D RID: 4365 RVA: 0x000520CC File Offset: 0x000502CC
		private void UpdateLobbyMembers()
		{
			if (!this.m_IsActive)
			{
				return;
			}
			this.Refresh();
		}

		// Token: 0x0600110E RID: 4366 RVA: 0x000027C8 File Offset: 0x000009C8
		private void UpdateLobbyData()
		{
		}

		// Token: 0x0600110F RID: 4367 RVA: 0x000027C8 File Offset: 0x000009C8
		public void Refresh()
		{
		}

		// Token: 0x06001110 RID: 4368 RVA: 0x000520DD File Offset: 0x000502DD
		private void UpdateCurrentLobby(CSteamID lobbyID)
		{
			this.CurrentLobby = lobbyID;
		}

		// Token: 0x06001111 RID: 4369 RVA: 0x000027C8 File Offset: 0x000009C8
		private void UpdateCurrentLobbyCallback()
		{
		}

		// Token: 0x06001112 RID: 4370 RVA: 0x000520E6 File Offset: 0x000502E6
		public void UpdateGear(string gearString)
		{
			if (!this.m_IsActive)
			{
				return;
			}
			SteamMatchmaking.SetLobbyMemberData(this.CurrentLobby, "GearKey", gearString);
		}

		// Token: 0x06001113 RID: 4371 RVA: 0x00008DB5 File Offset: 0x00006FB5
		public void RequestStartGame()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001114 RID: 4372 RVA: 0x00008DB5 File Offset: 0x00006FB5
		public void ReadyUp(bool ready)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001115 RID: 4373 RVA: 0x00008DB5 File Offset: 0x00006FB5
		public void UpdateRegion()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001116 RID: 4374 RVA: 0x00008DB5 File Offset: 0x00006FB5
		public void UpdateMatchMode()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001117 RID: 4375 RVA: 0x00052104 File Offset: 0x00050304
		private void OnInviteToLobby(LobbyInvite_t param)
		{
			if (param.m_ulGameID != (ulong)this.ROUNDS_APPID.m_AppId)
			{
				return;
			}
			Debug.Log("Invite recieved from: " + param.m_ulSteamIDUser);
			SteamFriends.GetFriendPersonaName(new CSteamID(param.m_ulSteamIDUser));
		}

		// Token: 0x06001118 RID: 4376 RVA: 0x00052174 File Offset: 0x00050374
		private void OnLobbyChatMessage(LobbyChatMsg_t param)
		{
			if (new CSteamID(param.m_ulSteamIDUser) == SteamUser.GetSteamID())
			{
				Debug.Log("Recieving callback for local message, returning...");
				return;
			}
			if (new CSteamID(param.m_ulSteamIDLobby) != this.CurrentLobby)
			{
				Debug.LogError("Recieved lobby message for another lobby, returning...");
				return;
			}
			EChatEntryType eChatEntryType = param.m_eChatEntryType;
			if (eChatEntryType == 1)
			{
				int num = 60;
				byte[] array = new byte[num];
				CSteamID csteamID;
				EChatEntryType echatEntryType;
				int lobbyChatEntry = SteamMatchmaking.GetLobbyChatEntry(this.CurrentLobby, (int)param.m_iChatID, ref csteamID, array, num, ref echatEntryType);
				Debug.Log("Lobby Chsat Msg: " + lobbyChatEntry);
				return;
			}
			Debug.LogError("Recived unexpected lobbychat message: " + eChatEntryType.ToString());
		}

		// Token: 0x06001119 RID: 4377 RVA: 0x00052227 File Offset: 0x00050427
		private void OnLobbyMembersUpdated(LobbyChatUpdate_t param)
		{
			uint rgfChatMemberStateChange = param.m_rgfChatMemberStateChange;
			if (new CSteamID(param.m_ulSteamIDLobby) == this.CurrentLobby)
			{
				this.UpdateLobbyMembers();
				return;
			}
			Debug.LogError("Getting update for another lobby!?");
		}

		// Token: 0x0600111A RID: 4378 RVA: 0x00052259 File Offset: 0x00050459
		private void OnLobbyMemberDataUpdated(LobbyDataUpdate_t param)
		{
			if (new CSteamID(param.m_ulSteamIDLobby) == this.CurrentLobby)
			{
				this.UpdateLobbyMembers();
				this.UpdateLobbyData();
				return;
			}
			Debug.LogError("Getting Lobbychat update for another lobby!?");
		}

		// Token: 0x0600111B RID: 4379 RVA: 0x0005228A File Offset: 0x0005048A
		private void OnLobbyJoinRequest(GameLobbyJoinRequested_t param)
		{
			Debug.Log("Lobby Join Request! " + param.m_steamIDLobby);
			this.LeaveCurrentLobby();
			this.JoinLobby(param.m_steamIDLobby);
		}

		// Token: 0x0600111C RID: 4380 RVA: 0x000522B8 File Offset: 0x000504B8
		private void OnLobbyCreated(LobbyCreated_t param, bool bIOFailure)
		{
			this.InternalOnLobbyCreated(param, bIOFailure, this.m_OnLobbyCreatedActionString, null);
		}

		// Token: 0x0600111D RID: 4381 RVA: 0x000522CC File Offset: 0x000504CC
		private void InternalOnLobbyCreated(LobbyCreated_t param, bool bIOFailure, Action<string> OnSuccess = null, Action OnFail = null)
		{
			if (bIOFailure)
			{
				Debug.LogError("BioFail");
				return;
			}
			if (param.m_eResult == 1)
			{
				Debug.Log("Successfully created A steam Lobby");
				this.UpdateCurrentLobby(new CSteamID(param.m_ulSteamIDLobby));
				SteamMatchmaking.SetLobbyData(this.CurrentLobby, "RegionKey", PhotonNetwork.CloudRegion);
				if (OnSuccess != null)
				{
					OnSuccess.Invoke(param.m_ulSteamIDLobby.ToString());
				}
				Action onLobbyCreatedAction = this.m_OnLobbyCreatedAction;
				if (onLobbyCreatedAction != null)
				{
					onLobbyCreatedAction.Invoke();
				}
				this.ClearActions();
				return;
			}
			Debug.Log("Failure creating A steam Lobby " + param.m_eResult.ToString());
		}

		// Token: 0x0600111E RID: 4382 RVA: 0x0005236F File Offset: 0x0005056F
		private void ClearActions()
		{
			this.m_OnLobbyCreatedAction = null;
			this.m_OnLobbyCreatedActionString = null;
		}

		// Token: 0x0600111F RID: 4383 RVA: 0x00052380 File Offset: 0x00050580
		private void OnLobbyEnter(LobbyEnter_t param, bool bIOFailure)
		{
			if (bIOFailure)
			{
				Debug.LogError("BioFail");
				return;
			}
			if (param.m_EChatRoomEnterResponse == 1U)
			{
				Debug.Log("Successfully Entered A steam Lobby");
				CSteamID csteamID;
				csteamID..ctor(param.m_ulSteamIDLobby);
				this.UpdateCurrentLobby(csteamID);
				if (SteamManager.Initialized)
				{
					string lobbyData = SteamMatchmaking.GetLobbyData(csteamID, "RegionKey");
					NetworkConnectionHandler.instance.ForceRegionJoin(lobbyData, csteamID.ToString());
				}
			}
		}

		// Token: 0x04000FDC RID: 4060
		private const string REGION_KEY = "RegionKey";

		// Token: 0x04000FDD RID: 4061
		private const string ROOM_KEY = "RoomKey";

		// Token: 0x04000FDE RID: 4062
		private const string GEAR_KEY = "GearKey";

		// Token: 0x04000FDF RID: 4063
		private CallResult<LobbyEnter_t> m_OnLobbyEnteredCallresult;

		// Token: 0x04000FE0 RID: 4064
		private CallResult<LobbyCreated_t> m_OnLobbyCreatedCallresult;

		// Token: 0x04000FE1 RID: 4065
		private Callback<GameLobbyJoinRequested_t> m_OnLobbyJoinRequest;

		// Token: 0x04000FE2 RID: 4066
		private Callback<LobbyInvite_t> m_OnInviteCallBack;

		// Token: 0x04000FE3 RID: 4067
		private Callback<LobbyChatUpdate_t> m_OnLobbyUpdated;

		// Token: 0x04000FE4 RID: 4068
		private Callback<LobbyDataUpdate_t> m_OnLobbyMemberDataUpdated;

		// Token: 0x04000FE5 RID: 4069
		private Callback<LobbyChatMsg_t> m_OnLobbyChatMessage;

		// Token: 0x04000FE6 RID: 4070
		private Action<string> m_OnLobbyCreatedActionString;

		// Token: 0x04000FE7 RID: 4071
		private Action m_OnLobbyCreatedAction;

		// Token: 0x04000FE9 RID: 4073
		private bool m_IsActive;

		// Token: 0x04000FEA RID: 4074
		private AppId_t ROUNDS_APPID = new AppId_t(1557740U);
	}
}
