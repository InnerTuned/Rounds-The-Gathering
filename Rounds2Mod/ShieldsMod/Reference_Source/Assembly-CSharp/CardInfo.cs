using System;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x02000025 RID: 37
public class CardInfo : MonoBehaviour
{
	// Token: 0x060000B3 RID: 179 RVA: 0x00005CA0 File Offset: 0x00003EA0
	private void Awake()
	{
		this.sourceCard = CardChoice.instance.GetSourceCard(this);
		this.cardBase = Object.Instantiate<GameObject>(this.cardBase, base.transform.position, base.transform.rotation);
		this.cardBase.transform.SetParent(base.transform, true);
		bool charge = false;
		Gun component = base.GetComponent<Gun>();
		if (component && component.useCharge)
		{
			charge = true;
		}
		this.cardBase.GetComponent<CardInfoDisplayer>().DrawCard(this.cardStats, this.cardName, this.cardDestription, this.sprite, charge);
		this.cardBase.GetComponentInChildren<GeneralParticleSystem>().particleSettings.randomColor = this.cardColor;
	}

	// Token: 0x060000B4 RID: 180 RVA: 0x00005D5B File Offset: 0x00003F5B
	[PunRPC]
	public void RPCA_ChangeSelected(bool setSelected)
	{
		base.GetComponentInChildren<CardVisuals>().ChangeSelected(setSelected);
	}

	// Token: 0x040000A0 RID: 160
	[Header("Sound Settings")]
	public bool soundDisableBlockBasic;

	// Token: 0x040000A1 RID: 161
	[Header("Settings")]
	public string cardName = "";

	// Token: 0x040000A2 RID: 162
	[TextArea]
	public string cardDestription = "";

	// Token: 0x040000A3 RID: 163
	public CardInfoStat[] cardStats;

	// Token: 0x040000A4 RID: 164
	public CardInfo.Rarity rarity;

	// Token: 0x040000A5 RID: 165
	public GameObject cardArt;

	// Token: 0x040000A6 RID: 166
	public Sprite sprite;

	// Token: 0x040000A7 RID: 167
	[HideInInspector]
	public CardInfo sourceCard;

	// Token: 0x040000A8 RID: 168
	public Color cardColor = new Color(0.14509805f, 0.14509805f, 0.14509805f);

	// Token: 0x040000A9 RID: 169
	public CardCategory[] categories;

	// Token: 0x040000AA RID: 170
	[FoldoutGroup("Restrictions", 0)]
	public bool allowMultiple = true;

	// Token: 0x040000AB RID: 171
	[FoldoutGroup("Restrictions", 0)]
	public CardCategory[] blacklistedCategories;

	// Token: 0x040000AC RID: 172
	public GameObject cardBase;

	// Token: 0x040000AD RID: 173
	public CardThemeColor.CardThemeColorType colorTheme;

	// Token: 0x02000335 RID: 821
	public enum Rarity
	{
		// Token: 0x04001052 RID: 4178
		Common,
		// Token: 0x04001053 RID: 4179
		Uncommon,
		// Token: 0x04001054 RID: 4180
		Rare
	}
}
