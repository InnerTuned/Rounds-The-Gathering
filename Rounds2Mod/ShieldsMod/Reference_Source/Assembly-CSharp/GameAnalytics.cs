using System;
using UnityEngine;
using UnityEngine.Analytics;

// Token: 0x0200014D RID: 333
public class GameAnalytics : MonoBehaviour
{
	// Token: 0x060006C1 RID: 1729 RVA: 0x000027C8 File Offset: 0x000009C8
	private void Start()
	{
	}

	// Token: 0x060006C2 RID: 1730 RVA: 0x0002574B File Offset: 0x0002394B
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			Analytics.SendEvent("PlayerKills", 5, 1, "");
		}
	}
}
