using System;
using System.Collections;
using Photon.Pun;
using Sonigon;
using UnityEngine;

// Token: 0x0200006B RID: 107
public class HealthHandler : Damagable
{
	// Token: 0x0600021A RID: 538 RVA: 0x0000D4CC File Offset: 0x0000B6CC
	private void Awake()
	{
		this.dot = base.GetComponent<DamageOverTime>();
		this.data = base.GetComponent<CharacterData>();
		this.anim = base.GetComponentInChildren<CodeAnimation>();
		this.player = base.GetComponent<Player>();
		this.stats = base.GetComponent<CharacterStatModifiers>();
	}

	// Token: 0x0600021B RID: 539 RVA: 0x0000D50A File Offset: 0x0000B70A
	private void Start()
	{
		this.startHealthSpriteScale = this.hpSprite.transform.localScale;
	}

	// Token: 0x0600021C RID: 540 RVA: 0x0000D522 File Offset: 0x0000B722
	private void Update()
	{
		this.flyingFor -= TimeHandler.deltaTime;
		if (this.regeneration > 0f)
		{
			this.Heal(this.regeneration * TimeHandler.deltaTime);
		}
	}

	// Token: 0x0600021D RID: 541 RVA: 0x0000D558 File Offset: 0x0000B758
	public void Heal(float healAmount)
	{
		if (healAmount == 0f || this.data.health == this.data.maxHealth)
		{
			return;
		}
		SoundManager.Instance.Play(this.soundHeal, base.transform);
		this.data.health += healAmount;
		this.data.health = Mathf.Clamp(this.data.health, float.NegativeInfinity, this.data.maxHealth);
		this.healPart.Emit((int)Mathf.Clamp(healAmount * 0.2f, 1f, 10f));
	}

	// Token: 0x0600021E RID: 542 RVA: 0x0000D5FC File Offset: 0x0000B7FC
	public void CallTakeForce(Vector2 force, ForceMode2D forceMode = 1, bool forceIgnoreMass = false, bool ignoreBlock = false, float setFlying = 0f)
	{
		if (!this.data.isPlaying)
		{
			return;
		}
		if (this.data.block.IsBlocking() && !ignoreBlock)
		{
			return;
		}
		this.data.view.RPC("RPCA_SendTakeForce", 0, new object[]
		{
			force,
			forceMode,
			forceIgnoreMass,
			true,
			setFlying
		});
	}

	// Token: 0x0600021F RID: 543 RVA: 0x0000D678 File Offset: 0x0000B878
	[PunRPC]
	public void RPCA_SendTakeForce(Vector2 force, int forceMode, bool forceIgnoreMass = false, bool ignoreBlock = false, float setFlying = 0f)
	{
		this.TakeForce(force, forceMode, forceIgnoreMass, ignoreBlock, setFlying);
		this.data.GetComponent<SyncPlayerMovement>().SetDontSyncFor((float)PhotonNetwork.GetPing() * 0.001f + 0.2f);
	}

	// Token: 0x06000220 RID: 544 RVA: 0x0000D6A9 File Offset: 0x0000B8A9
	[PunRPC]
	public void RPCA_SendForceOverTime(Vector2 force, float time, int forceMode, bool forceIgnoreMass = false, bool ignoreBlock = false)
	{
		base.StartCoroutine(this.IForceOverTime(force, time, forceMode, forceIgnoreMass, ignoreBlock));
	}

	// Token: 0x06000221 RID: 545 RVA: 0x0000D6BF File Offset: 0x0000B8BF
	private IEnumerator IForceOverTime(Vector2 force, float time, int forceMode, bool forceIgnoreMass = false, bool ignoreBlock = false)
	{
		for (float i = 0f; i < time; i += TimeHandler.deltaTime)
		{
			this.TakeForce(force, forceMode, forceIgnoreMass, ignoreBlock, 0f);
			this.data.GetComponent<SyncPlayerMovement>().SetDontSyncFor((float)PhotonNetwork.GetPing() * 0.001f + 0.2f);
			yield return new WaitForFixedUpdate();
		}
		yield break;
	}

	// Token: 0x06000222 RID: 546 RVA: 0x0000D6F4 File Offset: 0x0000B8F4
	[PunRPC]
	public void RPCA_SendForceTowardsPointOverTime(float force, float drag, float clampDistancce, Vector2 point, float time, int forceMode, bool forceIgnoreMass = false, bool ignoreBlock = false)
	{
		base.StartCoroutine(this.IForceTowardsPointOverTime(force, drag, clampDistancce, point, time, forceMode, forceIgnoreMass, ignoreBlock));
	}

	// Token: 0x06000223 RID: 547 RVA: 0x0000D71C File Offset: 0x0000B91C
	private IEnumerator IForceTowardsPointOverTime(float force, float drag, float clampDistancce, Vector2 point, float time, int forceMode, bool forceIgnoreMass = false, bool ignoreBlock = false)
	{
		for (float i = 0f; i < time; i += TimeHandler.fixedDeltaTime)
		{
			Vector2 vector = point - base.transform.position;
			vector = Vector2.ClampMagnitude(vector, clampDistancce);
			Vector2 a = this.data.playerVel.velocity * -drag * TimeHandler.fixedDeltaTime;
			this.TakeForce(force * vector + a * drag * TimeHandler.timeScale, forceMode, forceIgnoreMass, ignoreBlock, 0f);
			this.data.GetComponent<SyncPlayerMovement>().SetDontSyncFor((float)PhotonNetwork.GetPing() * 0.001f + 0.2f);
			yield return new WaitForFixedUpdate();
		}
		yield break;
	}

	// Token: 0x06000224 RID: 548 RVA: 0x0000D774 File Offset: 0x0000B974
	public void TakeForce(Vector2 force, ForceMode2D forceMode = 1, bool forceIgnoreMass = false, bool ignoreBlock = false, float setFlying = 0f)
	{
		if (!this.data.isPlaying)
		{
			return;
		}
		if (!this.data.playerVel.simulated)
		{
			return;
		}
		bool flag = this.data.block.IsBlocking();
		if (flag && !ignoreBlock)
		{
			return;
		}
		if (!flag && setFlying > this.flyingFor && setFlying > 0.25f)
		{
			this.flyingFor = setFlying;
			SoundManager.Instance.Play(this.soundBounce, base.transform);
		}
		if (forceIgnoreMass)
		{
			force *= this.data.playerVel.mass / 100f;
		}
		this.data.playerVel.AddForce(force, forceMode);
		if (force.y > 0f)
		{
			if (!forceIgnoreMass)
			{
				force.y /= this.data.playerVel.mass / 100f;
			}
			if (forceMode == null)
			{
				force *= 0.003f;
			}
			this.data.sinceGrounded -= force.y * 0.003f;
		}
		this.data.sinceGrounded = Mathf.Clamp(this.data.sinceGrounded, -0.5f, 100f);
	}

	// Token: 0x06000225 RID: 549 RVA: 0x0000D8A8 File Offset: 0x0000BAA8
	public override void CallTakeDamage(Vector2 damage, Vector2 position, GameObject damagingWeapon = null, Player damagingPlayer = null, bool lethal = true)
	{
		if (damage == Vector2.zero)
		{
			return;
		}
		if (this.data.block.IsBlocking())
		{
			return;
		}
		this.data.view.RPC("RPCA_SendTakeDamage", 0, new object[]
		{
			damage,
			position,
			lethal,
			(damagingPlayer != null) ? damagingPlayer.playerID : -1
		});
	}

	// Token: 0x06000226 RID: 550 RVA: 0x0000D92C File Offset: 0x0000BB2C
	[PunRPC]
	public void RPCA_SendTakeDamage(Vector2 damage, Vector2 position, bool lethal = true, int playerID = -1)
	{
		if (damage == Vector2.zero)
		{
			return;
		}
		Player playerWithID = PlayerManager.instance.GetPlayerWithID(playerID);
		GameObject damagingWeapon = null;
		if (playerWithID)
		{
			damagingWeapon = playerWithID.data.weaponHandler.gun.gameObject;
		}
		this.TakeDamage(damage, position, damagingWeapon, playerWithID, lethal, true);
	}

	// Token: 0x06000227 RID: 551 RVA: 0x0000D980 File Offset: 0x0000BB80
	public override void TakeDamage(Vector2 damage, Vector2 position, GameObject damagingWeapon = null, Player damagingPlayer = null, bool lethal = true, bool ignoreBlock = false)
	{
		if (damage == Vector2.zero)
		{
			return;
		}
		this.TakeDamage(damage, position, Color.white * 0.85f, damagingWeapon, damagingPlayer, lethal, ignoreBlock);
	}

	// Token: 0x06000228 RID: 552 RVA: 0x0000D9B0 File Offset: 0x0000BBB0
	public override void TakeDamage(Vector2 damage, Vector2 position, Color dmgColor, GameObject damagingWeapon = null, Player damagingPlayer = null, bool lethal = true, bool ignoreBlock = false)
	{
		if (damage == Vector2.zero)
		{
			return;
		}
		if (!this.data.isPlaying)
		{
			return;
		}
		if (!this.data.playerVel.simulated)
		{
			return;
		}
		if (this.data.dead)
		{
			return;
		}
		if (this.data.block.IsBlocking() && !ignoreBlock)
		{
			return;
		}
		if (dmgColor == Color.black)
		{
			dmgColor = Color.white * 0.85f;
		}
		if (this.stats.secondsToTakeDamageOver == 0f)
		{
			this.DoDamage(damage, position, dmgColor, damagingWeapon, damagingPlayer, false, lethal, ignoreBlock);
			return;
		}
		this.TakeDamageOverTime(damage, position, this.stats.secondsToTakeDamageOver, 0.25f, dmgColor, damagingWeapon, damagingPlayer, lethal);
	}

	// Token: 0x06000229 RID: 553 RVA: 0x0000DA74 File Offset: 0x0000BC74
	public void DoDamage(Vector2 damage, Vector2 position, Color blinkColor, GameObject damagingWeapon = null, Player damagingPlayer = null, bool healthRemoval = false, bool lethal = true, bool ignoreBlock = false)
	{
		if (damage == Vector2.zero)
		{
			return;
		}
		if (!this.data.isPlaying)
		{
			return;
		}
		if (this.data.dead)
		{
			return;
		}
		if (this.data.block.IsBlocking() && !ignoreBlock)
		{
			return;
		}
		if (this.isRespawning)
		{
			return;
		}
		if (damagingPlayer)
		{
			damagingPlayer.GetComponent<CharacterStatModifiers>().DealtDamage(damage, damagingPlayer != null && damagingPlayer.transform.root == base.transform, this.data.player);
		}
		base.StopAllCoroutines();
		this.DisplayDamage(blinkColor);
		this.data.lastSourceOfDamage = damagingPlayer;
		this.data.health -= damage.magnitude;
		this.stats.WasDealtDamage(damage, damagingPlayer != null && damagingPlayer.transform.root == base.transform);
		if (!lethal)
		{
			this.data.health = Mathf.Clamp(this.data.health, 1f, this.data.maxHealth);
		}
		if (this.data.health < 0f && !this.data.dead)
		{
			if (this.data.stats.remainingRespawns > 0)
			{
				this.data.view.RPC("RPCA_Die_Phoenix", 0, new object[]
				{
					damage
				});
			}
			else
			{
				this.data.view.RPC("RPCA_Die", 0, new object[]
				{
					damage
				});
			}
		}
		if (this.lastDamaged + 0.15f < Time.time && damagingPlayer != null && damagingPlayer.data.stats.lifeSteal != 0f)
		{
			SoundManager.Instance.Play(this.soundDamageLifeSteal, base.transform);
		}
		this.lastDamaged = Time.time;
	}

	// Token: 0x0600022A RID: 554 RVA: 0x0000DC78 File Offset: 0x0000BE78
	public void TakeDamageOverTime(Vector2 damage, Vector2 position, float time, float interval, Color color, GameObject damagingWeapon = null, Player damagingPlayer = null, bool lethal = true)
	{
		this.dot.TakeDamageOverTime(damage, position, time, interval, color, this.soundDamagePassive, damagingWeapon, damagingPlayer, lethal);
	}

	// Token: 0x0600022B RID: 555 RVA: 0x0000DCA3 File Offset: 0x0000BEA3
	private void DisplayDamage(Color blinkColor)
	{
		base.GetComponentInChildren<PlayerSkinHandler>().BlinkColor(blinkColor);
	}

	// Token: 0x0600022C RID: 556 RVA: 0x0000DCB1 File Offset: 0x0000BEB1
	private IEnumerator DelayReviveAction()
	{
		yield return new WaitForSecondsRealtime(2f);
		Action action = this.delayedReviveAction;
		if (action != null)
		{
			action.Invoke();
		}
		yield break;
	}

	// Token: 0x0600022D RID: 557 RVA: 0x0000DCC0 File Offset: 0x0000BEC0
	public void Revive(bool isFullRevive = true)
	{
		Action action = this.reviveAction;
		if (action != null)
		{
			action.Invoke();
		}
		if (base.gameObject.activeInHierarchy)
		{
			base.StartCoroutine(this.DelayReviveAction());
		}
		this.flyingFor = 0f;
		if (isFullRevive)
		{
			this.data.stats.remainingRespawns = this.data.stats.respawns;
		}
		this.data.healthHandler.isRespawning = false;
		this.data.health = this.data.maxHealth;
		this.data.playerVel.velocity = Vector2.zero;
		this.data.playerVel.angularVelocity = 0f;
		this.data.stunTime = 0f;
		this.data.block.ResetCD(false);
		this.data.weaponHandler.gun.GetComponentInChildren<GunAmmo>().ReloadAmmo(false);
		this.data.GetComponent<PlayerCollision>().IgnoreWallForFrames(5);
		base.gameObject.SetActive(true);
		if (this.deathEffect && this.data.dead)
		{
			this.anim.PlayIn();
		}
		this.data.dead = false;
		this.hpSprite.color = PlayerSkinBank.GetPlayerSkinColors(this.player.playerID).color;
		this.data.stunHandler.StopStun();
		this.data.silenceHandler.StopSilence();
		base.GetComponent<CharacterStatModifiers>().slow = 0f;
		base.GetComponent<CharacterStatModifiers>().slowSlow = 0f;
		base.GetComponent<CharacterStatModifiers>().fastSlow = 0f;
		base.GetComponent<WeaponHandler>().isOverHeated = false;
		base.GetComponent<Block>().sinceBlock = float.PositiveInfinity;
		this.dot.StopAllCoroutines();
	}

	// Token: 0x0600022E RID: 558 RVA: 0x0000DE98 File Offset: 0x0000C098
	[PunRPC]
	private void RPCA_Die(Vector2 deathDirection)
	{
		if (!this.data.isPlaying)
		{
			return;
		}
		if (this.data.dead)
		{
			return;
		}
		SoundManager.Instance.Play(this.soundDie, base.transform);
		this.data.dead = true;
		if (!this.DestroyOnDeath)
		{
			base.gameObject.SetActive(false);
			GamefeelManager.GameFeel(deathDirection.normalized * 3f);
			Object.Instantiate<GameObject>(this.deathEffect, base.transform.position, base.transform.rotation).GetComponent<DeathEffect>().PlayDeath(PlayerSkinBank.GetPlayerSkinColors(this.player.playerID).color, this.data.playerVel, deathDirection, -1);
			this.dot.StopAllCoroutines();
			this.data.stunHandler.StopStun();
			this.data.silenceHandler.StopSilence();
			PlayerManager.instance.PlayerDied(this.player);
			return;
		}
		Object.Destroy(base.transform.root.gameObject);
	}

	// Token: 0x0600022F RID: 559 RVA: 0x0000DFB0 File Offset: 0x0000C1B0
	[PunRPC]
	private void RPCA_Die_Phoenix(Vector2 deathDirection)
	{
		if (!this.data.isPlaying)
		{
			return;
		}
		if (this.data.dead)
		{
			return;
		}
		this.data.stats.remainingRespawns--;
		this.isRespawning = true;
		SoundManager.Instance.Play(this.soundDie, base.transform);
		if (!this.DestroyOnDeath)
		{
			base.gameObject.SetActive(false);
			GamefeelManager.GameFeel(deathDirection.normalized * 3f);
			Object.Instantiate<GameObject>(this.deathEffectPhoenix, base.transform.position, base.transform.rotation).GetComponent<DeathEffect>().PlayDeath(PlayerSkinBank.GetPlayerSkinColors(this.player.playerID).color, this.data.playerVel, deathDirection, this.player.playerID);
			this.dot.StopAllCoroutines();
			this.data.stunHandler.StopStun();
			this.data.silenceHandler.StopSilence();
		}
	}

	// Token: 0x040002FF RID: 767
	[Header("Sounds")]
	public SoundEvent soundDie;

	// Token: 0x04000300 RID: 768
	public SoundEvent soundHeal;

	// Token: 0x04000301 RID: 769
	public SoundEvent soundDamagePassive;

	// Token: 0x04000302 RID: 770
	public SoundEvent soundDamageLifeSteal;

	// Token: 0x04000303 RID: 771
	public SoundEvent soundBounce;

	// Token: 0x04000304 RID: 772
	[Header("Settings")]
	public SpriteRenderer hpSprite;

	// Token: 0x04000305 RID: 773
	public GameObject deathEffect;

	// Token: 0x04000306 RID: 774
	public float regeneration;

	// Token: 0x04000307 RID: 775
	private CharacterData data;

	// Token: 0x04000308 RID: 776
	private CodeAnimation anim;

	// Token: 0x04000309 RID: 777
	private Player player;

	// Token: 0x0400030A RID: 778
	private CharacterStatModifiers stats;

	// Token: 0x0400030B RID: 779
	public ParticleSystem healPart;

	// Token: 0x0400030C RID: 780
	private DamageOverTime dot;

	// Token: 0x0400030D RID: 781
	private Vector3 startHealthSpriteScale;

	// Token: 0x0400030E RID: 782
	public float flyingFor;

	// Token: 0x0400030F RID: 783
	private float lastDamaged;

	// Token: 0x04000310 RID: 784
	public Action delayedReviveAction;

	// Token: 0x04000311 RID: 785
	public Action reviveAction;

	// Token: 0x04000312 RID: 786
	[HideInInspector]
	public bool DestroyOnDeath;

	// Token: 0x04000313 RID: 787
	public bool isRespawning;

	// Token: 0x04000314 RID: 788
	public GameObject deathEffectPhoenix;
}
