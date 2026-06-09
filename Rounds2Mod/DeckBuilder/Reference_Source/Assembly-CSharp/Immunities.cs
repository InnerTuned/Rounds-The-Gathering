using System;

// Token: 0x0200018C RID: 396
public class Immunities
{
	// Token: 0x06000807 RID: 2055 RVA: 0x0002BE95 File Offset: 0x0002A095
	public Immunities(float time, float dmg, string name)
	{
		this.time = time;
		this.dmg = dmg;
		this.name = name;
	}

	// Token: 0x0400095F RID: 2399
	public float time;

	// Token: 0x04000960 RID: 2400
	public float dmg;

	// Token: 0x04000961 RID: 2401
	public string name;
}
