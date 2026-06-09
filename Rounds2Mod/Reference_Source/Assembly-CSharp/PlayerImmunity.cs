using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200018B RID: 395
public class PlayerImmunity : MonoBehaviour
{
	// Token: 0x06000803 RID: 2051 RVA: 0x000027C8 File Offset: 0x000009C8
	private void Start()
	{
	}

	// Token: 0x06000804 RID: 2052 RVA: 0x0002BD7C File Offset: 0x00029F7C
	private void Update()
	{
		for (int i = this.immunities.Count - 1; i >= 0; i--)
		{
			this.immunities[i].time -= TimeHandler.deltaTime;
			if (this.immunities[i].time <= 0f)
			{
				this.immunities.RemoveAt(i);
			}
		}
	}

	// Token: 0x06000805 RID: 2053 RVA: 0x0002BDE4 File Offset: 0x00029FE4
	public bool IsImune(float time, float dmg, string name)
	{
		int i = 0;
		while (i < this.immunities.Count)
		{
			if (this.immunities[i].name == name)
			{
				if (dmg > this.immunities[i].dmg)
				{
					this.immunities[i].dmg = dmg;
					this.immunities[i].name = name;
					this.immunities[i].time = time;
					return false;
				}
				return true;
			}
			else
			{
				i++;
			}
		}
		this.immunities.Add(new Immunities(time, dmg, name));
		return false;
	}

	// Token: 0x0400095E RID: 2398
	private List<Immunities> immunities = new List<Immunities>();
}
