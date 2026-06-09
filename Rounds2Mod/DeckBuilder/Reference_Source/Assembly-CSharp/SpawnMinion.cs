using System;
using UnityEngine;

// Token: 0x020001B8 RID: 440
public class SpawnMinion : MonoBehaviour
{
	// Token: 0x060008C0 RID: 2240 RVA: 0x0002E068 File Offset: 0x0002C268
	private void Start()
	{
		this.data = base.GetComponentInParent<CharacterData>();
		this.level = base.GetComponentInParent<AttackLevel>();
	}

	// Token: 0x060008C1 RID: 2241 RVA: 0x0002E084 File Offset: 0x0002C284
	public void Go()
	{
		for (int i = 0; i < this.level.attackLevel; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.minion, base.transform.position + Vector3.up * (((float)i + 1f) * 0.5f), base.transform.rotation);
			Object.Instantiate<GameObject>(this.minionAI, gameObject.transform.position, gameObject.transform.rotation, gameObject.transform);
			CharacterData component = gameObject.GetComponent<CharacterData>();
			component.SetAI(this.data.player);
			component.player.playerID = this.data.player.playerID;
			component.isPlaying = true;
			this.card.GetComponent<ApplyCardStats>().Pick(component.player.teamID, true, PickerType.Team);
			gameObject.GetComponentInChildren<PlayerSkinHandler>().ToggleSimpleSkin(true);
			component.healthHandler.DestroyOnDeath = true;
		}
	}

	// Token: 0x040009FA RID: 2554
	public GameObject card;

	// Token: 0x040009FB RID: 2555
	public GameObject minionAI;

	// Token: 0x040009FC RID: 2556
	public GameObject minion;

	// Token: 0x040009FD RID: 2557
	private CharacterData data;

	// Token: 0x040009FE RID: 2558
	private AttackLevel level;
}
