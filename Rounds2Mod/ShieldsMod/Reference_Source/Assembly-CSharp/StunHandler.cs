using System;
using Sonigon;
using UnityEngine;

// Token: 0x020000E2 RID: 226
public class StunHandler : MonoBehaviour
{
	// Token: 0x06000478 RID: 1144 RVA: 0x0001A974 File Offset: 0x00018B74
	private void Start()
	{
		this.player = base.GetComponent<Player>();
		this.data = this.player.data;
	}

	// Token: 0x06000479 RID: 1145 RVA: 0x0001A994 File Offset: 0x00018B94
	private void Update()
	{
		if (this.data.stunTime > 0f)
		{
			this.data.stunTime -= TimeHandler.deltaTime;
			this.data.sinceGrounded = 0f;
			if (!this.data.isStunned)
			{
				this.StartStun();
			}
		}
		else if (this.data.isStunned)
		{
			this.StopStun();
		}
		if (this.data.isStunned && this.data.isPlaying && !this.data.dead)
		{
			if (!this.soundStunIsPlaying)
			{
				this.soundStunIsPlaying = true;
				SoundManager.Instance.Play(this.soundCharacterStunLoop, base.transform);
				return;
			}
		}
		else if (this.soundStunIsPlaying)
		{
			this.soundStunIsPlaying = false;
			SoundManager.Instance.Stop(this.soundCharacterStunLoop, base.transform, true);
		}
	}

	// Token: 0x0600047A RID: 1146 RVA: 0x0001AA78 File Offset: 0x00018C78
	private void StartStun()
	{
		this.player.data.playerVel.velocity *= 0f;
		this.player.data.playerVel.isKinematic = true;
		this.player.data.input.stunnedInput = true;
		this.codeAnim.PlayIn();
		this.data.isStunned = true;
	}

	// Token: 0x0600047B RID: 1147 RVA: 0x0001AAF0 File Offset: 0x00018CF0
	public void StopStun()
	{
		this.player.data.playerVel.isKinematic = false;
		this.player.data.input.stunnedInput = false;
		if (this.codeAnim.currentState == CodeAnimationInstance.AnimationUse.In)
		{
			this.codeAnim.PlayOut();
		}
		this.data.isStunned = false;
		this.data.stunTime = 0f;
	}

	// Token: 0x0600047C RID: 1148 RVA: 0x0001AB5D File Offset: 0x00018D5D
	private void OnDisable()
	{
		this.codeAnim.transform.localScale = Vector3.zero;
		this.soundStunIsPlaying = false;
		SoundManager.Instance.Stop(this.soundCharacterStunLoop, base.transform, true);
	}

	// Token: 0x0600047D RID: 1149 RVA: 0x0001AB92 File Offset: 0x00018D92
	private void OnDestroy()
	{
		this.soundStunIsPlaying = false;
		SoundManager.Instance.Stop(this.soundCharacterStunLoop, base.transform, true);
	}

	// Token: 0x0600047E RID: 1150 RVA: 0x0001ABB4 File Offset: 0x00018DB4
	public void AddStun(float f)
	{
		if (this.data.block.IsBlocking())
		{
			return;
		}
		if (f > this.data.stunTime)
		{
			this.data.stunTime = f;
		}
		if (!this.data.isStunned)
		{
			this.StartStun();
		}
	}

	// Token: 0x04000604 RID: 1540
	[Header("Sounds")]
	public SoundEvent soundCharacterStunLoop;

	// Token: 0x04000605 RID: 1541
	private bool soundStunIsPlaying;

	// Token: 0x04000606 RID: 1542
	[Header("Settings")]
	public CodeAnimation codeAnim;

	// Token: 0x04000607 RID: 1543
	private Player player;

	// Token: 0x04000608 RID: 1544
	private CharacterData data;
}
