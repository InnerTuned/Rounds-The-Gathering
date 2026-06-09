using System;
using UnityEngine;

// Token: 0x02000035 RID: 53
public class CopyPlayerAim : MonoBehaviour
{
	// Token: 0x06000108 RID: 264 RVA: 0x00007AEE File Offset: 0x00005CEE
	private void Start()
	{
		this.level = base.GetComponentInParent<AttackLevel>();
	}

	// Token: 0x06000109 RID: 265 RVA: 0x00007AFC File Offset: 0x00005CFC
	public void Go()
	{
		float num = Random.Range(-this.spread, this.spread);
		float num2 = 0f;
		if (this.level)
		{
			num2 = (float)this.level.attackLevel * this.spreadPerLevel;
		}
		num += Random.Range(-num2, num2);
		Holding component = base.transform.root.GetComponent<Holding>();
		if (component)
		{
			base.transform.rotation = component.holdable.GetComponentInChildren<ShootPos>().transform.rotation;
		}
		base.transform.Rotate(Vector3.Cross(Vector3.forward, base.transform.forward) * num);
	}

	// Token: 0x0400015A RID: 346
	public float spreadPerLevel;

	// Token: 0x0400015B RID: 347
	public float spread;

	// Token: 0x0400015C RID: 348
	private AttackLevel level;
}
