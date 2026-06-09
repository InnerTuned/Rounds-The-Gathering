using System;
using UnityEngine;

// Token: 0x020000DF RID: 223
public class Spring : MonoBehaviour
{
	// Token: 0x06000471 RID: 1137 RVA: 0x0001A862 File Offset: 0x00018A62
	private void Start()
	{
		this.rig = base.GetComponent<Rigidbody2D>();
		this.up = base.transform.up;
	}

	// Token: 0x06000472 RID: 1138 RVA: 0x0001A884 File Offset: 0x00018A84
	private void FixedUpdate()
	{
		if (this.rig)
		{
			this.rig.AddTorque((Vector3.Cross(base.transform.up, this.up).normalized * Vector3.Angle(base.transform.up, this.up)).z * this.spring * this.rig.mass);
		}
	}

	// Token: 0x040005FF RID: 1535
	public float spring = 1f;

	// Token: 0x04000600 RID: 1536
	private Rigidbody2D rig;

	// Token: 0x04000601 RID: 1537
	private Vector3 up;
}
