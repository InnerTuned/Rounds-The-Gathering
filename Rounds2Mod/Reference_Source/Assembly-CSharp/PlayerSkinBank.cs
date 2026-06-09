using System;
using UnityEngine;

// Token: 0x020000A0 RID: 160
[CreateAssetMenu(fileName = "Skin Bank", menuName = "Custom/Skin Bank", order = 99999)]
public class PlayerSkinBank : ScriptableObject
{
	// Token: 0x1700000B RID: 11
	// (get) Token: 0x0600038E RID: 910 RVA: 0x00015DFC File Offset: 0x00013FFC
	private static PlayerSkinBank Instance
	{
		get
		{
			if (PlayerSkinBank.instance == null)
			{
				PlayerSkinBank.instance = (Resources.Load("SkinBank") as PlayerSkinBank);
			}
			return PlayerSkinBank.instance;
		}
	}

	// Token: 0x0600038F RID: 911 RVA: 0x00015E24 File Offset: 0x00014024
	public static PlayerSkin GetPlayerSkinColors(int team)
	{
		return PlayerSkinBank.Instance.skins[team].currentPlayerSkin;
	}

	// Token: 0x06000390 RID: 912 RVA: 0x00015E3B File Offset: 0x0001403B
	public static PlayerSkinBank.PlayerSkinInstance GetPlayerSkin(int team)
	{
		return PlayerSkinBank.Instance.skins[team];
	}

	// Token: 0x040004AC RID: 1196
	private static PlayerSkinBank instance;

	// Token: 0x040004AD RID: 1197
	public PlayerSkinBank.PlayerSkinInstance[] skins = new PlayerSkinBank.PlayerSkinInstance[0];

	// Token: 0x02000370 RID: 880
	[Serializable]
	public struct PlayerSkinInstance
	{
		// Token: 0x0400118F RID: 4495
		public PlayerSkin currentPlayerSkin;
	}
}
