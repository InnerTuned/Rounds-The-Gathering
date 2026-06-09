using System;
using UnityEngine;

// Token: 0x0200005E RID: 94
public class GameManager : MonoBehaviour
{
	// Token: 0x060001B2 RID: 434 RVA: 0x0000ADE3 File Offset: 0x00008FE3
	public void Awake()
	{
		GameManager.instance = this;
	}

	// Token: 0x060001B3 RID: 435 RVA: 0x0000ADEB File Offset: 0x00008FEB
	private void Start()
	{
		GameManager.lockInput = false;
	}

	// Token: 0x060001B4 RID: 436 RVA: 0x000027C8 File Offset: 0x000009C8
	private void Update()
	{
	}

	// Token: 0x060001B5 RID: 437 RVA: 0x0000ADF3 File Offset: 0x00008FF3
	public void GameOver(int winingTeamID, int killedTeamID)
	{
		if (this.GameOverAction != null)
		{
			this.GameOverAction.Invoke(winingTeamID, killedTeamID);
		}
	}

	// Token: 0x04000240 RID: 576
	[Header("Settings")]
	public static bool lockInput;

	// Token: 0x04000241 RID: 577
	public static GameManager instance;

	// Token: 0x04000242 RID: 578
	public bool isPlaying;

	// Token: 0x04000243 RID: 579
	public bool battleOngoing;

	// Token: 0x04000244 RID: 580
	public Action<int, int> GameOverAction;
}
