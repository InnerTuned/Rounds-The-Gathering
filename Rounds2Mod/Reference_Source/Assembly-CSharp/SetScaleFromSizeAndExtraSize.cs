using System;
using UnityEngine;

// Token: 0x020001AE RID: 430
public class SetScaleFromSizeAndExtraSize : MonoBehaviour
{
	// Token: 0x06000896 RID: 2198 RVA: 0x0002D8A4 File Offset: 0x0002BAA4
	private void Start()
	{
		float size = base.GetComponentInParent<RayCastTrail>().size;
		base.transform.localScale *= size * this.scalePerSize;
	}

	// Token: 0x040009D3 RID: 2515
	public float scalePerSize;
}
