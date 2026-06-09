using System;
using UnityEngine;

// Token: 0x0200012D RID: 301
public class DisableIfSimple : MonoBehaviour
{
	// Token: 0x060005D0 RID: 1488 RVA: 0x00020D60 File Offset: 0x0001EF60
	private void Start()
	{
		Unparent componentInParent = base.GetComponentInParent<Unparent>();
		if (componentInParent && componentInParent.parent)
		{
			base.gameObject.SetActive(!componentInParent.parent.root.GetComponentInChildren<PlayerSkinHandler>().simpleSkin);
			return;
		}
		base.gameObject.SetActive(!base.transform.root.GetComponentInChildren<PlayerSkinHandler>().simpleSkin);
	}
}
