using System;
using UnityEngine;

// Token: 0x020000D0 RID: 208
public class SetSpawnedParticleColor : MonoBehaviour
{
	// Token: 0x06000445 RID: 1093 RVA: 0x00019C90 File Offset: 0x00017E90
	private void Start()
	{
		SpawnObjects component = base.GetComponent<SpawnObjects>();
		component.SpawnedAction = (Action<GameObject>)Delegate.Combine(component.SpawnedAction, new Action<GameObject>(this.Go));
		this.myColor = PlayerSkinBank.GetPlayerSkinColors(base.GetComponentInParent<SpawnedAttack>().spawner.playerID).particleEffect;
	}

	// Token: 0x06000446 RID: 1094 RVA: 0x00019CE4 File Offset: 0x00017EE4
	private void Go(GameObject spawned)
	{
		ParticleSystem[] componentsInChildren = spawned.GetComponentsInChildren<ParticleSystem>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].startColor = this.myColor;
		}
		LineEffect[] componentsInChildren2 = spawned.GetComponentsInChildren<LineEffect>();
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			componentsInChildren2[j].useColorOverTime = false;
			componentsInChildren2[j].GetComponent<LineRenderer>().startColor = this.myColor;
			componentsInChildren2[j].GetComponent<LineRenderer>().endColor = this.myColor;
		}
	}

	// Token: 0x040005CA RID: 1482
	private Color myColor;
}
