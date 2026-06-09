using System;
using UnityEngine;

// Token: 0x0200006C RID: 108
public class Holdable : MonoBehaviour
{
	// Token: 0x06000231 RID: 561 RVA: 0x0000E0C5 File Offset: 0x0000C2C5
	private void Awake()
	{
		this.rig = base.GetComponent<Rigidbody2D>();
	}

	// Token: 0x06000232 RID: 562 RVA: 0x000027C8 File Offset: 0x000009C8
	private void Start()
	{
	}

	// Token: 0x06000233 RID: 563 RVA: 0x0000E0D3 File Offset: 0x0000C2D3
	public void SetTeamColors(PlayerSkin teamColor, Player player)
	{
		SetTeamColor.TeamColorThis(base.gameObject, teamColor);
	}

	// Token: 0x04000315 RID: 789
	public Rigidbody2D rig;

	// Token: 0x04000316 RID: 790
	public CharacterData holder;
}
