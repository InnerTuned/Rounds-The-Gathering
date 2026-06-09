using System;
using Sonigon;
using UnityEngine;

// Token: 0x0200003D RID: 61
public class DamageBox : MonoBehaviour
{
	// Token: 0x0600012D RID: 301 RVA: 0x00008410 File Offset: 0x00006610
	private void Start()
	{
		this.spawned = base.GetComponentInParent<SpawnedAttack>();
	}

	// Token: 0x0600012E RID: 302 RVA: 0x0000841E File Offset: 0x0000661E
	private void OnCollisionEnter2D(Collision2D collision)
	{
		this.Collide(collision);
	}

	// Token: 0x0600012F RID: 303 RVA: 0x0000841E File Offset: 0x0000661E
	private void OnCollisionStay2D(Collision2D collision)
	{
		this.Collide(collision);
	}

	// Token: 0x06000130 RID: 304 RVA: 0x00008428 File Offset: 0x00006628
	private void Collide(Collision2D collision)
	{
		if (Time.time < this.time + this.cd)
		{
			return;
		}
		Vector3 vector = base.transform.root.forward;
		if (this.towardsCenterOfMap)
		{
			vector = -collision.contacts[0].point.normalized;
		}
		if (this.awayFromMe)
		{
			vector = (collision.transform.position - base.transform.position).normalized;
		}
		Damagable componentInParent = collision.transform.GetComponentInParent<Damagable>();
		if (componentInParent)
		{
			this.time = Time.time;
			HealthHandler component = componentInParent.GetComponent<HealthHandler>();
			CharacterData component2 = component.GetComponent<CharacterData>();
			if (component2 && !component2.view.IsMine)
			{
				return;
			}
			if (component)
			{
				component.CallTakeForce(vector * this.force, 1, false, this.ignoreBlock, this.setFlyingFor);
			}
			componentInParent.CallTakeDamage(this.damage * vector, base.transform.position, null, (this.spawned != null) ? this.spawned.spawner : null, true);
			if (this.soundPlaySawDamage)
			{
				SoundManager.Instance.PlayAtPosition(this.soundSawDamage, SoundManager.Instance.GetTransform(), base.transform);
			}
			if (this.dmgPart)
			{
				Vector3 forward = vector;
				vector.z = 0f;
				this.dmgPart.transform.parent.rotation = Quaternion.LookRotation(forward);
				this.dmgPart.Play();
			}
			if (this.shake != 0f)
			{
				component2.player.Call_AllGameFeel(this.shake * vector);
			}
		}
	}

	// Token: 0x04000193 RID: 403
	[Header("Sound")]
	public bool soundPlaySawDamage;

	// Token: 0x04000194 RID: 404
	public SoundEvent soundSawDamage;

	// Token: 0x04000195 RID: 405
	[Header("Settings")]
	public bool towardsCenterOfMap;

	// Token: 0x04000196 RID: 406
	public bool awayFromMe;

	// Token: 0x04000197 RID: 407
	public float damage = 25f;

	// Token: 0x04000198 RID: 408
	public float force;

	// Token: 0x04000199 RID: 409
	public float setFlyingFor;

	// Token: 0x0400019A RID: 410
	public float shake;

	// Token: 0x0400019B RID: 411
	public float cd = 0.3f;

	// Token: 0x0400019C RID: 412
	public bool ignoreBlock;

	// Token: 0x0400019D RID: 413
	public ParticleSystem dmgPart;

	// Token: 0x0400019E RID: 414
	private float time;

	// Token: 0x0400019F RID: 415
	private SpawnedAttack spawned;
}
