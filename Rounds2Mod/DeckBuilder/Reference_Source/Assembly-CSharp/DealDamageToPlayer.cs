using System;
using Sonigon;
using UnityEngine;

// Token: 0x02000041 RID: 65
public class DealDamageToPlayer : MonoBehaviour
{
	// Token: 0x0600013D RID: 317 RVA: 0x00008968 File Offset: 0x00006B68
	private void Start()
	{
		this.data = base.GetComponentInParent<CharacterData>();
	}

	// Token: 0x0600013E RID: 318 RVA: 0x00008978 File Offset: 0x00006B78
	public void Go()
	{
		if (!this.target)
		{
			this.target = this.data.player;
			if (this.targetPlayer == DealDamageToPlayer.TargetPlayer.Other)
			{
				this.target = PlayerManager.instance.GetOtherPlayer(this.target);
			}
		}
		if (this.soundDamage != null && this.target != null && this.target.data != null && this.target.data.isPlaying && !this.target.data.dead && !this.target.data.block.IsBlocking())
		{
			SoundManager.Instance.Play(this.soundDamage, this.target.transform);
		}
		this.target.data.healthHandler.TakeDamage(this.damage * Vector2.up, base.transform.position, null, this.data.player, this.lethal, false);
	}

	// Token: 0x040001AB RID: 427
	[Header("Sounds")]
	public SoundEvent soundDamage;

	// Token: 0x040001AC RID: 428
	[Header("Settings")]
	public float damage = 25f;

	// Token: 0x040001AD RID: 429
	public bool lethal = true;

	// Token: 0x040001AE RID: 430
	public DealDamageToPlayer.TargetPlayer targetPlayer;

	// Token: 0x040001AF RID: 431
	private CharacterData data;

	// Token: 0x040001B0 RID: 432
	private Player target;

	// Token: 0x0200033F RID: 831
	public enum TargetPlayer
	{
		// Token: 0x04001092 RID: 4242
		Own,
		// Token: 0x04001093 RID: 4243
		Other
	}
}
