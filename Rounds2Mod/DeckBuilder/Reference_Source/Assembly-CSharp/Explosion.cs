using System;
using Photon.Pun;
using Sirenix.OdinInspector;
using Sonigon;
using UnityEngine;

// Token: 0x02000053 RID: 83
public class Explosion : MonoBehaviour
{
	// Token: 0x06000189 RID: 393 RVA: 0x00009EF7 File Offset: 0x000080F7
	private void Start()
	{
		this.spawned = base.GetComponent<SpawnedAttack>();
		this.view = base.GetComponent<PhotonView>();
		if (this.auto)
		{
			this.Explode();
		}
	}

	// Token: 0x0600018A RID: 394 RVA: 0x00009F20 File Offset: 0x00008120
	private void DoExplosionEffects(Collider2D hitCol, float rangeMultiplier, float distance)
	{
		float num = this.scaleDmg ? base.transform.localScale.x : 1f;
		float d = this.scaleForce ? base.transform.localScale.x : 1f;
		float num2 = this.scaleSlow ? base.transform.localScale.x : 1f;
		float num3 = this.scaleSilence ? base.transform.localScale.x : 1f;
		float num4 = this.scaleStun ? ((1f + base.transform.localScale.x) * 0.5f) : 1f;
		Damagable componentInParent = hitCol.gameObject.GetComponentInParent<Damagable>();
		CharacterData characterData = null;
		if (componentInParent)
		{
			characterData = hitCol.gameObject.GetComponentInParent<CharacterData>();
			if (this.immunity > 0f && characterData && characterData.GetComponent<PlayerImmunity>().IsImune(this.immunity, num * this.damage * rangeMultiplier, base.gameObject.name))
			{
				return;
			}
			if (!this.ignoreWalls && characterData && !PlayerManager.instance.CanSeePlayer(base.transform.position, characterData.player).canSee)
			{
				return;
			}
			if (this.slow != 0f && componentInParent.GetComponent<CharacterStatModifiers>())
			{
				if (this.locallySimulated)
				{
					if (this.spawned.IsMine() && !characterData.block.IsBlocking())
					{
						characterData.stats.RPCA_AddSlow(this.slow * rangeMultiplier * num2, this.fastSlow);
					}
				}
				else if (this.spawned.IsMine() && !characterData.block.IsBlocking())
				{
					characterData.view.RPC("RPCA_AddSlow", 0, new object[]
					{
						this.slow * rangeMultiplier * num2,
						this.fastSlow
					});
				}
			}
			if (this.silence != 0f && componentInParent.GetComponent<SilenceHandler>() && this.spawned.IsMine() && !characterData.block.IsBlocking())
			{
				characterData.view.RPC("RPCA_AddSilence", 0, new object[]
				{
					this.silence * rangeMultiplier * num3
				});
			}
			if (this.spawned)
			{
				Player spawner = this.spawned.spawner;
			}
			Action<CharacterData, float> action = this.hitPlayerAction;
			if (action != null)
			{
				action.Invoke(characterData, rangeMultiplier);
			}
			if (this.damage < 0f)
			{
				if (characterData)
				{
					characterData.healthHandler.Heal(-this.damage);
				}
				if (this.DealHealAction != null)
				{
					this.DealHealAction.Invoke(componentInParent);
				}
			}
			else if (this.damage > 0f)
			{
				if (this.soundDamage != null && characterData != null)
				{
					SoundManager.Instance.Play(this.soundDamage, characterData.transform);
				}
				Vector2 vector = (hitCol.bounds.ClosestPoint(base.transform.position) - base.transform.position).normalized;
				if (vector == Vector2.zero)
				{
					vector = Vector2.up;
				}
				if (this.spawned.IsMine())
				{
					componentInParent.CallTakeDamage(num * this.damage * rangeMultiplier * vector, base.transform.position, null, this.spawned.spawner, true);
				}
				if (this.DealDamageAction != null)
				{
					this.DealDamageAction.Invoke(componentInParent);
				}
			}
		}
		if (characterData)
		{
			if (this.HitTargetAction != null)
			{
				this.HitTargetAction.Invoke(componentInParent, distance);
			}
			if (this.force != 0f)
			{
				if (this.locallySimulated)
				{
					characterData.healthHandler.TakeForce((hitCol.bounds.ClosestPoint(base.transform.position) - base.transform.position).normalized * rangeMultiplier * this.force * d, 1, this.forceIgnoreMass, false, 0f);
				}
				else if (this.spawned.IsMine())
				{
					characterData.healthHandler.CallTakeForce((hitCol.bounds.ClosestPoint(base.transform.position) - base.transform.position).normalized * rangeMultiplier * this.force * d, 1, this.forceIgnoreMass, false, this.flyingFor * rangeMultiplier);
				}
			}
			if (this.stun > 0f)
			{
				characterData.stunHandler.AddStun(this.stun * num4);
				return;
			}
		}
		else if (hitCol.attachedRigidbody)
		{
			hitCol.attachedRigidbody.AddForce((hitCol.bounds.ClosestPoint(base.transform.position) - base.transform.position).normalized * rangeMultiplier * this.force * d, 1);
		}
	}

	// Token: 0x0600018B RID: 395 RVA: 0x0000A4BC File Offset: 0x000086BC
	public void Explode()
	{
		float num = this.scaleRadius ? base.transform.localScale.x : 1f;
		Collider2D[] array = Physics2D.OverlapCircleAll(base.transform.position, this.range * num);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].gameObject.layer != 19)
			{
				Object componentInParent = array[i].gameObject.GetComponentInParent<Damagable>();
				float num2 = Vector2.Distance(base.transform.position, array[i].bounds.ClosestPoint(base.transform.position));
				float num3 = 1f - num2 / (this.range * num);
				if (this.staticRangeMultiplier)
				{
					num3 = 1f;
				}
				num3 = Mathf.Clamp(num3, 0f, 1f);
				NetworkPhysicsObject component = array[i].GetComponent<NetworkPhysicsObject>();
				if (component && component.photonView.IsMine)
				{
					float d = this.scaleForce ? base.transform.localScale.x : 1f;
					component.BulletPush((component.transform.position - base.transform.position).normalized * this.objectForceMultiplier * 1f * num3 * this.force * d, Vector2.zero, null);
				}
				if ((componentInParent || array[i].attachedRigidbody) && (!this.ignoreTeam || !this.spawned || !(this.spawned.spawner.gameObject == array[i].transform.gameObject)))
				{
					this.DoExplosionEffects(array[i], num3, num2);
				}
			}
		}
	}

	// Token: 0x0400020A RID: 522
	[Header("Sounds")]
	public SoundEvent soundDamage;

	// Token: 0x0400020B RID: 523
	[Header("Settings")]
	public float slow;

	// Token: 0x0400020C RID: 524
	public float silence;

	// Token: 0x0400020D RID: 525
	public bool fastSlow;

	// Token: 0x0400020E RID: 526
	public float stun;

	// Token: 0x0400020F RID: 527
	public float force = 2000f;

	// Token: 0x04000210 RID: 528
	public float objectForceMultiplier = 1f;

	// Token: 0x04000211 RID: 529
	public bool forceIgnoreMass;

	// Token: 0x04000212 RID: 530
	public float damage = 25f;

	// Token: 0x04000213 RID: 531
	public Color dmgColor = Color.black;

	// Token: 0x04000214 RID: 532
	public float range = 2f;

	// Token: 0x04000215 RID: 533
	public float flyingFor;

	// Token: 0x04000216 RID: 534
	public bool auto = true;

	// Token: 0x04000217 RID: 535
	public bool ignoreTeam;

	// Token: 0x04000218 RID: 536
	public bool ignoreWalls;

	// Token: 0x04000219 RID: 537
	public bool staticRangeMultiplier;

	// Token: 0x0400021A RID: 538
	[FoldoutGroup("Scaling", 0)]
	public bool scaleSlow = true;

	// Token: 0x0400021B RID: 539
	[FoldoutGroup("Scaling", 0)]
	public bool scaleSilence = true;

	// Token: 0x0400021C RID: 540
	[FoldoutGroup("Scaling", 0)]
	public bool scaleDmg = true;

	// Token: 0x0400021D RID: 541
	[FoldoutGroup("Scaling", 0)]
	public bool scaleRadius = true;

	// Token: 0x0400021E RID: 542
	[FoldoutGroup("Scaling", 0)]
	public bool scaleStun = true;

	// Token: 0x0400021F RID: 543
	[FoldoutGroup("Scaling", 0)]
	public bool scaleForce = true;

	// Token: 0x04000220 RID: 544
	[FoldoutGroup("Immunity", 0)]
	public float immunity;

	// Token: 0x04000221 RID: 545
	private SpawnedAttack spawned;

	// Token: 0x04000222 RID: 546
	public bool locallySimulated;

	// Token: 0x04000223 RID: 547
	public Action<Damagable> DealDamageAction;

	// Token: 0x04000224 RID: 548
	public Action<Damagable> DealHealAction;

	// Token: 0x04000225 RID: 549
	public Action<Damagable, float> HitTargetAction;

	// Token: 0x04000226 RID: 550
	private PhotonView view;

	// Token: 0x04000227 RID: 551
	public Action<CharacterData, float> hitPlayerAction;
}
