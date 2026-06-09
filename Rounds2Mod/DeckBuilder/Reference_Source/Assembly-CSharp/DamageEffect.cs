using System;
using UnityEngine;

// Token: 0x02000128 RID: 296
public abstract class DamageEffect : MonoBehaviour
{
	// Token: 0x060005C4 RID: 1476
	public abstract void DoDamageEffect(Vector2 dmg, bool selfDmg, Player damagedPlayer = null);
}
