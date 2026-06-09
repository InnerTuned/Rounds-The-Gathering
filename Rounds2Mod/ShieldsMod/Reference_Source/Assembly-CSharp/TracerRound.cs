using System;
using UnityEngine;

// Token: 0x020001C0 RID: 448
public class TracerRound : MonoBehaviour
{
	// Token: 0x060008DA RID: 2266 RVA: 0x0002E804 File Offset: 0x0002CA04
	private void Start()
	{
		this.move = base.GetComponent<MoveTransform>();
		this.bullet = Object.Instantiate<GameObject>(this.bullet, this.bulletSpawnPos.transform.position, this.bulletSpawnPos.transform.rotation);
		this.bullet.GetComponent<ProjectileHit>().damage *= base.transform.localScale.x;
		base.GetComponentInParent<SpawnedAttack>().CopySpawnedAttackTo(this.bullet);
		this.target = this.bullet.GetComponent<TracerTarget>();
		base.GetComponentInParent<ProjectileHit>().AddHitActionWithData(new Action<HitInfo>(this.Hit));
		this.bullet.GetComponentInParent<ProjectileHit>().AddHitActionWithData(new Action<HitInfo>(this.Hit));
	}

	// Token: 0x060008DB RID: 2267 RVA: 0x0002E8CC File Offset: 0x0002CACC
	public void Hit(HitInfo hit)
	{
		if (hit.transform.root.GetComponent<Player>())
		{
			base.transform.SetParent(null);
			base.gameObject.AddComponent<RemoveAfterSeconds>().seconds = 10f;
			base.gameObject.AddComponent<FollowTransform>().target = hit.transform;
		}
	}

	// Token: 0x060008DC RID: 2268 RVA: 0x0002E928 File Offset: 0x0002CB28
	private void Update()
	{
		if (this.target != null)
		{
			this.target.SetPos(base.transform.position, Vector3.Cross(Vector3.forward, base.transform.forward), this.move);
		}
		if (this.bullet == null && !this.done)
		{
			this.done = true;
			base.GetComponentInChildren<ParticleSystem>().Stop();
		}
	}

	// Token: 0x04000A1A RID: 2586
	public GameObject bullet;

	// Token: 0x04000A1B RID: 2587
	public GameObject bulletSpawnPos;

	// Token: 0x04000A1C RID: 2588
	private TracerTarget target;

	// Token: 0x04000A1D RID: 2589
	private MoveTransform move;

	// Token: 0x04000A1E RID: 2590
	private bool done;
}
