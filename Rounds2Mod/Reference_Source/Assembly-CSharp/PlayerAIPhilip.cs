using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200008C RID: 140
public class PlayerAIPhilip : MonoBehaviour
{
	// Token: 0x060002F3 RID: 755 RVA: 0x00012EBA File Offset: 0x000110BA
	private void Awake()
	{
		this.InitReferences();
		this.MakeBoundaries();
		this.NextBehaviour();
	}

	// Token: 0x060002F4 RID: 756 RVA: 0x00012ECE File Offset: 0x000110CE
	private void InitReferences()
	{
		this.m_PlayerAPI = base.GetComponentInParent<PlayerAPI>();
	}

	// Token: 0x060002F5 RID: 757 RVA: 0x00012EDC File Offset: 0x000110DC
	private void MakeBoundaries()
	{
		int num = 20;
		int num2 = 35;
		Vector2 vector = new Vector2((float)num2, (float)num);
		Vector2 vector2 = new Vector2((float)(-(float)num2), (float)num);
		Vector2 vector3 = new Vector2((float)num2, (float)(-(float)num));
		Vector2 vector4 = new Vector2((float)(-(float)num2), (float)(-(float)num));
		this.m_Boundaries = new Vector2[]
		{
			vector,
			vector2,
			vector3,
			vector4
		};
		this.m_Segments = new Vector2[]
		{
			vector / 2f,
			vector2 / 2f,
			vector3 / 2f,
			vector4 / 2f
		};
	}

	// Token: 0x060002F6 RID: 758 RVA: 0x00012FA3 File Offset: 0x000111A3
	private bool CheckForValidEnemy()
	{
		this.m_Enemy = this.m_PlayerAPI.GetOtherPlayer();
		return this.m_Enemy != null;
	}

	// Token: 0x060002F7 RID: 759 RVA: 0x00012FC2 File Offset: 0x000111C2
	private void Start()
	{
		this.CheckForValidEnemy();
	}

	// Token: 0x060002F8 RID: 760 RVA: 0x00012FCC File Offset: 0x000111CC
	private void Update()
	{
		if (!this.CheckForValidEnemy())
		{
			return;
		}
		this.CheckMorale();
		this.CheckDirection();
		this.CheckGround();
		this.Move();
		this.Jump();
		this.DoAim();
		this.ShouldAttack();
		this.ShouldBlock();
		this.TickBehaviour();
	}

	// Token: 0x060002F9 RID: 761 RVA: 0x00013018 File Offset: 0x00011218
	private void SeedBehaviourChange()
	{
		this.m_BehaviourChangeTick = Random.Range(this.m_BehaviourTimeMin, this.m_BehaviourTimeMax);
	}

	// Token: 0x060002FA RID: 762 RVA: 0x00013031 File Offset: 0x00011231
	private void TickBehaviour()
	{
		this.m_Tick += TimeHandler.deltaTime;
		if (this.m_Tick >= (float)this.m_BehaviourChangeTick)
		{
			this.NextBehaviour();
		}
	}

	// Token: 0x060002FB RID: 763 RVA: 0x0001305C File Offset: 0x0001125C
	private void NextBehaviour()
	{
		this.ResetTick();
		int length = Enum.GetValues(typeof(PlayerAIPhilip.BattleBehaviour)).Length;
		this.m_CurrentBehaviour = (PlayerAIPhilip.BattleBehaviour)Random.Range(0, length);
		this.SeedBehaviourChange();
		this.CheckBehaviour();
	}

	// Token: 0x060002FC RID: 764 RVA: 0x0001309E File Offset: 0x0001129E
	private void ResetTick()
	{
		this.m_Tick = 0f;
	}

	// Token: 0x060002FD RID: 765 RVA: 0x000130AB File Offset: 0x000112AB
	private void ShouldBlock()
	{
		if (this.CheckIncommingBullets())
		{
			this.m_PlayerAPI.Block();
		}
	}

	// Token: 0x060002FE RID: 766 RVA: 0x000130C0 File Offset: 0x000112C0
	private bool CheckIncommingBullets()
	{
		Vector3 vector = this.m_PlayerAPI.PlayerPosition();
		Vector2 a = new Vector2(vector.x, vector.y);
		float num = 1.5f;
		List<BulletWrapper> allBullets = this.m_PlayerAPI.GetAllBullets();
		int count = allBullets.Count;
		for (int i = 0; i < count; i++)
		{
			Vector2 vector2 = allBullets[i].projectileHit.transform.position;
			float num2 = Vector2.Distance(a, vector2);
			if (num2 <= num && Vector2.Distance(a, vector2 + allBullets[i].velocity.normalized) < num2)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060002FF RID: 767 RVA: 0x0001316A File Offset: 0x0001136A
	private void ShouldAttack()
	{
		if (this.CanSee() && Vector3.Distance(this.m_PlayerAPI.PlayerPosition(), this.m_PlayerAPI.OtherPlayerPosition()) <= this.m_AttackDistance)
		{
			this.m_PlayerAPI.Attack();
		}
	}

	// Token: 0x06000300 RID: 768 RVA: 0x000131A4 File Offset: 0x000113A4
	private bool CanSee()
	{
		Vector2 vector = this.m_PlayerAPI.TowardsOtherPlayer();
		Vector3 vector2 = new Vector3(vector.x, vector.y, 0f);
		vector2.Normalize();
		Collider2D collider = Physics2D.Raycast(base.transform.position + vector2, vector2, 20f).collider;
		return collider && collider.GetComponent<Player>();
	}

	// Token: 0x06000301 RID: 769 RVA: 0x00013224 File Offset: 0x00011424
	private void DoAim()
	{
		this.m_PlayerAPI.SetAimDirection(this.m_PlayerAPI.TowardsOtherPlayer() + this.m_PlayerAPI.GetOtherPlayer().data.playerVel.velocity * 0.1f);
	}

	// Token: 0x06000302 RID: 770 RVA: 0x00013270 File Offset: 0x00011470
	private void Move()
	{
		this.m_PlayerAPI.Move(this.m_Direction);
	}

	// Token: 0x06000303 RID: 771 RVA: 0x00013283 File Offset: 0x00011483
	private void Jump()
	{
		this.m_PlayerAPI.Jump();
	}

	// Token: 0x06000304 RID: 772 RVA: 0x00013290 File Offset: 0x00011490
	private void CheckGround()
	{
		Vector2 vector = this.m_PlayerAPI.PlayerPosition();
		if (vector.y < 0f && !this.m_PlayerAPI.CheckGroundBelow(vector + Vector2.down * 0.5f, 10f))
		{
			this.m_Direction = Vector2.right;
			float num = float.PositiveInfinity;
			RaycastHit2D raycastHit2D = Physics2D.Raycast(base.transform.position + Vector3.right, Vector2.right, 5f);
			if (raycastHit2D.collider)
			{
				num = raycastHit2D.distance;
			}
			raycastHit2D = Physics2D.Raycast(base.transform.position + Vector3.left, Vector2.left, 5f);
			if (raycastHit2D.collider)
			{
				if (raycastHit2D.distance < num)
				{
					this.m_Direction = Vector2.left;
					return;
				}
			}
			else
			{
				this.m_Direction = Vector2.right;
			}
		}
	}

	// Token: 0x06000305 RID: 773 RVA: 0x00013394 File Offset: 0x00011594
	private void CheckDirection()
	{
		if (this.m_CurrentBehaviour != PlayerAIPhilip.BattleBehaviour.Attack)
		{
			Vector3 vector = this.m_PlayerAPI.PlayerPosition();
			Vector2 b = new Vector2(vector.x, vector.y);
			this.m_Direction = this.m_TargetPos - b;
			return;
		}
		switch (this.m_CurrentMorale)
		{
		case PlayerAIPhilip.BattleMorale.Blood:
			this.m_Direction = this.m_PlayerAPI.TowardsOtherPlayer();
			return;
		case PlayerAIPhilip.BattleMorale.Coward:
			this.m_Direction = -this.m_PlayerAPI.TowardsOtherPlayer();
			return;
		case PlayerAIPhilip.BattleMorale.Defend:
			this.m_Direction = -this.m_PlayerAPI.TowardsOtherPlayer();
			return;
		default:
			return;
		}
	}

	// Token: 0x06000306 RID: 774 RVA: 0x00013434 File Offset: 0x00011634
	private void CheckBehaviour()
	{
		switch (this.m_CurrentBehaviour)
		{
		case PlayerAIPhilip.BattleBehaviour.Attack:
			break;
		case PlayerAIPhilip.BattleBehaviour.HighGround:
			this.m_TargetPos = this.m_Segments[0];
			return;
		case PlayerAIPhilip.BattleBehaviour.MoveSegments:
		{
			int max = this.m_Segments.Length;
			int num;
			for (num = Random.Range(0, max); num == this.m_CurrentSegment; num = Random.Range(0, max))
			{
			}
			this.m_CurrentSegment = num;
			this.m_TargetPos = this.m_Segments[this.m_CurrentSegment];
			break;
		}
		default:
			return;
		}
	}

	// Token: 0x06000307 RID: 775 RVA: 0x000134B0 File Offset: 0x000116B0
	private void CheckMorale()
	{
		if (this.m_PlayerAPI.CanShoot())
		{
			this.m_CurrentMorale = PlayerAIPhilip.BattleMorale.Blood;
		}
	}

	// Token: 0x0400042A RID: 1066
	private PlayerAPI m_PlayerAPI;

	// Token: 0x0400042B RID: 1067
	private Player m_Enemy;

	// Token: 0x0400042C RID: 1068
	private PlayerAIPhilip.BattleMorale m_CurrentMorale;

	// Token: 0x0400042D RID: 1069
	private PlayerAIPhilip.BattleBehaviour m_CurrentBehaviour;

	// Token: 0x0400042E RID: 1070
	private Vector2 m_Direction;

	// Token: 0x0400042F RID: 1071
	private Vector2 m_TargetPos;

	// Token: 0x04000430 RID: 1072
	private Vector2[] m_Boundaries;

	// Token: 0x04000431 RID: 1073
	private Vector2[] m_Segments;

	// Token: 0x04000432 RID: 1074
	private int m_CurrentSegment;

	// Token: 0x04000433 RID: 1075
	private float m_Tick;

	// Token: 0x04000434 RID: 1076
	private int m_BehaviourChangeTick;

	// Token: 0x04000435 RID: 1077
	private int m_BehaviourTimeMin = 3;

	// Token: 0x04000436 RID: 1078
	private int m_BehaviourTimeMax = 8;

	// Token: 0x04000437 RID: 1079
	private float m_AttackDistance = 15f;

	// Token: 0x02000369 RID: 873
	private enum BattleMorale : byte
	{
		// Token: 0x04001169 RID: 4457
		Blood,
		// Token: 0x0400116A RID: 4458
		Coward,
		// Token: 0x0400116B RID: 4459
		Defend
	}

	// Token: 0x0200036A RID: 874
	private enum BattleBehaviour : byte
	{
		// Token: 0x0400116D RID: 4461
		Attack,
		// Token: 0x0400116E RID: 4462
		HighGround,
		// Token: 0x0400116F RID: 4463
		MoveSegments
	}
}
