using System;
using UnityEngine;

// Token: 0x0200004E RID: 78
public class DynamicParticles : MonoBehaviour
{
	// Token: 0x06000176 RID: 374 RVA: 0x00009748 File Offset: 0x00007948
	private void Start()
	{
		DynamicParticles.instance = this;
	}

	// Token: 0x06000177 RID: 375 RVA: 0x00009750 File Offset: 0x00007950
	private void Update()
	{
		this.spawnsThisFrame = 0;
	}

	// Token: 0x06000178 RID: 376 RVA: 0x0000975C File Offset: 0x0000795C
	public void PlayBulletHit(float damage, Transform spawnerTransform, HitInfo hit, Color projectielColor)
	{
		if ((float)this.spawnsThisFrame > 5f)
		{
			return;
		}
		this.spawnsThisFrame++;
		int num = 0;
		int num2 = 1;
		while (num2 < this.bulletHit.Length && this.bulletHit[num2].dmg <= damage)
		{
			num = num2;
			num2++;
		}
		GameObject[] array = ObjectsToSpawn.SpawnObject(base.transform, hit, this.bulletHit[num].objectsToSpawn, null, null, 55f, null, false);
		if (projectielColor != Color.black)
		{
			for (int i = 0; i < array.Length; i++)
			{
				for (int j = 0; j < array[i].transform.childCount; j++)
				{
					ChangeColor componentInChildren = array[i].transform.GetChild(j).GetComponentInChildren<ChangeColor>();
					if (componentInChildren)
					{
						componentInChildren.GetComponent<ParticleSystemRenderer>().material.color = projectielColor;
					}
				}
			}
		}
	}

	// Token: 0x040001E4 RID: 484
	public static DynamicParticles instance;

	// Token: 0x040001E5 RID: 485
	public DynamicParticleSet[] bulletHit;

	// Token: 0x040001E6 RID: 486
	private int spawnsThisFrame;
}
