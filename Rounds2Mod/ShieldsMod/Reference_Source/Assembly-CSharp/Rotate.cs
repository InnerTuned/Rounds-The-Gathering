using System;
using UnityEngine;

// Token: 0x020000BD RID: 189
public class Rotate : MonoBehaviour
{
	// Token: 0x060003FE RID: 1022 RVA: 0x000186C0 File Offset: 0x000168C0
	private void Update()
	{
		base.transform.Rotate(Vector3.forward * this.speed * TimeHandler.deltaTime, Space.World);
	}

	// Token: 0x04000579 RID: 1401
	public float speed;
}
