using System;
using UnityEngine;

// Token: 0x02000165 RID: 357
public class LowerScreenshakePerPlayer : MonoBehaviour
{
	// Token: 0x0600072E RID: 1838 RVA: 0x0002705C File Offset: 0x0002525C
	private void Update()
	{
		if (PlayerManager.instance.players.Count > 2)
		{
			this.shake.shakeforce *= 0.5f;
			Object.Destroy(this);
		}
	}

	// Token: 0x0400089E RID: 2206
	public Screenshaker shake;
}
