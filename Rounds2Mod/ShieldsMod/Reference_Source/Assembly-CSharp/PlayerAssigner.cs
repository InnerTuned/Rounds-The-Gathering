using System;
using System.Collections;
using System.Collections.Generic;
using InControl;
using Photon.Pun;
using SoundImplementation;
using UnityEngine;

// Token: 0x02000094 RID: 148
public class PlayerAssigner : MonoBehaviour
{
	// Token: 0x0600033B RID: 827 RVA: 0x0001457F File Offset: 0x0001277F
	private void Awake()
	{
		PlayerAssigner.instance = this;
	}

	// Token: 0x0600033C RID: 828 RVA: 0x00014587 File Offset: 0x00012787
	internal void SetPlayersCanJoin(bool canJoin)
	{
		this.playersCanJoin = canJoin;
	}

	// Token: 0x0600033D RID: 829 RVA: 0x00014590 File Offset: 0x00012790
	private void Start()
	{
		InputManager.OnDeviceDetached += new Action<InputDevice>(this.OnDeviceDetached);
	}

	// Token: 0x0600033E RID: 830 RVA: 0x000145A4 File Offset: 0x000127A4
	private void LateUpdate()
	{
		if (!this.playersCanJoin)
		{
			return;
		}
		if (this.players.Count >= this.maxPlayers)
		{
			return;
		}
		if (DevConsole.isTyping)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.B) && !GameManager.lockInput)
		{
			base.StartCoroutine(this.CreatePlayer(null, true));
		}
		if (Input.GetKey(KeyCode.Space))
		{
			bool flag = true;
			for (int i = 0; i < this.players.Count; i++)
			{
				if (this.players[i].playerActions.Device == null)
				{
					flag = false;
				}
			}
			if (flag)
			{
				base.StartCoroutine(this.CreatePlayer(null, false));
			}
		}
		for (int j = 0; j < InputManager.ActiveDevices.Count; j++)
		{
			InputDevice inputDevice = InputManager.ActiveDevices[j];
			if (this.JoinButtonWasPressedOnDevice(inputDevice) && this.ThereIsNoPlayerUsingDevice(inputDevice))
			{
				base.StartCoroutine(this.CreatePlayer(inputDevice, false));
			}
		}
	}

	// Token: 0x0600033F RID: 831 RVA: 0x00014684 File Offset: 0x00012884
	private bool JoinButtonWasPressedOnDevice(InputDevice inputDevice)
	{
		return inputDevice.Action1.WasPressed || inputDevice.Action2.WasPressed || inputDevice.Action3.WasPressed || inputDevice.Action4.WasPressed;
	}

	// Token: 0x06000340 RID: 832 RVA: 0x000146BC File Offset: 0x000128BC
	private CharacterData FindPlayerUsingDevice(InputDevice inputDevice)
	{
		int count = this.players.Count;
		for (int i = 0; i < count; i++)
		{
			CharacterData characterData = this.players[i];
			if (characterData.playerActions.Device == inputDevice)
			{
				return characterData;
			}
		}
		return null;
	}

	// Token: 0x06000341 RID: 833 RVA: 0x000146FF File Offset: 0x000128FF
	public void ClearPlayers()
	{
		this.players.Clear();
	}

	// Token: 0x06000342 RID: 834 RVA: 0x0001470C File Offset: 0x0001290C
	private bool ThereIsNoPlayerUsingDevice(InputDevice inputDevice)
	{
		return this.FindPlayerUsingDevice(inputDevice) == null;
	}

	// Token: 0x06000343 RID: 835 RVA: 0x0001471C File Offset: 0x0001291C
	private void OnDeviceDetached(InputDevice inputDevice)
	{
		CharacterData characterData = this.FindPlayerUsingDevice(inputDevice);
		if (characterData != null)
		{
			this.RemovePlayer(characterData);
		}
	}

	// Token: 0x06000344 RID: 836 RVA: 0x00014744 File Offset: 0x00012944
	[PunRPC]
	public void RPCM_RequestTeamAndPlayerID(int askingPlayer)
	{
		int count = PlayerManager.instance.players.Count;
		int num = (count % 2 == 0) ? 0 : 1;
		base.GetComponent<PhotonView>().RPC("RPC_ReturnPlayerAndTeamID", PhotonNetwork.CurrentRoom.GetPlayer(askingPlayer), new object[]
		{
			count,
			num
		});
		this.waitingForRegisterResponse = true;
	}

	// Token: 0x06000345 RID: 837 RVA: 0x000147A5 File Offset: 0x000129A5
	[PunRPC]
	public void RPC_ReturnPlayerAndTeamID(int teamId, int playerID)
	{
		this.waitingForRegisterResponse = false;
		this.playerIDToSet = playerID;
		this.teamIDToSet = teamId;
	}

	// Token: 0x06000346 RID: 838 RVA: 0x000147BC File Offset: 0x000129BC
	public void OtherPlayerWasCreated()
	{
		this.waitingForRegisterResponse = false;
	}

	// Token: 0x06000347 RID: 839 RVA: 0x000147C5 File Offset: 0x000129C5
	public IEnumerator CreatePlayer(InputDevice inputDevice, bool isAI = false)
	{
		if (this.waitingForRegisterResponse)
		{
			yield break;
		}
		if (!PhotonNetwork.OfflineMode && this.hasCreatedLocalPlayer)
		{
			yield break;
		}
		if (this.players.Count < this.maxPlayers)
		{
			if (!PhotonNetwork.OfflineMode && !PhotonNetwork.IsMasterClient)
			{
				base.GetComponent<PhotonView>().RPC("RPCM_RequestTeamAndPlayerID", 2, new object[]
				{
					PhotonNetwork.LocalPlayer.ActorNumber
				});
				this.waitingForRegisterResponse = true;
			}
			while (this.waitingForRegisterResponse)
			{
				yield return null;
			}
			if (!PhotonNetwork.OfflineMode)
			{
				if (PhotonNetwork.IsMasterClient)
				{
					this.playerIDToSet = PlayerManager.instance.players.Count;
					this.teamIDToSet = ((this.playerIDToSet % 2 == 0) ? 0 : 1);
				}
			}
			else
			{
				this.playerIDToSet = PlayerManager.instance.players.Count;
				this.teamIDToSet = ((this.playerIDToSet % 2 == 0) ? 0 : 1);
			}
			this.hasCreatedLocalPlayer = true;
			SoundPlayerStatic.Instance.PlayPlayerAdded();
			Vector3 vector = Vector3.up * 100f;
			CharacterData component = PhotonNetwork.Instantiate(this.playerPrefab.name, vector, Quaternion.identity, 0, null).GetComponent<CharacterData>();
			if (isAI)
			{
				GameObject original = this.player1AI;
				if (this.players.Count > 0)
				{
					original = this.player2AI;
				}
				component.GetComponent<CharacterData>().SetAI(null);
				Object.Instantiate<GameObject>(original, component.transform.position, component.transform.rotation, component.transform);
			}
			else
			{
				if (inputDevice != null)
				{
					component.input.inputType = GeneralInput.InputType.Controller;
					component.playerActions = PlayerActions.CreateWithControllerBindings();
				}
				else
				{
					component.input.inputType = GeneralInput.InputType.Keyboard;
					component.playerActions = PlayerActions.CreateWithKeyboardBindings();
				}
				component.playerActions.Device = inputDevice;
			}
			this.players.Add(component);
			this.RegisterPlayer(component, this.teamIDToSet, this.playerIDToSet);
			yield break;
		}
		yield break;
	}

	// Token: 0x06000348 RID: 840 RVA: 0x000147E2 File Offset: 0x000129E2
	private void RegisterPlayer(CharacterData player, int teamID, int playerID)
	{
		PlayerManager.RegisterPlayer(player.player);
		player.player.AssignPlayerID(playerID);
		player.player.AssignTeamID(teamID);
	}

	// Token: 0x06000349 RID: 841 RVA: 0x000027C8 File Offset: 0x000009C8
	private void RemovePlayer(CharacterData player)
	{
	}

	// Token: 0x0600034A RID: 842 RVA: 0x000027C8 File Offset: 0x000009C8
	private void AssignPlayer(CharacterData player, InputDevice device)
	{
	}

	// Token: 0x04000462 RID: 1122
	public GameObject player1AI;

	// Token: 0x04000463 RID: 1123
	public GameObject player2AI;

	// Token: 0x04000464 RID: 1124
	public static PlayerAssigner instance;

	// Token: 0x04000465 RID: 1125
	public GameObject playerPrefab;

	// Token: 0x04000466 RID: 1126
	public int maxPlayers = 4;

	// Token: 0x04000467 RID: 1127
	public List<CharacterData> players = new List<CharacterData>(4);

	// Token: 0x04000468 RID: 1128
	private bool playersCanJoin;

	// Token: 0x04000469 RID: 1129
	private int playerIDToSet = -1;

	// Token: 0x0400046A RID: 1130
	private int teamIDToSet = -1;

	// Token: 0x0400046B RID: 1131
	private bool waitingForRegisterResponse;

	// Token: 0x0400046C RID: 1132
	private bool hasCreatedLocalPlayer;
}
