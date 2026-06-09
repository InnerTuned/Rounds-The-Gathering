using System;
using UnityEngine;

// Token: 0x020000EE RID: 238
public class VelocityStretch : MonoBehaviour
{
	// Token: 0x060004CD RID: 1229 RVA: 0x0001BD7B File Offset: 0x00019F7B
	private void Start()
	{
		this.rig = base.GetComponentInParent<Rigidbody2D>();
	}

	// Token: 0x060004CE RID: 1230 RVA: 0x0001BD8C File Offset: 0x00019F8C
	private void Update()
	{
		base.transform.localScale = Vector3.one + new Vector3(Mathf.Abs(this.rig.velocity.x), Mathf.Abs(this.rig.velocity.y), 0f) * this.amount;
	}

	// Token: 0x0400065E RID: 1630
	private Rigidbody2D rig;

	// Token: 0x0400065F RID: 1631
	public float amount = 1f;
}
