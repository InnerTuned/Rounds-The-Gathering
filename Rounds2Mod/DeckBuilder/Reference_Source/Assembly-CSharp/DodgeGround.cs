using System;
using Photon.Pun;
using Sonigon;
using UnityEngine;

// Token: 0x0200004B RID: 75
public class DodgeGround : MonoBehaviour
{
	// Token: 0x0600016C RID: 364 RVA: 0x000092D8 File Offset: 0x000074D8
	private void Start()
	{
		this.spawned = base.GetComponentInParent<SpawnedAttack>();
		this.move = base.GetComponentInParent<MoveTransform>();
		base.GetComponentInParent<SyncProjectile>().active = true;
		if (this.spawned != null && this.spawned.spawner != null)
		{
			this.spawnedSpawnerTransform = this.spawned.spawner.transform;
		}
	}

	// Token: 0x0600016D RID: 365 RVA: 0x00009340 File Offset: 0x00007540
	private void SoundStart()
	{
		if (!this.soundIsPlaying && this.spawnedSpawnerTransform != null)
		{
			this.soundIsPlaying = true;
			SoundManager.Instance.PlayAtPosition(this.soundSneakyDodgeGroundLoop, this.spawnedSpawnerTransform, base.transform);
		}
	}

	// Token: 0x0600016E RID: 366 RVA: 0x0000937B File Offset: 0x0000757B
	private void SoundStop()
	{
		if (this.soundIsPlaying && this.spawnedSpawnerTransform != null)
		{
			this.soundIsPlaying = false;
			SoundManager.Instance.StopAtPosition(this.soundSneakyDodgeGroundLoop, base.transform, true);
		}
	}

	// Token: 0x0600016F RID: 367 RVA: 0x000093B1 File Offset: 0x000075B1
	private void OnDestroy()
	{
		this.SoundStop();
	}

	// Token: 0x06000170 RID: 368 RVA: 0x000093BC File Offset: 0x000075BC
	private void Update()
	{
		float magnitude = this.move.velocity.magnitude;
		this.c3 += TimeHandler.deltaTime;
		this.l1.enabled = false;
		this.l2.enabled = false;
		bool flag = false;
		float d = 1f;
		RaycastHit2D raycastHit2D = Physics2D.Raycast(base.transform.position, base.transform.forward + base.transform.right * this.rayUp, this.rayLength, this.mask);
		if (raycastHit2D && raycastHit2D.transform && !raycastHit2D.collider.GetComponent<Damagable>() && raycastHit2D.transform.gameObject.layer != 10)
		{
			flag = true;
		}
		bool flag2 = false;
		RaycastHit2D raycastHit2D2 = Physics2D.Raycast(base.transform.position, base.transform.forward + base.transform.right * -this.rayUp, this.rayLength, this.mask);
		if (raycastHit2D2 && raycastHit2D2.transform && !raycastHit2D2.collider.GetComponent<Damagable>() && raycastHit2D2.transform.gameObject.layer != 10)
		{
			flag2 = true;
		}
		if (flag && flag2 && raycastHit2D.transform == raycastHit2D2.transform)
		{
			if (raycastHit2D.distance < raycastHit2D2.distance)
			{
				flag2 = false;
			}
			else
			{
				flag = false;
			}
		}
		if (flag)
		{
			this.move.velocity += raycastHit2D.normal * this.force * this.move.velocity.magnitude * d * TimeHandler.deltaTime;
			this.l1.enabled = true;
			this.l1.SetPosition(0, base.transform.position);
			this.l1.SetPosition(1, raycastHit2D.point);
		}
		if (flag2)
		{
			this.move.velocity += raycastHit2D2.normal * this.force * this.move.velocity.magnitude * d * TimeHandler.deltaTime;
			this.l2.enabled = true;
			this.l2.SetPosition(0, base.transform.position);
			this.l2.SetPosition(1, raycastHit2D2.point);
		}
		if (flag || flag2)
		{
			this.soundTimeCurrent = this.soundTimeToStopPlaying;
			this.SoundStart();
		}
		else if (this.soundIsPlaying)
		{
			this.soundTimeCurrent -= TimeHandler.deltaTime;
			if (this.soundTimeCurrent < 0f)
			{
				this.SoundStop();
			}
		}
		this.move.velocity = this.move.velocity.normalized * magnitude;
		if (flag && this.c3 > 0.05f)
		{
			this.c3 = 0f;
			this.p3.Emit(1);
		}
	}

	// Token: 0x040001D1 RID: 465
	[Header("Sound")]
	public SoundEvent soundSneakyDodgeGroundLoop;

	// Token: 0x040001D2 RID: 466
	private bool soundIsPlaying;

	// Token: 0x040001D3 RID: 467
	private float soundTimeToStopPlaying = 0.5f;

	// Token: 0x040001D4 RID: 468
	private float soundTimeCurrent;

	// Token: 0x040001D5 RID: 469
	private Transform spawnedSpawnerTransform;

	// Token: 0x040001D6 RID: 470
	[Header("Settings")]
	public float rayLength = 3f;

	// Token: 0x040001D7 RID: 471
	public float rayUp = 0.3f;

	// Token: 0x040001D8 RID: 472
	public float force;

	// Token: 0x040001D9 RID: 473
	public LayerMask mask;

	// Token: 0x040001DA RID: 474
	private MoveTransform move;

	// Token: 0x040001DB RID: 475
	private SpawnedAttack spawned;

	// Token: 0x040001DC RID: 476
	public ParticleSystem p1;

	// Token: 0x040001DD RID: 477
	public ParticleSystem p2;

	// Token: 0x040001DE RID: 478
	public ParticleSystem p3;

	// Token: 0x040001DF RID: 479
	public LineRenderer l1;

	// Token: 0x040001E0 RID: 480
	public LineRenderer l2;

	// Token: 0x040001E1 RID: 481
	private float c3;
}
