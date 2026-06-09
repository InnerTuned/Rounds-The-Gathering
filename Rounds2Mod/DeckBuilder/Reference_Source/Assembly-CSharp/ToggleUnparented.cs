using System;
using UnityEngine;

// Token: 0x020001BF RID: 447
public class ToggleUnparented : MonoBehaviour
{
	// Token: 0x060008D6 RID: 2262 RVA: 0x0002E75C File Offset: 0x0002C95C
	private void Awake()
	{
		this.unparents = base.GetComponentsInChildren<Unparent>();
	}

	// Token: 0x060008D7 RID: 2263 RVA: 0x0002E76C File Offset: 0x0002C96C
	private void OnDisable()
	{
		for (int i = 0; i < this.unparents.Length; i++)
		{
			this.unparents[i].gameObject.SetActive(false);
		}
		base.GetComponent<Holding>().holdable.gameObject.SetActive(false);
	}

	// Token: 0x060008D8 RID: 2264 RVA: 0x0002E7B8 File Offset: 0x0002C9B8
	private void OnEnable()
	{
		for (int i = 0; i < this.unparents.Length; i++)
		{
			this.unparents[i].gameObject.SetActive(true);
		}
		base.GetComponent<Holding>().holdable.gameObject.SetActive(false);
	}

	// Token: 0x04000A19 RID: 2585
	private Unparent[] unparents;
}
