using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000062 RID: 98
public class GeneralShooter : MonoBehaviour
{
	// Token: 0x060001D1 RID: 465 RVA: 0x0000B843 File Offset: 0x00009A43
	private void Start()
	{
		this.gun = base.GetComponent<Gun>();
	}

	// Token: 0x060001D2 RID: 466 RVA: 0x0000B851 File Offset: 0x00009A51
	public void Attack()
	{
		if (!base.enabled)
		{
			return;
		}
		base.StartCoroutine(this.DoAttack());
	}

	// Token: 0x060001D3 RID: 467 RVA: 0x0000B869 File Offset: 0x00009A69
	private IEnumerator DoAttack()
	{
		int shotsToMake = Mathf.Clamp(Mathf.RoundToInt(0.5f / this.gun.attackSpeed), 1, 5);
		int num;
		for (int i = 0; i < shotsToMake; i = num + 1)
		{
			for (int j = 0; j < this.shooters.Length; j++)
			{
				this.gun.transform.position = this.shooters[j].position;
				this.gun.transform.rotation = this.shooters[j].rotation;
				this.gun.Attack(this.gun.currentCharge, true, 1f, 1f, true);
			}
			if (shotsToMake > i)
			{
				yield return new WaitForSeconds(0.1f);
			}
			num = i;
		}
		yield break;
	}

	// Token: 0x04000286 RID: 646
	private Gun gun;

	// Token: 0x04000287 RID: 647
	[HideInInspector]
	public float charge;

	// Token: 0x04000288 RID: 648
	public Transform[] shooters;
}
