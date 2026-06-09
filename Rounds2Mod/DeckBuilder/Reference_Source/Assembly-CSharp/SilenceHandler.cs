using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020001B3 RID: 435
public class SilenceHandler : MonoBehaviour
{
	// Token: 0x060008AA RID: 2218 RVA: 0x0002DD40 File Offset: 0x0002BF40
	private void Start()
	{
		this.player = base.GetComponent<Player>();
		this.data = this.player.data;
	}

	// Token: 0x060008AB RID: 2219 RVA: 0x0002DD60 File Offset: 0x0002BF60
	private void Update()
	{
		if (this.data.silenceTime > 0f)
		{
			this.data.silenceTime -= TimeHandler.deltaTime;
			if (!this.data.isSilenced)
			{
				this.StartSilence();
				return;
			}
		}
		else if (this.data.isSilenced)
		{
			this.StopSilence();
		}
	}

	// Token: 0x060008AC RID: 2220 RVA: 0x0002DDBD File Offset: 0x0002BFBD
	private void StartSilence()
	{
		this.player.data.input.silencedInput = true;
		this.codeAnim.PlayIn();
		this.data.isSilenced = true;
	}

	// Token: 0x060008AD RID: 2221 RVA: 0x0002DDEC File Offset: 0x0002BFEC
	public void StopSilence()
	{
		this.player.data.input.silencedInput = false;
		if (this.codeAnim.currentState == CodeAnimationInstance.AnimationUse.In)
		{
			this.codeAnim.PlayOut();
		}
		this.data.isSilenced = false;
		this.data.silenceTime = 0f;
	}

	// Token: 0x060008AE RID: 2222 RVA: 0x0002DE43 File Offset: 0x0002C043
	private void OnDisable()
	{
		this.codeAnim.transform.localScale = Vector3.zero;
	}

	// Token: 0x060008AF RID: 2223 RVA: 0x0002DE5A File Offset: 0x0002C05A
	[PunRPC]
	public void RPCA_AddSilence(float f)
	{
		if (f > this.data.silenceTime)
		{
			this.data.silenceTime = f;
		}
		if (!this.data.isSilenced)
		{
			this.StartSilence();
		}
	}

	// Token: 0x040009EE RID: 2542
	[Header("Settings")]
	public CodeAnimation codeAnim;

	// Token: 0x040009EF RID: 2543
	private Player player;

	// Token: 0x040009F0 RID: 2544
	private CharacterData data;
}
