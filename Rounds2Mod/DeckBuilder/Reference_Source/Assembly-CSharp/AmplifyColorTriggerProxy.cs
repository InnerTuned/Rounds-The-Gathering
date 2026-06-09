using System;
using UnityEngine;

// Token: 0x02000140 RID: 320
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
[AddComponentMenu("")]
public class AmplifyColorTriggerProxy : AmplifyColorTriggerProxyBase
{
	// Token: 0x0600062A RID: 1578 RVA: 0x00023098 File Offset: 0x00021298
	private void Start()
	{
		this.sphereCollider = base.GetComponent<SphereCollider>();
		this.sphereCollider.radius = 0.01f;
		this.sphereCollider.isTrigger = true;
		this.rigidBody = base.GetComponent<Rigidbody>();
		this.rigidBody.useGravity = false;
		this.rigidBody.isKinematic = true;
	}

	// Token: 0x0600062B RID: 1579 RVA: 0x000230F1 File Offset: 0x000212F1
	private void LateUpdate()
	{
		base.transform.position = this.Reference.position;
		base.transform.rotation = this.Reference.rotation;
	}

	// Token: 0x040007D1 RID: 2001
	private SphereCollider sphereCollider;

	// Token: 0x040007D2 RID: 2002
	private Rigidbody rigidBody;
}
