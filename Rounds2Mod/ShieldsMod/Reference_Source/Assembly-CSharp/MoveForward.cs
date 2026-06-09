using System;
using UnityEngine;

// Token: 0x02000082 RID: 130
public class MoveForward : MonoBehaviour
{
	// Token: 0x060002C9 RID: 713 RVA: 0x0001189E File Offset: 0x0000FA9E
	private void Start()
	{
		base.transform.position += Vector3.forward * this.amount;
	}

	// Token: 0x040003DD RID: 989
	public float amount = 3f;
}
