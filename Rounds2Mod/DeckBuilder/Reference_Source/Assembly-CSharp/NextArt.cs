using System;
using UnityEngine;

// Token: 0x02000177 RID: 375
public class NextArt : MonoBehaviour
{
	// Token: 0x0600079C RID: 1948 RVA: 0x0002927B File Offset: 0x0002747B
	private void Start()
	{
		ArtHandler.instance.NextArt();
	}
}
