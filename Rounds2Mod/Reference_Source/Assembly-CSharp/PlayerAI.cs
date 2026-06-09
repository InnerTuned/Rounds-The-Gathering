using System;
using UnityEngine;

// Token: 0x02000090 RID: 144
public class PlayerAI : MonoBehaviour
{
	// Token: 0x0600031E RID: 798 RVA: 0x00013D95 File Offset: 0x00011F95
	private void Start()
	{
		this.data = base.GetComponentInParent<CharacterData>();
		this.input = this.data.input;
	}

	// Token: 0x0600031F RID: 799 RVA: 0x00013DB4 File Offset: 0x00011FB4
	private void Update()
	{
		this.target = PlayerManager.instance.GetOtherPlayer(this.data.player);
		this.untilNextDataUpdate -= TimeHandler.deltaTime;
		if (this.target)
		{
			this.canSeeTarget = PlayerManager.instance.CanSeePlayer(base.transform.position, this.target).canSee;
			this.input.ResetInput();
			if (!this.canSeeTarget)
			{
				this.getRandomTargetPosCounter -= TimeHandler.deltaTime;
				if (this.getRandomTargetPosCounter < 0f)
				{
					this.getRandomTargetPosCounter = Random.Range(0.5f, 2f);
					this.GetRandomPos();
				}
			}
			if (this.untilNextDataUpdate <= 0f)
			{
				if (Random.value < 0.25f / Mathf.Clamp(this.distanceToTarget * 0.1f, 0.1f, 10f) && this.canSeeTarget)
				{
					this.input.shieldWasPressed = true;
				}
				if (Random.value < 0.4f && this.canSeeTarget)
				{
					this.isShooting = true;
				}
				else
				{
					this.isShooting = false;
				}
				if (Random.value < 0.2f || this.data.isWallGrab)
				{
					this.input.jumpWasPressed = true;
				}
				this.untilNextDataUpdate = Random.Range(0f, 0.25f);
				this.UpdateData();
			}
			this.input.shootIsPressed = this.isShooting;
			this.input.shootWasPressed = this.isShooting;
			this.input.aimDirection = this.aimDir;
			this.input.direction = this.moveDir;
		}
	}

	// Token: 0x06000320 RID: 800 RVA: 0x00013F6C File Offset: 0x0001216C
	private void GetRandomPos()
	{
		Vector3 lhs = Vector3.zero;
		int num = 200;
		while (lhs == Vector3.zero && num > 0)
		{
			num--;
			Vector3 vector = base.transform.position + Vector3.up * 5f + Random.insideUnitCircle * 15f;
			if (this.data.ThereIsGroundBelow(vector, 8f))
			{
				lhs = vector;
			}
		}
		this.targetPos = lhs;
	}

	// Token: 0x06000321 RID: 801 RVA: 0x00013FF4 File Offset: 0x000121F4
	private void UpdateData()
	{
		if (this.canSeeTarget)
		{
			this.targetPos = this.target.transform.position;
		}
		this.distanceToTarget = Vector3.Distance(base.transform.position, this.target.transform.position);
		this.aimDir = (this.targetPos - base.transform.position).normalized;
		this.moveDir = this.aimDir;
		if (this.moveDir.x > 0f)
		{
			this.moveDir.x = 1f;
		}
		if (this.moveDir.x < 0f)
		{
			this.moveDir.x = -1f;
		}
		if (this.canSeeTarget && this.distanceToTarget < this.range && this.data.ThereIsGroundBelow(base.transform.position, 10f))
		{
			this.moveDir = Vector3.zero;
		}
	}

	// Token: 0x0400044B RID: 1099
	private float range = 6f;

	// Token: 0x0400044C RID: 1100
	private CharacterData data;

	// Token: 0x0400044D RID: 1101
	private GeneralInput input;

	// Token: 0x0400044E RID: 1102
	private Vector3 moveDir = Vector3.zero;

	// Token: 0x0400044F RID: 1103
	private Vector3 aimDir = Vector3.zero;

	// Token: 0x04000450 RID: 1104
	private Vector3 targetPos;

	// Token: 0x04000451 RID: 1105
	private Player target;

	// Token: 0x04000452 RID: 1106
	private bool canSeeTarget;

	// Token: 0x04000453 RID: 1107
	private float untilNextDataUpdate;

	// Token: 0x04000454 RID: 1108
	private float getRandomTargetPosCounter;

	// Token: 0x04000455 RID: 1109
	private bool isShooting;

	// Token: 0x04000456 RID: 1110
	private float distanceToTarget = 5f;
}
