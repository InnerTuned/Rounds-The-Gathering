using System;
using UnityEngine;

// Token: 0x020000BB RID: 187
public class RemoveAfterSecondsScale : MonoBehaviour
{
	// Token: 0x060003F9 RID: 1017 RVA: 0x00018676 File Offset: 0x00016876
	private void Start()
	{
		base.GetComponent<RemoveAfterSeconds>().seconds *= base.transform.localScale.x;
	}
}
