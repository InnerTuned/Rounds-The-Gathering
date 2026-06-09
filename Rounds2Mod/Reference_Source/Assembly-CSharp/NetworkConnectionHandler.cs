using System;
using System.Collections;
using ExitGames.Client.Photon;
using Landfall.Network;
using Photon.Pun;
using Photon.Realtime;
using SoundImplementation;
using Steamworks;
using UnityEngine;

// Token: 0x0200016F RID: 367
public class NetworkConnectionHandler : MonoBehaviourPunCallbacks
{
	// Token: 0x06000760 RID: 1888 RVA: 0x00028048 File Offset: 0x00026248
	private void Start()
	{
		NetworkConnectionHandler.instance = this;
		PhotonNetwork.ServerPortOverrides = PhotonPortDefinition.AlternativeUdpPorts;
		PhotonNetwork.CrcCheckEnabled = true;
		PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout = 30000;
		if (NetworkConnectionHandler.m_SteamLobby == null)
		{
			NetworkConnectionHandler.m_SteamLobby = new ClientSteamLobby();
			return;
		}
		NetworkConnectionHandler.m_SteamLobby.LeaveLobby();
	}

	// Token: 0x06000761 RID: 1889 RVA: 0x0002809C File Offset: 0x0002629C
	private void Update()
	{
		if (!this.m_SearchingQuickMatch)
		{
			return;
		}
		if (PhotonNetwork.InRoom && !PhotonNetwork.OfflineMode && !GM_ArmsRace.instance)
		{
			this.untilTryOtherRegionCounter -= Time.deltaTime;
			if (this.untilTryOtherRegionCounter < 0f)
			{
				base.StartCoroutine(this.PlayOnBestActiveRegion());
			}
		}
	}

	// Token: 0x06000762 RID: 1890 RVA: 0x000280F8 File Offset: 0x000262F8
	public void QuickMatch()
	{
		this.m_SearchingQuickMatch = true;
		this.m_SearchingTwitch = false;
		TimeHandler.instance.gameStartTime = 1f;
		LoadingScreen loadingScreen = LoadingScreen.instance;
		if (loadingScreen != null)
		{
			loadingScreen.StartLoading(false);
		}
		base.StartCoroutine(this.DoActionWhenConnected(new Action(this.JoinRandomRoom)));
	}

	// Token: 0x06000763 RID: 1891 RVA: 0x0002814C File Offset: 0x0002634C
	public void TwitchJoin(int score)
	{
		this.currentViewers = Mathf.Clamp(score, 1, score);
		this.m_SearchingQuickMatch = false;
		this.m_SearchingTwitch = true;
		Hashtable customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
		if (customProperties.ContainsKey(NetworkConnectionHandler.TWITCH_PLAYER_SCORE_KEY))
		{
			customProperties[NetworkConnectionHandler.TWITCH_PLAYER_SCORE_KEY] = score;
		}
		else
		{
			customProperties.Add(NetworkConnectionHandler.TWITCH_PLAYER_SCORE_KEY, score);
		}
		PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties, null, null);
		TimeHandler.instance.gameStartTime = 1f;
		LoadingScreen loadingScreen = LoadingScreen.instance;
		if (loadingScreen != null)
		{
			loadingScreen.StartLoading(false);
		}
		base.StartCoroutine(this.DoActionWhenConnected(new Action(this.JoinSpecificTWITCHRoom)));
	}

	// Token: 0x06000764 RID: 1892 RVA: 0x000281F8 File Offset: 0x000263F8
	public void HostPrivateAndInviteFriend()
	{
		this.m_SearchingQuickMatch = false;
		this.m_SearchingTwitch = false;
		TimeHandler.instance.gameStartTime = 1f;
		LoadingScreen loadingScreen = LoadingScreen.instance;
		if (loadingScreen != null)
		{
			loadingScreen.StartLoading(true);
		}
		RoomOptions options = new RoomOptions();
		options.MaxPlayers = 2;
		options.IsOpen = true;
		options.IsVisible = false;
		NetworkConnectionHandler.m_SteamLobby.ShowInviteScreenWhenConnected();
		base.StartCoroutine(this.DoActionWhenConnected(delegate
		{
			this.CreateRoom(options);
		}));
	}

	// Token: 0x06000765 RID: 1893 RVA: 0x00028292 File Offset: 0x00026492
	private void JoinRandomRoom()
	{
		global::Debug.Log("Joining random room");
		PhotonNetwork.JoinRandomRoom();
	}

	// Token: 0x06000766 RID: 1894 RVA: 0x000282A4 File Offset: 0x000264A4
	private void CreateSpecificTWITCHRoom()
	{
		global::Debug.Log("Creating SPECIFIC TWITCH ROOM!");
		RoomOptions roomOptions = new RoomOptions();
		RoomOptions roomOptions2 = roomOptions;
		Hashtable hashtable = new Hashtable();
		hashtable.Add(NetworkConnectionHandler.TWITCH_ROOM_AUDIENCE_RATING_KEY, this.currentViewers);
		roomOptions2.CustomRoomProperties = hashtable;
		roomOptions.CustomRoomPropertiesForLobby = new string[]
		{
			NetworkConnectionHandler.TWITCH_ROOM_AUDIENCE_RATING_KEY
		};
		PhotonNetwork.CreateRoom(null, roomOptions, this.sqlLobby, null);
	}

	// Token: 0x06000767 RID: 1895 RVA: 0x00028308 File Offset: 0x00026508
	private void JoinSpecificTWITCHRoom()
	{
		global::Debug.Log("JOINING SPECIFIC TWITCH ROOM!");
		int num = 5;
		int num2 = this.currentViewers * num;
		int num3 = this.currentViewers / num;
		int num4 = 10;
		int num5 = this.currentViewers * num4;
		int num6 = this.currentViewers / num4;
		int num7 = 10000000;
		int num8 = this.currentViewers * num7;
		int num9 = this.currentViewers / num7;
		string text = "";
		text = string.Concat(new object[]
		{
			text,
			"C0 BETWEEN ",
			num3,
			" AND ",
			num2,
			";"
		});
		text = string.Concat(new object[]
		{
			text,
			"C0 BETWEEN ",
			num6,
			" AND ",
			num5,
			";"
		});
		text = string.Concat(new object[]
		{
			text,
			"C0 BETWEEN ",
			num9,
			" AND ",
			num8
		});
		PhotonNetwork.JoinRandomRoom(null, 0, 0, this.sqlLobby, text, null);
	}

	// Token: 0x06000768 RID: 1896 RVA: 0x00028430 File Offset: 0x00026630
	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		global::Debug.Log("JOINED RANDOM ROOM FAILED!");
		if (!this.m_SearchingTwitch)
		{
			this.JoinRandomRoom();
			return;
		}
		this.CreateSpecificTWITCHRoom();
	}

	// Token: 0x06000769 RID: 1897 RVA: 0x00028451 File Offset: 0x00026651
	private IEnumerator DoActionWhenConnected(Action action)
	{
		yield return this.WaitForConnect();
		action.Invoke();
		yield break;
	}

	// Token: 0x0600076A RID: 1898 RVA: 0x00028467 File Offset: 0x00026667
	private IEnumerator PlayOnBestActiveRegion()
	{
		if (PhotonNetwork.InRoom)
		{
			PhotonNetwork.LeaveRoom(true);
			while (PhotonNetwork.InRoom)
			{
				yield return null;
			}
		}
		string[] regionsToTry = new string[]
		{
			"usw",
			"eu",
			"us",
			"au",
			"ru",
			"za",
			"asia",
			"cae",
			"in",
			"jp",
			"rue",
			"sa",
			"kr"
		};
		float bestRegionScore = 0f;
		string bestRegion = "";
		int num2;
		for (int i = 0; i < regionsToTry.Length; i = num2 + 1)
		{
			this.isConnectedToMaster = false;
			PhotonNetwork.Disconnect();
			while (PhotonNetwork.IsConnected)
			{
				yield return null;
			}
			PhotonNetwork.ConnectToRegion(regionsToTry[i]);
			global::Debug.Log("connectToRegion " + regionsToTry[i]);
			this.isConnectedToMaster = false;
			while (!this.isConnectedToMaster)
			{
				yield return null;
			}
			int countOfPlayersInRooms = PhotonNetwork.CountOfPlayersInRooms;
			int ping = PhotonNetwork.GetPing();
			float num = (float)Mathf.Clamp(countOfPlayersInRooms, 0, 50) / Mathf.Clamp((float)ping, 10f, 1E+11f);
			global::Debug.Log("Ping: " + Mathf.Clamp((float)PhotonNetwork.GetPing(), 10f, 1E+11f));
			global::Debug.Log(regionsToTry[i] + ": " + PhotonNetwork.CountOfPlayersInRooms);
			if (num > bestRegionScore)
			{
				bestRegion = regionsToTry[i];
				bestRegionScore = num;
				if (ping < 50 && countOfPlayersInRooms > 50)
				{
					break;
				}
			}
			num2 = i;
		}
		this.isConnectedToMaster = false;
		PhotonNetwork.Disconnect();
		PhotonNetwork.LocalPlayer.NickName = "PlayerName";
		if (bestRegion == "")
		{
			PhotonNetwork.ConnectToBestCloudServer();
		}
		else
		{
			global::Debug.Log("Connecting to " + bestRegion);
			PhotonNetwork.ConnectToRegion(bestRegion);
		}
		while (!this.isConnectedToMaster)
		{
			yield return null;
		}
		this.JoinRandomRoom();
		yield break;
	}

	// Token: 0x0600076B RID: 1899 RVA: 0x00028476 File Offset: 0x00026676
	private IEnumerator WaitForConnect()
	{
		if (!PhotonNetwork.IsConnectedAndReady)
		{
			PhotonNetwork.LocalPlayer.NickName = "PlayerName";
			PhotonNetwork.ConnectUsingSettings();
			if (this.hasRegionSelect || this.m_ForceRegion)
			{
				PhotonNetwork.ConnectToRegion(RegionSelector.region);
			}
			else
			{
				PhotonNetwork.ConnectToBestCloudServer();
			}
		}
		while (!this.isConnectedToMaster)
		{
			global::Debug.Log("Trying to connect to photon");
			yield return null;
		}
		global::Debug.Log("Is connected");
		yield break;
	}

	// Token: 0x0600076C RID: 1900 RVA: 0x00028488 File Offset: 0x00026688
	public void ForceRegionJoin(string region, string room)
	{
		global::Debug.Log("CREEASDSSD");
		if (PhotonNetwork.InRoom)
		{
			PhotonNetwork.Disconnect();
		}
		CharacterCreatorHandler.instance.CloseMenus();
		MainMenuHandler.instance.Close();
		RegionSelector.region = region;
		TimeHandler.instance.gameStartTime = 1f;
		LoadingScreen loadingScreen = LoadingScreen.instance;
		if (loadingScreen != null)
		{
			loadingScreen.StartLoading(false);
		}
		this.m_ForceRegion = true;
		base.StartCoroutine(this.DoActionWhenConnected(delegate
		{
			this.JoinSpecificRoom(room);
		}));
	}

	// Token: 0x0600076D RID: 1901 RVA: 0x00028519 File Offset: 0x00026719
	private void JoinSpecificRoom(string room)
	{
		PhotonNetwork.JoinRoom(room, null);
		this.m_ForceRegion = false;
	}

	// Token: 0x0600076E RID: 1902 RVA: 0x0002852A File Offset: 0x0002672A
	public override void OnEnable()
	{
		base.OnEnable();
		global::Debug.Log("Add me!");
		PhotonNetwork.AddCallbackTarget(this);
	}

	// Token: 0x0600076F RID: 1903 RVA: 0x00028542 File Offset: 0x00026742
	public override void OnDisable()
	{
		base.OnDisable();
		global::Debug.Log("Remove me!");
		PhotonNetwork.RemoveCallbackTarget(this);
	}

	// Token: 0x06000770 RID: 1904 RVA: 0x0002855A File Offset: 0x0002675A
	public override void OnConnectedToMaster()
	{
		this.isConnectedToMaster = true;
	}

	// Token: 0x06000771 RID: 1905 RVA: 0x00028564 File Offset: 0x00026764
	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		global::Debug.Log("JOINED RANDOM ROOM FAILED!");
		if (this.m_SearchingTwitch)
		{
			this.CreateSpecificTWITCHRoom();
			return;
		}
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = 2;
		roomOptions.IsOpen = true;
		roomOptions.IsVisible = true;
		if (!SteamManager.Initialized)
		{
			global::Debug.LogError("SteamManager is not initialized!");
			return;
		}
		this.CreateRoom(roomOptions);
	}

	// Token: 0x06000772 RID: 1906 RVA: 0x000285C0 File Offset: 0x000267C0
	private void CreateRoom(RoomOptions roomOptions)
	{
		NetworkConnectionHandler.m_SteamLobby.CreateLobby((int)roomOptions.MaxPlayers, delegate(string RoomName)
		{
			PhotonNetwork.CreateRoom(RoomName, roomOptions, null, null);
		});
	}

	// Token: 0x06000773 RID: 1907 RVA: 0x000285FC File Offset: 0x000267FC
	public override void OnJoinedRoom()
	{
		if (PhotonNetwork.OfflineMode)
		{
			return;
		}
		this.isConnectedToMaster = false;
		global::Debug.Log("Room joined successfully");
		global::Debug.Log(PhotonNetwork.CloudRegion);
		this.untilTryOtherRegionCounter = 15f;
		PhotonNetwork.LocalPlayer.NickName = (this.m_SearchingTwitch ? TwitchUIHandler.TWITCH_NAME_KEY : SteamFriends.GetPersonaName());
	}

	// Token: 0x06000774 RID: 1908 RVA: 0x00028658 File Offset: 0x00026858
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		SoundPlayerStatic.Instance.PlayPlayerAdded();
		if (PhotonNetwork.PlayerList.Length == 2)
		{
			if (PhotonNetwork.IsMasterClient)
			{
				base.GetComponent<PhotonView>().RPC("RPCA_FoundGame", 0, Array.Empty<object>());
			}
			if (NetworkConnectionHandler.m_SteamLobby != null)
			{
				NetworkConnectionHandler.m_SteamLobby.HideLobby();
			}
		}
		global::Debug.Log("PlayerJoined");
		base.OnPlayerEnteredRoom(newPlayer);
	}

	// Token: 0x06000775 RID: 1909 RVA: 0x000286B8 File Offset: 0x000268B8
	[PunRPC]
	private void RPCA_FoundGame()
	{
		LoadingScreen loadingScreen = LoadingScreen.instance;
		if (loadingScreen == null)
		{
			return;
		}
		loadingScreen.StopLoading();
	}

	// Token: 0x06000776 RID: 1910 RVA: 0x000286C9 File Offset: 0x000268C9
	public override void OnLeftRoom()
	{
		this.isConnectedToMaster = false;
	}

	// Token: 0x06000777 RID: 1911 RVA: 0x000286D2 File Offset: 0x000268D2
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		GM_ArmsRace.instance == null;
		base.StartCoroutine(this.DoDisconnect("DISCONNECTED", "Other player left"));
		base.OnPlayerLeftRoom(otherPlayer);
	}

	// Token: 0x06000778 RID: 1912 RVA: 0x000286FE File Offset: 0x000268FE
	public override void OnDisconnected(DisconnectCause cause)
	{
		if (cause == null)
		{
			return;
		}
		if (cause == 13)
		{
			return;
		}
		base.StartCoroutine(this.DoDisconnect("DISCONNECTED", cause.ToString()));
		this.isConnectedToMaster = false;
	}

	// Token: 0x06000779 RID: 1913 RVA: 0x00028730 File Offset: 0x00026930
	private IEnumerator DoRetry()
	{
		LoadingScreen.instance.StartLoading(false);
		if (PhotonNetwork.InRoom)
		{
			PhotonNetwork.LeaveRoom(true);
			while (PhotonNetwork.InRoom)
			{
				yield return null;
			}
		}
		this.JoinRandomRoom();
		yield break;
	}

	// Token: 0x0600077A RID: 1914 RVA: 0x0002873F File Offset: 0x0002693F
	private IEnumerator DoDisconnect(string context, string reason)
	{
		ErrorHandler.instance.ShowError(context, reason);
		yield return new WaitForSecondsRealtime(2f);
		ErrorHandler.instance.HideError();
		this.NetworkRestart();
		yield break;
	}

	// Token: 0x0600077B RID: 1915 RVA: 0x0002875C File Offset: 0x0002695C
	public override void OnRegionListReceived(RegionHandler regionHandler)
	{
		global::Debug.Log(regionHandler);
	}

	// Token: 0x0600077C RID: 1916 RVA: 0x00028764 File Offset: 0x00026964
	public void NetworkRestart()
	{
		this.isConnectedToMaster = false;
		if (PhotonNetwork.OfflineMode)
		{
			Application.LoadLevel(Application.loadedLevel);
			return;
		}
		base.StartCoroutine(this.WaitForRestart());
	}

	// Token: 0x0600077D RID: 1917 RVA: 0x0002878C File Offset: 0x0002698C
	private IEnumerator WaitForRestart()
	{
		if (NetworkConnectionHandler.m_SteamLobby != null)
		{
			NetworkConnectionHandler.m_SteamLobby.LeaveLobby();
		}
		if (PhotonNetwork.InRoom)
		{
			PhotonNetwork.LeaveRoom(true);
			while (PhotonNetwork.InRoom)
			{
				yield return null;
			}
		}
		if (PhotonNetwork.IsConnected)
		{
			PhotonNetwork.Disconnect();
			while (PhotonNetwork.IsConnected)
			{
				yield return null;
			}
		}
		EscapeMenuHandler.isEscMenu = false;
		DevConsole.isTyping = false;
		Application.LoadLevel(Application.loadedLevel);
		yield break;
	}

	// Token: 0x040008C5 RID: 2245
	public static readonly string TWITCH_PLAYER_SCORE_KEY = "TwitchScore";

	// Token: 0x040008C6 RID: 2246
	public static readonly string TWITCH_ROOM_AUDIENCE_RATING_KEY = "C0";

	// Token: 0x040008C7 RID: 2247
	public static NetworkConnectionHandler instance;

	// Token: 0x040008C8 RID: 2248
	private static ClientSteamLobby m_SteamLobby;

	// Token: 0x040008C9 RID: 2249
	private bool m_SearchingQuickMatch;

	// Token: 0x040008CA RID: 2250
	private bool m_SearchingTwitch;

	// Token: 0x040008CB RID: 2251
	private int currentViewers = 100;

	// Token: 0x040008CC RID: 2252
	private TypedLobby sqlLobby = new TypedLobby("customSqlLobby", 2);

	// Token: 0x040008CD RID: 2253
	public bool hasRegionSelect;

	// Token: 0x040008CE RID: 2254
	private bool m_ForceRegion;

	// Token: 0x040008CF RID: 2255
	private bool isConnectedToMaster;

	// Token: 0x040008D0 RID: 2256
	private float untilTryOtherRegionCounter;
}
