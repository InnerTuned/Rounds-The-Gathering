using System;
using UnityEngine;

// Token: 0x02000141 RID: 321
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[AddComponentMenu("")]
public class AmplifyColorTriggerProxy2D : AmplifyColorTriggerProxyBase
{
	// Token: 0x0600062D RID: 1581 RVA: 0x00023128 File Offset: 0x00021328
	private void Start()
	{
		this.circleCollider = base.GetComponent<CircleCollider2D>();
		this.circleCollider.radius = 0.01f;
		this.circleCollider.isTrigger = true;
		this.rigidBody = base.GetComponent<Rigidbody2D>();
		this.rigidBody.gravityScale = 0f;
		this.rigidBody.isKinematic = true;
	}

	// Token: 0x0600062E RID: 1582 RVA: 0x000230F1 File Offset: 0x000212F1
	private void LateUpdate()
	{
		base.transform.position = this.Reference.position;
		base.transform.rotation = this.Reference.rotation;
	}

	// Token: 0x040007D3 RID: 2003
	private CircleCollider2D circleCollider;

	// Token: 0x040007D4 RID: 2004
	private Rigidbody2D rigidBody;
}
