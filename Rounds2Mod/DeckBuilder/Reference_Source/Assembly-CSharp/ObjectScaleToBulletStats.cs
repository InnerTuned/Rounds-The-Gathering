using System;
using UnityEngine;

// Token: 0x02000178 RID: 376
public class ObjectScaleToBulletStats : MonoBehaviour
{
	// Token: 0x0600079E RID: 1950 RVA: 0x00029288 File Offset: 0x00027488
	private void Start()
	{
		ProjectileHit component = base.GetComponent<ProjectileHit>();
		MoveTransform component2 = base.GetComponent<MoveTransform>();
		if (component)
		{
			component.damage = Mathf.Lerp(component.damage, component.damage * this.target.transform.localScale.x, this.dmgAmount);
		}
		if (component2)
		{
			component2.localForce.z = Mathf.Lerp(component2.localForce.z, component2.localForce.z * this.target.transform.localScale.x, this.speedAmount);
		}
	}

	// Token: 0x040008F7 RID: 2295
	public GameObject target;

	// Token: 0x040008F8 RID: 2296
	public float dmgAmount = 1f;

	// Token: 0x040008F9 RID: 2297
	public float speedAmount = 1f;
}
