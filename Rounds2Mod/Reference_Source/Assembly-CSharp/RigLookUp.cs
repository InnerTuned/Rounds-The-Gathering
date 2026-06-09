using System;
using UnityEngine;

// Token: 0x020001A7 RID: 423
public class RigLookUp : MonoBehaviour
{
	// Token: 0x0600087A RID: 2170 RVA: 0x0002D421 File Offset: 0x0002B621
	private void Start()
	{
		this.rig = base.GetComponent<Rigidbody2D>();
	}

	// Token: 0x0600087B RID: 2171 RVA: 0x0002D430 File Offset: 0x0002B630
	private void FixedUpdate()
	{
		this.rig.AddTorque(Vector3.Cross(base.transform.up, Vector3.up).normalized.z * this.force * this.rig.mass * Vector3.Angle(base.transform.up, Vector3.up), 0);
	}

	// Token: 0x040009BD RID: 2493
	private Rigidbody2D rig;

	// Token: 0x040009BE RID: 2494
	public float force;
}
