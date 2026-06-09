using System;
using UnityEngine;

// Token: 0x0200013B RID: 315
public class Explosion_Overpower : MonoBehaviour
{
	// Token: 0x060005F7 RID: 1527 RVA: 0x000212B7 File Offset: 0x0001F4B7
	private void Awake()
	{
		Explosion component = base.GetComponent<Explosion>();
		component.hitPlayerAction = (Action<CharacterData, float>)Delegate.Combine(component.hitPlayerAction, new Action<CharacterData, float>(this.HitPlayer));
	}

	// Token: 0x060005F8 RID: 1528 RVA: 0x000212E0 File Offset: 0x0001F4E0
	private void HitPlayer(CharacterData data, float rangeMultiplier)
	{
		SpawnedAttack component = base.GetComponent<SpawnedAttack>();
		if (!component.IsMine())
		{
			return;
		}
		float d = component.spawner.data.maxHealth * this.dmgPer100Hp * 0.01f * base.transform.localScale.x;
		data.healthHandler.CallTakeDamage(d * (data.transform.position - component.spawner.transform.position).normalized, base.transform.position, null, base.GetComponent<SpawnedAttack>().spawner, true);
	}

	// Token: 0x04000781 RID: 1921
	public float dmgPer100Hp;
}
