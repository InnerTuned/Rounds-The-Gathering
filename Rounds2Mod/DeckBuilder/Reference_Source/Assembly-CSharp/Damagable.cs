using System;
using UnityEngine;

// Token: 0x0200003B RID: 59
public abstract class Damagable : MonoBehaviour
{
	// Token: 0x06000120 RID: 288
	public abstract void CallTakeDamage(Vector2 damage, Vector2 damagePosition, GameObject damagingWeapon = null, Player damagingPlayer = null, bool lethal = true);

	// Token: 0x06000121 RID: 289
	public abstract void TakeDamage(Vector2 damage, Vector2 damagePosition, GameObject damagingWeapon = null, Player damagingPlayer = null, bool lethal = true, bool ignoreBlock = false);

	// Token: 0x06000122 RID: 290
	public abstract void TakeDamage(Vector2 damage, Vector2 damagePosition, Color dmgColor, GameObject damagingWeapon = null, Player damagingPlayer = null, bool lethal = true, bool ignoreBlock = false);
}
