using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200019D RID: 413
public class RadarShot : MonoBehaviour
{
	// Token: 0x06000854 RID: 2132 RVA: 0x0002CA3B File Offset: 0x0002AC3B
	private void Start()
	{
		this.wh = base.GetComponentInParent<WeaponHandler>();
		this.player = base.GetComponentInParent<Player>();
	}

	// Token: 0x06000855 RID: 2133 RVA: 0x0002CA58 File Offset: 0x0002AC58
	public void Go()
	{
		Player closestPlayerInTeam = PlayerManager.instance.GetClosestPlayerInTeam(base.transform.position, PlayerManager.instance.GetOtherTeam(this.player.teamID), true);
		if (closestPlayerInTeam && Vector2.Distance(this.player.transform.position, closestPlayerInTeam.transform.position) < this.range)
		{
			base.StartCoroutine(this.FollowTarget(closestPlayerInTeam));
			if (this.player.data.view.IsMine)
			{
				base.StartCoroutine(this.ShootAttacks(closestPlayerInTeam, base.GetComponent<AttackLevel>().attackLevel));
			}
		}
	}

	// Token: 0x06000856 RID: 2134 RVA: 0x0002CB09 File Offset: 0x0002AD09
	private IEnumerator ShootAttacks(Player target, int shots)
	{
		int num;
		for (int i = 0; i < shots; i = num + 1)
		{
			yield return new WaitForSeconds(0.1f);
			this.wh.gun.forceShootDir = this.wh.gun.GetRangeCompensation(Vector3.Distance(target.transform.position, this.player.transform.position)) * Vector3.up + target.transform.position - this.player.transform.position;
			this.wh.gun.Attack(0f, true, 1f, 1f, false);
			this.wh.gun.forceShootDir = Vector3.zero;
			num = i;
		}
		yield break;
	}

	// Token: 0x06000857 RID: 2135 RVA: 0x0002CB26 File Offset: 0x0002AD26
	private IEnumerator FollowTarget(Player target)
	{
		for (int i = 0; i < this.boops.Length; i++)
		{
			this.boops[i].Play();
		}
		float c = 0f;
		while (c < 1f)
		{
			c += TimeHandler.deltaTime;
			for (int j = 0; j < this.boops.Length; j++)
			{
				this.boops[j].transform.position = target.transform.position;
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x04000996 RID: 2454
	private WeaponHandler wh;

	// Token: 0x04000997 RID: 2455
	private Player player;

	// Token: 0x04000998 RID: 2456
	public ParticleSystem[] boops;

	// Token: 0x04000999 RID: 2457
	public float range = 12f;
}
