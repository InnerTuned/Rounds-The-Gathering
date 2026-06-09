using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000129 RID: 297
public class DealtDamageTrigger : MonoBehaviour
{
	// Token: 0x060005C6 RID: 1478 RVA: 0x00020BA4 File Offset: 0x0001EDA4
	private void Start()
	{
		CharacterStatModifiers stats = base.GetComponentInParent<Player>().data.stats;
		stats.DealtDamageAction = (Action<Vector2, bool>)Delegate.Combine(stats.DealtDamageAction, new Action<Vector2, bool>(this.DealtDamage));
	}

	// Token: 0x060005C7 RID: 1479 RVA: 0x00020BD7 File Offset: 0x0001EDD7
	private void DealtDamage(Vector2 dmg, bool lethal)
	{
		this.triggerEvent.Invoke();
	}

	// Token: 0x04000769 RID: 1897
	public UnityEvent triggerEvent;
}
