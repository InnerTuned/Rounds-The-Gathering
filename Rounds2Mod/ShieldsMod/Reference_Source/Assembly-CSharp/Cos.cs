using System;
using UnityEngine;

// Token: 0x02000036 RID: 54
public class Cos : MonoBehaviour
{
	// Token: 0x0600010B RID: 267 RVA: 0x00007BB0 File Offset: 0x00005DB0
	private void Update()
	{
		base.transform.root.position += base.transform.right * Mathf.Cos(Time.time * 20f * this.multiplier) * 10f * this.multiplier * Time.smoothDeltaTime;
	}

	// Token: 0x0400015D RID: 349
	public float multiplier = 1f;
}
