using System;
using UnityEngine;

// Token: 0x02000066 RID: 102
public class Gravity : MonoBehaviour
{
	// Token: 0x060001FA RID: 506 RVA: 0x0000C31C File Offset: 0x0000A51C
	private void Start()
	{
		this.data = base.GetComponent<CharacterData>();
		this.rig = base.GetComponent<PlayerVelocity>();
	}

	// Token: 0x060001FB RID: 507 RVA: 0x0000C338 File Offset: 0x0000A538
	private void FixedUpdate()
	{
		float num = this.data.sinceGrounded;
		if (this.data.sinceWallGrab < num)
		{
			num = this.data.sinceWallGrab;
		}
		if (num > 0f)
		{
			this.rig.AddForce(Vector3.down * TimeHandler.timeScale * Mathf.Pow(num, this.exponent) * this.gravityForce * this.rig.mass, 0);
			return;
		}
		this.rig.AddForce(Vector3.down * TimeHandler.timeScale * num * this.gravityForce * this.rig.mass, 0);
	}

	// Token: 0x040002A1 RID: 673
	public float gravityForce;

	// Token: 0x040002A2 RID: 674
	public float exponent = 1f;

	// Token: 0x040002A3 RID: 675
	private PlayerVelocity rig;

	// Token: 0x040002A4 RID: 676
	private CharacterData data;
}
