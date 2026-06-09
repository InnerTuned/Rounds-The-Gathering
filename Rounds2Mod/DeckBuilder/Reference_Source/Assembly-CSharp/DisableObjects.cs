using System;
using UnityEngine;

// Token: 0x0200012E RID: 302
public class DisableObjects : MonoBehaviour
{
	// Token: 0x060005D2 RID: 1490 RVA: 0x00020DD0 File Offset: 0x0001EFD0
	private void Start()
	{
		if (this.playOnAwake)
		{
			this.DoIt();
		}
	}

	// Token: 0x060005D3 RID: 1491 RVA: 0x00020DE0 File Offset: 0x0001EFE0
	private void DoIt()
	{
		for (int i = 0; i < this.objects.Length; i++)
		{
			this.objects[i].SetActive(false);
		}
	}

	// Token: 0x0400076D RID: 1901
	public bool playOnAwake = true;

	// Token: 0x0400076E RID: 1902
	public GameObject[] objects;
}
