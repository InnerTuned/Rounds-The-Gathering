using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000E9 RID: 233
public class TimeHandler : MonoBehaviour
{
	// Token: 0x06000494 RID: 1172 RVA: 0x0001B495 File Offset: 0x00019695
	private void Awake()
	{
		TimeHandler.instance = this;
	}

	// Token: 0x06000495 RID: 1173 RVA: 0x0001B4A0 File Offset: 0x000196A0
	private void Update()
	{
		float num = this.baseTimeScale;
		if (this.gameOverTime < 1f)
		{
			num *= this.gameOverTime;
		}
		if (this.gameStartTime < 1f)
		{
			num *= this.gameStartTime;
		}
		if (this.timeStop < 1f)
		{
			num *= this.timeStop;
		}
		if (PhotonNetwork.OfflineMode && EscapeMenuHandler.isEscMenu)
		{
			num *= 0f;
		}
		TimeHandler.timeScale = num;
		TimeHandler.deltaTime = Time.deltaTime * TimeHandler.timeScale;
		TimeHandler.fixedDeltaTime = Time.fixedDeltaTime * TimeHandler.timeScale;
		Time.timeScale = 1f;
	}

	// Token: 0x06000496 RID: 1174 RVA: 0x0001B53C File Offset: 0x0001973C
	public void StartGame()
	{
		this.gameStartTime = 1f;
	}

	// Token: 0x06000497 RID: 1175 RVA: 0x0001B549 File Offset: 0x00019749
	public void DoSpeedUp()
	{
		base.StartCoroutine(this.DoCurve(this.speedUp));
	}

	// Token: 0x06000498 RID: 1176 RVA: 0x0001B55E File Offset: 0x0001975E
	public void DoSlowDown()
	{
		base.StartCoroutine(this.DoCurve(this.slowDown));
	}

	// Token: 0x06000499 RID: 1177 RVA: 0x0001B573 File Offset: 0x00019773
	private IEnumerator DoCurve(AnimationCurve curve)
	{
		float c = 0f;
		float t = curve.keys[curve.keys.Length - 1].time;
		while (c < t)
		{
			this.gameOverTime = curve.Evaluate(c);
			c += Time.unscaledDeltaTime;
			yield return null;
		}
		this.gameOverTime = curve.Evaluate(t);
		yield break;
	}

	// Token: 0x0600049A RID: 1178 RVA: 0x0001B589 File Offset: 0x00019789
	public void HitStop()
	{
		base.StartCoroutine(this.DoHitStop());
	}

	// Token: 0x0600049B RID: 1179 RVA: 0x0001B598 File Offset: 0x00019798
	private IEnumerator DoHitStop()
	{
		this.timeStop = 0f;
		yield return new WaitForSeconds(0.3f);
		this.timeStop = 1f;
		yield break;
	}

	// Token: 0x04000634 RID: 1588
	public AnimationCurve slowDown;

	// Token: 0x04000635 RID: 1589
	public AnimationCurve speedUp;

	// Token: 0x04000636 RID: 1590
	public float baseTimeScale = 0.85f;

	// Token: 0x04000637 RID: 1591
	public float gameOverTime = 1f;

	// Token: 0x04000638 RID: 1592
	public float gameStartTime;

	// Token: 0x04000639 RID: 1593
	public float timeStop = 1f;

	// Token: 0x0400063A RID: 1594
	public static float timeScale = 1f;

	// Token: 0x0400063B RID: 1595
	public static TimeHandler instance;

	// Token: 0x0400063C RID: 1596
	public static float deltaTime;

	// Token: 0x0400063D RID: 1597
	public static float fixedDeltaTime;
}
