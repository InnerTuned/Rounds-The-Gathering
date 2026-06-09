using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200003C RID: 60
public class DamagableEvent : Damagable
{
	// Token: 0x06000124 RID: 292 RVA: 0x000081AB File Offset: 0x000063AB
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
		this.maxHP = this.currentHP;
	}

	// Token: 0x06000125 RID: 293 RVA: 0x000081C8 File Offset: 0x000063C8
	public override void TakeDamage(Vector2 damage, Vector2 damagePosition, GameObject damagingWeapon = null, Player damagingPlayer = null, bool lethal = true, bool ignoreBlock = false)
	{
		if (this.disabled)
		{
			return;
		}
		if (this.dead)
		{
			return;
		}
		if (this.networkedDamage)
		{
			if (damagingPlayer)
			{
				if (!damagingPlayer.data.view.IsMine)
				{
					return;
				}
				this.view.RPC("RPCA_TakeDamage", 1, new object[]
				{
					damage,
					damagePosition
				});
			}
			else
			{
				if (!this.view.IsMine)
				{
					return;
				}
				this.view.RPC("RPCA_TakeDamage", 1, new object[]
				{
					damage,
					damagePosition
				});
			}
		}
		this.DoDamage(damage, damagePosition, damagingWeapon, damagingPlayer, lethal, ignoreBlock);
	}

	// Token: 0x06000126 RID: 294 RVA: 0x00008282 File Offset: 0x00006482
	[PunRPC]
	public void RPCA_TakeDamage(Vector2 damage, Vector2 damagePosition)
	{
		this.DoDamage(damage, damagePosition, null, null, true, false);
	}

	// Token: 0x06000127 RID: 295 RVA: 0x00008290 File Offset: 0x00006490
	private void DoDamage(Vector2 damage, Vector2 damagePosition, GameObject damagingWeapon = null, Player damagingPlayer = null, bool lethal = true, bool ignoreBlock = false)
	{
		if (damagingPlayer)
		{
			this.lastPlayer = damagingPlayer;
		}
		if (damagingWeapon)
		{
			this.lastWeapon = damagingWeapon;
		}
		this.sinceDamage = 0f;
		this.currentHP -= damage.magnitude;
		if (this.currentHP <= 0f)
		{
			this.Die(damage);
			return;
		}
		this.damageEvent.Invoke();
	}

	// Token: 0x06000128 RID: 296 RVA: 0x000082FC File Offset: 0x000064FC
	private void Die(Vector2 damage = default(Vector2))
	{
		if (this.dead)
		{
			return;
		}
		if (this.disabled)
		{
			return;
		}
		Action<Vector2> dieAction = this.DieAction;
		if (dieAction != null)
		{
			dieAction.Invoke(damage);
		}
		this.deathEvent.Invoke();
		this.dead = true;
	}

	// Token: 0x06000129 RID: 297 RVA: 0x00008334 File Offset: 0x00006534
	private void Update()
	{
		if (this.dead)
		{
			return;
		}
		if (this.disabled)
		{
			return;
		}
		this.sinceDamage += TimeHandler.deltaTime;
		if (this.sinceDamage > this.regenDelay && this.currentHP < this.maxHP)
		{
			this.currentHP += this.regenPerSecond * TimeHandler.deltaTime;
			this.currentHP = Mathf.Clamp(this.currentHP, float.NegativeInfinity, this.maxHP);
		}
	}

	// Token: 0x0600012A RID: 298 RVA: 0x000083B6 File Offset: 0x000065B6
	public override void TakeDamage(Vector2 damage, Vector2 damagePosition, Color dmgColor, GameObject damagingWeapon = null, Player damagingPlayer = null, bool lethal = true, bool ignoreBlock = false)
	{
		this.TakeDamage(damage, damagePosition, damagingWeapon, damagingPlayer, lethal, false);
	}

	// Token: 0x0600012B RID: 299 RVA: 0x000083C7 File Offset: 0x000065C7
	public override void CallTakeDamage(Vector2 damage, Vector2 damagePosition, GameObject damagingWeapon = null, Player damagingPlayer = null, bool lethal = true)
	{
		this.TakeDamage(damage, damagePosition, Color.red, damagingWeapon, damagingPlayer, lethal, false);
	}

	// Token: 0x04000185 RID: 389
	public bool networkedDamage;

	// Token: 0x04000186 RID: 390
	public bool disabled;

	// Token: 0x04000187 RID: 391
	[HideInInspector]
	public bool dead;

	// Token: 0x04000188 RID: 392
	public float currentHP = 100f;

	// Token: 0x04000189 RID: 393
	public float regenDelay = 1f;

	// Token: 0x0400018A RID: 394
	public float regenPerSecond;

	// Token: 0x0400018B RID: 395
	[HideInInspector]
	public float maxHP = 100f;

	// Token: 0x0400018C RID: 396
	public UnityEvent damageEvent;

	// Token: 0x0400018D RID: 397
	public UnityEvent deathEvent;

	// Token: 0x0400018E RID: 398
	private float sinceDamage = 1f;

	// Token: 0x0400018F RID: 399
	[HideInInspector]
	public Player lastPlayer;

	// Token: 0x04000190 RID: 400
	[HideInInspector]
	public GameObject lastWeapon;

	// Token: 0x04000191 RID: 401
	private PhotonView view;

	// Token: 0x04000192 RID: 402
	public Action<Vector2> DieAction;
}
