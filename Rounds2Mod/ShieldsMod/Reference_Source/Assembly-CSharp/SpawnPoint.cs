using System;
using UnityEngine;

// Token: 0x020000DD RID: 221
public class SpawnPoint : MonoBehaviour
{
	// Token: 0x0600046B RID: 1131 RVA: 0x0001A60F File Offset: 0x0001880F
	private void Awake()
	{
		this.localStartPos = base.transform.localPosition;
	}

	// Token: 0x040005F9 RID: 1529
	public int ID;

	// Token: 0x040005FA RID: 1530
	public int TEAMID;

	// Token: 0x040005FB RID: 1531
	public Vector3 localStartPos;
}
