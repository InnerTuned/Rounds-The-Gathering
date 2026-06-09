using System;

// Token: 0x02000026 RID: 38
[Serializable]
public class CardInfoStat
{
	// Token: 0x060000B6 RID: 182 RVA: 0x00005DA8 File Offset: 0x00003FA8
	public string GetSimpleAmount()
	{
		string result = "";
		if (this.simepleAmount == CardInfoStat.SimpleAmount.aLittleBitOf)
		{
			result = "Slightly more ";
		}
		if (this.simepleAmount == CardInfoStat.SimpleAmount.Some)
		{
			result = "More ";
		}
		if (this.simepleAmount == CardInfoStat.SimpleAmount.aLotOf)
		{
			result = "A bunch more ";
		}
		if (this.simepleAmount == CardInfoStat.SimpleAmount.aHugeAmountOf)
		{
			result = "A huge amount of";
		}
		if (this.simepleAmount == CardInfoStat.SimpleAmount.slightlyLower)
		{
			result = "Slightly lower ";
		}
		if (this.simepleAmount == CardInfoStat.SimpleAmount.lower)
		{
			result = "Lower ";
		}
		if (this.simepleAmount == CardInfoStat.SimpleAmount.aLotLower)
		{
			result = "A lot lower";
		}
		if (this.simepleAmount == CardInfoStat.SimpleAmount.smaller)
		{
			result = "Smaller";
		}
		if (this.simepleAmount == CardInfoStat.SimpleAmount.slightlySmaller)
		{
			result = "Slightly smaller";
		}
		return result;
	}

	// Token: 0x040000AE RID: 174
	public bool positive;

	// Token: 0x040000AF RID: 175
	public string amount;

	// Token: 0x040000B0 RID: 176
	public CardInfoStat.SimpleAmount simepleAmount;

	// Token: 0x040000B1 RID: 177
	public string stat;

	// Token: 0x02000336 RID: 822
	public enum SimpleAmount
	{
		// Token: 0x04001056 RID: 4182
		notAssigned,
		// Token: 0x04001057 RID: 4183
		aLittleBitOf,
		// Token: 0x04001058 RID: 4184
		Some,
		// Token: 0x04001059 RID: 4185
		aLotOf,
		// Token: 0x0400105A RID: 4186
		aHugeAmountOf,
		// Token: 0x0400105B RID: 4187
		slightlyLower,
		// Token: 0x0400105C RID: 4188
		lower,
		// Token: 0x0400105D RID: 4189
		aLotLower,
		// Token: 0x0400105E RID: 4190
		slightlySmaller,
		// Token: 0x0400105F RID: 4191
		smaller
	}
}
