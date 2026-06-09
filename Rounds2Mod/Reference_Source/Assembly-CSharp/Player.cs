using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200008E RID: 142
public class Player : MonoBehaviour
{
	// Token: 0x0600030C RID: 780 RVA: 0x0001355A File Offset: 0x0001175A
	private void Awake()
	{
		this.data = base.GetComponent<CharacterData>();
	}

	// Token: 0x0600030D RID: 781 RVA: 0x00013568 File Offset: 0x00011768
	private void Start()
	{
		if (!this.data.view.IsMine)
		{
			this.ReadPlayerID();
			this.ReadTeamID();
			PlayerAssigner.instance.OtherPlayerWasCreated();
		}
		else
		{
			if (!PhotonNetwork.OfflineMode)
			{
				try
				{
					int num = 0;
					PlayerFace playerFace = CharacterCreatorHandler.instance.selectedPlayerFaces[num];
					this.data.view.RPC("RPCA_SetFace", 0, new object[]
					{
						playerFace.eyeID,
						playerFace.eyeOffset,
						playerFace.mouthID,
						playerFace.mouthOffset,
						playerFace.detailID,
						playerFace.detailOffset,
						playerFace.detail2ID,
						playerFace.detail2Offset
					});
					goto IL_10C;
				}
				catch
				{
					goto IL_10C;
				}
			}
			if (GM_ArmsRace.instance != null)
			{
				GM_ArmsRace instance = GM_ArmsRace.instance;
				instance.StartGameAction = (Action)Delegate.Combine(instance.StartGameAction, new Action(this.GetFaceOffline));
			}
		}
		IL_10C:
		PlayerManager.instance.PlayerJoined(this);
	}

	// Token: 0x0600030E RID: 782 RVA: 0x0001369C File Offset: 0x0001189C
	public void GetFaceOffline()
	{
		PlayerFace playerFace = CharacterCreatorHandler.instance.selectedPlayerFaces[this.playerID];
		this.data.view.RPC("RPCA_SetFace", 0, new object[]
		{
			playerFace.eyeID,
			playerFace.eyeOffset,
			playerFace.mouthID,
			playerFace.mouthOffset,
			playerFace.detailID,
			playerFace.detailOffset,
			playerFace.detail2ID,
			playerFace.detail2Offset
		});
	}

	// Token: 0x0600030F RID: 783 RVA: 0x00013748 File Offset: 0x00011948
	[PunRPC]
	public void RPCA_SetFace(int eyeID, Vector2 eyeOffset, int mouthID, Vector2 mouthOffset, int detailID, Vector2 detailOffset, int detail2ID, Vector2 detail2Offset)
	{
		PlayerFace face = PlayerFace.CreateFace(eyeID, eyeOffset, mouthID, mouthOffset, detailID, detailOffset, detail2ID, detail2Offset);
		base.GetComponentInChildren<CharacterCreatorItemEquipper>().EquipFace(face);
	}

	// Token: 0x06000310 RID: 784 RVA: 0x00013774 File Offset: 0x00011974
	internal void Call_AllGameFeel(Vector2 vector2)
	{
		this.data.view.RPC("RPCA_AllGameFeel", 0, new object[]
		{
			vector2
		});
	}

	// Token: 0x06000311 RID: 785 RVA: 0x0001379B File Offset: 0x0001199B
	[PunRPC]
	internal void RPCA_AllGameFeel(Vector2 vector2)
	{
		GamefeelManager.instance.AddGameFeel(vector2);
	}

	// Token: 0x06000312 RID: 786 RVA: 0x000137A8 File Offset: 0x000119A8
	public void AssignPlayerID(int ID)
	{
		this.playerID = ID;
		this.SetColors();
		if (!PhotonNetwork.OfflineMode)
		{
			Hashtable customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
			if (customProperties.ContainsKey("PlayerID"))
			{
				customProperties["PlayerID"] = this.playerID;
			}
			else
			{
				customProperties.Add("PlayerID", this.playerID);
			}
			PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties, null, null);
		}
	}

	// Token: 0x06000313 RID: 787 RVA: 0x0001381D File Offset: 0x00011A1D
	internal float GetRadius()
	{
		return 5f;
	}

	// Token: 0x06000314 RID: 788 RVA: 0x00013824 File Offset: 0x00011A24
	private void ReadPlayerID()
	{
		if (PhotonNetwork.OfflineMode)
		{
			return;
		}
		this.playerID = (int)this.data.view.Owner.CustomProperties["PlayerID"];
		this.SetColors();
	}

	// Token: 0x06000315 RID: 789 RVA: 0x00013860 File Offset: 0x00011A60
	public void AssignTeamID(int ID)
	{
		this.teamID = ID;
		if (!PhotonNetwork.OfflineMode)
		{
			Hashtable customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
			if (customProperties.ContainsKey("TeamID"))
			{
				customProperties["TeamID"] = this.playerID;
			}
			else
			{
				customProperties.Add("TeamID", this.teamID);
			}
			PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties, null, null);
		}
	}

	// Token: 0x06000316 RID: 790 RVA: 0x000138CF File Offset: 0x00011ACF
	private void ReadTeamID()
	{
		if (PhotonNetwork.OfflineMode)
		{
			return;
		}
		this.teamID = (int)this.data.view.Owner.CustomProperties["TeamID"];
	}

	// Token: 0x06000317 RID: 791 RVA: 0x00013903 File Offset: 0x00011B03
	public void SetColors()
	{
		SetTeamColor.TeamColorThis(base.gameObject, PlayerSkinBank.GetPlayerSkinColors(this.playerID));
	}

	// Token: 0x06000318 RID: 792 RVA: 0x0001391B File Offset: 0x00011B1B
	public PlayerSkin GetTeamColors()
	{
		return PlayerSkinBank.GetPlayerSkinColors(this.playerID);
	}

	// Token: 0x06000319 RID: 793 RVA: 0x00013928 File Offset: 0x00011B28
	internal void FullReset()
	{
		this.data.weaponHandler.NewGun();
		this.data.stats.ResetStats();
		this.data.block.ResetStats();
	}

	// Token: 0x04000439 RID: 1081
	public int playerID;

	// Token: 0x0400043A RID: 1082
	public int teamID;

	// Token: 0x0400043B RID: 1083
	[HideInInspector]
	public CharacterData data;

	// Token: 0x0400043C RID: 1084
	public PlayerSkinBank colors;
}
