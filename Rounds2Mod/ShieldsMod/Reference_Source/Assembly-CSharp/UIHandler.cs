using System;
using System.Collections;
using Sonigon;
using TMPro;
using UnityEngine;

// Token: 0x020000EB RID: 235
public class UIHandler : MonoBehaviour
{
	// Token: 0x060004B3 RID: 1203 RVA: 0x0001B8FB File Offset: 0x00019AFB
	private void Awake()
	{
		UIHandler.instance = this;
		this.popUpHandler = base.transform.root.GetComponentInChildren<PopUpHandler>();
	}

	// Token: 0x060004B4 RID: 1204 RVA: 0x0001B91C File Offset: 0x00019B1C
	public void ShowJoinGameText(string text, Color color)
	{
		SoundManager.Instance.Play(this.soundTextAppear, base.transform);
		if (text != "")
		{
			this.jointGameText.text = text;
		}
		if (color != Color.black)
		{
			this.joinGamePart.particleSettings.color = color;
		}
		this.joinGamePart.loop = true;
		this.joinGamePart.Play();
	}

	// Token: 0x060004B5 RID: 1205 RVA: 0x0001B98D File Offset: 0x00019B8D
	internal void SetNumberOfRounds(int roundsToWinGame)
	{
		this.roundCounter.SetNumberOfRounds(roundsToWinGame);
		this.roundCounterSmall.SetNumberOfRounds(roundsToWinGame);
	}

	// Token: 0x060004B6 RID: 1206 RVA: 0x0001B9A7 File Offset: 0x00019BA7
	public void HideJoinGameText()
	{
		SoundManager.Instance.Play(this.soundTextDisappear, base.transform);
		this.joinGamePart.Stop();
	}

	// Token: 0x060004B7 RID: 1207 RVA: 0x0001B9CA File Offset: 0x00019BCA
	public void DisplayScreenText(Color color, string text, float speed)
	{
		this.gameOverTextPart.particleSettings.color = color;
		this.gameOverTextPart.duration = 60f / speed;
		this.gameOverTextPart.Play();
		this.gameOverText.text = text;
	}

	// Token: 0x060004B8 RID: 1208 RVA: 0x0001BA06 File Offset: 0x00019C06
	public void DisplayScreenTextLoop(string text)
	{
		this.gameOverTextPart.duration = 60f;
		this.gameOverTextPart.loop = true;
		this.gameOverTextPart.Play();
		this.gameOverText.text = text;
	}

	// Token: 0x060004B9 RID: 1209 RVA: 0x0001BA3C File Offset: 0x00019C3C
	public void DisplayScreenTextLoop(Color color, string text)
	{
		this.gameOverTextPart.particleSettings.color = color;
		this.gameOverTextPart.duration = 60f;
		this.gameOverTextPart.loop = true;
		this.gameOverTextPart.Play();
		this.gameOverText.text = text;
	}

	// Token: 0x060004BA RID: 1210 RVA: 0x0001BA8D File Offset: 0x00019C8D
	public void StopScreenTextLoop()
	{
		this.gameOverTextPart.loop = false;
	}

	// Token: 0x060004BB RID: 1211 RVA: 0x0001BA9B File Offset: 0x00019C9B
	public void ShowAddPoint(Color color, string winTextBefore, string text, float speed)
	{
		this.gameOverTextPart.particleSettings.color = color;
		this.gameOverTextPart.duration = 60f / speed;
		this.gameOverTextPart.Play();
		base.StartCoroutine(this.DoShowAddPoint(winTextBefore, text));
	}

	// Token: 0x060004BC RID: 1212 RVA: 0x0001BADB File Offset: 0x00019CDB
	private IEnumerator DoShowAddPoint(string winTextBefore, string text)
	{
		this.gameOverText.text = winTextBefore;
		yield return new WaitForSecondsRealtime(0.7f);
		this.gameOverText.text = text;
		yield break;
	}

	// Token: 0x060004BD RID: 1213 RVA: 0x0001BAF8 File Offset: 0x00019CF8
	public void ShowRoundOver(int p1Rounds, int p2Rounds)
	{
		this.roundCounter.gameObject.SetActive(true);
		this.roundBackgroundPart.Play();
		this.roundTextPart.Play();
		this.roundCounterAnim.PlayIn();
		this.roundCounter.UpdateRounds(p1Rounds, p2Rounds);
	}

	// Token: 0x060004BE RID: 1214 RVA: 0x0001BB44 File Offset: 0x00019D44
	public void ShowRoundCounterSmall(int p1Rounds, int p2Rounds, int p1Points, int p2Points)
	{
		this.roundCounterSmall.gameObject.SetActive(true);
		this.roundCounterSmall.UpdateRounds(p1Rounds, p2Rounds);
		this.roundCounterSmall.UpdatePoints(p1Points, p2Points);
		if (this.roundCounterAnimSmall.currentState != CodeAnimationInstance.AnimationUse.In)
		{
			this.roundCounterAnimSmall.PlayIn();
		}
	}

	// Token: 0x060004BF RID: 1215 RVA: 0x0001BB95 File Offset: 0x00019D95
	public void HideRoundCounterSmall()
	{
		this.roundCounterAnimSmall.PlayOut();
	}

	// Token: 0x060004C0 RID: 1216 RVA: 0x0001BBA4 File Offset: 0x00019DA4
	public void ShowPicker(int pickerID, PickerType pickerType = PickerType.Team)
	{
		this.pickerObject.SetActive(true);
		if (pickerType == PickerType.Team)
		{
			this.pickerPart.particleSettings.color = PlayerManager.instance.GetColorFromTeam(pickerID).winText;
		}
		if (pickerType == PickerType.Player)
		{
			this.pickerPart.particleSettings.color = PlayerManager.instance.GetColorFromPlayer(pickerID).winText;
		}
		this.pickerPart.loop = true;
		this.pickerPart.Play();
	}

	// Token: 0x060004C1 RID: 1217 RVA: 0x0001BC1B File Offset: 0x00019E1B
	public void StopShowPicker()
	{
		base.StartCoroutine(this.DoStopShowPicker());
	}

	// Token: 0x060004C2 RID: 1218 RVA: 0x0001BC2A File Offset: 0x00019E2A
	private IEnumerator DoStopShowPicker()
	{
		this.pickerPart.loop = false;
		yield return new WaitForSeconds(0.3f);
		this.pickerObject.SetActive(false);
		yield break;
	}

	// Token: 0x060004C3 RID: 1219 RVA: 0x0001BC39 File Offset: 0x00019E39
	internal void DisplayYesNoLoop(Player pickingPlayer, Action<PopUpHandler.YesNo> functionToCall)
	{
		this.popUpHandler.StartPicking(pickingPlayer, functionToCall);
	}

	// Token: 0x04000647 RID: 1607
	[Header("Sounds")]
	public SoundEvent soundTextAppear;

	// Token: 0x04000648 RID: 1608
	public SoundEvent soundTextDisappear;

	// Token: 0x04000649 RID: 1609
	public static UIHandler instance;

	// Token: 0x0400064A RID: 1610
	[Header("Settings")]
	public TextMeshProUGUI gameOverText;

	// Token: 0x0400064B RID: 1611
	public GeneralParticleSystem gameOverTextPart;

	// Token: 0x0400064C RID: 1612
	public GeneralParticleSystem roundBackgroundPart;

	// Token: 0x0400064D RID: 1613
	public GeneralParticleSystem roundTextPart;

	// Token: 0x0400064E RID: 1614
	public CodeAnimation roundCounterAnim;

	// Token: 0x0400064F RID: 1615
	public RoundCounter roundCounter;

	// Token: 0x04000650 RID: 1616
	public RoundCounter roundCounterSmall;

	// Token: 0x04000651 RID: 1617
	public CodeAnimation roundCounterAnimSmall;

	// Token: 0x04000652 RID: 1618
	public GameObject pickerObject;

	// Token: 0x04000653 RID: 1619
	public GeneralParticleSystem pickerPart;

	// Token: 0x04000654 RID: 1620
	public GeneralParticleSystem joinGamePart;

	// Token: 0x04000655 RID: 1621
	public TextMeshProUGUI jointGameText;

	// Token: 0x04000656 RID: 1622
	public PopUpHandler popUpHandler;
}
