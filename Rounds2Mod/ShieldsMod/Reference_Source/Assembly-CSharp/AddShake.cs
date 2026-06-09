using System;
using UnityEngine;

// Token: 0x02000006 RID: 6
public class AddShake : MonoBehaviour
{
	// Token: 0x06000023 RID: 35 RVA: 0x00002CEF File Offset: 0x00000EEF
	private void Start()
	{
		if (this.auto)
		{
			this.DoShake();
		}
	}

	// Token: 0x06000024 RID: 36 RVA: 0x00002D00 File Offset: 0x00000F00
	public void DoShake()
	{
		GamefeelManager.GameFeel(this.shake * Mathf.Clamp(this.inheritScale ? base.transform.localScale.x : 1f, 0f, this.max));
	}

	// Token: 0x04000016 RID: 22
	public Vector2 shake;

	// Token: 0x04000017 RID: 23
	public bool auto = true;

	// Token: 0x04000018 RID: 24
	public bool inheritScale;

	// Token: 0x04000019 RID: 25
	public float max = float.PositiveInfinity;
}
