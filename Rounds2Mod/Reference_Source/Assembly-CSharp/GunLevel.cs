using System;
using UnityEngine;

// Token: 0x02000153 RID: 339
public class GunLevel : MonoBehaviour
{
	// Token: 0x060006DE RID: 1758 RVA: 0x000261BC File Offset: 0x000243BC
	private void Start()
	{
		this.copyFrom = base.GetComponent<Gun>();
		AttackLevel componentInParent = base.GetComponentInParent<AttackLevel>();
		componentInParent.LevelUpAction = (Action<int>)Delegate.Combine(componentInParent.LevelUpAction, new Action<int>(this.BuffGun));
	}

	// Token: 0x060006DF RID: 1759 RVA: 0x000261F1 File Offset: 0x000243F1
	public void BuffGun(int level)
	{
		ApplyCardStats.CopyGunStats(this.copyFrom, this.copyTo);
	}

	// Token: 0x04000846 RID: 2118
	public Gun copyTo;

	// Token: 0x04000847 RID: 2119
	private Gun copyFrom;
}
