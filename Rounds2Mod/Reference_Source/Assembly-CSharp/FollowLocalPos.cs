using System;
using UnityEngine;

// Token: 0x02000056 RID: 86
public class FollowLocalPos : MonoBehaviour
{
	// Token: 0x06000194 RID: 404 RVA: 0x0000A7A6 File Offset: 0x000089A6
	private void Start()
	{
		if (this.target && this.relative == Vector3.zero)
		{
			this.Follow(this.target);
		}
	}

	// Token: 0x06000195 RID: 405 RVA: 0x0000A7D4 File Offset: 0x000089D4
	private void LateUpdate()
	{
		if (this.target && this.target.gameObject.activeInHierarchy)
		{
			base.transform.position = this.target.TransformPoint(this.relative);
			base.transform.rotation = Quaternion.LookRotation(this.target.TransformDirection(this.relativeForward), this.target.TransformDirection(this.relativeUp));
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06000196 RID: 406 RVA: 0x0000A85C File Offset: 0x00008A5C
	public void Follow(Transform targetTransform)
	{
		if (!targetTransform)
		{
			return;
		}
		this.target = targetTransform;
		Player component = this.target.transform.root.GetComponent<Player>();
		Vector3 position = base.transform.position;
		if (component)
		{
			this.targetPlayer = component;
			base.transform.position -= base.transform.forward * 2f;
			Vector3 position2 = this.target.position;
			Vector3 b = (base.transform.position - this.target.position).normalized * 1.1f;
			position = position2 + b;
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, component.transform.position.z);
		}
		this.relative = targetTransform.InverseTransformPoint(position);
		this.relativeForward = targetTransform.InverseTransformDirection(base.transform.forward);
		this.relativeUp = targetTransform.InverseTransformDirection(base.transform.up);
	}

	// Token: 0x0400022C RID: 556
	private Vector3 relative;

	// Token: 0x0400022D RID: 557
	private Vector3 relativeForward;

	// Token: 0x0400022E RID: 558
	private Vector3 relativeUp;

	// Token: 0x0400022F RID: 559
	public Transform target;

	// Token: 0x04000230 RID: 560
	public Player targetPlayer;
}
