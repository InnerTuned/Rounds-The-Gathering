using System;
using UnityEngine;

// Token: 0x0200017C RID: 380
public class Orbit : MonoBehaviour
{
	// Token: 0x060007BE RID: 1982 RVA: 0x00029B59 File Offset: 0x00027D59
	private void Start()
	{
		AttackLevel component = base.GetComponent<AttackLevel>();
		component.LevelUpAction = (Action<int>)Delegate.Combine(component.LevelUpAction, new Action<int>(this.UpdateLevel));
	}

	// Token: 0x060007BF RID: 1983 RVA: 0x00029B84 File Offset: 0x00027D84
	private void UpdateLevel(int newLevel)
	{
		for (int i = 0; i < this.objectsPerLevel.Length; i++)
		{
			if (i <= newLevel - 1)
			{
				this.objectsPerLevel[i].SetActive(true);
			}
		}
	}

	// Token: 0x04000910 RID: 2320
	public GameObject[] objectsPerLevel;
}
