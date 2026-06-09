using System;
using UnityEngine;

// Token: 0x0200000A RID: 10
public class ArmRotator : MonoBehaviour
{
	// Token: 0x06000034 RID: 52 RVA: 0x00003C78 File Offset: 0x00001E78
	private void Start()
	{
		base.transform.localEulerAngles = this.rotation;
	}

	// Token: 0x04000029 RID: 41
	public Vector3 rotation;
}
