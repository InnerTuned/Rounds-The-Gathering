using System;
using UnityEngine;

// Token: 0x020001B7 RID: 439
public class SpawnedAttack_ParentPlayer : MonoBehaviour
{
	// Token: 0x060008BE RID: 2238 RVA: 0x0002E055 File Offset: 0x0002C255
	private void Start()
	{
		base.GetComponent<SpawnedAttack>().spawner = base.GetComponentInParent<Player>();
	}
}
