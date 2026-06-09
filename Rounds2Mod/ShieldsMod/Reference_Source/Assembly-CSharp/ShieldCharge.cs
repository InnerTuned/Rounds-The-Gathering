using System;
using System.Collections;
using System.Collections.Generic;
using Sonigon;
using UnityEngine;

// Token: 0x020001B2 RID: 434
public class ShieldCharge : MonoBehaviour
{
	// Token: 0x060008A0 RID: 2208 RVA: 0x0002D9BC File Offset: 0x0002BBBC
	private void Start()
	{
		this.level = base.GetComponent<AttackLevel>();
		this.data = base.GetComponentInParent<CharacterData>();
		PlayerCollision component = this.data.GetComponent<PlayerCollision>();
		component.collideWithPlayerAction = (Action<Vector2, Vector2, Player>)Delegate.Combine(component.collideWithPlayerAction, new Action<Vector2, Vector2, Player>(this.Collide));
		base.GetComponentInParent<ChildRPC>().childRPCsVector2Vector2Int.Add("ShieldChargeCollide", new Action<Vector2, Vector2, int>(this.RPCA_Collide));
		Block componentInParent = base.GetComponentInParent<Block>();
		componentInParent.SuperFirstBlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Combine(componentInParent.SuperFirstBlockAction, new Action<BlockTrigger.BlockTriggerType>(this.DoBlock));
	}

	// Token: 0x060008A1 RID: 2209 RVA: 0x0002DA58 File Offset: 0x0002BC58
	private void OnDestroy()
	{
		PlayerCollision component = this.data.GetComponent<PlayerCollision>();
		component.collideWithPlayerAction = (Action<Vector2, Vector2, Player>)Delegate.Remove(component.collideWithPlayerAction, new Action<Vector2, Vector2, Player>(this.Collide));
		base.GetComponentInParent<ChildRPC>().childRPCsVector2Vector2Int.Remove("ShieldChargeCollide");
		Block componentInParent = base.GetComponentInParent<Block>();
		componentInParent.SuperFirstBlockAction = (Action<BlockTrigger.BlockTriggerType>)Delegate.Remove(componentInParent.SuperFirstBlockAction, new Action<BlockTrigger.BlockTriggerType>(this.DoBlock));
	}

	// Token: 0x060008A2 RID: 2210 RVA: 0x0002DACE File Offset: 0x0002BCCE
	private void Update()
	{
		this.blockTime -= TimeHandler.deltaTime;
	}

	// Token: 0x060008A3 RID: 2211 RVA: 0x0002DAE2 File Offset: 0x0002BCE2
	public void DoBlock(BlockTrigger.BlockTriggerType trigger)
	{
		if (trigger != BlockTrigger.BlockTriggerType.ShieldCharge)
		{
			this.Charge(trigger);
		}
	}

	// Token: 0x060008A4 RID: 2212 RVA: 0x0002DAEF File Offset: 0x0002BCEF
	public void Charge(BlockTrigger.BlockTriggerType trigger)
	{
		base.StartCoroutine(this.DoCharge(trigger));
	}

	// Token: 0x060008A5 RID: 2213 RVA: 0x0002DAFF File Offset: 0x0002BCFF
	private IEnumerator DoCharge(BlockTrigger.BlockTriggerType trigger)
	{
		SoundManager.Instance.Play(this.soundShieldCharge, base.transform);
		this.cancelForce = false;
		this.hitDatas.Clear();
		if (trigger == BlockTrigger.BlockTriggerType.Empower)
		{
			Vector3 currentPos = base.transform.position;
			yield return new WaitForSeconds(0f);
			this.dir = (currentPos - base.transform.position).normalized;
			currentPos = default(Vector3);
		}
		else
		{
			this.dir = this.data.aimDirection;
		}
		float usedTime = this.time + (float)this.level.LevelsUp() * this.timePerLevel;
		this.blockTime = usedTime;
		float num = this.time * 0.1f + (float)this.level.LevelsUp() * this.time * 0.15f;
		for (int i = 0; i < this.level.LevelsUp(); i++)
		{
			float num2 = this.time / (float)this.level.attackLevel;
			float num3 = this.time;
			num += num2;
			base.StartCoroutine(this.DelayBlock(num));
		}
		float c = 0f;
		while (c < 1f)
		{
			c += Time.fixedDeltaTime / usedTime;
			if (!this.cancelForce)
			{
				this.data.healthHandler.TakeForce(this.dir * this.forceCurve.Evaluate(c) * (this.force + (float)this.level.LevelsUp() * this.forcePerLevel), 0, true, true, 0f);
				this.data.healthHandler.TakeForce(-this.data.playerVel.velocity * this.drag * Time.fixedDeltaTime, 0, true, true, 0f);
			}
			this.data.sinceGrounded = 0f;
			yield return new WaitForFixedUpdate();
		}
		this.data.block.RPCA_DoBlock(false, true, BlockTrigger.BlockTriggerType.ShieldCharge, default(Vector3), false);
		yield break;
	}

	// Token: 0x060008A6 RID: 2214 RVA: 0x0002DB15 File Offset: 0x0002BD15
	private IEnumerator DelayBlock(float delay)
	{
		yield return new WaitForSeconds(delay);
		this.data.block.RPCA_DoBlock(false, true, BlockTrigger.BlockTriggerType.ShieldCharge, default(Vector3), false);
		yield break;
	}

	// Token: 0x060008A7 RID: 2215 RVA: 0x0002DB2C File Offset: 0x0002BD2C
	public void RPCA_Collide(Vector2 pos, Vector2 colDir, int playerID)
	{
		CharacterData componentInParent = PlayerManager.instance.GetPlayerWithID(playerID).gameObject.GetComponentInParent<CharacterData>();
		if (componentInParent)
		{
			this.cancelForce = true;
			this.hitPart.transform.rotation = Quaternion.LookRotation(this.dir);
			this.hitPart.Play();
			componentInParent.healthHandler.TakeDamage(this.dir * (this.damage + (float)this.level.LevelsUp() * this.damagePerLevel), base.transform.position, null, this.data.player, true, false);
			componentInParent.healthHandler.TakeForce(this.dir * (this.knockBack + (float)this.level.LevelsUp() * this.knockBackPerLevel), 1, false, false, 0f);
			this.data.healthHandler.TakeForce(-this.dir * this.knockBack, 1, false, true, 0f);
			this.data.healthHandler.TakeForce(-this.dir * this.stopForce, 1, true, true, 0f);
			this.data.block.RPCA_DoBlock(false, true, BlockTrigger.BlockTriggerType.ShieldCharge, default(Vector3), false);
			GamefeelManager.GameFeel(this.dir * this.shake);
		}
	}

	// Token: 0x060008A8 RID: 2216 RVA: 0x0002DCB8 File Offset: 0x0002BEB8
	public void Collide(Vector2 pos, Vector2 colDir, Player player)
	{
		if (!this.data.view.IsMine)
		{
			return;
		}
		if (this.blockTime < 0f)
		{
			return;
		}
		CharacterData componentInParent = player.gameObject.GetComponentInParent<CharacterData>();
		if (this.hitDatas.Contains(componentInParent))
		{
			return;
		}
		this.hitDatas.Add(componentInParent);
		if (componentInParent)
		{
			base.GetComponentInParent<ChildRPC>().CallFunction("ShieldChargeCollide", pos, colDir, player.playerID);
		}
	}

	// Token: 0x040009DA RID: 2522
	[Header("Sounds")]
	public SoundEvent soundShieldCharge;

	// Token: 0x040009DB RID: 2523
	[Header("Settings")]
	public float damagePerLevel;

	// Token: 0x040009DC RID: 2524
	[Header("Settings")]
	public float knockBackPerLevel;

	// Token: 0x040009DD RID: 2525
	public float forcePerLevel;

	// Token: 0x040009DE RID: 2526
	public float timePerLevel;

	// Token: 0x040009DF RID: 2527
	public ParticleSystem hitPart;

	// Token: 0x040009E0 RID: 2528
	public float shake;

	// Token: 0x040009E1 RID: 2529
	public float damage;

	// Token: 0x040009E2 RID: 2530
	public float knockBack;

	// Token: 0x040009E3 RID: 2531
	public float stopForce;

	// Token: 0x040009E4 RID: 2532
	public AnimationCurve forceCurve;

	// Token: 0x040009E5 RID: 2533
	public float force;

	// Token: 0x040009E6 RID: 2534
	public float drag;

	// Token: 0x040009E7 RID: 2535
	public float time;

	// Token: 0x040009E8 RID: 2536
	private CharacterData data;

	// Token: 0x040009E9 RID: 2537
	private AttackLevel level;

	// Token: 0x040009EA RID: 2538
	private Vector3 dir;

	// Token: 0x040009EB RID: 2539
	private bool cancelForce;

	// Token: 0x040009EC RID: 2540
	private List<CharacterData> hitDatas = new List<CharacterData>();

	// Token: 0x040009ED RID: 2541
	private float blockTime;
}
