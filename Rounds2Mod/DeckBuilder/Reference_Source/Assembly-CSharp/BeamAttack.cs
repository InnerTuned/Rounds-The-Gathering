using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000011 RID: 17
public class BeamAttack : MonoBehaviour
{
	// Token: 0x06000053 RID: 83 RVA: 0x00004104 File Offset: 0x00002304
	private void Start()
	{
		this.lineEffects = base.GetComponentsInChildren<LineEffect>(true);
		this.parts = base.GetComponentsInChildren<ParticleSystem>();
		this.thisPlayer = base.GetComponentInParent<Player>();
		this.stats = this.thisPlayer.GetComponent<CharacterStatModifiers>();
		this.attacker = PlayerManager.instance.GetOtherPlayer(this.thisPlayer);
		this.scaleMultiplier = base.transform.localScale.x;
		this.spawnedAttack = base.GetComponentInParent<SpawnedAttack>();
		if (this.thisPlayer == this.spawnedAttack.spawner)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000054 RID: 84 RVA: 0x000041A4 File Offset: 0x000023A4
	private void Update()
	{
		if (!this.attacker || !this.thisPlayer)
		{
			return;
		}
		this.counter += TimeHandler.deltaTime;
		if (this.counter > this.interval)
		{
			CanSeeInfo canSeeInfo = PlayerManager.instance.CanSeePlayer(this.attacker.transform.position, this.thisPlayer);
			if (canSeeInfo.canSee)
			{
				Vector2 a = this.thisPlayer.transform.position - this.attacker.transform.position;
				Vector2 normalized = a.normalized;
				if (this.force != 0f)
				{
					this.thisPlayer.data.healthHandler.TakeForce(normalized * this.scaleMultiplier * this.force, 1, false, false, 0f);
				}
				if (this.scalingForce != 0f)
				{
					this.thisPlayer.data.healthHandler.TakeForce(a * this.scaleMultiplier * this.scalingForce, 1, false, false, 0f);
				}
				if (this.damage != 0f)
				{
					this.thisPlayer.data.healthHandler.TakeDamage(this.damage * this.scaleMultiplier * normalized, base.transform.position, this.dmgColor, null, this.attacker, true, false);
				}
				if (this.selfHeal != 0f)
				{
					this.attacker.data.healthHandler.Heal(this.selfHeal * this.scaleMultiplier);
				}
				for (int i = 0; i < this.lineEffects.Length; i++)
				{
					this.lineEffects[i].Play(this.attacker.transform, this.thisPlayer.transform, 0f);
				}
				base.StartCoroutine(this.DoOverTimeEffects(this.attacker));
				if (this.slow > 0f && this.stats)
				{
					this.stats.AddSlowAddative(this.slow * this.scaleMultiplier, this.maxSlow, false);
				}
			}
			else
			{
				for (int j = 0; j < this.lineEffects.Length; j++)
				{
					this.lineEffects[j].Play(this.attacker.transform, canSeeInfo.hitPoint, 0f);
				}
				for (int k = 0; k < this.parts.Length; k++)
				{
					this.parts[k].transform.position = canSeeInfo.hitPoint;
					this.parts[k].transform.localScale = Vector3.one * this.scaleMultiplier;
					this.parts[k].Play();
				}
			}
			this.counter = 0f;
		}
	}

	// Token: 0x06000055 RID: 85 RVA: 0x0000449B File Offset: 0x0000269B
	private IEnumerator DoOverTimeEffects(Player attacker)
	{
		float c = 0f;
		while (c < this.effectOverTimeTime)
		{
			c += TimeHandler.deltaTime;
			if (attacker && this.thisPlayer)
			{
				Vector2 a = this.thisPlayer.transform.position - attacker.transform.position;
				Vector2 normalized = a.normalized;
				if (this.overTimeForce != 0f)
				{
					this.thisPlayer.data.healthHandler.TakeForce(normalized * this.scaleMultiplier * TimeHandler.deltaTime * this.overTimeForce, 1, false, false, 0f);
				}
				if (this.overTimeScalingForce != 0f)
				{
					this.thisPlayer.data.healthHandler.TakeForce(a * this.scaleMultiplier * TimeHandler.deltaTime * this.overTimeScalingForce, 0, false, false, 0f);
				}
				if (this.overTimeDrag > 0f)
				{
					this.thisPlayer.data.playerVel.AddForce(-this.thisPlayer.data.playerVel.velocity * Mathf.Clamp(TimeHandler.deltaTime * this.scaleMultiplier * this.overTimeDrag, 0f, 0.95f), 0);
				}
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x0400003B RID: 59
	public float selfHeal;

	// Token: 0x0400003C RID: 60
	public float damage = 8f;

	// Token: 0x0400003D RID: 61
	public float force = 2500f;

	// Token: 0x0400003E RID: 62
	public float scalingForce;

	// Token: 0x0400003F RID: 63
	public float overTimeForce;

	// Token: 0x04000040 RID: 64
	public float overTimeScalingForce;

	// Token: 0x04000041 RID: 65
	public float overTimeDrag;

	// Token: 0x04000042 RID: 66
	public float effectOverTimeTime = 0.1f;

	// Token: 0x04000043 RID: 67
	public float interval = 0.2f;

	// Token: 0x04000044 RID: 68
	public float slow;

	// Token: 0x04000045 RID: 69
	public float maxSlow = 1f;

	// Token: 0x04000046 RID: 70
	public Color dmgColor;

	// Token: 0x04000047 RID: 71
	private Player attacker;

	// Token: 0x04000048 RID: 72
	private Player thisPlayer;

	// Token: 0x04000049 RID: 73
	private LineEffect[] lineEffects;

	// Token: 0x0400004A RID: 74
	private ParticleSystem[] parts;

	// Token: 0x0400004B RID: 75
	private CharacterStatModifiers stats;

	// Token: 0x0400004C RID: 76
	private float scaleMultiplier = 1f;

	// Token: 0x0400004D RID: 77
	private SpawnedAttack spawnedAttack;

	// Token: 0x0400004E RID: 78
	private float counter;
}
