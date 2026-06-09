using System;
using UnityEngine;

// Token: 0x02000047 RID: 71
public class DestroyObjects : MonoBehaviour
{
	// Token: 0x0600015B RID: 347 RVA: 0x00008D7A File Offset: 0x00006F7A
	public void DestroySelf()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x0600015C RID: 348 RVA: 0x00008D88 File Offset: 0x00006F88
	public void DestroyAllObjects()
	{
		for (int i = 0; i < this.objectsToDestroy.Length; i++)
		{
			Object.Destroy(this.objectsToDestroy[i]);
		}
	}

	// Token: 0x040001CB RID: 459
	public GameObject[] objectsToDestroy;
}
