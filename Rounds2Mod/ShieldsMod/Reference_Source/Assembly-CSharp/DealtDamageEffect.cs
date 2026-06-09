using System;
using UnityEngine;

// Token: 0x02000042 RID: 66
public abstract class DealtDamageEffect : MonoBehaviour
{
	// Token: 0x06000140 RID: 320
	public abstract void DealtDamage(Vector2 damage, bool selfDamage, Player damagedPlayer = null);
}
