using System;
using UnityEngine;

// Token: 0x02000188 RID: 392
public class PlayerDoJump : MonoBehaviour
{
	// Token: 0x060007FA RID: 2042 RVA: 0x0002BBF3 File Offset: 0x00029DF3
	private void Start()
	{
		this.jump = base.GetComponentInParent<PlayerJump>();
	}

	// Token: 0x060007FB RID: 2043 RVA: 0x0002BC01 File Offset: 0x00029E01
	public void DoJump()
	{
		this.jump.Jump(true, this.multiplier);
	}

	// Token: 0x04000957 RID: 2391
	private PlayerJump jump;

	// Token: 0x04000958 RID: 2392
	public float multiplier = 0.25f;
}
