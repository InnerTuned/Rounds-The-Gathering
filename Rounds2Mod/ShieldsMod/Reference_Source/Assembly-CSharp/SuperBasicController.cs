using System;
using Photon.Pun;
using Photon.Pun.Simple;
using UnityEngine;

// Token: 0x02000180 RID: 384
public class SuperBasicController : NetComponent
{
	// Token: 0x060007C8 RID: 1992 RVA: 0x00029D10 File Offset: 0x00027F10
	public override void OnAwake()
	{
		base.OnAwake();
		this.animator = NestedComponentUtilities.GetNestedComponentInChildren<Animator, NetObject>(base.transform, true);
		this.syncAnimator = NestedComponentUtilities.GetNestedComponentInChildren<SyncAnimator, NetObject>(base.transform, true);
		this.syncTransform = base.GetComponent<SyncTransform>();
		this.syncLauncher = NestedComponentUtilities.GetNestedComponentInChildren<SyncCannon, NetObject>(base.transform, true);
		this.syncHitscan = NestedComponentUtilities.GetNestedComponentInChildren<SyncContactScan, NetObject>(base.transform, true);
	}

	// Token: 0x060007C9 RID: 1993 RVA: 0x00029D78 File Offset: 0x00027F78
	private void Update()
	{
		if (!base.IsMine)
		{
			return;
		}
		float t = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
		this.Interpolate(t);
		if (Input.GetKeyDown(KeyCode.Space))
		{
			this.triggerJump = true;
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			this.triggerFade = true;
		}
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			this.triggerTurnLeft = true;
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			if (this.freakingOut)
			{
				this.triggerUpperBodyIdle = true;
			}
			else
			{
				this.triggerUpperBodyRun = true;
			}
			this.freakingOut = !this.freakingOut;
		}
		if (Input.GetKeyDown(KeyCode.F))
		{
			this.triggerProjectile = true;
		}
		if (Input.GetKeyDown(KeyCode.R))
		{
			this.triggerHitscan = true;
		}
		if (Input.GetKeyDown(KeyCode.T))
		{
			this.triggerTeleport = true;
		}
		if (Input.GetKeyDown(KeyCode.B))
		{
			this.triggerBlend = true;
		}
	}

	// Token: 0x060007CA RID: 1994 RVA: 0x00029E48 File Offset: 0x00028048
	private void FixedUpdate()
	{
		if (!base.IsMine)
		{
			return;
		}
		Vector3 vector = new Vector3(0f, 0f, 0f);
		Vector3 vector2 = new Vector3(0f, 0f, 0f);
		if (this.animator && this.animator.isActiveAndEnabled)
		{
			if (Input.GetKey(KeyCode.W))
			{
				this.animator.SetBool("walking", true);
				this.animator.SetFloat("speed", 1f);
			}
			else if (Input.GetKey(KeyCode.S))
			{
				this.animator.SetBool("walking", true);
				this.animator.SetFloat("speed", -0.5f);
			}
			else
			{
				this.animator.SetBool("walking", false);
				this.animator.SetFloat("speed", 0f);
			}
			if (this.triggerJump)
			{
				if (this.syncAnimator)
				{
					this.syncAnimator.SetTrigger("jump", LocalApplyTiming.OnSend);
				}
				this.triggerJump = false;
			}
			else if (this.triggerTurnLeft)
			{
				if (this.syncAnimator)
				{
					this.syncAnimator.SetTrigger("turnLeft", LocalApplyTiming.OnSend);
				}
				this.triggerTurnLeft = false;
			}
			if (this.triggerFade)
			{
				if (this.syncAnimator)
				{
					this.syncAnimator.CrossFadeInFixedTime("Jump", 0.25f, -1, 0f, LocalApplyTiming.OnSend);
				}
				this.triggerFade = false;
			}
			if (this.triggerBlend)
			{
				this.animator.SetFloat("blender", Mathf.Abs(Mathf.Sin(Time.time)));
			}
			else
			{
				this.animator.SetFloat("blender", -1f);
			}
			if (this.triggerUpperBodyRun)
			{
				if (this.syncAnimator)
				{
					this.syncAnimator.SetTrigger("upperBodyRun", LocalApplyTiming.OnSend);
				}
				this.triggerUpperBodyRun = false;
			}
			else if (this.triggerUpperBodyIdle)
			{
				if (this.syncAnimator)
				{
					this.syncAnimator.SetTrigger("upperBodyIdle", LocalApplyTiming.OnSend);
				}
				this.triggerUpperBodyIdle = false;
			}
		}
		if (!this.animator || !this.animator.applyRootMotion)
		{
			if (Input.GetKey(KeyCode.W))
			{
				vector += Vector3.forward;
			}
			else if (Input.GetKey(KeyCode.S))
			{
				vector -= Vector3.forward;
			}
		}
		if (Input.GetKey(KeyCode.A))
		{
			vector -= Vector3.right;
		}
		if (Input.GetKey(KeyCode.D))
		{
			vector += Vector3.right;
		}
		if (Input.GetKey(KeyCode.E))
		{
			vector2 += Vector3.up;
		}
		if (Input.GetKey(KeyCode.Q))
		{
			vector2 -= Vector3.up;
		}
		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);
			Vector2 vector3 = new Vector2(touch.rawPosition.x / (float)Screen.width, touch.rawPosition.y / (float)Screen.height);
			if (vector3.y > 0.66f)
			{
				if (vector3.x > 0.66f)
				{
					this.triggerHitscan = true;
				}
				else if (vector3.x < 0.33f)
				{
					this.triggerJump = true;
				}
			}
			else if (vector3.y < 0.33f)
			{
				if (vector3.x > 0.66f)
				{
					vector += Vector3.right;
				}
				else if (vector3.x < 0.33f)
				{
					vector -= Vector3.right;
				}
				else if (this.animator)
				{
					this.animator.SetBool("walking", true);
					this.animator.SetFloat("speed", -0.5f);
				}
			}
			else if (vector3.x > 0.66f)
			{
				vector2 += Vector3.up;
			}
			else if (vector3.x < 0.33f)
			{
				vector2 -= Vector3.up;
			}
			else if (this.animator)
			{
				this.animator.SetBool("walking", true);
				this.animator.SetFloat("speed", 1f);
			}
		}
		if (this.autoMove && !Application.isFocused)
		{
			vector2 += new Vector3(0f, Mathf.Sin(Time.time * 0.5f), 0f);
			if (this.animator)
			{
				this.animator.SetBool("walking", true);
				this.animator.SetFloat("speed", Mathf.Sin(Time.time) * 0.5f);
			}
		}
		this.Interpolate(1f);
		this.Move(vector, vector2);
		this.appliedDeltaT = 0f;
		if (this.triggerHitscan)
		{
			if (this.syncHitscan)
			{
				this.syncHitscan.QueueTrigger();
			}
			this.triggerHitscan = false;
		}
		if (this.triggerProjectile)
		{
			if (this.syncLauncher)
			{
				this.syncLauncher.QueueTrigger();
			}
			this.triggerProjectile = false;
		}
		if (this.triggerTeleport)
		{
			if (this.syncTransform)
			{
				this.syncTransform.FlagTeleport();
				base.transform.localPosition = default(Vector3);
				base.transform.localRotation = default(Quaternion);
			}
			this.triggerTeleport = false;
		}
	}

	// Token: 0x060007CB RID: 1995 RVA: 0x0002A398 File Offset: 0x00028598
	private void OnAnimatorMove()
	{
		if (!base.IsMine)
		{
			return;
		}
		this.animator.ApplyBuiltinRootMotion();
		base.transform.rotation = this.animator.rootRotation;
		base.transform.position = this.animator.rootPosition;
	}

	// Token: 0x060007CC RID: 1996 RVA: 0x0002A3E8 File Offset: 0x000285E8
	private void Move(Vector3 move, Vector3 turn)
	{
		if (this.is2D)
		{
			move = new Vector3(move.x, move.z, 0f);
			turn = new Vector3(0f, 0f, turn.y);
		}
		this.targRotDelta = turn * this.turnSpeed * Time.fixedDeltaTime;
		this.targPosDelta = move * this.moveSpeed * Time.fixedDeltaTime;
	}

	// Token: 0x060007CD RID: 1997 RVA: 0x0002A464 File Offset: 0x00028664
	private void Interpolate(float t)
	{
		t -= this.appliedDeltaT;
		this.appliedDeltaT += t;
		base.transform.rotation = base.transform.rotation * Quaternion.Euler(this.targRotDelta * t);
		base.transform.position += base.transform.rotation * (this.targPosDelta * t);
	}

	// Token: 0x04000918 RID: 2328
	public bool is2D;

	// Token: 0x04000919 RID: 2329
	[Range(0f, 300f)]
	public float turnSpeed = 150f;

	// Token: 0x0400091A RID: 2330
	[Range(0f, 4f)]
	public float moveSpeed = 4f;

	// Token: 0x0400091B RID: 2331
	public bool autoMove = true;

	// Token: 0x0400091C RID: 2332
	private Vector3 targRotDelta;

	// Token: 0x0400091D RID: 2333
	private Vector3 targPosDelta;

	// Token: 0x0400091E RID: 2334
	private float appliedDeltaT;

	// Token: 0x0400091F RID: 2335
	private Animator animator;

	// Token: 0x04000920 RID: 2336
	private SyncAnimator syncAnimator;

	// Token: 0x04000921 RID: 2337
	private SyncTransform syncTransform;

	// Token: 0x04000922 RID: 2338
	private SyncCannon syncLauncher;

	// Token: 0x04000923 RID: 2339
	private SyncContactScan syncHitscan;

	// Token: 0x04000924 RID: 2340
	private bool triggerJump;

	// Token: 0x04000925 RID: 2341
	private bool triggerFade;

	// Token: 0x04000926 RID: 2342
	private bool triggerTurnLeft;

	// Token: 0x04000927 RID: 2343
	private bool triggerUpperBodyRun;

	// Token: 0x04000928 RID: 2344
	private bool triggerUpperBodyIdle;

	// Token: 0x04000929 RID: 2345
	private bool triggerTeleport;

	// Token: 0x0400092A RID: 2346
	private bool freakingOut;

	// Token: 0x0400092B RID: 2347
	private bool triggerHitscan;

	// Token: 0x0400092C RID: 2348
	private bool triggerProjectile;

	// Token: 0x0400092D RID: 2349
	private bool triggerBlend;
}
