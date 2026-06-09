using System;
using UnityEngine;

// Token: 0x02000192 RID: 402
public class PopUpHandler : MonoBehaviour
{
	// Token: 0x0600082B RID: 2091 RVA: 0x000027C8 File Offset: 0x000009C8
	private void Start()
	{
	}

	// Token: 0x0600082C RID: 2092 RVA: 0x0002C3AC File Offset: 0x0002A5AC
	public void StartPicking(Player player, Action<PopUpHandler.YesNo> functionToCall)
	{
		PlayerManager.instance.RevivePlayers();
		PlayerManager.instance.SetPlayersSimulated(true);
		this.isPicking = true;
		this.fToCall = functionToCall;
		this.yesPart.particleSettings.color = PlayerManager.instance.GetColorFromTeam(player.teamID).winText;
		this.noPart.particleSettings.color = PlayerManager.instance.GetColorFromTeam(player.teamID).winText;
		this.yesPart.loop = true;
		this.yesPart.Play();
		this.yesAnim.PlayIn();
		this.noPart.loop = true;
		this.noPart.Play();
	}

	// Token: 0x0600082D RID: 2093 RVA: 0x0002C45F File Offset: 0x0002A65F
	private void DonePicking()
	{
		this.fToCall.Invoke(this.currentYesNo);
		this.isPicking = false;
		this.noPart.loop = false;
		this.yesPart.loop = false;
	}

	// Token: 0x0600082E RID: 2094 RVA: 0x0002C494 File Offset: 0x0002A694
	private void Update()
	{
		if (this.isPicking)
		{
			for (int i = 0; i < PlayerManager.instance.players.Count; i++)
			{
				if (PlayerManager.instance.players[i].data.view.IsMine)
				{
					this.playerInput = PlayerManager.instance.players[i].data.input;
					if (this.playerInput.stickPressDir == GeneralInput.StickDirection.Left && this.currentYesNo != PopUpHandler.YesNo.Yes)
					{
						this.currentYesNo = PopUpHandler.YesNo.Yes;
						this.UpdateUI();
					}
					if (this.playerInput.stickPressDir == GeneralInput.StickDirection.Right && this.currentYesNo != PopUpHandler.YesNo.No)
					{
						this.currentYesNo = PopUpHandler.YesNo.No;
						this.UpdateUI();
					}
					if (this.playerInput.acceptWasPressed)
					{
						this.DonePicking();
					}
				}
			}
		}
	}

	// Token: 0x0600082F RID: 2095 RVA: 0x0002C566 File Offset: 0x0002A766
	private void UpdateUI()
	{
		if (this.currentYesNo == PopUpHandler.YesNo.Yes)
		{
			this.yesAnim.PlayIn();
			this.noAnim.PlayOut();
			return;
		}
		this.yesAnim.PlayOut();
		this.noAnim.PlayIn();
	}

	// Token: 0x04000982 RID: 2434
	public CurveAnimation yesAnim;

	// Token: 0x04000983 RID: 2435
	public CurveAnimation noAnim;

	// Token: 0x04000984 RID: 2436
	public bool isPicking;

	// Token: 0x04000985 RID: 2437
	public GeneralParticleSystem yesPart;

	// Token: 0x04000986 RID: 2438
	public GeneralParticleSystem noPart;

	// Token: 0x04000987 RID: 2439
	public GeneralInput playerInput;

	// Token: 0x04000988 RID: 2440
	private Action<PopUpHandler.YesNo> fToCall;

	// Token: 0x04000989 RID: 2441
	private PopUpHandler.YesNo currentYesNo;

	// Token: 0x020003A8 RID: 936
	public enum YesNo
	{
		// Token: 0x0400127B RID: 4731
		Yes,
		// Token: 0x0400127C RID: 4732
		No
	}
}
