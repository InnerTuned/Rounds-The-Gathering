using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000154 RID: 340
public class HealthBar : MonoBehaviour
{
	// Token: 0x060006E1 RID: 1761 RVA: 0x00026204 File Offset: 0x00024404
	private void Start()
	{
		this.data = base.GetComponentInParent<CharacterData>();
		CharacterStatModifiers componentInParent = base.GetComponentInParent<CharacterStatModifiers>();
		componentInParent.WasDealtDamageAction = (Action<Vector2, bool>)Delegate.Combine(componentInParent.WasDealtDamageAction, new Action<Vector2, bool>(this.TakeDamage));
	}

	// Token: 0x060006E2 RID: 1762 RVA: 0x0002623C File Offset: 0x0002443C
	private void Update()
	{
		this.hpTarg = this.data.health / this.data.maxHealth;
		this.sinceDamage += TimeHandler.deltaTime;
		this.hpVel = FRILerp.Lerp(this.hpVel, (this.hpTarg - this.hpCur) * this.spring, this.drag);
		this.whiteVel = FRILerp.Lerp(this.whiteVel, (this.whiteTarg - this.whiteCur) * this.spring, this.drag);
		this.hpCur += this.hpVel * TimeHandler.deltaTime;
		this.whiteCur += this.whiteVel * TimeHandler.deltaTime;
		this.hp.fillAmount = this.hpCur;
		this.white.fillAmount = this.whiteCur;
		if (this.sinceDamage > 0.5f)
		{
			this.whiteTarg = this.hpTarg;
		}
	}

	// Token: 0x060006E3 RID: 1763 RVA: 0x0002633B File Offset: 0x0002453B
	public void TakeDamage(Vector2 dmg, bool selfDmg)
	{
		this.sinceDamage = 0f;
	}

	// Token: 0x04000848 RID: 2120
	public Image hp;

	// Token: 0x04000849 RID: 2121
	public Image white;

	// Token: 0x0400084A RID: 2122
	private float drag = 25f;

	// Token: 0x0400084B RID: 2123
	private float spring = 25f;

	// Token: 0x0400084C RID: 2124
	private float hpCur;

	// Token: 0x0400084D RID: 2125
	private float hpVel;

	// Token: 0x0400084E RID: 2126
	private float hpTarg;

	// Token: 0x0400084F RID: 2127
	private float whiteCur;

	// Token: 0x04000850 RID: 2128
	private float whiteVel;

	// Token: 0x04000851 RID: 2129
	private float whiteTarg;

	// Token: 0x04000852 RID: 2130
	private float sinceDamage;

	// Token: 0x04000853 RID: 2131
	private CharacterData data;
}
