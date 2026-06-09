using System;
using UnityEngine;

// Token: 0x020000FF RID: 255
public class BuildOnly : MonoBehaviour
{
	// Token: 0x06000511 RID: 1297 RVA: 0x0001CEE1 File Offset: 0x0001B0E1
	private void Start()
	{
		if (Application.isEditor)
		{
			base.gameObject.SetActive(false);
		}
	}
}
