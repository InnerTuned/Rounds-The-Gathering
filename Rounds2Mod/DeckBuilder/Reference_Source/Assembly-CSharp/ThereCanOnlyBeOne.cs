using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001BD RID: 445
public class ThereCanOnlyBeOne : MonoBehaviour
{
	// Token: 0x060008CF RID: 2255 RVA: 0x0002E5FC File Offset: 0x0002C7FC
	private void Start()
	{
		bool flag = false;
		ThereCanOnlyBeOne[] componentsInChildren = base.transform.root.GetComponentsInChildren<ThereCanOnlyBeOne>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (!(componentsInChildren[i] == this))
			{
				flag = true;
				componentsInChildren[i].Poke();
			}
		}
		if (flag)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060008D0 RID: 2256 RVA: 0x0002E64D File Offset: 0x0002C84D
	public void Poke()
	{
		this.PokeEvent.Invoke();
	}

	// Token: 0x04000A15 RID: 2581
	public UnityEvent PokeEvent;
}
