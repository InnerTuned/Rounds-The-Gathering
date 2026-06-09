using System;
using UnityEngine;

// Token: 0x020000F8 RID: 248
public class ZapEffect : MonoBehaviour
{
	// Token: 0x060004F8 RID: 1272 RVA: 0x0001C55C File Offset: 0x0001A75C
	private void Start()
	{
		this.damage *= base.transform.localScale.x;
		this.range *= (1f + base.transform.localScale.x) * 0.5f;
		Player closestPlayer = PlayerManager.instance.GetClosestPlayer(base.transform.position, true);
		if (closestPlayer && Vector3.Distance(base.transform.position, closestPlayer.transform.position) < this.range)
		{
			closestPlayer.data.healthHandler.TakeDamage(this.damage * (closestPlayer.transform.position - base.transform.position).normalized, base.transform.position, null, PlayerManager.instance.GetOtherPlayer(closestPlayer), true, false);
			base.GetComponentInChildren<LineEffect>(true).Play(base.transform, closestPlayer.transform, 2f);
		}
	}

	// Token: 0x04000688 RID: 1672
	public float damage;

	// Token: 0x04000689 RID: 1673
	public float range = 1f;
}
