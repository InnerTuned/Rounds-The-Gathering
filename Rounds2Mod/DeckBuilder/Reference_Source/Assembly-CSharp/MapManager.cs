using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000080 RID: 128
public class MapManager : MonoBehaviour
{
	// Token: 0x060002A8 RID: 680 RVA: 0x00011329 File Offset: 0x0000F529
	private void Awake()
	{
		MapManager.instance = this;
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x060002A9 RID: 681 RVA: 0x0001133D File Offset: 0x0000F53D
	internal void ReportMapLoaded(int levelID)
	{
		this.view.RPC("RPCA_ReportMapLoaded", 1, new object[]
		{
			levelID
		});
	}

	// Token: 0x060002AA RID: 682 RVA: 0x0001135F File Offset: 0x0000F55F
	[PunRPC]
	internal void RPCA_ReportMapLoaded(int levelID)
	{
		this.otherPlayersMostRecentlyLoadedLevel = levelID;
	}

	// Token: 0x060002AB RID: 683 RVA: 0x00011368 File Offset: 0x0000F568
	private string GetRandomMap()
	{
		if (this.forceMap != "")
		{
			return this.forceMap;
		}
		int num = Random.Range(0, this.levels.Length);
		while (num == this.currentLevelID && this.levels.Length > 1)
		{
			num = Random.Range(0, this.levels.Length);
		}
		return this.levels[num];
	}

	// Token: 0x060002AC RID: 684 RVA: 0x000113CC File Offset: 0x0000F5CC
	public void LoadNextLevel(bool callInImidetly = false, bool forceLoad = false)
	{
		if (!forceLoad && !PhotonNetwork.IsMasterClient && !PhotonNetwork.OfflineMode)
		{
			return;
		}
		this.view.RPC("RPCA_SetCallInNextMap", 0, new object[]
		{
			callInImidetly
		});
		this.view.RPC("RPCA_LoadLevel", 0, new object[]
		{
			this.GetRandomMap()
		});
	}

	// Token: 0x060002AD RID: 685 RVA: 0x0001142B File Offset: 0x0000F62B
	[PunRPC]
	private void RPCA_SetCallInNextMap(bool toSet)
	{
		this.callInNextMap = toSet;
	}

	// Token: 0x060002AE RID: 686 RVA: 0x00011434 File Offset: 0x0000F634
	public void LoadLevelFromID(int ID, bool onlyMaster = false, bool callInImidetly = false)
	{
		if (!PhotonNetwork.IsMasterClient && onlyMaster)
		{
			return;
		}
		this.callInNextMap = callInImidetly;
		this.RPCA_LoadLevel(this.levels[ID]);
	}

	// Token: 0x060002AF RID: 687 RVA: 0x00011458 File Offset: 0x0000F658
	[PunRPC]
	public void RPCA_CallInNewMapAndMovePlayers(int mapID)
	{
		base.StartCoroutine(this.WaitForMapToBeLoaded(mapID));
	}

	// Token: 0x060002B0 RID: 688 RVA: 0x00011468 File Offset: 0x0000F668
	private IEnumerator WaitForMapToBeLoaded(int mapID)
	{
		while (this.currentLevelID != mapID)
		{
			yield return null;
		}
		global::Debug.Log("CALL IN NEW MAP AND MOVE PLAYERS");
		if (this.currentMap != null)
		{
			MapTransition.instance.Enter(this.currentMap.Map);
		}
		MapTransition.instance.ClearObjects();
		PlayerManager.instance.RPCA_MovePlayers();
		yield break;
	}

	// Token: 0x060002B1 RID: 689 RVA: 0x0001147E File Offset: 0x0000F67E
	public void CallInNewMapAndMovePlayers(int mapID)
	{
		if (!PhotonNetwork.IsMasterClient && !PhotonNetwork.OfflineMode)
		{
			return;
		}
		this.view.RPC("RPCA_CallInNewMapAndMovePlayers", 0, new object[]
		{
			mapID
		});
	}

	// Token: 0x060002B2 RID: 690 RVA: 0x000114AF File Offset: 0x0000F6AF
	[PunRPC]
	public void RPCA_CallInNewMap()
	{
		if (this.currentMap != null)
		{
			MapTransition.instance.Enter(this.currentMap.Map);
		}
		MapTransition.instance.ClearObjects();
	}

	// Token: 0x060002B3 RID: 691 RVA: 0x000114D8 File Offset: 0x0000F6D8
	public void CallInNewMap()
	{
		if (!PhotonNetwork.IsMasterClient && !PhotonNetwork.OfflineMode)
		{
			return;
		}
		this.view.RPC("RPCA_CallInNewMap", 0, Array.Empty<object>());
	}

	// Token: 0x060002B4 RID: 692 RVA: 0x000114FF File Offset: 0x0000F6FF
	public SpawnPoint[] GetSpawnPoints()
	{
		return this.currentMap.Map.GetComponentsInChildren<SpawnPoint>();
	}

	// Token: 0x060002B5 RID: 693 RVA: 0x00011514 File Offset: 0x0000F714
	private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		Map map = null;
		for (int i = 0; i < scene.GetRootGameObjects().Length; i++)
		{
			map = scene.GetRootGameObjects()[i].GetComponent<Map>();
			if (map)
			{
				break;
			}
		}
		if (!map)
		{
			global::Debug.LogError("NO MAP WAS FOUND WHEN LOADING NEW MAP");
		}
		map.wasSpawned = true;
		SceneManager.sceneLoaded -= this.OnLevelFinishedLoading;
		if (this.currentMap != null)
		{
			base.StartCoroutine(this.UnloadAfterSeconds(this.currentMap.Scene));
			MapTransition.instance.Exit(this.currentMap.Map);
		}
		MapTransition.instance.SetStartPos(map);
		map.levelID = this.currentLevelID;
		this.currentMap = new MapWrapper(map, scene);
		this.currentLevelID = this.GetIDFromScene(scene);
		if (this.callInNextMap)
		{
			this.CallInNewMap();
			this.callInNextMap = false;
		}
		global::Debug.Log("FINISHED LOADING SCENE");
	}

	// Token: 0x060002B6 RID: 694 RVA: 0x00011600 File Offset: 0x0000F800
	private int GetIDFromScene(Scene scene)
	{
		int result = -1;
		for (int i = 0; i < this.levels.Length; i++)
		{
			if (this.levels[i] == scene.name)
			{
				result = i;
			}
		}
		return result;
	}

	// Token: 0x060002B7 RID: 695 RVA: 0x0001163B File Offset: 0x0000F83B
	[PunRPC]
	public void RPCA_LoadLevel(string sceneName)
	{
		global::Debug.Log("LOADING SCENE");
		SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
		SceneManager.sceneLoaded += this.OnLevelFinishedLoading;
	}

	// Token: 0x060002B8 RID: 696 RVA: 0x0001165F File Offset: 0x0000F85F
	private IEnumerator UnloadAfterSeconds(Scene scene)
	{
		yield return new WaitForSecondsRealtime(2f);
		SceneManager.UnloadSceneAsync(scene);
		yield break;
	}

	// Token: 0x060002B9 RID: 697 RVA: 0x0001166E File Offset: 0x0000F86E
	public void UnloadScene(Scene scene)
	{
		SceneManager.UnloadSceneAsync(scene);
	}

	// Token: 0x040003CE RID: 974
	[SerializeField]
	public string[] levels;

	// Token: 0x040003CF RID: 975
	public int currentLevelID;

	// Token: 0x040003D0 RID: 976
	public static MapManager instance;

	// Token: 0x040003D1 RID: 977
	[HideInInspector]
	public bool isTestingMap;

	// Token: 0x040003D2 RID: 978
	public MapWrapper currentMap;

	// Token: 0x040003D3 RID: 979
	private PhotonView view;

	// Token: 0x040003D4 RID: 980
	internal int otherPlayersMostRecentlyLoadedLevel = -1;

	// Token: 0x040003D5 RID: 981
	public string forceMap = "";

	// Token: 0x040003D6 RID: 982
	private bool callInNextMap;
}
