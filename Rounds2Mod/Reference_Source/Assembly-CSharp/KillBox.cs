using System;
using UnityEngine;

// Token: 0x02000073 RID: 115
public class KillBox : MonoBehaviour
{
	// Token: 0x0600025F RID: 607 RVA: 0x0000F6AC File Offset: 0x0000D8AC
	private void Start()
	{
		this.box = base.GetComponent<BoxCollider2D>();
	}

	// Token: 0x06000260 RID: 608 RVA: 0x0000F6BC File Offset: 0x0000D8BC
	private void Update()
	{
		this.readyToKill = (this.timeActivated + this.toggleTime > Time.time);
		if (this.readyToKill)
		{
			for (int i = 0; i < PlayerManager.instance.players.Count; i++)
			{
				if (this.box.OverlapPoint(PlayerManager.instance.players[i].transform.position))
				{
					PlayerManager.instance.players[i].data.healthHandler.TakeDamage(Vector2.up * 1000f, base.transform.position, null, null, true, false);
				}
			}
		}
	}

	// Token: 0x06000261 RID: 609 RVA: 0x0000F779 File Offset: 0x0000D979
	public void Activate()
	{
		this.timeActivated = Time.time;
	}

	// Token: 0x04000368 RID: 872
	public bool alwaysOn;

	// Token: 0x04000369 RID: 873
	public float toggleTime = 0.3f;

	// Token: 0x0400036A RID: 874
	private float timeActivated = -10f;

	// Token: 0x0400036B RID: 875
	public bool readyToKill;

	// Token: 0x0400036C RID: 876
	private BoxCollider2D box;
}
