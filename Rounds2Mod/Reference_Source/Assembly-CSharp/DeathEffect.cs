using System;
using System.Collections;
using Sonigon;
using Sonigon.Internal;
using UnityEngine;

// Token: 0x02000043 RID: 67
public class DeathEffect : MonoBehaviour
{
	// Token: 0x06000142 RID: 322 RVA: 0x00008AAC File Offset: 0x00006CAC
	private void Update()
	{
	}

	// Token: 0x06000143 RID: 323 RVA: 0x00008ABC File Offset: 0x00006CBC
	public void PlayDeath(Color color, PlayerVelocity playerRig, Vector2 vel, int playerIDToRevive = -1)
	{
		if (vel.magnitude < 30f)
		{
			vel = vel.normalized * 30f;
		}
		vel *= 1f;
		this.parts = base.GetComponentsInChildren<ParticleSystem>();
		for (int i = 0; i < this.parts.Length; i++)
		{
			ParticleSystem.MainModule main = this.parts[i].main;
			if (this.parts[i].name.Contains("ROT"))
			{
				this.parts[i].transform.rotation = Quaternion.LookRotation(vel);
			}
		}
		this.partToColor.main.startColor = PlayerSkinBank.GetPlayerSkinColors(playerRig.GetComponent<Player>().playerID).color;
		for (int j = 0; j < this.partsToColor.Length; j++)
		{
			this.partsToColor[j].main.startColor = PlayerSkinBank.GetPlayerSkinColors(playerRig.GetComponent<Player>().playerID).color;
		}
		if (playerIDToRevive != -1)
		{
			SoundManager.Instance.Play(this.soundPhoenixActivate, base.transform);
			SoundManager.Instance.Play(this.soundPhoenixChargeLoop, base.transform, new SoundParameterBase[]
			{
				this.soundParameterChargeLoopIntensity
			});
			base.StartCoroutine(this.RespawnPlayer(playerIDToRevive));
		}
	}

	// Token: 0x06000144 RID: 324 RVA: 0x00008C18 File Offset: 0x00006E18
	private IEnumerator RespawnPlayer(int playerIDToRevive = -1)
	{
		while (this.respawnTimeCurrent < this.respawnTime)
		{
			this.soundParameterChargeLoopIntensity.intensity = this.respawnTimeCurrent / this.respawnTime;
			this.respawnTimeCurrent += 0.1f;
			yield return new WaitForSeconds(0.1f);
		}
		SoundManager.Instance.Play(this.soundPhoenixRespawn, base.transform);
		SoundManager.Instance.Stop(this.soundPhoenixChargeLoop, base.transform, true);
		PlayerManager.instance.players[playerIDToRevive].data.healthHandler.Revive(false);
		PlayerManager.instance.players[playerIDToRevive].data.block.RPCA_DoBlock(true, false, BlockTrigger.BlockTriggerType.Default, default(Vector3), false);
		yield break;
	}

	// Token: 0x06000145 RID: 325 RVA: 0x00008C2E File Offset: 0x00006E2E
	private IEnumerator DoEffect(Rigidbody2D rig)
	{
		yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
		rig.GetComponent<SpriteRenderer>().color = this.baseColor;
		yield break;
	}

	// Token: 0x040001B1 RID: 433
	[Header("Sounds")]
	public SoundEvent soundPhoenixActivate;

	// Token: 0x040001B2 RID: 434
	public SoundEvent soundPhoenixChargeLoop;

	// Token: 0x040001B3 RID: 435
	public SoundEvent soundPhoenixRespawn;

	// Token: 0x040001B4 RID: 436
	private SoundParameterIntensity soundParameterChargeLoopIntensity = new SoundParameterIntensity(0f, 0);

	// Token: 0x040001B5 RID: 437
	[Header("Settings")]
	public float forceMulti = 1f;

	// Token: 0x040001B6 RID: 438
	public float minScale = 0.9f;

	// Token: 0x040001B7 RID: 439
	public float maxScale = 1.1f;

	// Token: 0x040001B8 RID: 440
	public float minDrag = 0.9f;

	// Token: 0x040001B9 RID: 441
	public float maxDrag = 1.1f;

	// Token: 0x040001BA RID: 442
	public float minForce = 0.9f;

	// Token: 0x040001BB RID: 443
	public float maxForce = 1.1f;

	// Token: 0x040001BC RID: 444
	public float spread = 0.5f;

	// Token: 0x040001BD RID: 445
	private Rigidbody2D[] rigs;

	// Token: 0x040001BE RID: 446
	private Color baseColor;

	// Token: 0x040001BF RID: 447
	private ParticleSystem[] parts;

	// Token: 0x040001C0 RID: 448
	public ParticleSystem partToColor;

	// Token: 0x040001C1 RID: 449
	public ParticleSystem[] partsToColor;

	// Token: 0x040001C2 RID: 450
	private float respawnTimeCurrent;

	// Token: 0x040001C3 RID: 451
	private float respawnTime = 2.53f;
}
