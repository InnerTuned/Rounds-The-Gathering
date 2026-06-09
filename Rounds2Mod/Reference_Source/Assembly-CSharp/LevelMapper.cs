using System;
using UnityEngine;

// Token: 0x02000077 RID: 119
public class LevelMapper : MonoBehaviour
{
	// Token: 0x0600026E RID: 622 RVA: 0x0000FED4 File Offset: 0x0000E0D4
	private void Start()
	{
		this.data = base.GetComponentInParent<CharacterData>();
		this.jump = this.data.jump;
		this.movement = this.data.movement;
		PlayerJump playerJump = this.jump;
		playerJump.JumpAction = (Action)Delegate.Combine(playerJump.JumpAction, new Action(this.Jump));
		CharacterData characterData = this.data;
		characterData.TouchGroundAction = (Action<float, Vector3, Vector3, Transform>)Delegate.Combine(characterData.TouchGroundAction, new Action<float, Vector3, Vector3, Transform>(this.Ground));
		CharacterData characterData2 = this.data;
		characterData2.TouchWallAction = (Action<float, Vector3, Vector3>)Delegate.Combine(characterData2.TouchWallAction, new Action<float, Vector3, Vector3>(this.Wall));
	}

	// Token: 0x0600026F RID: 623 RVA: 0x0000FF84 File Offset: 0x0000E184
	private void Update()
	{
		if (!this.data.isGrounded)
		{
			this.LeaveGround();
		}
	}

	// Token: 0x06000270 RID: 624 RVA: 0x0000FF99 File Offset: 0x0000E199
	private void Jump()
	{
		this.isJumping = true;
		this.jumpPos = base.transform.position;
	}

	// Token: 0x06000271 RID: 625 RVA: 0x0000FFB3 File Offset: 0x0000E1B3
	private void Wall(float sinceWall, Vector3 pos, Vector3 normal)
	{
		if (this.data.isWallGrab)
		{
			return;
		}
		if (this.data.sinceJump < 0.1f)
		{
			this.isJumping = false;
			return;
		}
		this.Land();
	}

	// Token: 0x06000272 RID: 626 RVA: 0x0000FFE4 File Offset: 0x0000E1E4
	private void Ground(float sinceGround, Vector3 pos, Vector3 groundNormal, Transform groundTransform = null)
	{
		if (!this.isRunning)
		{
			this.startRunPos = pos;
			this.isRunning = true;
		}
		if (this.data.isGrounded)
		{
			return;
		}
		if (this.data.sinceJump < 0.1f)
		{
			this.isJumping = false;
			return;
		}
		this.Land();
	}

	// Token: 0x06000273 RID: 627 RVA: 0x00010035 File Offset: 0x0000E235
	private void Land()
	{
		this.landPos = base.transform.position;
		if (Vector3.Distance(this.landPos, this.jumpPos) > 3f && this.isJumping)
		{
			this.SaveJump();
		}
		this.isJumping = false;
	}

	// Token: 0x06000274 RID: 628 RVA: 0x00010075 File Offset: 0x0000E275
	private void LeaveGround()
	{
		if (this.isRunning)
		{
			this.leaveGroundPos = this.data.groundPos;
			this.SaveRun();
			this.isRunning = false;
		}
	}

	// Token: 0x06000275 RID: 629 RVA: 0x000100A0 File Offset: 0x0000E2A0
	private void SaveJump()
	{
		Object.Instantiate<GameObject>(this.node, this.jumpPos, Quaternion.identity).GetComponent<SpriteRenderer>().color = Color.green;
		Object.Instantiate<GameObject>(this.node, this.landPos, Quaternion.identity).GetComponent<SpriteRenderer>().color = Color.red;
		LineRenderer component = Object.Instantiate<GameObject>(this.line, Vector3.zero, Quaternion.identity).GetComponent<LineRenderer>();
		component.SetPosition(0, this.jumpPos);
		component.SetPosition(1, this.landPos);
	}

	// Token: 0x06000276 RID: 630 RVA: 0x0001012C File Offset: 0x0000E32C
	private void SaveRun()
	{
		if (Vector3.Distance(this.startRunPos, this.jumpPos) < 2f)
		{
			return;
		}
		Object.Instantiate<GameObject>(this.node, this.startRunPos, Quaternion.identity).GetComponent<SpriteRenderer>().color = Color.blue;
		Object.Instantiate<GameObject>(this.node, this.leaveGroundPos, Quaternion.identity).GetComponent<SpriteRenderer>().color = Color.blue;
		LineRenderer component = Object.Instantiate<GameObject>(this.line2, Vector3.zero, Quaternion.identity).GetComponent<LineRenderer>();
		component.SetPosition(0, this.startRunPos);
		component.SetPosition(1, this.leaveGroundPos);
	}

	// Token: 0x04000380 RID: 896
	private CharacterData data;

	// Token: 0x04000381 RID: 897
	private PlayerJump jump;

	// Token: 0x04000382 RID: 898
	private PlayerMovement movement;

	// Token: 0x04000383 RID: 899
	private bool isJumping;

	// Token: 0x04000384 RID: 900
	private bool isRunning;

	// Token: 0x04000385 RID: 901
	public GameObject node;

	// Token: 0x04000386 RID: 902
	public GameObject line;

	// Token: 0x04000387 RID: 903
	public GameObject line2;

	// Token: 0x04000388 RID: 904
	private Vector3 jumpPos;

	// Token: 0x04000389 RID: 905
	private Vector3 landPos;

	// Token: 0x0400038A RID: 906
	private Vector3 startRunPos;

	// Token: 0x0400038B RID: 907
	private Vector3 leaveGroundPos;
}
