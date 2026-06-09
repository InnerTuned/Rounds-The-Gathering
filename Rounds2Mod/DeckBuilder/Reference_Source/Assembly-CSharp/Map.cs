using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200007E RID: 126
public class Map : MonoBehaviour
{
	// Token: 0x0600029B RID: 667 RVA: 0x00010FE0 File Offset: 0x0000F1E0
	internal bool LoadedForAll()
	{
		return MapManager.instance.otherPlayersMostRecentlyLoadedLevel == this.levelID;
	}

	// Token: 0x0600029C RID: 668 RVA: 0x00010FF4 File Offset: 0x0000F1F4
	private void Awake()
	{
		if (!GameManager.instance || !GameManager.instance.isPlaying)
		{
			GM_Test componentInChildren = GameManager.instance.transform.root.GetComponentInChildren<GM_Test>(true);
			componentInChildren.gameObject.SetActive(true);
			componentInChildren.testMap = true;
			MapManager.instance.isTestingMap = true;
			componentInChildren.transform.root.Find("UI/UI_MainMenu").gameObject.SetActive(false);
			this.hasEntered = true;
		}
	}

	// Token: 0x0600029D RID: 669 RVA: 0x00011074 File Offset: 0x0000F274
	private void Update()
	{
		this.counter += Time.deltaTime;
		if (this.hasCalledReady)
		{
			return;
		}
		if ((PhotonNetwork.OfflineMode && this.counter > 1f && this.hasEntered) || (this.hasEntered && this.LoadedForAll()))
		{
			if (this.missingObjects <= 0)
			{
				this.readyForFrames++;
			}
			if (this.readyForFrames > 2)
			{
				base.StartCoroutine(this.StartMatch());
			}
		}
	}

	// Token: 0x0600029E RID: 670 RVA: 0x000110F5 File Offset: 0x0000F2F5
	public void MapMoveOut()
	{
		Action action = this.mapMovingOutAction;
		if (action == null)
		{
			return;
		}
		action.Invoke();
	}

	// Token: 0x0600029F RID: 671 RVA: 0x00011107 File Offset: 0x0000F307
	internal Vector3 GetRandomSpawnPos()
	{
		if (this.spawnPoints == null)
		{
			this.spawnPoints = base.GetComponentsInChildren<SpawnPoint>();
		}
		return this.spawnPoints[Random.Range(0, this.spawnPoints.Length)].transform.position;
	}

	// Token: 0x060002A0 RID: 672 RVA: 0x0001113C File Offset: 0x0000F33C
	private IEnumerator StartMatch()
	{
		this.hasCalledReady = true;
		Action action = this.mapIsReadyEarlyAction;
		if (action != null)
		{
			action.Invoke();
		}
		yield return new WaitForSecondsRealtime(0f);
		this.allRigs = base.GetComponentsInChildren<Rigidbody2D>();
		Action action2 = this.mapIsReadyAction;
		if (action2 != null)
		{
			action2.Invoke();
		}
		yield break;
	}

	// Token: 0x060002A1 RID: 673 RVA: 0x0001114C File Offset: 0x0000F34C
	private void Start()
	{
		if (!PhotonNetwork.OfflineMode)
		{
			MapManager.instance.ReportMapLoaded(this.levelID);
		}
		if (MapManager.instance.isTestingMap)
		{
			this.wasSpawned = true;
		}
		if (!this.wasSpawned)
		{
			MapManager.instance.UnloadScene(base.gameObject.scene);
		}
		SpriteRenderer[] componentsInChildren = base.GetComponentsInChildren<SpriteRenderer>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if ((double)componentsInChildren[i].color.a >= 0.5)
			{
				componentsInChildren[i].transform.position = new Vector3(componentsInChildren[i].transform.position.x, componentsInChildren[i].transform.position.y, -3f);
				if (!(componentsInChildren[i].gameObject.tag == "NoMask"))
				{
					componentsInChildren[i].color = new Color(0.21568628f, 0.21568628f, 0.21568628f);
					if (!componentsInChildren[i].GetComponent<SpriteMask>())
					{
						componentsInChildren[i].gameObject.AddComponent<SpriteMask>().sprite = componentsInChildren[i].sprite;
					}
				}
			}
		}
		SpriteMask[] componentsInChildren2 = base.GetComponentsInChildren<SpriteMask>();
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			if (!(componentsInChildren2[j].gameObject.tag == "NoMask"))
			{
				componentsInChildren2[j].isCustomRangeActive = true;
				componentsInChildren2[j].frontSortingLayerID = SortingLayer.NameToID("MapParticle");
				componentsInChildren2[j].frontSortingOrder = 1;
				componentsInChildren2[j].backSortingLayerID = SortingLayer.NameToID("MapParticle");
				componentsInChildren2[j].backSortingOrder = 0;
			}
		}
	}

	// Token: 0x040003BE RID: 958
	public bool wasSpawned;

	// Token: 0x040003BF RID: 959
	public float size = 15f;

	// Token: 0x040003C0 RID: 960
	public bool hasEntered;

	// Token: 0x040003C1 RID: 961
	internal int levelID;

	// Token: 0x040003C2 RID: 962
	internal int missingObjects;

	// Token: 0x040003C3 RID: 963
	private float counter;

	// Token: 0x040003C4 RID: 964
	private int readyForFrames;

	// Token: 0x040003C5 RID: 965
	private bool hasCalledReady;

	// Token: 0x040003C6 RID: 966
	public Rigidbody2D[] allRigs;

	// Token: 0x040003C7 RID: 967
	private SpawnPoint[] spawnPoints;

	// Token: 0x040003C8 RID: 968
	public Action mapIsReadyAction;

	// Token: 0x040003C9 RID: 969
	public Action mapIsReadyEarlyAction;

	// Token: 0x040003CA RID: 970
	public Action mapMovingOutAction;

	// Token: 0x040003CB RID: 971
	internal bool hasRope;
}
