using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200018E RID: 398
public class PlayerSpawnVisualEffect : MonoBehaviour
{
	// Token: 0x0600080A RID: 2058 RVA: 0x0002BECF File Offset: 0x0002A0CF
	public void RPCA_SpawnVisualEffect(string effect)
	{
		Object.Instantiate<GameObject>(this.visualEffects[effect], base.transform.position, base.transform.rotation, base.transform);
	}

	// Token: 0x04000962 RID: 2402
	public Dictionary<string, GameObject> visualEffects;
}
