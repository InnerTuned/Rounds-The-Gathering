using System;
using UnityEngine;

// Token: 0x0200006D RID: 109
public class Holding : MonoBehaviour
{
	// Token: 0x06000235 RID: 565 RVA: 0x0000E0E4 File Offset: 0x0000C2E4
	public void Awake()
	{
		if (this.hasSpawnedGun)
		{
			Object.Destroy(this.holdable.gameObject);
		}
		this.hasSpawnedGun = true;
		this.holdable = Object.Instantiate<Holdable>(this.holdable, base.transform.position, Quaternion.identity);
		this.player = base.GetComponent<Player>();
		this.holdable.GetComponent<Holdable>().holder = this.player.data;
		this.handPos = base.GetComponentInChildren<HandPos>().transform;
		this.rig = base.GetComponent<PlayerVelocity>();
		this.input = base.GetComponent<GeneralInput>();
		this.data = base.GetComponent<CharacterData>();
		if (this.holdable)
		{
			this.gun = this.holdable.GetComponent<Gun>();
			base.GetComponentInChildren<WeaponHandler>().gun = this.holdable.GetComponent<Gun>();
		}
	}

	// Token: 0x06000236 RID: 566 RVA: 0x0000E1C1 File Offset: 0x0000C3C1
	private void Start()
	{
		if (this.holdable)
		{
			this.holdable.SetTeamColors(PlayerSkinBank.GetPlayerSkinColors(this.player.playerID), this.player);
		}
	}

	// Token: 0x06000237 RID: 567 RVA: 0x0000E1F4 File Offset: 0x0000C3F4
	private void FixedUpdate()
	{
		if (this.holdable && this.holdable.rig)
		{
			this.holdable.rig.AddForce((this.handPos.transform.position + this.rig.velocity * 0.04f - this.holdable.transform.position) * this.force * this.holdable.rig.mass, 0);
			this.holdable.rig.AddForce(this.holdable.rig.velocity * -this.drag * this.holdable.rig.mass, 0);
			this.holdable.rig.transform.rotation = Quaternion.LookRotation(Vector3.forward, this.handPos.transform.forward);
		}
	}

	// Token: 0x06000238 RID: 568 RVA: 0x000027C8 File Offset: 0x000009C8
	private void Update()
	{
	}

	// Token: 0x06000239 RID: 569 RVA: 0x0000E311 File Offset: 0x0000C511
	private void OnDestroy()
	{
		if (this.holdable)
		{
			Object.Destroy(this.holdable.gameObject);
		}
	}

	// Token: 0x04000317 RID: 791
	public float force;

	// Token: 0x04000318 RID: 792
	public float drag;

	// Token: 0x04000319 RID: 793
	public Holdable holdable;

	// Token: 0x0400031A RID: 794
	private Transform handPos;

	// Token: 0x0400031B RID: 795
	private PlayerVelocity rig;

	// Token: 0x0400031C RID: 796
	private GeneralInput input;

	// Token: 0x0400031D RID: 797
	private CharacterData data;

	// Token: 0x0400031E RID: 798
	private Player player;

	// Token: 0x0400031F RID: 799
	private Gun gun;

	// Token: 0x04000320 RID: 800
	private bool hasSpawnedGun;
}
