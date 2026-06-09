using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000065 RID: 101
public class GM_Test : MonoBehaviour
{
	// Token: 0x060001F4 RID: 500 RVA: 0x0000C179 File Offset: 0x0000A379
	private void Awake()
	{
		GM_Test.instance = this;
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		if (!Application.isEditor)
		{
			this.testMap = false;
		}
	}

	// Token: 0x060001F5 RID: 501 RVA: 0x0000C1A0 File Offset: 0x0000A3A0
	private void Start()
	{
		if (this.testMap)
		{
			base.transform.root.GetComponent<SetOfflineMode>().SetOffline();
		}
		if (this.testMap)
		{
			MapManager.instance.isTestingMap = true;
		}
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		if (!this.testMap)
		{
			MapManager.instance.LoadNextLevel(true, true);
		}
		else
		{
			MapManager.instance.currentMap = new MapWrapper(Object.FindObjectOfType<Map>(), Object.FindObjectOfType<Map>().gameObject.scene);
			ArtHandler.instance.NextArt();
		}
		PlayerAssigner.instance.SetPlayersCanJoin(true);
		TimeHandler.instance.StartGame();
		PlayerManager playerManager = PlayerManager.instance;
		playerManager.PlayerJoinedAction = (Action<Player>)Delegate.Combine(playerManager.PlayerJoinedAction, new Action<Player>(this.PlayerWasAdded));
		PlayerManager.instance.AddPlayerDiedAction(new Action<Player, int>(this.PlayerDied));
		GameManager.instance.isPlaying = true;
		GameManager.instance.battleOngoing = true;
	}

	// Token: 0x060001F6 RID: 502 RVA: 0x0000C298 File Offset: 0x0000A498
	private void PlayerWasAdded(Player player)
	{
		PlayerManager.instance.SetPlayersSimulated(true);
		player.data.GetComponent<PlayerCollision>().IgnoreWallForFrames(2);
		player.transform.position = MapManager.instance.currentMap.Map.GetRandomSpawnPos();
		PlayerManager.instance.SetPlayersSimulated(true);
		PlayerManager.instance.SetPlayersPlaying(true);
	}

	// Token: 0x060001F7 RID: 503 RVA: 0x0000C2F6 File Offset: 0x0000A4F6
	private void PlayerDied(Player player, int unused)
	{
		base.StartCoroutine(this.DelayRevive(player));
	}

	// Token: 0x060001F8 RID: 504 RVA: 0x0000C306 File Offset: 0x0000A506
	private IEnumerator DelayRevive(Player player)
	{
		yield return new WaitForSecondsRealtime(2.5f);
		this.PlayerWasAdded(player);
		player.data.healthHandler.Revive(true);
		yield break;
	}

	// Token: 0x0400029F RID: 671
	public bool testMap;

	// Token: 0x040002A0 RID: 672
	public static GM_Test instance;
}
