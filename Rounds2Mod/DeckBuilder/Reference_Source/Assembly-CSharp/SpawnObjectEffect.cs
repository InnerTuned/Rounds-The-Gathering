using System;
using Sirenix.OdinInspector;
using UnityEngine;

// Token: 0x020000D9 RID: 217
public class SpawnObjectEffect : MonoBehaviour
{
	// Token: 0x0600045F RID: 1119 RVA: 0x0001A075 File Offset: 0x00018275
	private void Start()
	{
		this.holding = base.GetComponentInParent<Holding>();
		this.player = base.GetComponentInParent<Player>();
	}

	// Token: 0x06000460 RID: 1120 RVA: 0x0001A090 File Offset: 0x00018290
	public void DoEffect(Vector2 dmg)
	{
		int num = 0;
		if (this.spawnAttack)
		{
			for (int i = 0; i < this.numberOfattacks; i++)
			{
				if (!this.gun && this.holding && this.holding.holdable)
				{
					this.gun = this.holding.holdable.GetComponent<Gun>();
				}
				if (this.gun)
				{
					Quaternion rotation = this.gun.transform.rotation;
					Vector3 a = (PlayerManager.instance.GetOtherPlayer(this.player).transform.position - base.transform.position).normalized;
					if (this.dir == SpawnObjectEffect.Dir.DamageDirction)
					{
						a = dmg.normalized;
					}
					if (this.dir == SpawnObjectEffect.Dir.ReverseDamageDir)
					{
						a = -dmg.normalized;
					}
					if (this.dir == SpawnObjectEffect.Dir.TowardsRecentlyDamaged)
					{
						a = (this.player.data.lastDamagedPlayer.transform.position - base.transform.position).normalized;
					}
					this.gun.transform.rotation = Quaternion.LookRotation(Vector3.forward, a + this.gun.transform.right * Random.Range(-this.spread, this.spread));
					this.gun.projectileSpeed *= this.speedMultiplier;
					this.gun.gravity *= this.gravityMultiplier;
					this.gun.spread += this.extraSpread;
					this.gun.Attack(1f, true, this.bulletDamageMultiplier, this.recoilMultiplier, true);
					num += (int)((float)this.gun.numberOfProjectiles + this.gun.chargeNumberOfProjectilesTo);
					this.gun.projectileSpeed /= this.speedMultiplier;
					this.gun.gravity /= this.gravityMultiplier;
					this.gun.spread -= this.extraSpread;
					this.gun.transform.rotation = rotation;
					if (num > this.maxBullets)
					{
						break;
					}
				}
			}
		}
	}

	// Token: 0x040005D6 RID: 1494
	public SpawnObjectEffect.Dir dir;

	// Token: 0x040005D7 RID: 1495
	[FoldoutGroup("Gun", 0)]
	public bool spawnAttack;

	// Token: 0x040005D8 RID: 1496
	[FoldoutGroup("Gun", 0)]
	public float bulletDamageMultiplier = 0.5f;

	// Token: 0x040005D9 RID: 1497
	[FoldoutGroup("Gun", 0)]
	public int numberOfattacks = 1;

	// Token: 0x040005DA RID: 1498
	[FoldoutGroup("Gun", 0)]
	public float spread;

	// Token: 0x040005DB RID: 1499
	[FoldoutGroup("Gun", 0)]
	public float gravityMultiplier = 1f;

	// Token: 0x040005DC RID: 1500
	[FoldoutGroup("Gun", 0)]
	public float speedMultiplier = 1f;

	// Token: 0x040005DD RID: 1501
	[FoldoutGroup("Gun", 0)]
	public float extraSpread;

	// Token: 0x040005DE RID: 1502
	[FoldoutGroup("Gun", 0)]
	public float recoilMultiplier = 1f;

	// Token: 0x040005DF RID: 1503
	[FoldoutGroup("Gun", 0)]
	public int maxBullets = 100;

	// Token: 0x040005E0 RID: 1504
	private Holding holding;

	// Token: 0x040005E1 RID: 1505
	private Gun gun;

	// Token: 0x040005E2 RID: 1506
	private Player player;

	// Token: 0x0200037A RID: 890
	public enum Dir
	{
		// Token: 0x040011B5 RID: 4533
		TowardsEnemy,
		// Token: 0x040011B6 RID: 4534
		DamageDirction,
		// Token: 0x040011B7 RID: 4535
		ReverseDamageDir,
		// Token: 0x040011B8 RID: 4536
		TowardsRecentlyDamaged
	}
}
