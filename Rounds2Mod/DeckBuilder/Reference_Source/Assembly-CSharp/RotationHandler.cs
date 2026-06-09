using System;
using UnityEngine;

// Token: 0x020000BF RID: 191
public class RotationHandler : MonoBehaviour
{
	// Token: 0x06000404 RID: 1028 RVA: 0x000187B7 File Offset: 0x000169B7
	private void Start()
	{
		this.data = base.GetComponent<CharacterData>();
		this.rig = base.GetComponent<PlayerVelocity>();
	}

	// Token: 0x06000405 RID: 1029 RVA: 0x000187D4 File Offset: 0x000169D4
	private void FixedUpdate()
	{
		this.rig.AddTorque(this.torque * TimeHandler.timeScale * (Vector3.Angle(Vector3.up, base.transform.up) * Vector3.Cross(Vector3.up, base.transform.up).normalized).z);
	}

	// Token: 0x04000582 RID: 1410
	private CharacterData data;

	// Token: 0x04000583 RID: 1411
	private PlayerVelocity rig;

	// Token: 0x04000584 RID: 1412
	public float torque;
}
