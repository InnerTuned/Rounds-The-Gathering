using System;
using UnityEngine;

// Token: 0x0200001E RID: 30
public class CappedDeltaTime : MonoBehaviour
{
	// Token: 0x06000095 RID: 149 RVA: 0x000052C6 File Offset: 0x000034C6
	private void Update()
	{
		CappedDeltaTime.time = Mathf.Clamp(TimeHandler.deltaTime, 0f, 0.02f);
	}

	// Token: 0x04000087 RID: 135
	public static float time;
}
