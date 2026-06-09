using System;
using UnityEngine;

// Token: 0x0200002D RID: 45
public class ChargeFeedback : MonoBehaviour
{
	// Token: 0x060000E4 RID: 228 RVA: 0x000070EC File Offset: 0x000052EC
	private void Start()
	{
		this.gun = base.GetComponentInParent<Gun>();
		this.gun.AddAttackAction(new Action(this.Shoot));
	}

	// Token: 0x060000E5 RID: 229 RVA: 0x00007114 File Offset: 0x00005314
	private void Update()
	{
		this.charge = 0f;
		if (!this.gun.IsReady(0.15f))
		{
			this.charge = 1f;
		}
		this.velocity += (this.charge * this.chargeAngle - this.currentAngle) * CappedDeltaTime.time * this.spring;
		this.velocity -= this.velocity * CappedDeltaTime.time * this.drag;
		this.currentAngle += CappedDeltaTime.time * this.velocity;
		base.transform.localEulerAngles = new Vector3(this.currentAngle, 0f, 0f);
	}

	// Token: 0x060000E6 RID: 230 RVA: 0x000071D0 File Offset: 0x000053D0
	public void Shoot()
	{
		this.velocity += -this.angle * 1000f * (this.gun.damage / 55f);
	}

	// Token: 0x04000123 RID: 291
	private Gun gun;

	// Token: 0x04000124 RID: 292
	public float drag = 1f;

	// Token: 0x04000125 RID: 293
	public float spring = 1f;

	// Token: 0x04000126 RID: 294
	public float angle = 45f;

	// Token: 0x04000127 RID: 295
	public float chargeAngle = 15f;

	// Token: 0x04000128 RID: 296
	private float currentAngle;

	// Token: 0x04000129 RID: 297
	private float velocity;

	// Token: 0x0400012A RID: 298
	public float charge;
}
