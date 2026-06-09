using System;
using System.Collections;
using Sirenix.OdinInspector;
using Sonigon;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000191 RID: 401
public class PointVisualizer : MonoBehaviour
{
	// Token: 0x06000819 RID: 2073 RVA: 0x0002C09D File Offset: 0x0002A29D
	private void Awake()
	{
		PointVisualizer.instance = this;
	}

	// Token: 0x0600081A RID: 2074 RVA: 0x0002C0A8 File Offset: 0x0002A2A8
	private void Start()
	{
		this.orangeBallRT = this.orangeBall.GetComponent<RectTransform>();
		this.blueBallRT = this.blueBall.GetComponent<RectTransform>();
		this.orangeSP = this.orangeBall.GetComponent<RectTransform>().anchoredPosition;
		this.blueSP = this.blueBall.GetComponent<RectTransform>().anchoredPosition;
		this.Close();
	}

	// Token: 0x0600081B RID: 2075 RVA: 0x0002C113 File Offset: 0x0002A313
	[Button]
	private void TestWinSequence()
	{
		base.StartCoroutine(this.DoWinSequence(2, 1, 2, 1, true));
	}

	// Token: 0x0600081C RID: 2076 RVA: 0x0002C127 File Offset: 0x0002A327
	[Button]
	private void TestPoint()
	{
		base.StartCoroutine(this.DoSequence(1, 0, true));
	}

	// Token: 0x0600081D RID: 2077 RVA: 0x0002C139 File Offset: 0x0002A339
	[Button]
	private void Reset()
	{
		base.StartCoroutine(this.DoSequence(0, 0, true));
	}

	// Token: 0x0600081E RID: 2078 RVA: 0x0002C14B File Offset: 0x0002A34B
	public void ResetPoints()
	{
		this.orangeFill.fillAmount = 0f;
		this.blueFill.fillAmount = 0f;
	}

	// Token: 0x0600081F RID: 2079 RVA: 0x0002C170 File Offset: 0x0002A370
	private void ResetBalls()
	{
		this.orangeBallRT.sizeDelta = Vector2.one * this.ballBaseSize;
		this.blueBallRT.sizeDelta = Vector2.one * this.ballBaseSize;
		this.orangeBall.GetComponent<RectTransform>().anchoredPosition = this.orangeSP;
		this.blueBall.GetComponent<RectTransform>().anchoredPosition = this.blueSP;
		this.orangeVel = Vector3.zero;
		this.blueVel = Vector3.zero;
	}

	// Token: 0x06000820 RID: 2080 RVA: 0x0002C1FF File Offset: 0x0002A3FF
	public IEnumerator DoWinSequence(int orangePoints, int bluePoints, int orangeRounds, int blueRounds, bool orangeWinner)
	{
		yield return new WaitForSecondsRealtime(0.35f);
		SoundManager.Instance.Play(this.soundWinRound, base.transform);
		this.ResetBalls();
		this.bg.SetActive(true);
		this.blueBall.gameObject.SetActive(true);
		this.orangeBall.gameObject.SetActive(true);
		yield return new WaitForSecondsRealtime(0.2f);
		GamefeelManager.instance.AddUIGameFeelOverTime(10f, 0.1f);
		this.DoShowPoints(orangePoints, bluePoints, orangeWinner);
		yield return new WaitForSecondsRealtime(0.35f);
		SoundManager.Instance.Play(this.sound_UI_Arms_Race_A_Ball_Shrink_Go_To_Left_Corner, base.transform);
		float c = 0f;
		while (c < this.timeToScale)
		{
			if (orangeWinner)
			{
				this.orangeBallRT.sizeDelta = Vector2.LerpUnclamped(this.orangeBallRT.sizeDelta, Vector2.one * this.ballSmallSize, this.scaleCurve.Evaluate(c / this.timeToScale));
			}
			else
			{
				this.blueBallRT.sizeDelta = Vector2.LerpUnclamped(this.blueBallRT.sizeDelta, Vector2.one * this.ballSmallSize, this.scaleCurve.Evaluate(c / this.timeToScale));
			}
			c += Time.unscaledDeltaTime;
			yield return null;
		}
		yield return new WaitForSecondsRealtime(this.timeBetween);
		c = 0f;
		while (c < this.timeToMove)
		{
			if (orangeWinner)
			{
				this.orangeBall.position = Vector3.LerpUnclamped(this.orangeBall.position, UIHandler.instance.roundCounterSmall.GetPointPos(0), this.scaleCurve.Evaluate(c / this.timeToMove));
			}
			else
			{
				this.blueBall.position = Vector3.LerpUnclamped(this.blueBall.position, UIHandler.instance.roundCounterSmall.GetPointPos(1), this.scaleCurve.Evaluate(c / this.timeToMove));
			}
			c += Time.unscaledDeltaTime;
			yield return null;
		}
		SoundManager.Instance.Play(this.sound_UI_Arms_Race_B_Ball_Go_Down_Then_Expand, base.transform);
		if (orangeWinner)
		{
			this.orangeBall.position = UIHandler.instance.roundCounterSmall.GetPointPos(0);
		}
		else
		{
			this.blueBall.position = UIHandler.instance.roundCounterSmall.GetPointPos(1);
		}
		yield return new WaitForSecondsRealtime(this.timeBetween);
		c = 0f;
		while (c < this.timeToMove)
		{
			if (!orangeWinner)
			{
				this.orangeBall.position = Vector3.LerpUnclamped(this.orangeBall.position, CardChoiceVisuals.instance.transform.position, this.scaleCurve.Evaluate(c / this.timeToMove));
			}
			else
			{
				this.blueBall.position = Vector3.LerpUnclamped(this.blueBall.position, CardChoiceVisuals.instance.transform.position, this.scaleCurve.Evaluate(c / this.timeToMove));
			}
			c += Time.unscaledDeltaTime;
			yield return null;
		}
		if (!orangeWinner)
		{
			this.orangeBall.position = CardChoiceVisuals.instance.transform.position;
		}
		else
		{
			this.blueBall.position = CardChoiceVisuals.instance.transform.position;
		}
		yield return new WaitForSecondsRealtime(this.timeBetween);
		c = 0f;
		while (c < this.timeToScale)
		{
			if (!orangeWinner)
			{
				this.orangeBallRT.sizeDelta = Vector2.LerpUnclamped(this.orangeBallRT.sizeDelta, Vector2.one * this.bigBallScale, this.scaleCurve.Evaluate(c / this.timeToScale));
			}
			else
			{
				this.blueBallRT.sizeDelta = Vector2.LerpUnclamped(this.blueBallRT.sizeDelta, Vector2.one * this.bigBallScale, this.scaleCurve.Evaluate(c / this.timeToScale));
			}
			c += Time.unscaledDeltaTime;
			yield return null;
		}
		SoundManager.Instance.Play(this.sound_UI_Arms_Race_C_Ball_Pop_Shake, base.transform);
		GamefeelManager.instance.AddUIGameFeelOverTime(10f, 0.2f);
		CardChoiceVisuals.instance.Show((!orangeWinner) ? 0 : 1, false);
		UIHandler.instance.roundCounterSmall.UpdateRounds(orangeRounds, blueRounds);
		UIHandler.instance.roundCounterSmall.UpdatePoints(0, 0);
		this.DoShowPoints(0, 0, orangeWinner);
		this.Close();
		yield break;
	}

	// Token: 0x06000821 RID: 2081 RVA: 0x000027C8 File Offset: 0x000009C8
	private void MoveTowards()
	{
	}

	// Token: 0x06000822 RID: 2082 RVA: 0x0002C233 File Offset: 0x0002A433
	public IEnumerator DoSequence(int currentOrange, int currentBlue, bool orangeWinner)
	{
		yield return new WaitForSecondsRealtime(0.45f);
		SoundManager.Instance.Play(this.soundWinRound, base.transform);
		this.ResetBalls();
		this.bg.SetActive(true);
		this.blueBall.gameObject.SetActive(true);
		this.orangeBall.gameObject.SetActive(true);
		yield return new WaitForSecondsRealtime(0.2f);
		GamefeelManager.instance.AddUIGameFeelOverTime(10f, 0.1f);
		this.DoShowPoints(currentOrange, currentBlue, orangeWinner);
		yield return new WaitForSecondsRealtime(1.8f);
		this.orangeBall.GetComponent<CurveAnimation>().PlayOut();
		this.blueBall.GetComponent<CurveAnimation>().PlayOut();
		yield return new WaitForSecondsRealtime(0.25f);
		this.Close();
		yield break;
	}

	// Token: 0x06000823 RID: 2083 RVA: 0x0002C258 File Offset: 0x0002A458
	public void DoShowPoints(int currentOrange, int currentBlue, bool orangeWinner)
	{
		this.orangeFill.fillAmount = (float)currentOrange * 0.5f;
		this.blueFill.fillAmount = (float)currentBlue * 0.5f;
		if (orangeWinner)
		{
			this.text.color = PlayerSkinBank.GetPlayerSkinColors(0).winText;
		}
		else
		{
			this.text.color = PlayerSkinBank.GetPlayerSkinColors(1).winText;
		}
		if (orangeWinner)
		{
			if (currentOrange > 1)
			{
				this.RoundOrange();
				return;
			}
			this.HalfOrange();
			return;
		}
		else
		{
			if (currentBlue > 1)
			{
				this.RoundOBlue();
				return;
			}
			this.HalfBlue();
			return;
		}
	}

	// Token: 0x06000824 RID: 2084 RVA: 0x0002C2E2 File Offset: 0x0002A4E2
	private void Close()
	{
		this.text.text = "";
		this.bg.SetActive(false);
		this.blueBall.gameObject.SetActive(false);
		this.orangeBall.gameObject.SetActive(false);
	}

	// Token: 0x06000825 RID: 2085 RVA: 0x0002C322 File Offset: 0x0002A522
	private void HalfOrange()
	{
		this.text.text = "HALF ORANGE";
	}

	// Token: 0x06000826 RID: 2086 RVA: 0x0002C334 File Offset: 0x0002A534
	private void RoundOrange()
	{
		this.text.text = "ROUND ORANGE";
	}

	// Token: 0x06000827 RID: 2087 RVA: 0x0002C346 File Offset: 0x0002A546
	private void HalfBlue()
	{
		this.text.text = "HALF BLUE";
	}

	// Token: 0x06000828 RID: 2088 RVA: 0x0002C358 File Offset: 0x0002A558
	private void RoundOBlue()
	{
		this.text.text = "ROUND BLUE";
	}

	// Token: 0x06000829 RID: 2089 RVA: 0x000027C8 File Offset: 0x000009C8
	private void Update()
	{
	}

	// Token: 0x04000969 RID: 2409
	public SoundEvent soundWinRound;

	// Token: 0x0400096A RID: 2410
	public SoundEvent sound_UI_Arms_Race_A_Ball_Shrink_Go_To_Left_Corner;

	// Token: 0x0400096B RID: 2411
	public SoundEvent sound_UI_Arms_Race_B_Ball_Go_Down_Then_Expand;

	// Token: 0x0400096C RID: 2412
	public SoundEvent sound_UI_Arms_Race_C_Ball_Pop_Shake;

	// Token: 0x0400096D RID: 2413
	public static PointVisualizer instance;

	// Token: 0x0400096E RID: 2414
	public AnimationCurve moveCurve;

	// Token: 0x0400096F RID: 2415
	public AnimationCurve scaleCurve;

	// Token: 0x04000970 RID: 2416
	public float timeBetween;

	// Token: 0x04000971 RID: 2417
	public float timeToMove = 0.2f;

	// Token: 0x04000972 RID: 2418
	public float timeToScale = 0.2f;

	// Token: 0x04000973 RID: 2419
	public TextMeshProUGUI text;

	// Token: 0x04000974 RID: 2420
	public GameObject bg;

	// Token: 0x04000975 RID: 2421
	public Transform orangeBall;

	// Token: 0x04000976 RID: 2422
	public Transform blueBall;

	// Token: 0x04000977 RID: 2423
	private RectTransform orangeBallRT;

	// Token: 0x04000978 RID: 2424
	private RectTransform blueBallRT;

	// Token: 0x04000979 RID: 2425
	public Image orangeFill;

	// Token: 0x0400097A RID: 2426
	public Image blueFill;

	// Token: 0x0400097B RID: 2427
	private Vector3 orangeVel;

	// Token: 0x0400097C RID: 2428
	private Vector3 blueVel;

	// Token: 0x0400097D RID: 2429
	private Vector3 orangeSP;

	// Token: 0x0400097E RID: 2430
	private Vector3 blueSP;

	// Token: 0x0400097F RID: 2431
	private float ballBaseSize = 200f;

	// Token: 0x04000980 RID: 2432
	private float ballSmallSize = 20f;

	// Token: 0x04000981 RID: 2433
	private float bigBallScale = 900f;
}
