using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200005F RID: 95
public class GeneralInput : MonoBehaviour
{
	// Token: 0x060001B8 RID: 440 RVA: 0x0000AE0A File Offset: 0x0000900A
	private void Start()
	{
		this.data = base.GetComponent<CharacterData>();
	}

	// Token: 0x060001B9 RID: 441 RVA: 0x0000AE18 File Offset: 0x00009018
	public void ResetInput()
	{
		this.direction = Vector3.zero;
		this.aimDirection = Vector3.zero;
		this.jumpIsPressed = false;
		this.jumpWasPressed = false;
		this.shootIsPressed = false;
		this.shootWasPressed = false;
		this.shootWasReleased = false;
		this.shieldWasPressed = false;
		this.acceptWasPressed = false;
	}

	// Token: 0x060001BA RID: 442 RVA: 0x0000AE6C File Offset: 0x0000906C
	private void Update()
	{
		if (this.controlledElseWhere)
		{
			return;
		}
		this.ResetInput();
		this.DoUIInput();
		if (GameManager.lockInput)
		{
			return;
		}
		if (this.stunnedInput)
		{
			return;
		}
		if (!this.data.isPlaying)
		{
			return;
		}
		if (this.data.playerActions != null)
		{
			this.direction += this.data.playerActions.Move;
			this.direction = this.MakeEightDirections(this.direction);
			if (this.direction != Vector3.zero)
			{
				this.latestPressedDirection = this.direction;
			}
			if (this.data.playerActions.Device == null)
			{
				this.aimDirection = MainCam.instance.cam.ScreenToWorldPoint(Input.mousePosition) - base.transform.position;
				this.aimDirection.z = 0f;
				this.aimDirection.Normalize();
				if (Optionshandler.lockMouse)
				{
					this.aimDirection = this.MakeEightDirections(this.aimDirection);
				}
			}
			else
			{
				this.aimDirection.x = this.aimDirection.x + this.data.playerActions.Aim.X;
				this.aimDirection.y = this.aimDirection.y + this.data.playerActions.Aim.Y;
				if (Optionshandler.lockStick)
				{
					this.aimDirection = this.MakeEightDirections(this.aimDirection);
				}
			}
			if (this.aimDirection != Vector3.zero)
			{
				this.aimDirection += Vector3.up * 0.13f / Mathf.Clamp(this.data.weaponHandler.gun.projectileSpeed, 1f, 100f);
			}
			if (this.aimDirection != Vector3.zero)
			{
				this.lastAimDirection = this.aimDirection;
			}
			if (this.data.playerActions.Jump.IsPressed)
			{
				this.jumpIsPressed = true;
			}
			if (this.data.playerActions.Jump.WasPressed)
			{
				this.jumpWasPressed = true;
			}
			if (this.silencedInput)
			{
				return;
			}
			if (this.data.playerActions.Fire.IsPressed)
			{
				this.shootIsPressed = true;
			}
			if (this.data.playerActions.Fire.WasPressed)
			{
				this.shootWasPressed = true;
			}
			if (this.data.playerActions.Fire.WasReleased)
			{
				this.shootWasReleased = true;
			}
			if (this.data.playerActions.Block.WasPressed)
			{
				this.shieldWasPressed = true;
			}
		}
	}

	// Token: 0x060001BB RID: 443 RVA: 0x0000B11C File Offset: 0x0000931C
	private void DoUIInput()
	{
		GeneralInput.StickDirection stickDirection = GeneralInput.StickDirection.None;
		if (this.data.playerActions.Move.X > 0.7f)
		{
			stickDirection = GeneralInput.StickDirection.Right;
		}
		if (this.data.playerActions.Move.X < -0.7f)
		{
			stickDirection = GeneralInput.StickDirection.Left;
		}
		if (stickDirection != this.lastStickDirection)
		{
			this.stickPressDir = stickDirection;
			this.lastStickDirection = stickDirection;
		}
		else
		{
			this.stickPressDir = GeneralInput.StickDirection.None;
		}
		if (this.data.playerActions.Jump.WasPressed)
		{
			this.acceptWasPressed = true;
		}
	}

	// Token: 0x060001BC RID: 444 RVA: 0x0000B1A5 File Offset: 0x000093A5
	public void DoStun(float stun)
	{
		if (base.gameObject.activeSelf)
		{
			base.StartCoroutine(this.Stun(stun));
		}
	}

	// Token: 0x060001BD RID: 445 RVA: 0x0000B1C2 File Offset: 0x000093C2
	private IEnumerator Stun(float stun)
	{
		base.enabled = false;
		this.ResetInput();
		yield return new WaitForSeconds(stun);
		base.enabled = true;
		yield break;
	}

	// Token: 0x060001BE RID: 446 RVA: 0x0000B1D8 File Offset: 0x000093D8
	public void SetState(Vector3 pos, bool isGrounded)
	{
		base.transform.position = pos;
	}

	// Token: 0x060001BF RID: 447 RVA: 0x0000B1E8 File Offset: 0x000093E8
	public void Move(bool right, bool left, bool up, bool down, bool jump)
	{
		this.ResetInput();
		if (left)
		{
			this.direction += Vector3.left;
		}
		if (right)
		{
			this.direction += Vector3.right;
		}
		if (up)
		{
			this.direction += Vector3.up;
		}
		if (down)
		{
			this.direction += Vector3.down;
		}
		if (jump)
		{
			this.jumpWasPressed = true;
		}
		global::Debug.Log(string.Concat(new string[]
		{
			"Move: Right: ",
			right.ToString(),
			" Left: ",
			left.ToString(),
			" Jump: ",
			jump.ToString()
		}));
		if (Input.GetKey(this.shoot))
		{
			this.shootIsPressed = true;
		}
		if (Input.GetKeyDown(this.shoot))
		{
			this.shootWasPressed = true;
		}
		if (Input.GetKeyUp(this.shoot))
		{
			this.shootWasReleased = true;
		}
	}

	// Token: 0x060001C0 RID: 448 RVA: 0x0000B2F0 File Offset: 0x000094F0
	private Vector3 MakeEightDirections(Vector3 dir)
	{
		Vector3 zero = Vector3.zero;
		float d = 360f / (float)this.snapNumber;
		Vector3 point = Vector3.up;
		float num = 999f;
		Vector3 result = dir;
		for (int i = 0; i < this.snapNumber; i++)
		{
			Vector3 vector = Quaternion.Euler(Vector3.forward * d * (float)i) * point;
			float num2 = Vector3.Angle(dir, vector);
			if (num2 < num)
			{
				num = num2;
				result = vector;
			}
		}
		if (dir == Vector3.zero)
		{
			return Vector3.zero;
		}
		dir = dir.normalized;
		return result;
	}

	// Token: 0x04000245 RID: 581
	public bool controlledElseWhere;

	// Token: 0x04000246 RID: 582
	public bool stunnedInput;

	// Token: 0x04000247 RID: 583
	public bool silencedInput;

	// Token: 0x04000248 RID: 584
	public KeyCode left;

	// Token: 0x04000249 RID: 585
	public KeyCode right;

	// Token: 0x0400024A RID: 586
	public KeyCode up;

	// Token: 0x0400024B RID: 587
	public KeyCode down;

	// Token: 0x0400024C RID: 588
	public KeyCode jump;

	// Token: 0x0400024D RID: 589
	public KeyCode shoot;

	// Token: 0x0400024E RID: 590
	public KeyCode shield;

	// Token: 0x0400024F RID: 591
	public Vector3 direction;

	// Token: 0x04000250 RID: 592
	public Vector3 latestPressedDirection;

	// Token: 0x04000251 RID: 593
	public Vector3 aimDirection;

	// Token: 0x04000252 RID: 594
	public Vector3 lastAimDirection;

	// Token: 0x04000253 RID: 595
	public bool jumpWasPressed;

	// Token: 0x04000254 RID: 596
	public bool jumpIsPressed;

	// Token: 0x04000255 RID: 597
	public bool shootWasPressed;

	// Token: 0x04000256 RID: 598
	public bool shootIsPressed;

	// Token: 0x04000257 RID: 599
	public bool shootWasReleased;

	// Token: 0x04000258 RID: 600
	public bool shieldWasPressed;

	// Token: 0x04000259 RID: 601
	public bool acceptWasPressed;

	// Token: 0x0400025A RID: 602
	public GeneralInput.InputType inputType;

	// Token: 0x0400025B RID: 603
	private GeneralInput.StickDirection lastStickDirection;

	// Token: 0x0400025C RID: 604
	public GeneralInput.StickDirection stickPressDir = GeneralInput.StickDirection.None;

	// Token: 0x0400025D RID: 605
	private CharacterData data;

	// Token: 0x0400025E RID: 606
	private int snapNumber = 16;

	// Token: 0x02000346 RID: 838
	public enum InputType
	{
		// Token: 0x040010AD RID: 4269
		Controller,
		// Token: 0x040010AE RID: 4270
		Keyboard,
		// Token: 0x040010AF RID: 4271
		Either
	}

	// Token: 0x02000347 RID: 839
	public enum StickDirection
	{
		// Token: 0x040010B1 RID: 4273
		Left,
		// Token: 0x040010B2 RID: 4274
		Right,
		// Token: 0x040010B3 RID: 4275
		None
	}
}
