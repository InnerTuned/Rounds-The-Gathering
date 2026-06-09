using System;
using UnityEngine;

// Token: 0x020000D4 RID: 212
public class SetTeamColorSpecific : MonoBehaviour
{
	// Token: 0x06000450 RID: 1104 RVA: 0x00019EFC File Offset: 0x000180FC
	private void Start()
	{
		Player player = base.GetComponentInParent<Player>();
		if (!player)
		{
			player = base.GetComponentInParent<SpawnedAttack>().spawner;
		}
		ParticleSystem component = base.GetComponent<ParticleSystem>();
		if (component)
		{
			ParticleSystem.MainModule main = component.main;
			component.startColor = this.colors[player.playerID];
		}
		LineRenderer component2 = base.GetComponent<LineRenderer>();
		if (component2)
		{
			component2.startColor = this.colors[player.playerID];
			component2.endColor = this.colors[player.playerID];
		}
	}

	// Token: 0x040005D1 RID: 1489
	public Color[] colors;
}
