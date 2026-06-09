using System;
using UnityEngine;

// Token: 0x0200012A RID: 298
public class DestroyOnAwake : MonoBehaviour
{
	// Token: 0x060005C9 RID: 1481 RVA: 0x00008D7A File Offset: 0x00006F7A
	private void Awake()
	{
		Object.Destroy(base.gameObject);
	}
}
