using System;
using UnityEngine;

// Token: 0x0200015D RID: 349
public class LevelScale : MonoBehaviour
{
	// Token: 0x06000702 RID: 1794 RVA: 0x000267ED File Offset: 0x000249ED
	private void Init()
	{
		if (this.inited)
		{
			return;
		}
		this.inited = true;
		this.startScale = base.transform.localScale;
	}

	// Token: 0x06000703 RID: 1795 RVA: 0x00026810 File Offset: 0x00024A10
	private void Start()
	{
		this.Init();
		this.level = base.GetComponent<AttackLevel>();
		if (this.onStart)
		{
			base.transform.localScale *= this.level.LevelScale();
		}
		if (this.onLevelUp)
		{
			AttackLevel attackLevel = this.level;
			attackLevel.LevelUpAction = (Action<int>)Delegate.Combine(attackLevel.LevelUpAction, new Action<int>(this.LevelUp));
		}
	}

	// Token: 0x06000704 RID: 1796 RVA: 0x00026887 File Offset: 0x00024A87
	public void LevelUp(int lvl)
	{
		this.Init();
		base.transform.localScale = this.startScale * this.level.LevelScale();
	}

	// Token: 0x04000869 RID: 2153
	public bool onLevelUp = true;

	// Token: 0x0400086A RID: 2154
	public bool onStart;

	// Token: 0x0400086B RID: 2155
	private AttackLevel level;

	// Token: 0x0400086C RID: 2156
	private Vector3 startScale;

	// Token: 0x0400086D RID: 2157
	private bool inited;
}
