using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000093 RID: 147
public class PlayerAPI : MonoBehaviour
{
	// Token: 0x06000327 RID: 807 RVA: 0x000141BF File Offset: 0x000123BF
	private void Awake()
	{
		this.player = base.GetComponent<Player>();
		this.data = base.GetComponent<CharacterData>();
		this.input = base.GetComponent<GeneralInput>();
	}

	// Token: 0x06000328 RID: 808 RVA: 0x000141E5 File Offset: 0x000123E5
	public void Move(Vector2 direction)
	{
		direction = Vector2.ClampMagnitude(direction, 1f);
		this.movedThisFrame = true;
		this.data.input.direction = direction;
	}

	// Token: 0x06000329 RID: 809 RVA: 0x00014211 File Offset: 0x00012411
	public void Jump()
	{
		this.data.jump.Jump(false, 1f);
	}

	// Token: 0x0600032A RID: 810 RVA: 0x00014229 File Offset: 0x00012429
	public void Attack()
	{
		this.attackedThisFrame = true;
		this.data.input.shootWasPressed = true;
		this.data.input.shootIsPressed = true;
	}

	// Token: 0x0600032B RID: 811 RVA: 0x00014254 File Offset: 0x00012454
	public void Block()
	{
		this.data.input.shieldWasPressed = true;
	}

	// Token: 0x0600032C RID: 812 RVA: 0x00014267 File Offset: 0x00012467
	public void SetAimDirection(Vector2 direction)
	{
		this.data.input.aimDirection = direction;
	}

	// Token: 0x0600032D RID: 813 RVA: 0x00014280 File Offset: 0x00012480
	public void AimForOtherPlayer()
	{
		Player otherPlayer = PlayerManager.instance.GetOtherPlayer(this.player);
		if (otherPlayer)
		{
			this.data.input.aimDirection = otherPlayer.transform.position - base.transform.position;
		}
	}

	// Token: 0x0600032E RID: 814 RVA: 0x000142D4 File Offset: 0x000124D4
	public Vector2 TowardsOtherPlayer()
	{
		if (PlayerManager.instance.players.Count < 2)
		{
			return Vector2.zero;
		}
		return PlayerManager.instance.GetOtherPlayer(this.player).transform.position - base.transform.position;
	}

	// Token: 0x0600032F RID: 815 RVA: 0x00014328 File Offset: 0x00012528
	public RaycastHit2D RayCastDirection(Vector2 direction, float distance)
	{
		return Physics2D.Raycast(base.transform.position, direction, distance);
	}

	// Token: 0x06000330 RID: 816 RVA: 0x00014341 File Offset: 0x00012541
	public bool CheckGroundBelow(Vector2 pos, float range)
	{
		return this.data.ThereIsGroundBelow(pos, range);
	}

	// Token: 0x06000331 RID: 817 RVA: 0x00014355 File Offset: 0x00012555
	public bool CanBlock()
	{
		return !this.data.block.IsOnCD();
	}

	// Token: 0x06000332 RID: 818 RVA: 0x0001436A File Offset: 0x0001256A
	public Player GetOtherPlayer()
	{
		return PlayerManager.instance.GetOtherPlayer(this.player);
	}

	// Token: 0x06000333 RID: 819 RVA: 0x0001437C File Offset: 0x0001257C
	public Vector3 OtherPlayerPosition()
	{
		Player otherPlayer = PlayerManager.instance.GetOtherPlayer(this.player);
		if (otherPlayer)
		{
			return otherPlayer.transform.position;
		}
		return Vector3.zero;
	}

	// Token: 0x06000334 RID: 820 RVA: 0x000143B3 File Offset: 0x000125B3
	public Vector3 PlayerPosition()
	{
		return base.transform.position;
	}

	// Token: 0x06000335 RID: 821 RVA: 0x000143C0 File Offset: 0x000125C0
	public List<BulletWrapper> GetAllBullets()
	{
		List<BulletWrapper> list = new List<BulletWrapper>();
		ProjectileHit[] array = Object.FindObjectsOfType<ProjectileHit>();
		for (int i = 0; i < array.Length; i++)
		{
			BulletWrapper bulletWrapper = new BulletWrapper();
			bulletWrapper.projectileHit = array[i].GetComponent<ProjectileHit>();
			bulletWrapper.projectileMovement = array[i].GetComponent<MoveTransform>();
			bulletWrapper.damage = bulletWrapper.projectileHit.damage;
			bulletWrapper.velocity = bulletWrapper.projectileMovement.velocity;
			list.Add(bulletWrapper);
		}
		return list;
	}

	// Token: 0x06000336 RID: 822 RVA: 0x00014438 File Offset: 0x00012638
	public SpawnedAttack[] GetAllSpawnedAttacks()
	{
		return Object.FindObjectsOfType<SpawnedAttack>();
	}

	// Token: 0x06000337 RID: 823 RVA: 0x0001443F File Offset: 0x0001263F
	public bool CanShoot()
	{
		return this.player.data.weaponHandler.gun.IsReady(0f);
	}

	// Token: 0x06000338 RID: 824 RVA: 0x00014460 File Offset: 0x00012660
	public BulletWrapper GetMyBullet()
	{
		BulletWrapper bulletWrapper = new BulletWrapper();
		GameObject objectToSpawn = this.player.data.weaponHandler.gun.projectiles[0].objectToSpawn;
		MoveTransform component = objectToSpawn.GetComponent<MoveTransform>();
		ProjectileHit component2 = objectToSpawn.GetComponent<ProjectileHit>();
		bulletWrapper.projectileMovement = component;
		bulletWrapper.projectileHit = component2;
		bulletWrapper.damage = component2.damage;
		bulletWrapper.velocity = this.player.data.aimDirection.normalized * component.localForce.magnitude + component.worldForce;
		return bulletWrapper;
	}

	// Token: 0x06000339 RID: 825 RVA: 0x000144F8 File Offset: 0x000126F8
	private void Update()
	{
		if (this.blockedThisFrame)
		{
			this.blockedThisFrame = false;
		}
		else
		{
			this.data.input.shieldWasPressed = false;
		}
		if (this.movedThisFrame)
		{
			this.movedThisFrame = false;
		}
		else
		{
			this.data.input.direction = Vector3.zero;
		}
		if (this.attackedThisFrame)
		{
			this.attackedThisFrame = false;
			return;
		}
		this.data.input.shootWasPressed = false;
		this.data.input.shootIsPressed = false;
	}

	// Token: 0x0400045C RID: 1116
	public Player player;

	// Token: 0x0400045D RID: 1117
	public CharacterData data;

	// Token: 0x0400045E RID: 1118
	private GeneralInput input;

	// Token: 0x0400045F RID: 1119
	private bool movedThisFrame;

	// Token: 0x04000460 RID: 1120
	private bool attackedThisFrame;

	// Token: 0x04000461 RID: 1121
	private bool blockedThisFrame;
}
