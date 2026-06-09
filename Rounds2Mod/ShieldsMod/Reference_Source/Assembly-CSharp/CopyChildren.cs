using System;
using UnityEngine;

// Token: 0x02000125 RID: 293
public class CopyChildren : MonoBehaviour
{
	// Token: 0x060005BC RID: 1468 RVA: 0x000209A0 File Offset: 0x0001EBA0
	public void DoUpdate()
	{
		for (int i = base.transform.childCount - 1; i >= 0; i--)
		{
			Object.Destroy(base.transform.GetChild(i).gameObject);
		}
		for (int j = 0; j < this.target.transform.childCount; j++)
		{
			Transform child = this.target.transform.GetChild(j);
			Object.Instantiate<GameObject>(child.gameObject, base.transform.TransformPoint(child.localPosition), Quaternion.identity, base.transform).transform.localScale = child.localScale;
		}
	}

	// Token: 0x060005BD RID: 1469 RVA: 0x00020A3F File Offset: 0x0001EC3F
	private void Update()
	{
		this.DoUpdate();
	}

	// Token: 0x0400075D RID: 1885
	public GameObject target;
}
