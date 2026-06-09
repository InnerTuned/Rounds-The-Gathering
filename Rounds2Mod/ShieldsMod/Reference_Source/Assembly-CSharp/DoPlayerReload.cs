using System;
using UnityEngine;

// Token: 0x02000130 RID: 304
public class DoPlayerReload : MonoBehaviour
{
	// Token: 0x060005D7 RID: 1495 RVA: 0x00020F00 File Offset: 0x0001F100
	private void Start()
	{
		this.wh = base.GetComponentInParent<WeaponHandler>();
	}

	// Token: 0x060005D8 RID: 1496 RVA: 0x00020F0E File Offset: 0x0001F10E
	public void DoReload()
	{
		this.wh.DoReload();
	}

	// Token: 0x04000771 RID: 1905
	private WeaponHandler wh;
}
