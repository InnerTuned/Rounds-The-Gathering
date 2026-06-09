using System;
using UnityEngine;

// Token: 0x02000124 RID: 292
public class CooldownWindUp : MonoBehaviour
{
	// Token: 0x060005B7 RID: 1463 RVA: 0x000208E1 File Offset: 0x0001EAE1
	private void Start()
	{
		this.cooldown = base.GetComponent<CooldownCondition>();
		this.startValue = this.cooldown.cooldown;
	}

	// Token: 0x060005B8 RID: 1464 RVA: 0x00020900 File Offset: 0x0001EB00
	private void Update()
	{
		this.currentMultiplier = this.multiplierCurve.Evaluate(this.currentValue);
		this.currentValue = Mathf.Clamp(this.currentValue, 0f, 100f);
		this.cooldown.cooldown = this.startValue / this.currentMultiplier;
	}

	// Token: 0x060005B9 RID: 1465 RVA: 0x00020957 File Offset: 0x0001EB57
	public void Reset()
	{
		this.currentValue = 0f;
	}

	// Token: 0x060005BA RID: 1466 RVA: 0x00020964 File Offset: 0x0001EB64
	public void Add()
	{
		this.currentValue += this.increasePerShot / this.currentMultiplier;
	}

	// Token: 0x04000757 RID: 1879
	public AnimationCurve multiplierCurve;

	// Token: 0x04000758 RID: 1880
	private float currentMultiplier = 1f;

	// Token: 0x04000759 RID: 1881
	private float startValue;

	// Token: 0x0400075A RID: 1882
	private float currentValue;

	// Token: 0x0400075B RID: 1883
	public float increasePerShot = 1f;

	// Token: 0x0400075C RID: 1884
	private CooldownCondition cooldown;
}
