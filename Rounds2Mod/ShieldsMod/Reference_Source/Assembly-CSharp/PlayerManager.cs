using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Sonigon;
using UnityEngine;

// Token: 0x0200009D RID: 157
public class PlayerManager : MonoBehaviour
{
	// Token: 0x06000363 RID: 867 RVA: 0x000151D6 File Offset: 0x000133D6
	private void Awake()
	{
		PlayerManager.instance = this;
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000364 RID: 868 RVA: 0x000151EA File Offset: 0x000133EA
	public Player GetOtherPlayer(Player asker)
	{
		return this.GetClosestPlayerInTeam(asker.transform.position, this.GetOtherTeam(asker.teamID), false);
	}

	// Token: 0x06000365 RID: 869 RVA: 0x0001520C File Offset: 0x0001340C
	public Player GetClosestPlayer(Vector2 refPos, bool needVision = false)
	{
		Player result = null;
		float num = float.PositiveInfinity;
		for (int i = 0; i < this.players.Count; i++)
		{
			if (!this.players[i].data.dead)
			{
				float num2 = Vector2.Distance(refPos, this.players[i].data.playerVel.position);
				if ((!needVision || this.CanSeePlayer(refPos, this.players[i]).canSee) && num2 < num)
				{
					num = num2;
					result = this.players[i];
				}
			}
		}
		return result;
	}

	// Token: 0x06000366 RID: 870 RVA: 0x000152A4 File Offset: 0x000134A4
	internal Player GetPlayerWithActorID(int actorID)
	{
		for (int i = 0; i < this.players.Count; i++)
		{
			if (this.players[i].data.view.OwnerActorNr == actorID)
			{
				return this.players[i];
			}
		}
		return null;
	}

	// Token: 0x06000367 RID: 871 RVA: 0x000152F4 File Offset: 0x000134F4
	public Player GetClosestPlayerInTeam(Vector3 position, int team, bool needVision = false)
	{
		float num = float.MaxValue;
		Player[] playersInTeam = this.GetPlayersInTeam(team);
		Player result = null;
		for (int i = 0; i < playersInTeam.Length; i++)
		{
			if (!this.players[i].data.dead)
			{
				float num2 = Vector2.Distance(position, playersInTeam[i].transform.position);
				if ((!needVision || this.CanSeePlayer(position, playersInTeam[i]).canSee) && num2 < num)
				{
					num = num2;
					result = playersInTeam[i];
				}
			}
		}
		return result;
	}

	// Token: 0x06000368 RID: 872 RVA: 0x0001537C File Offset: 0x0001357C
	public Player GetClosestPlayer(Vector2 refPos, Vector2 forward)
	{
		Player result = null;
		float num = float.PositiveInfinity;
		for (int i = 0; i < this.players.Count; i++)
		{
			if (!this.players[i].data.dead && this.CanSeePlayer(refPos, this.players[i]).canSee)
			{
				float num2 = Vector2.Distance(refPos, this.players[i].data.playerVel.position);
				num2 += Vector2.Angle(forward, this.players[i].data.playerVel.position - refPos);
				if (num2 < num)
				{
					num = num2;
					result = this.players[i];
				}
			}
		}
		return result;
	}

	// Token: 0x06000369 RID: 873 RVA: 0x00015440 File Offset: 0x00013640
	public CanSeeInfo CanSeePlayer(Vector2 from, Player player)
	{
		CanSeeInfo canSeeInfo = new CanSeeInfo();
		canSeeInfo.canSee = true;
		canSeeInfo.distance = float.PositiveInfinity;
		if (!player)
		{
			canSeeInfo.canSee = false;
			return canSeeInfo;
		}
		RaycastHit2D[] array = Physics2D.RaycastAll(from, (player.data.playerVel.position - from).normalized, Vector2.Distance(from, player.data.playerVel.position), this.canSeePlayerMask);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].transform && !array[i].transform.root.GetComponent<SpawnedAttack>() && !array[i].transform.root.GetComponent<Player>() && array[i].distance < canSeeInfo.distance)
			{
				canSeeInfo.canSee = false;
				canSeeInfo.hitPoint = array[i].point;
				canSeeInfo.distance = array[i].distance;
			}
		}
		return canSeeInfo;
	}

	// Token: 0x0600036A RID: 874 RVA: 0x00015560 File Offset: 0x00013760
	internal Player GetPlayerWithID(int playerID)
	{
		for (int i = 0; i < this.players.Count; i++)
		{
			if (this.players[i].playerID == playerID)
			{
				return this.players[i];
			}
		}
		return null;
	}

	// Token: 0x0600036B RID: 875 RVA: 0x000155A8 File Offset: 0x000137A8
	public Player GetLastPlayerAlive()
	{
		Player result = null;
		for (int i = 0; i < this.players.Count; i++)
		{
			if (!this.players[i].data.dead)
			{
				result = this.players[i];
				break;
			}
		}
		return result;
	}

	// Token: 0x0600036C RID: 876 RVA: 0x000155F5 File Offset: 0x000137F5
	public int GetLastTeamAlive()
	{
		return this.GetLastPlayerAlive().teamID;
	}

	// Token: 0x0600036D RID: 877 RVA: 0x00015604 File Offset: 0x00013804
	public int TeamsAlive()
	{
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < this.players.Count; i++)
		{
			if (this.players[i].teamID == 0 && !this.players[i].data.dead)
			{
				flag = true;
			}
			if (this.players[i].teamID == 1 && !this.players[i].data.dead)
			{
				flag2 = true;
			}
		}
		int num = 0;
		if (flag)
		{
			num++;
		}
		if (flag2)
		{
			num++;
		}
		return num;
	}

	// Token: 0x0600036E RID: 878 RVA: 0x00015697 File Offset: 0x00013897
	public static void RegisterPlayer(Player player)
	{
		PlayerManager.instance.players.Add(player);
		if (PlayerManager.instance.playersShouldBeActive)
		{
			player.data.isPlaying = true;
		}
	}

	// Token: 0x0600036F RID: 879 RVA: 0x000156C1 File Offset: 0x000138C1
	public void RemovePlayer(Player player)
	{
		this.players.Remove(player);
		Object.Destroy(player.gameObject);
	}

	// Token: 0x06000370 RID: 880 RVA: 0x000156DC File Offset: 0x000138DC
	public void RemovePlayers()
	{
		for (int i = this.players.Count - 1; i >= 0; i--)
		{
			if (this.players[i])
			{
				Object.Destroy(this.players[i].gameObject);
			}
		}
		this.players.Clear();
		PlayerAssigner.instance.ClearPlayers();
	}

	// Token: 0x06000371 RID: 881 RVA: 0x00015740 File Offset: 0x00013940
	public void RevivePlayers()
	{
		for (int i = 0; i < this.players.Count; i++)
		{
			this.players[i].data.healthHandler.Revive(true);
			this.players[i].GetComponent<GeneralInput>().enabled = true;
		}
	}

	// Token: 0x06000372 RID: 882 RVA: 0x00015796 File Offset: 0x00013996
	[PunRPC]
	public void RPCA_MovePlayers()
	{
		this.MovePlayers(MapManager.instance.GetSpawnPoints());
	}

	// Token: 0x06000373 RID: 883 RVA: 0x000157A8 File Offset: 0x000139A8
	public void MovePlayers(SpawnPoint[] spawnPoints)
	{
		for (int i = 0; i < this.players.Count; i++)
		{
			base.StartCoroutine(this.Move(this.players[i].data.playerVel, spawnPoints[i].localStartPos));
			int j;
			for (j = i; j >= this.soundCharacterSpawn.Length; j -= this.soundCharacterSpawn.Length)
			{
			}
			SoundManager.Instance.Play(this.soundCharacterSpawn[j], this.players[i].transform);
		}
	}

	// Token: 0x06000374 RID: 884 RVA: 0x00015833 File Offset: 0x00013A33
	public void AddPlayerDiedAction(Action<Player, int> action)
	{
		this.PlayerDiedAction = (Action<Player, int>)Delegate.Combine(this.PlayerDiedAction, action);
	}

	// Token: 0x06000375 RID: 885 RVA: 0x0001584C File Offset: 0x00013A4C
	public void PlayerDied(Player player)
	{
		int num = 0;
		for (int i = 0; i < this.players.Count; i++)
		{
			if (!PlayerManager.instance.players[i].data.dead)
			{
				num++;
			}
		}
		if (this.PlayerDiedAction != null)
		{
			this.PlayerDiedAction.Invoke(player, num);
		}
	}

	// Token: 0x06000376 RID: 886 RVA: 0x000158A6 File Offset: 0x00013AA6
	public PlayerSkin GetColorFromTeam(int teamID)
	{
		return PlayerSkinBank.GetPlayerSkinColors(this.GetPlayersInTeam(teamID)[0].playerID);
	}

	// Token: 0x06000377 RID: 887 RVA: 0x000158BB File Offset: 0x00013ABB
	public PlayerSkin GetColorFromPlayer(int playerID)
	{
		return PlayerSkinBank.GetPlayerSkinColors(playerID);
	}

	// Token: 0x06000378 RID: 888 RVA: 0x000158C4 File Offset: 0x00013AC4
	public Player[] GetPlayersInTeam(int teamID)
	{
		List<Player> list = new List<Player>();
		for (int i = 0; i < this.players.Count; i++)
		{
			if (this.players[i].teamID == teamID)
			{
				list.Add(this.players[i]);
			}
		}
		return list.ToArray();
	}

	// Token: 0x06000379 RID: 889 RVA: 0x00015919 File Offset: 0x00013B19
	internal Player GetFirstPlayerInTeam(int teamID)
	{
		return this.GetPlayersInTeam(teamID)[0];
	}

	// Token: 0x0600037A RID: 890 RVA: 0x00015924 File Offset: 0x00013B24
	public int GetOtherTeam(int team)
	{
		if (team == 0)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x0600037B RID: 891 RVA: 0x0001592C File Offset: 0x00013B2C
	public void SetPlayersPlaying(bool playing)
	{
		this.playersShouldBeActive = playing;
		for (int i = 0; i < this.players.Count; i++)
		{
			this.players[i].data.isPlaying = playing;
		}
	}

	// Token: 0x0600037C RID: 892 RVA: 0x00015970 File Offset: 0x00013B70
	public void SetPlayersSimulated(bool simulated)
	{
		this.playersShouldBeActive = simulated;
		for (int i = 0; i < this.players.Count; i++)
		{
			this.players[i].data.playerVel.simulated = simulated;
		}
	}

	// Token: 0x0600037D RID: 893 RVA: 0x000159B8 File Offset: 0x00013BB8
	internal void SetPlayersVisible(bool visible)
	{
		for (int i = 0; i < this.players.Count; i++)
		{
			this.players[i].data.gameObject.transform.position = Vector3.up * 200f;
		}
	}

	// Token: 0x1700000A RID: 10
	// (get) Token: 0x0600037E RID: 894 RVA: 0x00015A0A File Offset: 0x00013C0A
	// (set) Token: 0x0600037F RID: 895 RVA: 0x00015A12 File Offset: 0x00013C12
	public Action<Player> PlayerJoinedAction { get; internal set; }

	// Token: 0x06000380 RID: 896 RVA: 0x00015A1B File Offset: 0x00013C1B
	public void PlayerJoined(Player player)
	{
		if (this.PlayerJoinedAction != null)
		{
			this.PlayerJoinedAction.Invoke(player);
		}
	}

	// Token: 0x06000381 RID: 897 RVA: 0x00015A31 File Offset: 0x00013C31
	private IEnumerator Move(PlayerVelocity player, Vector3 targetPos)
	{
		global::Debug.Log("MOVE PLAYERS START " + Time.unscaledTime);
		player.GetComponent<Player>().data.isPlaying = false;
		player.simulated = false;
		player.isKinematic = true;
		Vector3 distance = targetPos - player.transform.position;
		Vector3 targetStartPos = player.transform.position;
		PlayerCollision col = player.GetComponent<PlayerCollision>();
		float t = this.playerMoveCurve.keys[this.playerMoveCurve.keys.Length - 1].time;
		float c = 0f;
		while (c < t)
		{
			col.IgnoreWallForFrames(2);
			c += Mathf.Clamp(Time.unscaledDeltaTime, 0f, 0.02f);
			player.transform.position = targetStartPos + distance * this.playerMoveCurve.Evaluate(c);
			yield return null;
		}
		int frames = 0;
		while (frames < 10)
		{
			player.transform.position = targetStartPos + distance;
			int num = frames;
			frames = num + 1;
			yield return null;
		}
		player.simulated = true;
		player.isKinematic = false;
		global::Debug.Log("MOVE PLAYERS END " + Time.unscaledTime);
		player.GetComponent<Player>().data.isPlaying = true;
		player.GetComponent<Player>().data.healthHandler.Revive(true);
		CardChoiceVisuals.instance.Hide();
		yield break;
	}

	// Token: 0x06000382 RID: 898 RVA: 0x00015A4E File Offset: 0x00013C4E
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R) && !DevConsole.isTyping && Application.isEditor)
		{
			this.ResetCharacters();
		}
	}

	// Token: 0x06000383 RID: 899 RVA: 0x00015A70 File Offset: 0x00013C70
	internal void ResetCharacters()
	{
		CardBarHandler.instance.ResetCardBards();
		for (int i = 0; i < this.players.Count; i++)
		{
			this.players[i].FullReset();
		}
	}

	// Token: 0x06000384 RID: 900 RVA: 0x00015AB0 File Offset: 0x00013CB0
	public PlayerActions[] GetActionsFromTeam(int selectingTeamID)
	{
		List<PlayerActions> list = new List<PlayerActions>();
		for (int i = 0; i < this.players.Count; i++)
		{
			if (this.players[i].teamID == selectingTeamID)
			{
				list.Add(this.players[i].data.playerActions);
			}
		}
		return list.ToArray();
	}

	// Token: 0x06000385 RID: 901 RVA: 0x00015B10 File Offset: 0x00013D10
	public PlayerActions[] GetActionsFromPlayer(int selectingPlayerID)
	{
		List<PlayerActions> list = new List<PlayerActions>();
		for (int i = 0; i < this.players.Count; i++)
		{
			if (this.players[i].playerID == selectingPlayerID)
			{
				list.Add(this.players[i].data.playerActions);
			}
		}
		return list.ToArray();
	}

	// Token: 0x04000497 RID: 1175
	[Header("Sounds")]
	public SoundEvent[] soundCharacterSpawn;

	// Token: 0x04000498 RID: 1176
	public static PlayerManager instance;

	// Token: 0x04000499 RID: 1177
	public LayerMask canSeePlayerMask;

	// Token: 0x0400049A RID: 1178
	public List<Player> players = new List<Player>();

	// Token: 0x0400049B RID: 1179
	public PhotonView view;

	// Token: 0x0400049C RID: 1180
	private Action<Player, int> PlayerDiedAction;

	// Token: 0x0400049D RID: 1181
	private bool playersShouldBeActive;

	// Token: 0x0400049E RID: 1182
	public AnimationCurve playerMoveCurve;
}
