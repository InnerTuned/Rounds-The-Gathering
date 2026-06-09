using System;
using UnityEngine;

// Token: 0x02000023 RID: 35
[Serializable]
public class CardThemeColor
{
	// Token: 0x0400009A RID: 154
	public CardThemeColor.CardThemeColorType themeType;

	// Token: 0x0400009B RID: 155
	public Color targetColor;

	// Token: 0x0400009C RID: 156
	public Color bgColor;

	// Token: 0x02000334 RID: 820
	public enum CardThemeColorType
	{
		// Token: 0x04001048 RID: 4168
		DestructiveRed,
		// Token: 0x04001049 RID: 4169
		FirepowerYellow,
		// Token: 0x0400104A RID: 4170
		DefensiveBlue,
		// Token: 0x0400104B RID: 4171
		TechWhite,
		// Token: 0x0400104C RID: 4172
		EvilPurple,
		// Token: 0x0400104D RID: 4173
		PoisonGreen,
		// Token: 0x0400104E RID: 4174
		NatureBrown,
		// Token: 0x0400104F RID: 4175
		ColdBlue,
		// Token: 0x04001050 RID: 4176
		MagicPink
	}
}
