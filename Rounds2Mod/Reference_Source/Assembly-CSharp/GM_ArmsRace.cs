using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000064 RID: 100
public class GM_ArmsRace : MonoBehaviour
{
	// Token: 0x060001D9 RID: 473 RVA: 0x0000B8D0 File Offset: 0x00009AD0
	private void Awake()
	{
		GM_ArmsRace.instance = this;
	}

	// Token: 0x060001DA RID: 474 RVA: 0x0000B8D8 File Offset: 0x00009AD8
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
		PlayerManager.instance.SetPlayersSimulated(false);
		PlayerAssigner.instance.maxPlayers = this.playersNeededToStart;
		PlayerAssigner.instance.SetPlayersCanJoin(true);
		PlayerManager.instance.AddPlayerDiedAction(new Action<Player, int>(this.PlayerDied));
		PlayerManager playerManager = PlayerManager.instance;
		playerManager.PlayerJoinedAction = (Action<Player>)Delegate.Combine(playerManager.PlayerJoinedAction, new Action<Player>(this.PlayerJoined));
		ArtHandler.instance.NextArt();
		this.playersNeededToStart = 2;
		UIHandler.instance.SetNumberOfRounds(this.roundsToWinGame);
		PlayerAssigner.instance.maxPlayers = this.playersNeededToStart;
		if (!PhotonNetwork.OfflineMode)
		{
			UIHandler.instance.ShowJoinGameText("PRESS JUMP\n TO JOIN", PlayerSkinBank.GetPlayerSkinColors(0).winText);
		}
	}

	// Token: 0x060001DB RID: 475 RVA: 0x0000B9A8 File Offset: 0x00009BA8
	private void Update()
	{
		if (Input.GetKey(KeyCode.Alpha4))
		{
			this.playersNeededToStart = 4;
			PlayerAssigner.instance.maxPlayers = this.playersNeededToStart;
		}
		if (Input.GetKey(KeyCode.Alpha2))
		{
			this.playersNeededToStart = 2;
			PlayerAssigner.instance.maxPlayers = this.playersNeededToStart;
		}
	}

	// Token: 0x060001DC RID: 476 RVA: 0x0000B9F8 File Offset: 0x00009BF8
	public void PlayerJoined(Player player)
	{
		if (PhotonNetwork.OfflineMode)
		{
			return;
		}
		if (!PhotonNetwork.OfflineMode)
		{
			if (player.data.view.IsMine)
			{
				UIHandler.instance.ShowJoinGameText("WAITING", PlayerSkinBank.GetPlayerSkinColors(1).winText);
			}
			else
			{
				UIHandler.instance.ShowJoinGameText("PRESS JUMP\n TO JOIN", PlayerSkinBank.GetPlayerSkinColors(1).winText);
			}
		}
		player.data.isPlaying = false;
		int count = PlayerManager.instance.players.Count;
		if (count >= this.playersNeededToStart)
		{
			this.StartGame();
			return;
		}
		if (PhotonNetwork.OfflineMode)
		{
			if (this.playersNeededToStart - count == 3)
			{
				UIHandler.instance.ShowJoinGameText("ADD THREE MORE PLAYER TO START", PlayerSkinBank.GetPlayerSkinColors(count).winText);
			}
			if (this.playersNeededToStart - count == 2)
			{
				UIHandler.instance.ShowJoinGameText("ADD TWO MORE PLAYER TO START", PlayerSkinBank.GetPlayerSkinColors(count).winText);
			}
			if (this.playersNeededToStart - count == 1)
			{
				UIHandler.instance.ShowJoinGameText("ADD ONE MORE PLAYER TO START", PlayerSkinBank.GetPlayerSkinColors(count).winText);
			}
		}
	}

	// Token: 0x060001DD RID: 477 RVA: 0x0000BAFE File Offset: 0x00009CFE
	[PunRPC]
	private void RPCO_RequestSyncUp()
	{
		this.view.RPC("RPCM_ReturnSyncUp", 1, Array.Empty<object>());
	}

	// Token: 0x060001DE RID: 478 RVA: 0x0000BB16 File Offset: 0x00009D16
	[PunRPC]
	private void RPCM_ReturnSyncUp()
	{
		this.isWaiting = false;
	}

	// Token: 0x060001DF RID: 479 RVA: 0x0000BB1F File Offset: 0x00009D1F
	private IEnumerator WaitForSyncUp()
	{
		if (PhotonNetwork.OfflineMode)
		{
			yield break;
		}
		this.isWaiting = true;
		this.view.RPC("RPCO_RequestSyncUp", 1, Array.Empty<object>());
		while (this.isWaiting)
		{
			yield return null;
		}
		yield break;
	}

	// Token: 0x060001E0 RID: 480 RVA: 0x0000BB2E File Offset: 0x00009D2E
	public void StartGame()
	{
		if (GameManager.instance.isPlaying)
		{
			return;
		}
		Action startGameAction = this.StartGameAction;
		if (startGameAction != null)
		{
			startGameAction.Invoke();
		}
		GameManager.instance.isPlaying = true;
		base.StartCoroutine(this.DoStartGame());
	}

	// Token: 0x060001E1 RID: 481 RVA: 0x0000BB66 File Offset: 0x00009D66
	private IEnumerator DoStartGame()
	{
		GameManager.instance.battleOngoing = false;
		UIHandler.instance.ShowJoinGameText("LETS GOO!", PlayerSkinBank.GetPlayerSkinColors(1).winText);
		yield return new WaitForSeconds(0.25f);
		UIHandler.instance.HideJoinGameText();
		PlayerManager.instance.SetPlayersSimulated(false);
		PlayerManager.instance.SetPlayersVisible(false);
		MapManager.instance.LoadNextLevel(false, false);
		TimeHandler.instance.DoSpeedUp();
		yield return new WaitForSecondsRealtime(1f);
		if (this.pickPhase)
		{
			int num;
			for (int i = 0; i < PlayerManager.instance.players.Count; i = num + 1)
			{
				yield return base.StartCoroutine(this.WaitForSyncUp());
				CardChoiceVisuals.instance.Show(i, true);
				yield return CardChoice.instance.DoPick(1, PlayerManager.instance.players[i].playerID, PickerType.Player);
				yield return new WaitForSecondsRealtime(0.1f);
				num = i;
			}
			yield return base.StartCoroutine(this.WaitForSyncUp());
			CardChoiceVisuals.instance.Hide();
		}
		MapManager.instance.CallInNewMapAndMovePlayers(MapManager.instance.currentLevelID);
		TimeHandler.instance.DoSpeedUp();
		TimeHandler.instance.StartGame();
		GameManager.instance.battleOngoing = true;
		UIHandler.instance.ShowRoundCounterSmall(this.p1Rounds, this.p2Rounds, this.p1Points, this.p2Points);
		PlayerManager.instance.SetPlayersVisible(true);
		yield break;
	}

	// Token: 0x060001E2 RID: 482 RVA: 0x0000BB75 File Offset: 0x00009D75
	private IEnumerator PointTransition(int winningTeamID, string winTextBefore, string winText)
	{
		base.StartCoroutine(PointVisualizer.instance.DoSequence(this.p1Points, this.p2Points, winningTeamID == 0));
		yield return new WaitForSecondsRealtime(1f);
		MapManager.instance.LoadNextLevel(false, false);
		yield return new WaitForSecondsRealtime(0.5f);
		yield return base.StartCoroutine(this.WaitForSyncUp());
		MapManager.instance.CallInNewMapAndMovePlayers(MapManager.instance.currentLevelID);
		PlayerManager.instance.RevivePlayers();
		yield return new WaitForSecondsRealtime(0.3f);
		TimeHandler.instance.DoSpeedUp();
		GameManager.instance.battleOngoing = true;
		this.isTransitioning = false;
		yield break;
	}

	// Token: 0x060001E3 RID: 483 RVA: 0x0000BB8C File Offset: 0x00009D8C
	private void PointOver(int winningTeamID)
	{
		int num = this.p1Points;
		int num2 = this.p2Points;
		if (winningTeamID == 0)
		{
			num--;
		}
		else
		{
			num2--;
		}
		string winTextBefore = num.ToString() + " - " + num2.ToString();
		string winText = this.p1Points.ToString() + " - " + this.p2Points.ToString();
		base.StartCoroutine(this.PointTransition(winningTeamID, winTextBefore, winText));
		UIHandler.instance.ShowRoundCounterSmall(this.p1Rounds, this.p2Rounds, this.p1Points, this.p2Points);
	}

	// Token: 0x060001E4 RID: 484 RVA: 0x0000BC20 File Offset: 0x00009E20
	private IEnumerator RoundTransition(int winningTeamID, int killedTeamID)
	{
		base.StartCoroutine(PointVisualizer.instance.DoWinSequence(this.p1Points, this.p2Points, this.p1Rounds, this.p2Rounds, winningTeamID == 0));
		yield return new WaitForSecondsRealtime(1f);
		MapManager.instance.LoadNextLevel(false, false);
		yield return new WaitForSecondsRealtime(0.3f);
		yield return new WaitForSecondsRealtime(1f);
		TimeHandler.instance.DoSpeedUp();
		if (this.pickPhase)
		{
			global::Debug.Log("PICK PHASE");
			PlayerManager.instance.SetPlayersVisible(false);
			Player[] players = PlayerManager.instance.GetPlayersInTeam(killedTeamID);
			int num;
			for (int i = 0; i < players.Length; i = num + 1)
			{
				yield return base.StartCoroutine(this.WaitForSyncUp());
				yield return CardChoice.instance.DoPick(1, players[i].playerID, PickerType.Player);
				yield return new WaitForSecondsRealtime(0.1f);
				num = i;
			}
			PlayerManager.instance.SetPlayersVisible(true);
			players = null;
		}
		yield return base.StartCoroutine(this.WaitForSyncUp());
		TimeHandler.instance.DoSlowDown();
		MapManager.instance.CallInNewMapAndMovePlayers(MapManager.instance.currentLevelID);
		PlayerManager.instance.RevivePlayers();
		yield return new WaitForSecondsRealtime(0.3f);
		TimeHandler.instance.DoSpeedUp();
		this.isTransitioning = false;
		GameManager.instance.battleOngoing = true;
		UIHandler.instance.ShowRoundCounterSmall(this.p1Rounds, this.p2Rounds, this.p1Points, this.p2Points);
		yield break;
	}

	// Token: 0x060001E5 RID: 485 RVA: 0x0000BC3D File Offset: 0x00009E3D
	private void RoundOver(int winningTeamID, int losingTeamID)
	{
		this.currentWinningTeamID = winningTeamID;
		base.StartCoroutine(this.RoundTransition(winningTeamID, losingTeamID));
		this.p1Points = 0;
		this.p2Points = 0;
	}

	// Token: 0x060001E6 RID: 486 RVA: 0x0000BC63 File Offset: 0x00009E63
	private IEnumerator GameOverTransition(int winningTeamID)
	{
		UIHandler.instance.ShowRoundCounterSmall(this.p1Rounds, this.p2Rounds, this.p1Points, this.p2Points);
		UIHandler.instance.DisplayScreenText(PlayerManager.instance.GetColorFromTeam(winningTeamID).winText, "VICTORY!", 1f);
		yield return new WaitForSecondsRealtime(2f);
		this.GameOverRematch(winningTeamID);
		yield break;
	}

	// Token: 0x060001E7 RID: 487 RVA: 0x0000BC7C File Offset: 0x00009E7C
	private void GameOverRematch(int winningTeamID)
	{
		UIHandler.instance.DisplayScreenTextLoop(PlayerManager.instance.GetColorFromTeam(winningTeamID).winText, "REMATCH?");
		UIHandler.instance.DisplayYesNoLoop(PlayerManager.instance.GetFirstPlayerInTeam(winningTeamID), new Action<PopUpHandler.YesNo>(this.GetRematchYesNo));
		MapManager.instance.LoadNextLevel(false, false);
	}

	// Token: 0x060001E8 RID: 488 RVA: 0x0000BCD5 File Offset: 0x00009ED5
	private void GetRematchYesNo(PopUpHandler.YesNo yesNo)
	{
		if (yesNo == PopUpHandler.YesNo.Yes)
		{
			base.StartCoroutine(this.IDoRematch());
			return;
		}
		this.DoRestart();
	}

	// Token: 0x060001E9 RID: 489 RVA: 0x0000BCEE File Offset: 0x00009EEE
	[PunRPC]
	public void RPCA_PlayAgain()
	{
		this.waitingForOtherPlayer = false;
	}

	// Token: 0x060001EA RID: 490 RVA: 0x0000BCF7 File Offset: 0x00009EF7
	private IEnumerator IDoRematch()
	{
		if (!PhotonNetwork.OfflineMode)
		{
			base.GetComponent<PhotonView>().RPC("RPCA_PlayAgain", 1, Array.Empty<object>());
			UIHandler.instance.DisplayScreenTextLoop("WAITING");
			float c = 0f;
			while (this.waitingForOtherPlayer)
			{
				c += Time.unscaledDeltaTime;
				if (c > 10f)
				{
					this.DoRestart();
					yield break;
				}
				yield return null;
			}
		}
		yield return null;
		UIHandler.instance.StopScreenTextLoop();
		PlayerManager.instance.ResetCharacters();
		this.ResetMatch();
		base.StartCoroutine(this.DoStartGame());
		this.waitingForOtherPlayer = true;
		yield break;
	}

	// Token: 0x060001EB RID: 491 RVA: 0x0000BD08 File Offset: 0x00009F08
	private void ResetMatch()
	{
		this.p1Points = 0;
		this.p1Rounds = 0;
		this.p2Points = 0;
		this.p2Rounds = 0;
		this.isTransitioning = false;
		this.waitingForOtherPlayer = false;
		UIHandler.instance.ShowRoundCounterSmall(this.p1Rounds, this.p2Rounds, this.p1Points, this.p2Points);
		CardBarHandler.instance.ResetCardBards();
		PointVisualizer.instance.ResetPoints();
	}

	// Token: 0x060001EC RID: 492 RVA: 0x0000BD78 File Offset: 0x00009F78
	private void GameOverContinue(int winningTeamID)
	{
		UIHandler.instance.DisplayScreenTextLoop(PlayerManager.instance.GetColorFromTeam(winningTeamID).winText, "CONTINUE?");
		UIHandler.instance.DisplayYesNoLoop(PlayerManager.instance.GetFirstPlayerInTeam(winningTeamID), new Action<PopUpHandler.YesNo>(this.GetContinueYesNo));
		MapManager.instance.LoadNextLevel(false, false);
	}

	// Token: 0x060001ED RID: 493 RVA: 0x0000BDD1 File Offset: 0x00009FD1
	private void GetContinueYesNo(PopUpHandler.YesNo yesNo)
	{
		if (yesNo == PopUpHandler.YesNo.Yes)
		{
			this.DoContinue();
			return;
		}
		this.DoRestart();
	}

	// Token: 0x060001EE RID: 494 RVA: 0x0000BDE4 File Offset: 0x00009FE4
	private void DoContinue()
	{
		UIHandler.instance.StopScreenTextLoop();
		this.roundsToWinGame += 2;
		UIHandler.instance.SetNumberOfRounds(this.roundsToWinGame);
		this.RoundOver(this.currentWinningTeamID, PlayerManager.instance.GetOtherTeam(this.currentWinningTeamID));
	}

	// Token: 0x060001EF RID: 495 RVA: 0x0000BE35 File Offset: 0x0000A035
	private void DoRestart()
	{
		GameManager.instance.battleOngoing = false;
		if (PhotonNetwork.OfflineMode)
		{
			Application.LoadLevel(Application.loadedLevel);
			return;
		}
		NetworkConnectionHandler.instance.NetworkRestart();
	}

	// Token: 0x060001F0 RID: 496 RVA: 0x0000BE5E File Offset: 0x0000A05E
	private void GameOver(int winningTeamID)
	{
		this.currentWinningTeamID = winningTeamID;
		base.StartCoroutine(this.GameOverTransition(winningTeamID));
	}

	// Token: 0x060001F1 RID: 497 RVA: 0x0000BE78 File Offset: 0x0000A078
	public void PlayerDied(Player killedPlayer, int playersAlive)
	{
		if (!PhotonNetwork.OfflineMode)
		{
			global::Debug.Log("PlayerDied: " + killedPlayer.data.view.Owner.NickName);
		}
		if (PlayerManager.instance.TeamsAlive() < 2)
		{
			TimeHandler.instance.DoSlowDown();
			if (PhotonNetwork.IsMasterClient)
			{
				this.view.RPC("RPCA_NextRound", 0, new object[]
				{
					PlayerManager.instance.GetOtherTeam(PlayerManager.instance.GetLastTeamAlive()),
					PlayerManager.instance.GetLastTeamAlive(),
					this.p1Points,
					this.p2Points,
					this.p1Rounds,
					this.p2Rounds
				});
			}
		}
	}

	// Token: 0x060001F2 RID: 498 RVA: 0x0000BF50 File Offset: 0x0000A150
	[PunRPC]
	public void RPCA_NextRound(int losingTeamID, int winningTeamID, int p1PointsSet, int p2PointsSet, int p1RoundsSet, int p2RoundsSet)
	{
		if (this.isTransitioning)
		{
			return;
		}
		GameManager.instance.battleOngoing = false;
		this.p1Points = p1PointsSet;
		this.p2Points = p2PointsSet;
		this.p1Rounds = p1RoundsSet;
		this.p2Rounds = p2RoundsSet;
		global::Debug.Log("Winning team: " + winningTeamID);
		global::Debug.Log("Losing team: " + losingTeamID);
		this.isTransitioning = true;
		GameManager.instance.GameOver(winningTeamID, losingTeamID);
		PlayerManager.instance.SetPlayersSimulated(false);
		if (winningTeamID == 0)
		{
			this.p1Points++;
			if (this.p1Points < this.pointsToWinRound)
			{
				global::Debug.Log("Point over, winning team: " + winningTeamID);
				this.PointOver(winningTeamID);
				this.pointOverAction.Invoke();
				return;
			}
			this.p1Rounds++;
			if (this.p1Rounds >= this.roundsToWinGame)
			{
				global::Debug.Log("Game over, winning team: " + winningTeamID);
				this.GameOver(winningTeamID);
				this.pointOverAction.Invoke();
				return;
			}
			global::Debug.Log("Round over, winning team: " + winningTeamID);
			this.RoundOver(winningTeamID, losingTeamID);
			this.pointOverAction.Invoke();
			return;
		}
		else
		{
			if (winningTeamID != 1)
			{
				return;
			}
			this.p2Points++;
			if (this.p2Points < this.pointsToWinRound)
			{
				global::Debug.Log("Point over, winning team: " + winningTeamID);
				this.PointOver(winningTeamID);
				this.pointOverAction.Invoke();
				return;
			}
			this.p2Rounds++;
			if (this.p2Rounds >= this.roundsToWinGame)
			{
				global::Debug.Log("Game over, winning team: " + winningTeamID);
				this.GameOver(winningTeamID);
				this.pointOverAction.Invoke();
				return;
			}
			global::Debug.Log("Round over, winning team: " + winningTeamID);
			this.RoundOver(winningTeamID, losingTeamID);
			this.pointOverAction.Invoke();
			return;
		}
	}

	// Token: 0x0400028E RID: 654
	private int playersNeededToStart = 2;

	// Token: 0x0400028F RID: 655
	private int pointsToWinRound = 2;

	// Token: 0x04000290 RID: 656
	public int roundsToWinGame = 5;

	// Token: 0x04000291 RID: 657
	public int p1Points;

	// Token: 0x04000292 RID: 658
	public int p2Points;

	// Token: 0x04000293 RID: 659
	public int p1Rounds;

	// Token: 0x04000294 RID: 660
	public int p2Rounds;

	// Token: 0x04000295 RID: 661
	private PhotonView view;

	// Token: 0x04000296 RID: 662
	public static GM_ArmsRace instance;

	// Token: 0x04000297 RID: 663
	private bool isWaiting;

	// Token: 0x04000298 RID: 664
	public Action StartGameAction;

	// Token: 0x04000299 RID: 665
	public bool pickPhase = true;

	// Token: 0x0400029A RID: 666
	[HideInInspector]
	public bool isPicking;

	// Token: 0x0400029B RID: 667
	private bool waitingForOtherPlayer = true;

	// Token: 0x0400029C RID: 668
	private int currentWinningTeamID = -1;

	// Token: 0x0400029D RID: 669
	public Action pointOverAction;

	// Token: 0x0400029E RID: 670
	private bool isTransitioning;
}
