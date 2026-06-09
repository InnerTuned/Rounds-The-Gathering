using System;
using UnityEngine;

// Token: 0x020000F1 RID: 241
public class GridObject : MonoBehaviour
{
	// Token: 0x060004D9 RID: 1241 RVA: 0x000027C8 File Offset: 0x000009C8
	public virtual void BopCall(float power)
	{
	}

	// Token: 0x060004DA RID: 1242 RVA: 0x0001BEFD File Offset: 0x0001A0FD
	public virtual void BulletCall(float distance)
	{
		distance = Mathf.Clamp(0f, 1f, distance);
		this.BopCall(1f - distance);
	}

	// Token: 0x060004DB RID: 1243 RVA: 0x000027C8 File Offset: 0x000009C8
	public virtual void OnPlayerKilled(float distance)
	{
	}

	// Token: 0x060004DC RID: 1244 RVA: 0x000027C8 File Offset: 0x000009C8
	public virtual void OnGameOverOver(float distance)
	{
	}

	// Token: 0x060004DD RID: 1245 RVA: 0x000027C8 File Offset: 0x000009C8
	public virtual void OnSetSize(float size)
	{
	}
}
