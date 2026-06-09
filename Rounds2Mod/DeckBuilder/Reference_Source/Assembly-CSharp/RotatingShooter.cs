using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x020000BE RID: 190
public class RotatingShooter : MonoBehaviour
{
	// Token: 0x06000400 RID: 1024 RVA: 0x000186E8 File Offset: 0x000168E8
	private void Start()
	{
		this.level = base.GetComponentInParent<AttackLevel>();
		this.gun = base.GetComponent<Gun>();
		if (this.disableTrailRenderer)
		{
			base.transform.root.GetComponentInChildren<TrailRenderer>(true).enabled = false;
		}
	}

	// Token: 0x06000401 RID: 1025 RVA: 0x00018724 File Offset: 0x00016924
	public void Attack()
	{
		this.currentBulletsToFire = this.bulletsToFire;
		if (this.level)
		{
			this.currentBulletsToFire = this.bulletsToFire * this.level.attackLevel;
		}
		this.degreesPerBullet = 360f / (float)this.currentBulletsToFire;
		base.StartCoroutine(this.RotateAndShoot());
	}

	// Token: 0x06000402 RID: 1026 RVA: 0x00018782 File Offset: 0x00016982
	private IEnumerator RotateAndShoot()
	{
		int shotsToMake = Mathf.Clamp(Mathf.RoundToInt(0.5f / this.gun.attackSpeed), 1, 5);
		int num;
		for (int i = 0; i < shotsToMake; i = num + 1)
		{
			for (int j = this.currentBulletsToFire; j > 0; j--)
			{
				base.transform.localEulerAngles = new Vector3(0f, (float)j * this.degreesPerBullet, 0f);
				this.gun.Attack(this.gun.currentCharge, true, 1f, 1f, true);
			}
			if (shotsToMake > i)
			{
				yield return new WaitForSeconds(0.1f);
			}
			num = i;
		}
		base.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		yield return null;
		if (this.destroyAfterAttack)
		{
			Object.Destroy(base.transform.root.gameObject);
		}
		yield break;
	}

	// Token: 0x0400057A RID: 1402
	private Gun gun;

	// Token: 0x0400057B RID: 1403
	[HideInInspector]
	public float charge;

	// Token: 0x0400057C RID: 1404
	public int bulletsToFire = 10;

	// Token: 0x0400057D RID: 1405
	private int currentBulletsToFire = 10;

	// Token: 0x0400057E RID: 1406
	private float degreesPerBullet;

	// Token: 0x0400057F RID: 1407
	[FoldoutGroup("Weird settings", 0)]
	public bool destroyAfterAttack = true;

	// Token: 0x04000580 RID: 1408
	[FoldoutGroup("Weird settings", 0)]
	public bool disableTrailRenderer = true;

	// Token: 0x04000581 RID: 1409
	private AttackLevel level;
}
