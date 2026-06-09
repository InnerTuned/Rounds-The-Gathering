using System;
using UnityEngine;

// Token: 0x02000034 RID: 52
public class CopyOwnerGunStats : MonoBehaviour
{
	// Token: 0x06000106 RID: 262 RVA: 0x00007AC2 File Offset: 0x00005CC2
	private void Start()
	{
		ApplyCardStats.CopyGunStats(base.transform.root.GetComponent<SpawnedAttack>().spawner.GetComponent<WeaponHandler>().gun, base.GetComponent<Gun>());
	}
}
