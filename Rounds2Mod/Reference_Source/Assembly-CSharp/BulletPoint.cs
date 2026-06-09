using System;
using UnityEngine;

// Token: 0x02000102 RID: 258
public class BulletPoint : MonoBehaviour
{
	// Token: 0x06000516 RID: 1302 RVA: 0x0001CF46 File Offset: 0x0001B146
	private void Start()
	{
		this.gun = base.GetComponent<Gun>();
		Gun gun = this.gun;
		gun.ShootPojectileAction = (Action<GameObject>)Delegate.Combine(gun.ShootPojectileAction, new Action<GameObject>(this.Fire));
	}

	// Token: 0x06000517 RID: 1303 RVA: 0x0001CF7B File Offset: 0x0001B17B
	private void Attack()
	{
		this.gun.Attack(1f, true, 1f, 1f, true);
	}

	// Token: 0x06000518 RID: 1304 RVA: 0x0001CF9A File Offset: 0x0001B19A
	private void Fire(GameObject projectile)
	{
		this.move = projectile.GetComponent<MoveTransform>();
	}

	// Token: 0x06000519 RID: 1305 RVA: 0x0001CFA8 File Offset: 0x0001B1A8
	private void Update()
	{
		if (this.move)
		{
			this.counter = 0f;
			Vector3 v = base.transform.position - base.transform.root.position;
			RaycastHit2D raycastHit2D = Physics2D.Raycast(base.transform.root.position, v, v.magnitude, this.mask, -10000f);
			Vector3 a = base.transform.position;
			if (raycastHit2D.transform != null)
			{
				a = raycastHit2D.point - v.normalized * 0.1f;
			}
			this.move.velocity = Vector2.Lerp(this.move.velocity, (a - this.move.transform.position) * 50f, TimeHandler.deltaTime * 50f);
			return;
		}
		this.counter += TimeHandler.deltaTime;
		if (this.counter > 2f)
		{
			this.Attack();
		}
	}

	// Token: 0x040006AF RID: 1711
	public LayerMask mask;

	// Token: 0x040006B0 RID: 1712
	private Gun gun;

	// Token: 0x040006B1 RID: 1713
	private MoveTransform move;

	// Token: 0x040006B2 RID: 1714
	private ProjectileHit hit;

	// Token: 0x040006B3 RID: 1715
	private float counter = 5f;
}
