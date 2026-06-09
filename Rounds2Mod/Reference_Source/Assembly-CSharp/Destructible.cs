using System;
using UnityEngine;

// Token: 0x02000048 RID: 72
public class Destructible : Damagable
{
	// Token: 0x0600015E RID: 350 RVA: 0x00008DB5 File Offset: 0x00006FB5
	public override void CallTakeDamage(Vector2 damage, Vector2 damagePosition, GameObject damagingWeapon = null, Player damagingPlayer = null, bool lethal = true)
	{
		throw new NotImplementedException();
	}

	// Token: 0x0600015F RID: 351 RVA: 0x00008DBC File Offset: 0x00006FBC
	public override void TakeDamage(Vector2 damage, Vector2 position, GameObject damagingWeapon = null, Player damagingPlayer = null, bool lethal = true, bool ignoreBlock = false)
	{
		if (damage.magnitude < this.threshold)
		{
			return;
		}
		damage = damage.normalized * 100f;
		Transform[] array = new Transform[base.transform.childCount];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = base.transform.GetChild(i);
		}
		foreach (Transform transform in array)
		{
			transform.gameObject.SetActive(true);
			transform.SetParent(null, true);
			float num = damage.magnitude * 0.02f * this.rangeMulti;
			float d = Mathf.Clamp((num - Vector2.Distance(transform.position, position)) / num, 0f, 1f);
			Rigidbody2D component = transform.GetComponent<Rigidbody2D>();
			component.AddForce(damage * d * this.force * 0.1f * component.mass, 1);
			transform.gameObject.AddComponent<RemoveAfterSeconds>().seconds = Random.Range(1f, 3f);
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x06000160 RID: 352 RVA: 0x00008EE8 File Offset: 0x000070E8
	public override void TakeDamage(Vector2 damage, Vector2 damagePosition, Color dmgColor, GameObject damagingWeapon = null, Player damagingPlayer = null, bool lethal = true, bool ignoreBlock = false)
	{
		this.TakeDamage(damage, damagePosition, dmgColor, damagingWeapon, damagingPlayer, lethal, false);
	}

	// Token: 0x040001CC RID: 460
	public float threshold = 25f;

	// Token: 0x040001CD RID: 461
	public float force = 1f;

	// Token: 0x040001CE RID: 462
	public float rangeMulti = 1f;
}
