using System;
using UnityEngine;

// Token: 0x0200005C RID: 92
public abstract class GameFeeler : MonoBehaviour
{
	// Token: 0x060001A5 RID: 421 RVA: 0x0000ACC3 File Offset: 0x00008EC3
	private void Awake()
	{
		GamefeelManager.RegisterGamefeeler(this);
		this.OnAwake();
	}

	// Token: 0x060001A6 RID: 422
	public abstract void OnGameFeel(Vector2 feelDirection);

	// Token: 0x060001A7 RID: 423
	public abstract void OnUIGameFeel(Vector2 feelDirection);

	// Token: 0x060001A8 RID: 424 RVA: 0x000027C8 File Offset: 0x000009C8
	public virtual void OnAwake()
	{
	}
}
