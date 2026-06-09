using System;
using UnityEngine;

// Token: 0x0200000D RID: 13
public class AttackLevel : MonoBehaviour
{
	// Token: 0x06000041 RID: 65 RVA: 0x00003E14 File Offset: 0x00002014
	private void Start()
	{
		SpawnedAttack componentInParent = base.GetComponentInParent<SpawnedAttack>();
		if (componentInParent)
		{
			this.attackLevel = componentInParent.attackLevel;
		}
		bool flag = false;
		AttackLevel[] componentsInChildren = base.transform.root.GetComponentsInChildren<AttackLevel>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (!(componentsInChildren[i] == this) && componentsInChildren[i].gameObject.name == base.gameObject.name)
			{
				componentsInChildren[i].LevelUp();
				flag = true;
			}
		}
		if (flag)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000042 RID: 66 RVA: 0x00003E9F File Offset: 0x0000209F
	public float LevelScale()
	{
		return 1f + ((float)this.attackLevel - 1f) * this.levelScaleM;
	}

	// Token: 0x06000043 RID: 67 RVA: 0x00003EBB File Offset: 0x000020BB
	public int LevelsUp()
	{
		return this.attackLevel - 1;
	}

	// Token: 0x06000044 RID: 68 RVA: 0x00003EC5 File Offset: 0x000020C5
	public void LevelUp()
	{
		this.attackLevel++;
		if (this.LevelUpAction != null)
		{
			this.LevelUpAction.Invoke(this.attackLevel);
		}
	}

	// Token: 0x04000031 RID: 49
	public int attackLevel = 1;

	// Token: 0x04000032 RID: 50
	public float levelScaleM = 0.7f;

	// Token: 0x04000033 RID: 51
	public Action<int> LevelUpAction;
}
