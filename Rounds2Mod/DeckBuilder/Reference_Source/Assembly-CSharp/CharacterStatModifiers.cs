using System;
using System.Collections.Generic;
using Photon.Pun;
using Sonigon;
using UnityEngine;

// Token: 0x0200002C RID: 44
public class CharacterStatModifiers : MonoBehaviour
{
	// Token: 0x060000D5 RID: 213 RVA: 0x00006A71 File Offset: 0x00004C71
	public bool SoundTransformScaleThresholdReached()
	{
		return base.transform.localScale.x > this.soundBigThreshold;
	}

	// Token: 0x060000D6 RID: 214 RVA: 0x00006A8E File Offset: 0x00004C8E
	private void Start()
	{
		this.data = base.GetComponent<CharacterData>();
	}

	// Token: 0x060000D7 RID: 215 RVA: 0x00006A9C File Offset: 0x00004C9C
	public float GetSlow()
	{
		return Mathf.Clamp(this.slow, 0f, 0.9f);
	}

	// Token: 0x060000D8 RID: 216 RVA: 0x00006AB4 File Offset: 0x00004CB4
	private void Update()
	{
		this.attackSpeedMultiplier = this.tasteOfBloodSpeed * this.rageSpeed;
		this.sinceDealtDamage += TimeHandler.deltaTime;
		if (this.data && !this.data.isPlaying)
		{
			this.sinceDealtDamage = 100f;
		}
		this.slow = this.slowSlow;
		if (this.fastSlow > this.slowSlow)
		{
			this.slow = this.fastSlow;
		}
		if (this.slowSlow > 0f)
		{
			this.slowSlow = Mathf.Clamp(this.slowSlow - TimeHandler.deltaTime * 0.3f, 0f, 10f);
		}
		if (this.fastSlow > 0f)
		{
			this.fastSlow = Mathf.Clamp(this.fastSlow - TimeHandler.deltaTime * 2f, 0f, 1f);
		}
	}

	// Token: 0x060000D9 RID: 217 RVA: 0x00006B9C File Offset: 0x00004D9C
	public void DealtDamage(Vector2 damage, bool selfDamage, Player damagedPlayer = null)
	{
		if (this.lifeSteal != 0f && !selfDamage)
		{
			base.GetComponent<HealthHandler>().Heal(damage.magnitude * this.lifeSteal);
		}
		if (this.refreshOnDamage)
		{
			base.GetComponent<Holding>().holdable.GetComponent<Weapon>().sinceAttack = float.PositiveInfinity;
		}
		if (this.DealtDamageAction != null)
		{
			this.DealtDamageAction.Invoke(damage, selfDamage);
		}
		if (damagedPlayer)
		{
			this.data.lastDamagedPlayer = damagedPlayer;
		}
		if (!selfDamage)
		{
			this.sinceDealtDamage = 0f;
		}
		if (this.dealtDamageEffects != null)
		{
			for (int i = 0; i < this.dealtDamageEffects.Length; i++)
			{
				this.dealtDamageEffects[i].DealtDamage(damage, selfDamage, damagedPlayer);
			}
		}
	}

	// Token: 0x060000DA RID: 218 RVA: 0x00006C58 File Offset: 0x00004E58
	internal void ResetStats()
	{
		for (int i = 0; i < this.objectsAddedToPlayer.Count; i++)
		{
			Object.Destroy(this.objectsAddedToPlayer[i]);
		}
		this.objectsAddedToPlayer.Clear();
		this.data.health = 100f;
		this.data.maxHealth = 100f;
		this.sizeMultiplier = 1f;
		this.health = 1f;
		this.movementSpeed = 1f;
		this.jump = 1f;
		this.gravity = 1f;
		this.slow = 0f;
		this.slowSlow = 0f;
		this.fastSlow = 0f;
		this.secondsToTakeDamageOver = 0f;
		this.numberOfJumps = 0;
		this.regen = 0f;
		this.lifeSteal = 0f;
		this.respawns = 0;
		this.refreshOnDamage = false;
		this.automaticReload = true;
		this.tasteOfBloodSpeed = 1f;
		this.rageSpeed = 1f;
		this.attackSpeedMultiplier = 1f;
		this.WasUpdated();
		this.ConfigureMassAndSize();
	}

	// Token: 0x060000DB RID: 219 RVA: 0x00006D7C File Offset: 0x00004F7C
	public void WasDealtDamage(Vector2 damage, bool selfDamage)
	{
		if (this.WasDealtDamageAction != null)
		{
			this.WasDealtDamageAction.Invoke(damage, selfDamage);
		}
		if (this.wasDealtDamageEffects != null)
		{
			for (int i = 0; i < this.wasDealtDamageEffects.Length; i++)
			{
				this.wasDealtDamageEffects[i].WasDealtDamage(damage, selfDamage);
			}
		}
	}

	// Token: 0x060000DC RID: 220 RVA: 0x00006DC8 File Offset: 0x00004FC8
	public void WasUpdated()
	{
		base.GetComponent<ForceMultiplier>().multiplier = 1f / base.transform.root.localScale.x;
		this.wasDealtDamageEffects = base.GetComponentsInChildren<WasDealtDamageEffect>();
		this.dealtDamageEffects = base.GetComponentsInChildren<DealtDamageEffect>();
	}

	// Token: 0x060000DD RID: 221 RVA: 0x00006E08 File Offset: 0x00005008
	public void AddSlowAddative(float slowToAdd, float maxValue = 1f, bool isFastSlow = false)
	{
		if (this.data.block.IsBlocking())
		{
			return;
		}
		this.DoSlowDown(slowToAdd);
		if (isFastSlow)
		{
			if (this.fastSlow < maxValue)
			{
				this.fastSlow += slowToAdd;
				this.fastSlow = Mathf.Clamp(this.slow, 0f, maxValue);
				return;
			}
		}
		else if (this.slowSlow < maxValue)
		{
			this.slowSlow += slowToAdd;
			this.slowSlow = Mathf.Clamp(this.slowSlow, 0f, maxValue);
		}
	}

	// Token: 0x060000DE RID: 222 RVA: 0x00006E8F File Offset: 0x0000508F
	[PunRPC]
	public void RPCA_AddSlow(float slowToAdd, bool isFastSlow = false)
	{
		this.DoSlowDown(slowToAdd);
		if (isFastSlow)
		{
			this.fastSlow = Mathf.Clamp(this.fastSlow, slowToAdd, 1f);
			return;
		}
		this.slowSlow = Mathf.Clamp(this.slowSlow, slowToAdd, 10f);
	}

	// Token: 0x060000DF RID: 223 RVA: 0x00006ECC File Offset: 0x000050CC
	private void DoSlowDown(float newSlow)
	{
		if (this.soundSlowTime + this.soundSlowSpeedSec < Time.time)
		{
			this.soundSlowTime = Time.time;
			SoundManager.Instance.Play(this.soundCharacterSlowFreeze, base.transform);
		}
		float num = Mathf.Clamp(newSlow - this.slow, 0f, 1f);
		this.slowPart.Emit((int)Mathf.Clamp((newSlow * 0.1f + num * 0.7f) * 50f, 1f, 50f));
		this.data.playerVel.velocity *= 1f - num * 1f;
		this.data.sinceGrounded *= 1f - num * 1f;
	}

	// Token: 0x060000E0 RID: 224 RVA: 0x00006FA0 File Offset: 0x000051A0
	internal void ConfigureMassAndSize()
	{
		base.transform.localScale = Vector3.one * 1.2f * Mathf.Pow(this.data.maxHealth / 100f * 1.2f, 0.2f) * this.sizeMultiplier;
		this.data.playerVel.mass = 100f * Mathf.Pow(this.data.maxHealth / 100f * 1.2f, 0.8f) * this.sizeMultiplier;
	}

	// Token: 0x060000E1 RID: 225 RVA: 0x00007036 File Offset: 0x00005236
	internal void OnReload(int bulletsReloaded)
	{
		if (this.OnReloadDoneAction != null)
		{
			this.OnReloadDoneAction.Invoke(bulletsReloaded);
		}
	}

	// Token: 0x060000E2 RID: 226 RVA: 0x0000704C File Offset: 0x0000524C
	internal void OnOutOfAmmp(int maxAmmo)
	{
		if (this.OutOfAmmpAction != null)
		{
			this.OutOfAmmpAction.Invoke(maxAmmo);
		}
	}

	// Token: 0x04000101 RID: 257
	[Header("Sounds")]
	public SoundEvent soundCharacterSlowFreeze;

	// Token: 0x04000102 RID: 258
	private float soundSlowTime;

	// Token: 0x04000103 RID: 259
	private float soundSlowSpeedSec = 0.3f;

	// Token: 0x04000104 RID: 260
	[Header("Settings")]
	public GameObject AddObjectToPlayer;

	// Token: 0x04000105 RID: 261
	[HideInInspector]
	public List<GameObject> objectsAddedToPlayer;

	// Token: 0x04000106 RID: 262
	[Header("Multiply")]
	public float sizeMultiplier = 1f;

	// Token: 0x04000107 RID: 263
	public float health = 1f;

	// Token: 0x04000108 RID: 264
	public float movementSpeed = 1f;

	// Token: 0x04000109 RID: 265
	public float jump = 1f;

	// Token: 0x0400010A RID: 266
	public float gravity = 1f;

	// Token: 0x0400010B RID: 267
	public float slow;

	// Token: 0x0400010C RID: 268
	public float slowSlow;

	// Token: 0x0400010D RID: 269
	public float fastSlow;

	// Token: 0x0400010E RID: 270
	[Header("Add")]
	public float secondsToTakeDamageOver;

	// Token: 0x0400010F RID: 271
	public int numberOfJumps;

	// Token: 0x04000110 RID: 272
	public float regen;

	// Token: 0x04000111 RID: 273
	public float lifeSteal;

	// Token: 0x04000112 RID: 274
	public bool refreshOnDamage;

	// Token: 0x04000113 RID: 275
	public bool automaticReload = true;

	// Token: 0x04000114 RID: 276
	public int respawns;

	// Token: 0x04000115 RID: 277
	[HideInInspector]
	public int remainingRespawns;

	// Token: 0x04000116 RID: 278
	[HideInInspector]
	public float tasteOfBloodSpeed = 1f;

	// Token: 0x04000117 RID: 279
	[HideInInspector]
	public float rageSpeed = 1f;

	// Token: 0x04000118 RID: 280
	public float attackSpeedMultiplier = 1f;

	// Token: 0x04000119 RID: 281
	private WasDealtDamageEffect[] wasDealtDamageEffects;

	// Token: 0x0400011A RID: 282
	private DealtDamageEffect[] dealtDamageEffects;

	// Token: 0x0400011B RID: 283
	private CharacterData data;

	// Token: 0x0400011C RID: 284
	public ParticleSystem slowPart;

	// Token: 0x0400011D RID: 285
	private float soundBigThreshold = 1.5f;

	// Token: 0x0400011E RID: 286
	public Action<Vector2, bool> DealtDamageAction;

	// Token: 0x0400011F RID: 287
	public Action<Vector2, bool> WasDealtDamageAction;

	// Token: 0x04000120 RID: 288
	public Action<int> OnReloadDoneAction;

	// Token: 0x04000121 RID: 289
	public Action<int> OutOfAmmpAction;

	// Token: 0x04000122 RID: 290
	internal float sinceDealtDamage;
}
