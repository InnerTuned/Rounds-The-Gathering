using System;
using System.Collections;
using Sonigon;
using UnityEngine;

// Token: 0x0200003E RID: 62
public class DamageOverTime : MonoBehaviour
{
	// Token: 0x06000132 RID: 306 RVA: 0x00008625 File Offset: 0x00006825
	private void Start()
	{
		this.health = base.GetComponent<HealthHandler>();
		this.data = base.GetComponent<CharacterData>();
	}

	// Token: 0x06000133 RID: 307 RVA: 0x00008640 File Offset: 0x00006840
	public void TakeDamageOverTime(Vector2 damage, Vector2 position, float time, float interval, Color color, SoundEvent soundDamageOverTime, GameObject damagingWeapon = null, Player damagingPlayer = null, bool lethal = true)
	{
		base.StartCoroutine(this.DoDamageOverTime(damage, position, time, interval, color, soundDamageOverTime, damagingWeapon, damagingPlayer, lethal));
	}

	// Token: 0x06000134 RID: 308 RVA: 0x0000866C File Offset: 0x0000686C
	private IEnumerator DoDamageOverTime(Vector2 damage, Vector2 position, float time, float interval, Color color, SoundEvent soundDamageOverTime, GameObject damagingWeapon = null, Player damagingPlayer = null, bool lethal = true)
	{
		float damageDealt = 0f;
		float damageToDeal = damage.magnitude;
		float dpt = damageToDeal / time * interval;
		while (damageDealt < damageToDeal)
		{
			if (soundDamageOverTime != null && this.data.isPlaying && !this.data.dead)
			{
				SoundManager.Instance.Play(soundDamageOverTime, base.transform);
			}
			damageDealt += dpt;
			this.health.DoDamage(damage.normalized * dpt, position, color, damagingWeapon, damagingPlayer, true, lethal, false);
			yield return new WaitForSeconds(interval / TimeHandler.timeScale);
		}
		yield break;
	}

	// Token: 0x040001A0 RID: 416
	private HealthHandler health;

	// Token: 0x040001A1 RID: 417
	private CharacterData data;
}
