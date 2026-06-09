using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200005D RID: 93
public class GamefeelManager : MonoBehaviour
{
	// Token: 0x060001AA RID: 426 RVA: 0x0000ACD1 File Offset: 0x00008ED1
	private void Awake()
	{
		GamefeelManager.instance = this;
	}

	// Token: 0x060001AB RID: 427 RVA: 0x0000ACD9 File Offset: 0x00008ED9
	public static void RegisterGamefeeler(GameFeeler gameFeeler)
	{
		GamefeelManager.instance.m_GameFeelers.Add(gameFeeler);
	}

	// Token: 0x060001AC RID: 428 RVA: 0x0000ACEC File Offset: 0x00008EEC
	public void AddUIGameFeel(Vector2 directionForce)
	{
		for (int i = 0; i < GamefeelManager.instance.m_GameFeelers.Count; i++)
		{
			GamefeelManager.instance.m_GameFeelers[i].OnUIGameFeel(directionForce);
		}
	}

	// Token: 0x060001AD RID: 429 RVA: 0x0000AD2C File Offset: 0x00008F2C
	public void AddGameFeel(Vector2 directionForce)
	{
		for (int i = 0; i < GamefeelManager.instance.m_GameFeelers.Count; i++)
		{
			GamefeelManager.instance.m_GameFeelers[i].OnGameFeel(directionForce);
		}
	}

	// Token: 0x060001AE RID: 430 RVA: 0x0000AD6C File Offset: 0x00008F6C
	public static void GameFeel(Vector2 directionForce)
	{
		for (int i = 0; i < GamefeelManager.instance.m_GameFeelers.Count; i++)
		{
			GamefeelManager.instance.m_GameFeelers[i].OnGameFeel(directionForce);
		}
	}

	// Token: 0x060001AF RID: 431 RVA: 0x0000ADA9 File Offset: 0x00008FA9
	public void AddUIGameFeelOverTime(float amount, float time)
	{
		base.StartCoroutine(this.DoUIGameFeelOverTime(amount, time));
	}

	// Token: 0x060001B0 RID: 432 RVA: 0x0000ADBA File Offset: 0x00008FBA
	private IEnumerator DoUIGameFeelOverTime(float amount, float time)
	{
		float startTime = time;
		while (time > 0f)
		{
			GamefeelManager.instance.AddUIGameFeel(amount * Random.onUnitSphere * Time.unscaledDeltaTime * (time / startTime));
			time -= Time.unscaledDeltaTime;
			yield return null;
		}
		yield break;
	}

	// Token: 0x0400023E RID: 574
	public static GamefeelManager instance;

	// Token: 0x0400023F RID: 575
	private List<GameFeeler> m_GameFeelers = new List<GameFeeler>();
}
