using System;
using UnityEngine;

// Token: 0x020000C5 RID: 197
public class ScaleWithHp : MonoBehaviour
{
	// Token: 0x06000423 RID: 1059 RVA: 0x000193CA File Offset: 0x000175CA
	private void Start()
	{
		this.dmg = base.GetComponentInParent<DamagableEvent>();
	}

	// Token: 0x06000424 RID: 1060 RVA: 0x000193D8 File Offset: 0x000175D8
	private void Update()
	{
		base.transform.localScale = Vector3.one * this.curve.Evaluate(this.dmg.currentHP);
	}

	// Token: 0x040005A9 RID: 1449
	private DamagableEvent dmg;

	// Token: 0x040005AA RID: 1450
	public AnimationCurve curve;
}
