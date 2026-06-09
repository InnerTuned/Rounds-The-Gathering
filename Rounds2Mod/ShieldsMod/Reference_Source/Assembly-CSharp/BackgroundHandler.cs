using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000010 RID: 16
public class BackgroundHandler : MonoBehaviour
{
	// Token: 0x0600004E RID: 78 RVA: 0x00004071 File Offset: 0x00002271
	private void Start()
	{
		this.backgrounds = base.GetComponentsInChildren<Background>(true);
	}

	// Token: 0x0600004F RID: 79 RVA: 0x00004080 File Offset: 0x00002280
	private void Update()
	{
		this.untilSwitch -= TimeHandler.deltaTime;
		if (this.untilSwitch < 0f)
		{
			this.SwitchBackground();
		}
	}

	// Token: 0x06000050 RID: 80 RVA: 0x000040A8 File Offset: 0x000022A8
	private void SwitchBackground()
	{
		this.untilSwitch = (float)Random.Range(30, 60);
		for (int i = 0; i < this.backgrounds.Length; i++)
		{
			this.backgrounds[i].ToggleBackground(false);
		}
		base.StartCoroutine(this.StartBackGroundSoon());
	}

	// Token: 0x06000051 RID: 81 RVA: 0x000040F3 File Offset: 0x000022F3
	private IEnumerator StartBackGroundSoon()
	{
		yield return new WaitForSeconds(5f);
		this.backgrounds[Random.Range(0, this.backgrounds.Length)].ToggleBackground(true);
		yield break;
	}

	// Token: 0x04000039 RID: 57
	private Background[] backgrounds;

	// Token: 0x0400003A RID: 58
	private float untilSwitch;
}
