using System;
using Photon.Pun;
using Sonigon;
using SoundImplementation;
using UnityEngine;

// Token: 0x020000B9 RID: 185
public class RemoteControl : MonoBehaviour
{
	// Token: 0x060003F0 RID: 1008 RVA: 0x0001814C File Offset: 0x0001634C
	private void OnDestroy()
	{
		if (this.soundRemoteSteeringLoop != null && this.spawned != null && this.spawned.spawner != null && this.soundIsPlaying)
		{
			this.soundIsPlaying = false;
			SoundStaticRemoteControl.remoteControl.AddNumberOf(this.spawned.spawner.transform, -1);
			if (SoundStaticRemoteControl.remoteControl.GetNumberOf(this.spawned.spawner.transform) <= 0)
			{
				SoundManager.Instance.Stop(this.soundRemoteSteeringLoop, this.spawned.spawner.transform, true);
			}
		}
	}

	// Token: 0x060003F1 RID: 1009 RVA: 0x000181F4 File Offset: 0x000163F4
	private void Start()
	{
		this.childRPC = base.GetComponentInParent<ChildRPC>();
		this.view = base.GetComponentInParent<PhotonView>();
		this.hit = base.GetComponentInParent<ProjectileHit>();
		this.move = base.GetComponentInParent<MoveTransform>();
		this.spawned = base.GetComponentInParent<SpawnedAttack>();
		this.startVelocity = this.move.velocity.magnitude;
		this.part = base.GetComponentInChildren<ParticleSystem>();
		base.GetComponentInParent<SyncProjectile>().active = true;
		if (this.soundRemoteSteeringLoop != null && this.spawned != null && this.spawned.spawner != null && !this.soundIsPlaying)
		{
			this.soundIsPlaying = true;
			if (SoundStaticRemoteControl.remoteControl.GetNumberOf(this.spawned.spawner.transform) <= 0)
			{
				SoundManager.Instance.Play(this.soundRemoteSteeringLoop, this.spawned.spawner.transform);
			}
			SoundStaticRemoteControl.remoteControl.AddNumberOf(this.spawned.spawner.transform, 1);
		}
	}

	// Token: 0x060003F2 RID: 1010 RVA: 0x000027C8 File Offset: 0x000009C8
	public void ToggleOn(bool isOn)
	{
	}

	// Token: 0x060003F3 RID: 1011 RVA: 0x00018304 File Offset: 0x00016504
	private void Update()
	{
		if (!this.view.IsMine)
		{
			return;
		}
		Vector3 vector = Vector3.zero;
		if (this.spawned.spawner.data.playerActions.Device != null)
		{
			vector = this.spawned.spawner.data.input.aimDirection;
		}
		else
		{
			vector = MainCam.instance.cam.ScreenToWorldPoint(Input.mousePosition) - base.transform.position;
			vector.z = 0f;
			vector.Normalize();
		}
		vector += Vector3.Cross(Vector3.forward, vector) * this.move.selectedSpread;
		this.c += TimeHandler.deltaTime;
		if (this.snap)
		{
			if (this.spawned.spawner.data.block.blockedThisFrame)
			{
				this.part.Play();
				this.move.velocity = this.move.velocity * -1f;
				base.enabled = false;
				return;
			}
		}
		else
		{
			if (vector.magnitude > 0.2f && this.hit.sinceReflect > 2f)
			{
				this.move.velocity = Vector3.RotateTowards(this.move.velocity, vector.normalized * this.startVelocity, this.rotateSpeed * TimeHandler.deltaTime, this.rotateSpeed * TimeHandler.deltaTime * 10f);
				if (this.c > 0.1f)
				{
					this.boopPart.transform.parent.rotation = Quaternion.LookRotation(vector);
					ParticleSystem particleSystem = this.boopPart;
					if (particleSystem != null)
					{
						particleSystem.Emit(1);
					}
					this.c = 0f;
				}
				if (!this.isOn)
				{
					this.move.simulateGravity++;
				}
				this.isOn = true;
				return;
			}
			if (this.isOn)
			{
				this.move.simulateGravity--;
			}
			this.isOn = false;
		}
	}

	// Token: 0x04000562 RID: 1378
	[Header("Sound")]
	public SoundEvent soundRemoteSteeringLoop;

	// Token: 0x04000563 RID: 1379
	private bool soundIsPlaying;

	// Token: 0x04000564 RID: 1380
	[Header("Settings")]
	public bool snap;

	// Token: 0x04000565 RID: 1381
	public float rotateSpeed = 1f;

	// Token: 0x04000566 RID: 1382
	private SpawnedAttack spawned;

	// Token: 0x04000567 RID: 1383
	private MoveTransform move;

	// Token: 0x04000568 RID: 1384
	private float startVelocity;

	// Token: 0x04000569 RID: 1385
	private bool isOn;

	// Token: 0x0400056A RID: 1386
	private ProjectileHit hit;

	// Token: 0x0400056B RID: 1387
	private ParticleSystem part;

	// Token: 0x0400056C RID: 1388
	public ParticleSystem boopPart;

	// Token: 0x0400056D RID: 1389
	private float c;

	// Token: 0x0400056E RID: 1390
	private PhotonView view;

	// Token: 0x0400056F RID: 1391
	private ChildRPC childRPC;
}
